# Theaterverdeling

Desktop tool for the coordinator of educational theater for elementary schools.
One show has several showings. School groups are distributed over the showings and
then seated in the hall. The result is a PDF per showing with a colour-coded seat map
and a "row / first seat / last seat" list per group.

**Direction (decided 2026-11-14):** the from-scratch "ShowPlanner" rewrite is abandoned.
We continue with the existing WinForms app in [Theaterverdeling/](Theaterverdeling/) and
bring that to the next level, step by step, keeping it working after every step.
Parity with the 2013 output and with old `.wet` save files is not required.

## Requirements that still stand

- One show at a time, with multiple showings (date + time), each in a hall.
- Groups have a name, a size, preferred showings and a "front rows" wish. No preferred showing means any showing is fine.
- Every group goes to exactly one showing and sits together.
- Halls are data (JSON), not code. Rows, walkways, unusable seats and the sections that can be switched off per showing are described in the hall file.
- Save and load the whole plan as one local JSON file.
- PDF output per showing: seat map coloured per group plus the seat ranges per group.

Dropped from the old plan: WPF, layered "clean architecture", dependency injection, CLI.

## Current state

| Part | State | Verdict |
| --- | --- | --- |
| `Theaterverdeling/` WinForms app, .NET 9 | Builds and its tests pass. A full plan works again: add showings and groups, distribute, export the PDF, save and load. | **Keep, this is the base.** |
| `src/`, `tests/`, `ShowPlanner.sln` | Did not build. Persistence project compiles zero files (wrong include path), test project references a non-existing path and ran zero tests. Content was DTOs, a one-way mapper with placeholder data, a migration that only bumps a version string and an empty PDF stub. No seating logic, no UI. | **Removed 2026-11-14.** Still in git history up to commit `62074fb`. |
| `docs/superpowers/plans/2026-03-14-workspace-persistence.md` | Plan for the dropped persistence layer. | **Removed 2026-11-14.** |

Ideas worth carrying over from the rewrite, as ideas and not as code:

- One versioned JSON workspace file instead of the line-based `.wet` format.
- Preferences per showing (date and time) instead of "dates plus a morning flag".
- An xUnit test project, aimed at the distribution and seating logic.

### Worth keeping inside Theaterverdeling

- `Graphics.cs`: one drawing interface with a PDF and a screen implementation. This makes an on-screen preview cheap.
- `Zaal.cs` row model (`ChairRow`, `WalkWay`) with JSON load/save, and `Zalen/Lampegiet.json` (558 chairs, 550 usable: the two outer seats on both sides of rows 1 and 2 are never assigned, as in the old app).
- `Group`, `Voorstelling`, `SeatRange` and the three forms.
- The distribution rules, now in `Plan.Distribute`: front-row groups first, then morning groups, then largest first. A group goes to the showing with the most free seats on a date it prefers, and to any other showing when that fails.

### Known defects in the app

1. `EditorForm` is an empty window.

## Action points

### Phase 0: cleanup

Removed on 2026-11-14 (abandoned ShowPlanner rewrite):

- [x] `src/` (whole folder)
- [x] `tests/` (whole folder)
- [x] `ShowPlanner.sln`
- [x] `docs/` (whole folder)

Inside `Theaterverdeling/`:

- [x] `Theaterverdeling.zip` (5.4 MB: an old `.git` folder and ReSharper cache)
- [x] `Theaterverdeling/PdfSharp.dll`, `PdfSharp.Charting.dll`, `Word.dll`, `Theaterverdeling.exe` (unreferenced binaries; PDF comes from the PdfSharpCore NuGet package)
- [x] `Setuptheater/` and `Theaterverdelingsetup/` (WiX v3 installers pointing at `D:\backup\Visual Studio 2012\...`)
- [x] `Theaterverdeling/App.config` (.NET Framework 4.8 leftover)
- [x] `Theaterverdeling/Zalen/Test.json`, `Zalen/Test.pdf` (debug output)
- [x] `Theaterverdeling/Properties/Settings.settings`, `Settings.Designer.cs`, `Resources.resx`, `Resources.Designer.cs` (unused)
- [x] `Theaterverdeling/Theaterverdeling.csproj.DotSettings`
- [x] ClickOnce publish properties in `Theaterverdeling.csproj` (`PublishUrl` up to `BootstrapperEnabled`)
- [x] `Wet.ico` (went with the `.wet` format)
- [x] `Crountch-One-Piece-Jolly-Roger-Ace.ico` (decided 2026-11-14: it does not become the app icon)
- [x] `ZaalOld.cs` (everything it documented is ported)
- [x] Merge `Theaterverdeling/.gitignore` and `.gitattributes` into the repo root

### Phase 1: back to a working app

- [x] Load the hall from `Zalen/Lampegiet.json` next to the exe (copy to output, resolve via `AppContext.BaseDirectory`). Remove the hard-coded paths and the test PDF write.
- [x] Fill the showings list again.
- [x] Rewrite seat filling on top of `Rows`. Remove `Places` and `CreatePlaces`. Move the Lampegiet seat exceptions into the hall JSON as unusable seats (`"Unusable"` per row, seat numbers from 1).
- [x] Make "front rows off" and "balcony off" data driven: rows in the hall JSON carry a `"Section"` name, and the add-showing dialog offers the sections of the hall to switch off for that showing. The plan-wide "Rij 1 en 2 uit" checkbox is gone.
- [x] Port colours, "Podium" label, title with date and time, and the group legend from `ZaalOld.cs` into the shared drawing code. The legend uses a colour marker with a black name instead of a coloured name.
- [x] Replace the comparers and the hand-written merge sort with `OrderBy` chains.

### Phase 2: solid base

- [x] Move distribution and seating out of the form into plain classes without UI dependencies. Distribution lives in `Plan`, seating in `Zaal`, PDF export in `PdfExport`.
- [x] Tests for hall loading, seat filling, sections, drawing, the plan file, distribution and the add, save and load flows of the forms. The PDF export is covered too. Not covered: the file dialogs themselves.
- [x] Replace `.wet` with one versioned JSON plan file: show name, showings with hall and switched-off sections, groups with their wishes. Dates do not depend on language settings. Old `.wet` files are not supported.
- [x] Input validation, shown as a message in the dialog: empty or duplicate group name, group larger than the largest showing, two showings in the same minute. A group without a preferred date is allowed and means "no preference".
- [x] Edit an existing group or showing with "Wijzig groep" and "Wijzig voorstelling". When a showing moves to another date and no showing is left on the old date, the groups that preferred the old date follow.
- [x] Report all unplaced groups in one summary instead of one message box per group.

### Phase 3: next level

- [x] On-screen preview: "Verdeel groepen" distributes and opens a preview window with the page of each showing, drawn by the same code as the PDF. "Exporteer PDF" in that window writes the files. The preview closes when the plan changes, because it is then out of date.
- [x] Decided 2026-11-14: preferences stay as preferred dates plus the "Ochtend" flag. Preferences per showing are not built.
- [x] Double-click or Enter on a group or showing opens it for editing. The group dialog already listed the dates as short dates.
- [x] Group colours for any number of groups: twelve clearly different light colours, and from the thirteenth group on the same colours with one or two dark stripes on each seat. That tells 36 groups in one showing apart.
- [x] PDF legend that always fits: one row per group with its rows and seats, four per line, and the lines are squeezed together for a full hall.
- [x] Show switched-off rows and seats that are never used in grey on the seat map.
- [x] Bulk input of groups: "Meerdere groepen" opens a grid to add many groups at once. This replaces the planned CSV or Excel import, because rows copied from Excel can be pasted into the grid with Ctrl+V or "Plakken".
  - Columns: name, size, "Vooraan", "Ochtend", one tick column per showing date, and a message column.
  - Each row is checked with the same rules as the group dialog, plus duplicate names within the grid. "Toevoegen" adds the valid rows; a row with a problem stays in the grid with its message.
  - Decided: a paste carries the name and the size, separated by a tab or a semicolon. The ticks are set in the grid.
  - Decided: a first pasted row whose size is not a number is treated as a header and skipped. A number inside a name, such as "groep 5", stays part of the name.
  - Decided: it is a separate dialog. The group list in the main window stays as it is.
  - Decided: no file import.

Open, in this order (decided 2026-11-14):

1. [ ] Hall editor in `EditorForm`, so a second venue needs no code.
2. [ ] Choose the hall per showing in the add-showing dialog. Halls already load by name and the plan file stores the hall per showing.
3. [ ] Manual corrections: move a group to another showing, pin a group, then re-run seating.
4. [ ] Smarter seating: balance showings, avoid splitting a group across the walkway, keep a group in adjacent rows, avoid a single leftover seat at the far edge of the next row.
5. [ ] Distribution: single-file self-contained publish instead of an MSI.

## Build and run

```bash
dotnet build Theaterverdeling/Theaterverdeling.sln
```

```bash
dotnet test Theaterverdeling/Theaterverdeling.sln
```

```bash
dotnet run --project Theaterverdeling/Theaterverdeling/Theaterverdeling.csproj
```

Windows only (`net9.0-windows`, WinForms). The UI is in Dutch.
