# Working on Theaterverdeling

[README.md](README.md) says what the app does, how to build it and what the files look like. Read it first. This file has what you need on top of that to change the code.

## Commands

```bash
dotnet build Theaterverdeling/Theaterverdeling.sln
```

```bash
dotnet test Theaterverdeling/Theaterverdeling.sln
```

```bash
dotnet publish Theaterverdeling/Theaterverdeling/Theaterverdeling.csproj -p:PublishProfile=Portable
```

The app is Windows only (`net9.0-windows`, WinForms). The test run takes a few seconds; run all of it after every change.

## Language

- The user interface is Dutch: every label, button, message and column header.
- Code comments, test names, commit messages, the README and this file are English.
- The older domain classes have Dutch names (`Zaal`, `Voorstelling`, `Personen`, `Datum`). Keep them. New names are English.

## How the code is put together

- **Logic has no UI.** Distribution, moving, pinning, the checks and the plan file are in `Plan`. Seating, hall files and drawing are in `Zaal`. Forms only show state and call these classes. Put new rules in `Plan` or `Zaal` and test them there.
- **One drawing path.** `Zaal.DrawMap` and `Zaal.Draw` draw on `IDrawingContext`. `XGraphicsDrawingContext` writes the PDF and `GdiDrawingContext` draws on screen. Anything new on the seat map goes through this interface, so the PDF and the screen stay the same.
- **One seat layout.** `SeatMapLayout` decides where rows and seats are. The drawing code and the hit test of `SeatMapControl` both use it. Never compute seat positions anywhere else.
- **`SeatMapControl` is reusable.** It draws a hall and raises `SeatMapClick` with the row, the seat and the group that sits there. What a click means is decided by the window: the hall editor switches seats on and off, the preview chooses groups and changes seats.
- **Text first, then checked.** What the user types in a grid is kept as text (`GroupEntry`, `RowDraft`) and checked afterwards. A check returns a Dutch message, or null when all is well. Follow this for new input.
- **Designer files are written by hand.** There are no `.resx` files. Edit `*.Designer.cs` directly and keep the control names stable: the tests find controls by name.
- **Public properties on a control or form** need `[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]`, or the build fails with WFO1000.

## Rules of the domain that are easy to break

- **Seating order.** Groups choose a showing and are seated in one order: front-row groups, then morning groups, then larger groups, then the order of the plan (`Plan.InSeatingOrder`). `Zaal.AssignSeats` seats `Groups` in list order, so keep `Voorstelling.Groups` in that order.
- **Seating has two modes.** `AssignSeatsWithRules` applies the rules in the README. When a group finds no place that way, `AssignSeatsRowByRow` seats everybody with every seat used. A new seating rule must keep this fallback, so a rule never costs a group its place.
- **Pins.** `Group.Pinned` means the group stays in `Group.Voorstelling` when the plan is distributed. A pin is dropped when its showing is gone or has no room.
- **Hand-chosen seats.** `Group.HandSeated` means the group keeps exactly the seats it has in the hall of its showing. `AssignSeats` puts those back before it seats the others. A hand-seated group is always pinned. The flag is dropped when the group has no seat in the hall that is being seated.
- **What is saved.** The plan file holds the plan, the pins and the hand-chosen seats, and nothing else of a distribution. After loading, "Verdeel groepen" rebuilds the rest. New fields in the plan file are optional and left out when empty, so older files keep loading; raise `FileVersion` only for a change that breaks them.
- **Halls live next to the exe.** `Zaal.HallsFolder` is `Zalen` under `AppContext.BaseDirectory`. Halls are loaded by name, and only the file name counts, so a name from a plan file cannot point outside that folder.
- **A showing owns its hall.** Every `Voorstelling` has its own `Zaal` object with its own seats. Changing a hall file does not change showings that are already in the open plan.
- **The preview closes when the plan changes** in the main window, because it would be out of date. Corrections made inside the preview refresh it instead.

## Tests

- xUnit, in `Theaterverdeling/Theaterverdeling.Tests/`. Test names are sentences that say what the app does, such as `Nobody_sits_alone_in_the_last_row_of_a_group`.
- Write the test with the change. Logic gets a plain test on `Plan` or `Zaal`; a window gets a test that drives the real form.
- `TestHalls` builds small halls and groups. `RecordingDrawingContext` records what is drawn. `FormDriver` runs a test on an STA thread with the Dutch culture and finds controls by name.
- Tests that open a form are in `[Collection("Forms")]`, because they share the list of open windows and must not run side by side.
- A test must never open a message box or a file dialog: nobody can answer it and the run hangs. Forms take questions through a replaceable hook (`EditorForm.Confirm`), and offer a public method next to each dialog button (`SavePlan`, `LoadPlan`, `ExportPdf`, `SaveHall`).
- Do not pump messages in a test (`Application.DoEvents`). A paint error then shows a crash dialog on the screen of whoever runs the tests. To look at a window or check pixels, use `Control.DrawToBitmap`, which throws inside the test.
- Tests that need a hall file save it in the halls folder of the test output under a unique name and delete it in a `finally`.
- The file dialogs themselves are not covered.

## Way of working

- Small steps that leave the app working and the tests green.
- One commit per finished step, pushed right away. Commit messages are short imperative English sentences, like the existing history.
- Keep the README true: when behaviour changes, change the README in the same commit. It describes what the app is now, not how it got there, and has no to-do list.
- A change to how groups are distributed or seated changes the outcome for every user. Agree on the rule before building it.
