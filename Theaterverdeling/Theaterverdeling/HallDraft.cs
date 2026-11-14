using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Theaterverdeling
{
  // One line of the hall editor: a row of chairs or a walkway, as it was typed, before it is checked.
  // The numbers are text, because a typed cell can hold anything.
  public sealed class RowDraft
  {
    public const int MaxSeats = 100;

    public bool IsWalkway { get; set; }
    public string RowNr { get; set; } = "";
    public string SeatCount { get; set; } = "";
    public string Section { get; set; } = "";

    // Seat numbers that are never given to a group, such as "1, 2, 22-23".
    public string Unusable { get; set; } = "";

    // Not shown in the editor; kept so a hall file that switches a row off stays as it is.
    public bool InUse { get; set; } = true;

    // True when the row number is a whole number of at least one.
    public bool TryGetRowNr(out int rowNr)
    {
      return TryParse(RowNr, out rowNr) && rowNr > 0;
    }

    public bool TryGetSeatCount(out int seatCount)
    {
      return TryParse(SeatCount, out seatCount) && seatCount > 0 && seatCount <= MaxSeats;
    }

    // Reads the unusable seats: numbers and ranges such as 22-23, separated by commas, semicolons or spaces.
    public bool TryGetUnusable(out SortedSet<int> seats)
    {
      seats = new SortedSet<int>();
      foreach (var part in (Unusable ?? "").Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
      {
        var ends = part.Split('-');
        if (ends.Length > 2 || !TryParse(ends[0], out var first) || !TryParse(ends[ends.Length - 1], out var last) || first < 1 || last < first || last > MaxSeats)
        {
          return false;
        }
        for (var seat = first; seat <= last; seat++)
        {
          seats.Add(seat);
        }
      }
      return true;
    }

    // Makes a usable seat unusable and the other way round. Seat numbers start at 1.
    public void ToggleSeat(int seat)
    {
      // Text that cannot be read is replaced; the click says what is meant.
      TryGetUnusable(out var seats);
      if (!seats.Remove(seat))
      {
        seats.Add(seat);
      }
      Unusable = Format(seats);
    }

    // Writes seat numbers the short way: 1, 2, 22, 23 becomes "1-2, 22-23".
    public static string Format(IEnumerable<int> seats)
    {
      var parts = new List<string>();
      var sorted = seats.Distinct().OrderBy(seat => seat).ToList();
      for (var i = 0; i < sorted.Count; i++)
      {
        var first = sorted[i];
        while (i + 1 < sorted.Count && sorted[i + 1] == sorted[i] + 1)
        {
          i++;
        }
        parts.Add(first == sorted[i] ? $"{first}" : $"{first}-{sorted[i]}");
      }
      return string.Join(", ", parts);
    }

    private static bool TryParse(string text, out int number)
    {
      return int.TryParse((text ?? "").Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out number);
    }
  }

  // A hall while it is being edited: its rows from the stage to the back.
  public sealed class HallDraft
  {
    public List<RowDraft> Rows { get; } = new List<RowDraft>();

    public static HallDraft FromHall(Zaal hall)
    {
      var draft = new HallDraft();
      foreach (var row in hall.Rows)
      {
        if (row is ChairRow chairRow)
        {
          draft.Rows.Add(new RowDraft
          {
            RowNr = chairRow.RowNr.ToString(CultureInfo.InvariantCulture),
            SeatCount = chairRow.Seats.Length.ToString(CultureInfo.InvariantCulture),
            Section = chairRow.Section ?? "",
            Unusable = RowDraft.Format(Enumerable.Range(1, chairRow.Seats.Length).Where(seat => !chairRow.Seats[seat - 1].Usable)),
            InUse = chairRow.InUse
          });
        }
        else
        {
          draft.Rows.Add(new RowDraft { IsWalkway = true });
        }
      }
      return draft;
    }

    // Adds a row of chairs below the given line, or at the back when there is no such line.
    // It gets the next row number, and the seat count and section of the row of chairs before it.
    public int AddRow(int after)
    {
      var index = InsertIndex(after);
      var numbers = Rows.Where(row => !row.IsWalkway && row.TryGetRowNr(out _)).Select(row => { row.TryGetRowNr(out var number); return number; }).ToList();
      var example = Rows.Take(index).LastOrDefault(row => !row.IsWalkway);
      Rows.Insert(index, new RowDraft
      {
        RowNr = ((numbers.Count == 0 ? 0 : numbers.Max()) + 1).ToString(CultureInfo.InvariantCulture),
        SeatCount = example?.SeatCount ?? "10",
        Section = example?.Section ?? ""
      });
      return index;
    }

    public int AddWalkway(int after)
    {
      var index = InsertIndex(after);
      Rows.Insert(index, new RowDraft { IsWalkway = true });
      return index;
    }

    // Moves a line one place towards the stage (-1) or towards the back (+1). Returns where it is now.
    public int Move(int index, int direction)
    {
      var target = index + direction;
      if (index < 0 || index >= Rows.Count || target < 0 || target >= Rows.Count)
      {
        return index;
      }
      (Rows[index], Rows[target]) = (Rows[target], Rows[index]);
      return target;
    }

    // Says for every line why it cannot be saved, or null when it can.
    public List<string> Check()
    {
      var messages = new List<string>();
      var seen = new HashSet<int>();
      foreach (var row in Rows)
      {
        messages.Add(row.IsWalkway ? null : CheckRow(row, seen));
      }
      return messages;
    }

    // Says why the hall as a whole cannot be saved under this name, or null when it can.
    // Problems of single lines are reported by Check.
    public string CheckHall(string name)
    {
      var trimmed = (name ?? "").Trim();
      if (trimmed == "")
      {
        return "Vul een naam voor de zaal in.";
      }
      if (trimmed.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || trimmed.EndsWith("."))
      {
        return "De naam van de zaal mag geen tekens als \\ / : * ? \" < > | bevatten en niet op een punt eindigen.";
      }
      if (Rows.All(row => row.IsWalkway))
      {
        return "Voeg minstens één rij met stoelen toe.";
      }
      if (Check().Any(message => message != null))
      {
        return "Los eerst de meldingen bij de rijen op.";
      }
      return null;
    }

    // The hall as far as it can be read. A line with a problem is left out or shortened,
    // so the seat map can be shown while the hall is being typed.
    public Zaal ToHall()
    {
      var rows = new List<Row>();
      foreach (var row in Rows)
      {
        if (row.IsWalkway)
        {
          rows.Add(new WalkWay());
          continue;
        }
        row.TryGetRowNr(out var rowNr);
        if (!row.TryGetSeatCount(out var seatCount))
        {
          seatCount = 0;
        }
        row.TryGetUnusable(out var unusable);
        var section = (row.Section ?? "").Trim();
        rows.Add(new ChairRow(seatCount, row.InUse, rowNr, unusable, section == "" ? null : section));
      }
      return Zaal.FromRows(rows);
    }

    private int InsertIndex(int after)
    {
      return after >= 0 && after < Rows.Count ? after + 1 : Rows.Count;
    }

    private static string CheckRow(RowDraft row, HashSet<int> seen)
    {
      if (!row.TryGetRowNr(out var rowNr))
      {
        return "Vul bij rij een heel getal in.";
      }
      if (!seen.Add(rowNr))
      {
        return $"Rij {rowNr} komt al eerder voor.";
      }
      if (!row.TryGetSeatCount(out var seatCount))
      {
        return $"Vul bij stoelen een heel getal van 1 tot {RowDraft.MaxSeats} in.";
      }
      if (!row.TryGetUnusable(out var unusable))
      {
        return "Vul bij niet gebruikt stoelnummers in, zoals 1, 2 of 22-23.";
      }
      if (unusable.Count > 0 && unusable.Max > seatCount)
      {
        return $"Stoel {unusable.Max} bestaat niet in deze rij.";
      }
      return null;
    }
  }
}
