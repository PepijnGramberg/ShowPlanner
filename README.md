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
- Groups have a name, a size, preferred showings and a "front rows" wish.
- Every group goes to exactly one showing and sits together.
- Halls are data (JSON), not code. Rows, walkways, unusable seats and the sections that can be switched off per showing are described in the hall file.
- Save and load the whole plan as one local JSON file.
- PDF output per showing: seat map coloured per group plus the seat ranges per group.

Dropped from the old plan: WPF, layered "clean architecture", dependency injection, CLI.

## Current state

| Part | State | Verdict |
| --- | --- | --- |
| `Theaterverdeling/` WinForms app, .NET 9 | Builds. Halfway through a refactor from hard-coded hall to JSON hall, so it does not run a full plan today. | **Keep, this is the base.** |
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
- The distribution rules in `Divideshows`: front-row groups first, then morning groups, then largest first, preferred date first with a fallback to any showing.

### Known defects in the app

1. Save and load still use the old line-based `.wet` format, and loading does not restore the showings. This gets replaced by the JSON plan file, not repaired.
2. The showing comparers return 0/1 instead of negative/zero/positive, so `List.Sort` ordering is not reliable (`Zaalsort`, `Zaaldatesort`).
3. `EditorForm` is an empty window.

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
- [ ] `Crountch-One-Piece-Jolly-Roger-Ace.ico`, unless it becomes the app icon; `Wet.ico` together with the `.wet` format
- [x] `ZaalOld.cs` (everything it documented is ported)
- [x] Merge `Theaterverdeling/.gitignore` and `.gitattributes` into the repo root

### Phase 1: back to a working app

- [x] Load the hall from `Zalen/Lampegiet.json` next to the exe (copy to output, resolve via `AppContext.BaseDirectory`). Remove the hard-coded paths and the test PDF write.
- [x] Fill the showings list again.
- [x] Rewrite seat filling on top of `Rows`. Remove `Places` and `CreatePlaces`. Move the Lampegiet seat exceptions into the hall JSON as unusable seats (`"Unusable"` per row, seat numbers from 1).
- [x] Make "front rows off" and "balcony off" data driven: rows in the hall JSON carry a `"Section"` name, and the add-showing dialog offers the sections of the hall to switch off for that showing. The plan-wide "Rij 1 en 2 uit" checkbox is gone.
- [x] Port colours, "Podium" label, title with date and time, and the group legend from `ZaalOld.cs` into the shared drawing code. The legend uses a colour marker with a black name instead of a coloured name.
- [ ] Replace the comparers and the hand-written merge sort with `OrderBy` chains.

### Phase 2: solid base

- [ ] Move distribution and seating out of the form into plain classes without UI dependencies.
- [ ] Extend the xUnit project (`Theaterverdeling.Tests`, started in phase 1). Cover: capacity, every group in one showing, groups seated contiguously, preferences honoured when possible.
- [ ] Replace `.wet` with one versioned JSON plan file (show name, showings, groups, hall, options). Culture-invariant dates. Old `.wet` files are not supported.
- [ ] Input validation: empty or duplicate group names, group larger than the hall, no preferred date ticked.
- [ ] Edit an existing group or showing; today only add and remove exist.
- [ ] Report all unplaced groups in one summary instead of one message box per group.

### Phase 3: next level

- [ ] On-screen preview of each showing before exporting, using the existing screen drawing context.
- [ ] Preferences per showing (date and time) instead of the morning flag.
- [ ] Manual corrections: move a group to another showing, pin a group, then re-run seating.
- [ ] Smarter seating: balance showings, avoid splitting a group across the walkway, keep a group in adjacent rows, avoid a single leftover seat at the far edge of the next row.
- [ ] PDF legend: colours repeat after nine groups and the legend holds 50 lines in two columns. Extend both.
- [ ] Show switched-off rows as closed in the seat map.
- [ ] Choose the hall per showing in the add-showing dialog, once there is more than one hall file.
- [ ] Hall editor in `EditorForm`, so a second venue needs no code.
- [ ] Import groups from CSV or Excel.
- [ ] Distribution: single-file self-contained publish instead of an MSI.

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
