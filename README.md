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
- The distribution rules, now in `Plan.Distribute`: front-row groups first, then morning groups, then largest first. A group goes to the showing that stays the least full on a date it prefers, and to any other showing when that fails.

### Known defects in the app

None known.

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

1. [x] Hall editor: "Zalen" in the main window opens a window to make a hall or change one, so a second venue needs no code.
   - The rows are in a grid: row number, number of seats, section and the seats that are not used, such as `1-2, 22-23`. Rows and walkways can be added, removed and moved.
   - Next to the grid is the seat map. A click on a seat switches it between used and not used; a click next to the seats chooses the row.
   - Every line is checked while typing: row number, duplicate row number, number of seats, seats that do not exist. A hall with a problem is not saved.
   - "Opslaan" writes the hall to `Zalen/<name>.json` next to the exe. Saving under the name of another hall asks first.
   - A changed hall counts for showings that are added afterwards and for plans that are loaded afterwards.
   - The seat map is its own control (`SeatMapControl`, with `SeatMapLayout` for the positions). It is drawn by the PDF drawing code and reports the row, seat and group of a click, so the manual corrections can use it too.
2. [x] Choose the hall per showing: the showing dialog lists the halls in the halls folder and shows the sections of the chosen hall. A new showing starts in the hall of the showing before it. Once a plan uses more than one hall, the lists of showings name the hall. The PDF page of a showing names its hall after the date and time.
3. [x] Manual corrections in the preview window, which now shows the clickable seat map of the chosen showing with its groups in a list.
   - A click on a seat or on a list entry chooses a group and outlines its seats. A click next to the seats chooses none.
   - "Verplaats naar" lists the other showings with enough free seats. Moving seats both showings again and pins the group to its new showing, also against its wishes, such as a morning group in an afternoon showing.
   - "Vastzetten" pins the chosen group and "Losmaken" lets it go. "Verdeel groepen" leaves pinned groups in their showing and distributes the rest around them.
   - Groups that fit in no showing are listed under "Niet ingedeeld" and can be placed by hand from there.
   - Single seats: with a group chosen, a click on one of its seats makes that seat free and a click on a free seat gives it to the group. The list shows how many people of the group still have no seat.
   - Pins are saved in the plan file. A pin is dropped when its showing is removed or has no room left.
   - A group whose seats were changed by hand keeps exactly those seats and is pinned to its showing. When the showing is seated again, the other groups are seated around it. The list marks it with "eigen stoelen".
   - "Automatisch plaatsen" lets go of the hand-chosen seats and seats the group by the rules again. "Losmaken" and moving the group to another showing do the same.
   - Hand-chosen seats are saved in the plan file, per group as rows with a first and last seat. After loading, "Verdeel groepen" puts them back, as far as the hall still has those seats.
4. [x] Smarter seating. A group still sits row by row on one side of the hall, in adjacent rows. On top of that:
   - Balance: a group goes to the preferred showing that stays the least full for its size with the group in it. This only differs from "most free seats" when the halls differ in size.
   - Nobody sits alone: when the last row of a group would hold one person, the group leaves a seat free in the row before, so two sit together. A group also does not start on a row with one seat left.
   - Walkway: a group that would sit on both sides of the walkway starts behind it, when it fits there.
   - Seats that these rules leave open go to a later group that fits in them completely.
   - A showing that is too full for these rules is seated the plain way, row by row with every seat used, so no group loses its place because of them.
5. [x] Distribution: one self-contained exe instead of an MSI, made with the publish command below. It needs no installed .NET. The folder to hand out is `publish/`: `Theaterverdeling.exe` (about 50 MB) with the halls folder `Zalen` next to it. Halls made in the hall editor are saved in that `Zalen` folder, so keep the folder when the exe is replaced by a newer one.

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

```bash
dotnet publish Theaterverdeling/Theaterverdeling/Theaterverdeling.csproj -p:PublishProfile=Portable
```

Windows only (`net9.0-windows`, WinForms). The UI is in Dutch.
