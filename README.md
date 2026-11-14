# Theaterverdeling

Desktop tool for the coordinator of educational theater for elementary schools.
One show has several showings. School groups are distributed over the showings and
then seated in the hall. The result is a PDF per showing with a colour-coded seat map
and the rows and seats of every group.

It is a Windows app (WinForms, .NET 9). The user interface is in Dutch. Every window uses one font, Segoe UI 10, set in `Program.cs`. The main window fits a laptop screen of 1366 by 768; the preview and the hall editor open maximised.

## What it does

### The plan

- A plan is one show with its showings and the groups that come to see it.
- The main window follows the order of the work: 1. the showings, 2. the groups, 3. "Verdeel groepen", which leads to the seating and the PDF.
- A showing has a date, a time and a hall. Sections of the hall, such as the balcony, can be switched off per showing. A new showing starts with the date, the hall and the switched-off sections of the last showing in the plan, so only the time is left to set.
- A group has a name, a size, preferred dates, a "Vooraan" wish for the front rows and an "Ochtend" wish for a showing before noon. No preferred date means any date is fine.
- Groups are added one at a time, or many at once in a grid with "Meerdere groepen". Rows copied from a spreadsheet can be pasted into that grid: name and size, separated by a tab or a semicolon.
- While a dialog for a showing or for groups is open, the main window waits. Enter accepts a dialog and Esc cancels it.
- The dialogs check the input and show the problem as a message: empty or duplicate group name, a group name of more than 25 characters, a group larger than the largest showing, two showings at the same moment.
- "Plan opslaan" and "Plan laden" save and load the whole plan as one JSON file.
- Removing a group or a showing asks first.
- "Nieuw plan", "Plan laden" and closing the window ask first when the plan has changes that are not saved.

### Distributing groups over showings

"Verdeel groepen" gives every group one showing and its seats, and opens the preview.

- Front-row groups choose first, then morning groups, then the larger groups.
- A group goes to the showing on a date it prefers that stays the least full for its size with the group in it. When no preferred date has room, it goes to any other showing. The earlier showing wins a tie.
- A morning group only goes to a showing before noon.
- Groups that fit in no showing are reported in one message that says why, and listed under "Niet ingedeeld" in the preview.
- When the groups have more people than the showings have seats, nothing is distributed and a message gives both numbers.

A change to the plan in the main window closes the preview, because it is out of date. The main window says so and asks to distribute again.

### Seating a showing

- Groups are seated in the order above, so front-row groups sit in front.
- A group sits row by row on one side of the hall, from the edge towards the middle. The next group goes to the side that is the fewest rows along.
- Nobody sits alone: when the last row of a group would hold one person, the group leaves a seat free in the row before, so two sit together. A group also does not start on a row with one seat left.
- A group that would sit on both sides of a walkway starts behind the walkway, when it fits there.
- Seats that these rules leave open go to a later group that fits in them completely.
- A showing that is too full for these rules is seated the plain way, row by row with every seat used, so no group loses its place because of them.

### Correcting by hand

The preview shows the seat map of the chosen showing, with its groups in a list. Every group in the list has the colour of its seats. A click on a seat or on a list entry chooses a group and outlines its seats.

- "Verplaats naar" moves the chosen group to another showing with enough free seats, also against its wishes. Both showings are seated again and the group is pinned to its new showing.
- "Vastzetten" pins a group to its showing and "Losmaken" lets it go. "Verdeel groepen" leaves pinned groups where they are and distributes the rest around them.
- Single seats: with a group chosen and "Stoelen van deze groep aanpassen" ticked, a click on one of its seats makes that seat free, and a click on a free seat gives it to the group. Without the tick a click on a seat changes nothing, so a stray click costs no seat. The group then keeps exactly those seats, is pinned to its showing, and is marked "eigen stoelen". The other groups are seated around it.
- "Automatisch plaatsen" lets go of the hand-chosen seats and seats the group by the rules again.
- Pins and hand-chosen seats are saved in the plan file.

### PDF

"Exporteer PDF" in the preview writes one file with a page for every showing, and one file per showing next to it. Afterwards the preview says what was saved and offers to open the PDF. A page has the show name, the date, time and hall, the seat map coloured per group, and a legend with the rows and seats of every group.

The first twelve groups of a showing each get their own light colour. Later groups get the same colours with one or two dark stripes on each seat, which tells 36 groups apart. Seats nobody can sit on are grey.

### Halls

A hall is a JSON file in the folder `Zalen` next to the exe. The app ships with `Lampegiet.json` (558 chairs, 550 in use).

"Zalen beheren" in the top corner of the main window opens the hall editor, so a new venue needs no code:

- The rows are in a grid: row number, number of seats, section, and the seats that are not used, such as `1-2, 22-23`. Rows and walkways can be added, removed and moved.
- Next to the grid is the seat map. A click on a seat switches it between used and not used.
- Every line is checked while typing. A hall with a problem is not saved.
- A changed hall counts for showings that are added afterwards and for plans that are loaded afterwards.

The hall is chosen per showing in the showing dialog. Once a plan uses more than one hall, the lists of showings name the hall.

## Build, test and run

```bash
dotnet build Theaterverdeling/Theaterverdeling.sln
```

```bash
dotnet test Theaterverdeling/Theaterverdeling.sln
```

```bash
dotnet run --project Theaterverdeling/Theaterverdeling/Theaterverdeling.csproj
```

## Hand out the app

```bash
dotnet publish Theaterverdeling/Theaterverdeling/Theaterverdeling.csproj -p:PublishProfile=Portable
```

This writes the folder `publish/`: `Theaterverdeling.exe` (about 50 MB) with the halls folder `Zalen` next to it. The exe needs no installed .NET. Hand out the whole folder.

Halls made in the hall editor are saved in that `Zalen` folder, so keep the folder when the exe is replaced by a newer one.

## Files

### Plan file

```json
{
  "Version": 1,
  "ShowName": "Kikker is jarig",
  "Showings": [
    { "Date": "2026-03-16", "Time": "10:00:00", "Hall": "Lampegiet", "SwitchedOff": ["Balkon"] }
  ],
  "Groups": [
    {
      "Name": "De Regenboog",
      "People": 28,
      "FrontRows": false,
      "Morning": true,
      "PreferredDates": ["2026-03-16"],
      "PinnedTo": "2026-03-16 10:00",
      "Seats": [{ "Row": 3, "From": 1, "To": 12 }]
    }
  ]
}
```

`PinnedTo` is only there for a pinned group, and `Seats` only for a group with hand-chosen seats. The outcome of a distribution is not saved otherwise: press "Verdeel groepen" after loading. The main window says so after "Plan laden".

### Hall file

```json
{
  "Rows": [
    { "Type": "ChairRow", "RowNr": 1, "SeatCount": 23, "Section": "Rij 1 en 2", "Unusable": [1, 2, 22, 23] },
    { "Type": "ChairRow", "RowNr": 2, "SeatCount": 25 },
    { "Type": "WalkWay" }
  ]
}
```

Rows run from the stage to the back. `Type` must be the first property of a row. `Section` names a part of the hall that can be switched off per showing. `Unusable` lists the seats, numbered from 1, that are never given to a group.

## Code

Everything is in [Theaterverdeling/](Theaterverdeling/): the app in `Theaterverdeling/Theaterverdeling/` and its xUnit tests in `Theaterverdeling/Theaterverdeling.Tests/`.

| File | What it holds |
| --- | --- |
| `Plan.cs` | The plan: distribution, moving and pinning, the input checks, the plan file. |
| `Zaal.cs` | A hall: rows and seats, seating, drawing the seat map and legend, the hall file. |
| `Voorstelling.cs`, `Group.cs` | A showing and a group. |
| `SeatMapLayout.cs`, `SeatMapControl.cs` | Where the seats are on a map, and the clickable seat map on screen. |
| `Graphics.cs`, `PdfExport.cs` | One drawing interface with a PDF and a screen implementation, and the PDF export. |
| `HallDraft.cs`, `GroupEntry.cs` | A hall and a group as typed in a grid, before they are checked. |
| `Theaterverdeling.cs` | The main window. |
| `Addshowform.cs`, `Addgroupform.cs`, `Bulkgroupform.cs` | The dialogs for showings and groups. |
| `PreviewForm.cs`, `EditorForm.cs` | The preview with the corrections by hand, and the hall editor. |
| `AppIcon.cs`, `Theaterverdeling.ico` | The icon of the app and of every window. |

[AGENTS.md](AGENTS.md) has the conventions for working on the code.
