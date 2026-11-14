# To do: make the app intuitive for a teacher

Outcome of the UX review. Each point is one commit; tick it off in that commit.

## 1. Loses work or blocks the user

- [x] Ask before the plan is lost: "Reset", "Laden" and closing the main window with changes that are not saved.
- [x] Ask before a group or a showing is removed.
- [x] A new showing starts at 10:00 and the time has no seconds.
- [x] After "Laden", tell the user to press "Verdeel groepen" to see the seating again.
- [x] Say why the preview closed when the plan changes in the main window.
- [x] After "Exporteer PDF", say where the files are and offer to open the PDF.
- [x] Make the showing and group dialogs modal, so only one is open and the main window waits.

## 2. Unclear wording and missing guidance

- [x] Use two words for the show and a showing: "Voorstelling naam" and "Maak voorstelling" mean different things now.
- [ ] Show the order of the steps in the main window and make "Verdeel groepen" the main button, away from "Reset". The steps name the PDF, so the way to it is visible from the main window.
- [ ] Show the groups in columns (Naam, Aantal, Vooraan, Ochtend) instead of "O" and "V", and write the seat counter in words.
- [ ] Give the numbers with "Te weinig stoelen" and the reason with "Niet ingedeeld".
- [ ] Clearer labels: "Uitzetten", "Eisen", "Datum voorkeuren" (nothing ticked means any date), "Vastzetten", the window title "Voorbeeld".
- [ ] Show the whole help text in the preview; it is cut off now.
- [ ] Show the colour of each group in the group list of the preview.
- [ ] Change single seats only in a mode that is switched on, so a stray click does not free a seat.

## 3. Friction

- [ ] Enter and Esc work in the dialogs, and the group dialog starts in the name.
- [ ] One limit for the length of a group name in the dialog and in the grid.
- [ ] The lists of the main window grow with the window.
- [ ] "Meerdere groepen": make pasting from Excel easy to find and say that the date columns are preferences.
- [ ] Move "Zalen" away from "Opslaan" and "Laden".
- [ ] An app icon, and the same readable font in every window.
