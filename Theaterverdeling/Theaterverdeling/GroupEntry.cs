using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Theaterverdeling
{
  // One row of the bulk input: a group as it was typed or pasted, before it is checked.
  // The size is text, because a pasted or typed cell can hold anything.
  public class GroupEntry
  {
    public string Name { get; set; } = "";
    public string People { get; set; } = "";
    public bool FrontRows { get; set; }
    public bool Morning { get; set; }
    public List<DateTime> Dates { get; set; } = new List<DateTime>();

    // True when the size is a whole number of at least one.
    public bool TryGetPeople(out int people)
    {
      return int.TryParse((People ?? "").Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out people) && people > 0;
    }

    public Group ToGroup()
    {
      TryGetPeople(out var people);
      return new Group((Name ?? "").Trim(), people, FrontRows, Morning, new List<DateTime>(Dates));
    }

    // Reads rows that were copied from a spreadsheet or a list: one group per line, with the name
    // and the size separated by a tab or a semicolon. Only the name and the size are read.
    // A number inside a name stays part of the name, so "De Vlieger groep 5" is a name without a size.
    public static List<GroupEntry> FromPastedText(string text)
    {
      var entries = new List<GroupEntry>();
      foreach (var line in (text ?? "").Split('\n'))
      {
        var cells = line.Split('\t');
        if (cells.Length == 1)
        {
          cells = line.Split(';');
        }
        var entry = new GroupEntry
        {
          Name = cells[0].Trim(),
          People = cells.Length > 1 ? cells[1].Trim() : ""
        };
        if (entry.Name == "" && entry.People == "")
        {
          continue;
        }
        // A first row with a word as size is a header, such as "Naam / Aantal". A size with a digit
        // in it, such as "0" or "ca. 28", is a group with a wrong size, which the grid then says.
        var isHeader = entries.Count == 0 && entry.People != "" && !entry.People.Any(char.IsDigit);
        if (!isHeader)
        {
          entries.Add(entry);
        }
      }
      return entries;
    }
  }
}
