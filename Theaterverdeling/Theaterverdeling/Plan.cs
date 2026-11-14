using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Theaterverdeling
{
  // The plan the coordinator works on: one show, its showings and the groups that come to see it.
  // It is saved as one JSON file. The outcome of a distribution is not saved; distribute again after loading.
  public class Plan
  {
    private const int FileVersion = 1;
    private const string DateFormat = "yyyy-MM-dd";
    private const string TimeFormat = "HH:mm:ss";
    private const string MomentFormat = "yyyy-MM-dd HH:mm";

    public string ShowName { get; set; } = "";
    public List<Voorstelling> Shows { get; } = new List<Voorstelling>();
    public List<Group> Groups { get; } = new List<Group>();

    // How a showing is named in lists. The hall is part of the name once the plan uses more than one hall.
    public string Describe(Voorstelling show)
    {
      var severalHalls = Shows.Select(other => other.Zaal.Name).Distinct().Count() > 1;
      return severalHalls ? $"{show.Description} {show.Zaal.Name}" : show.Description;
    }

    // Divides the groups over the showings and gives them their seats.
    // Returns the groups that fit in no showing.
    public List<Group> Distribute()
    {
      foreach (var show in Shows)
      {
        show.Groups = new List<Group>();
      }

      var unplaced = new List<Group>();
      // A pinned group keeps its showing, as long as that showing is still there and has room.
      foreach (var group in InSeatingOrder(Groups.Where(group => group.Pinned)).ToList())
      {
        if (Shows.Contains(group.Voorstelling) && group.Voorstelling.Freeseats >= group.Personen)
        {
          group.Voorstelling.Groups.Add(group);
        }
        else
        {
          group.Pinned = false;
        }
      }

      // Front-row groups choose first, then morning groups, then the larger groups.
      foreach (var group in InSeatingOrder(Groups.Where(group => !group.Pinned)).ToList())
      {
        // The showing with the most free seats comes first. The earlier showing wins a tie.
        var possible = Shows
          .Where(show => Fits(group, show))
          .OrderByDescending(show => show.Freeseats)
          .ThenBy(show => show.Start)
          .ToList();
        var chosen = possible.FirstOrDefault(show => group.Datum.Contains(show.Datum)) ?? possible.FirstOrDefault();

        group.Voorstelling = chosen;
        if (chosen == null)
        {
          group.Seats = new List<SeatRange>();
          unplaced.Add(group);
        }
        else
        {
          chosen.Groups.Add(group);
        }
      }

      foreach (var show in Shows)
      {
        // Pinned groups were placed first, but they sit in the same order as the others.
        show.Groups = InSeatingOrder(show.Groups).ToList();
        show.Assignseats();
      }
      Shows.Sort((x, y) => x.Start.CompareTo(y.Start));
      return unplaced;
    }

    // The groups that are in no showing after the last distribution.
    public List<Group> Unplaced => Groups.Where(group => !Shows.Any(show => show.Groups.Contains(group))).ToList();

    // Moves a group to another showing by hand and pins it there, also when its wishes say otherwise.
    // Both showings are seated again. Says why the group cannot move, or null when it did.
    public string Move(Group group, Voorstelling target)
    {
      if (target.Groups.Contains(group))
      {
        return "De groep zit al in die voorstelling.";
      }
      if (target.Freeseats < group.Personen)
      {
        return $"De groep past niet: er zijn {target.Freeseats} stoelen vrij.";
      }
      foreach (var show in Shows)
      {
        if (show.Groups.Remove(group))
        {
          show.Assignseats();
        }
      }
      target.Groups = InSeatingOrder(target.Groups.Append(group)).ToList();
      group.Voorstelling = target;
      group.Pinned = true;
      target.Assignseats();
      return null;
    }

    // The order in which groups choose a showing and are seated: front-row groups first, then
    // morning groups, then the larger groups. Groups that are equal keep the order of the plan.
    private IEnumerable<Group> InSeatingOrder(IEnumerable<Group> groups)
    {
      return groups
        .OrderByDescending(group => group.Firstrows)
        .ThenByDescending(group => group.Morning)
        .ThenByDescending(group => group.Personen)
        .ThenBy(group => Groups.IndexOf(group));
    }

    private static bool Fits(Group group, Voorstelling show)
    {
      var timeSuits = !group.Morning || show.Time.Hour < 12;
      return timeSuits && show.Freeseats >= group.Personen;
    }

    // Says why a group with these details cannot be in the plan, or null when it can.
    // Pass the group that is being edited as replacing, so it is not compared with itself.
    public string CheckGroup(string name, int people, Group replacing = null)
    {
      var trimmed = (name ?? "").Trim();
      if (trimmed == "")
      {
        return "Vul een groepsnaam in.";
      }
      if (Groups.Any(group => group != replacing && string.Equals(group.Name.Trim(), trimmed, StringComparison.CurrentCultureIgnoreCase)))
      {
        return $"Er is al een groep met de naam \"{trimmed}\".";
      }
      var largest = Shows.Count == 0 ? 0 : Shows.Max(show => show.Zaal.Seats);
      if (people > largest)
      {
        return $"De groep past in geen enkele voorstelling. De grootste heeft {largest} stoelen.";
      }
      return null;
    }

    // Says why a showing at this date and time cannot be in the plan, or null when it can.
    // Pass the showing that is being edited as replacing, so it is not compared with itself.
    public string CheckShowing(DateTime date, DateTime time, Voorstelling replacing = null)
    {
      var start = ToMinute(date.Date + time.TimeOfDay);
      if (Shows.Any(show => show != replacing && ToMinute(show.Start) == start))
      {
        return "Er is al een voorstelling op dat moment.";
      }
      return null;
    }

    // Checks rows that are to be added together. Gives for every row why it cannot be added,
    // or null when it can. A name that is twice in the rows is refused the second time.
    public List<string> CheckNewGroups(IReadOnlyList<GroupEntry> entries)
    {
      var messages = new List<string>();
      var accepted = new List<string>();
      foreach (var entry in entries)
      {
        var name = (entry.Name ?? "").Trim();
        string message;
        if (name == "")
        {
          message = "Vul een groepsnaam in.";
        }
        else if (!entry.TryGetPeople(out var people))
        {
          message = "Vul bij aantal een heel getal in.";
        }
        else
        {
          message = CheckGroup(name, people);
          if (message == null && accepted.Any(other => string.Equals(other, name, StringComparison.CurrentCultureIgnoreCase)))
          {
            message = $"Er is al een groep met de naam \"{name}\".";
          }
        }

        if (message == null)
        {
          accepted.Add(name);
        }
        messages.Add(message);
      }
      return messages;
    }

    public void ReplaceGroup(Group old, Group replacement)
    {
      // The dialogs stay open next to the main window, so the old group can be removed meanwhile.
      var index = Groups.IndexOf(old);
      if (index < 0)
      {
        Groups.Add(replacement);
        return;
      }
      // The changed group stays pinned to its showing.
      replacement.Voorstelling = old.Voorstelling;
      replacement.Pinned = old.Pinned;
      Groups[index] = replacement;
    }

    // Puts a changed showing in the place of the old one. When the showing moves to another date
    // and no showing is left on the old date, the groups that preferred the old date follow.
    public void ReplaceShow(Voorstelling old, Voorstelling replacement)
    {
      var index = Shows.IndexOf(old);
      if (index < 0)
      {
        Shows.Add(replacement);
        return;
      }
      Shows[index] = replacement;
      // Groups that are pinned to the showing stay pinned to it.
      foreach (var group in Groups.Where(group => group.Voorstelling == old))
      {
        group.Voorstelling = replacement;
      }
      if (old.Datum == replacement.Datum || Shows.Any(show => show.Datum == old.Datum))
      {
        return;
      }
      foreach (var group in Groups)
      {
        if (group.Datum.Remove(old.Datum) && !group.Datum.Contains(replacement.Datum))
        {
          group.Datum.Add(replacement.Datum);
        }
      }
    }

    private static DateTime ToMinute(DateTime moment)
    {
      return moment.AddTicks(-(moment.Ticks % TimeSpan.TicksPerMinute));
    }

    public void Save(string path)
    {
      var file = new PlanFile
      {
        Version = FileVersion,
        ShowName = ShowName,
        Showings = Shows.Select(show => new ShowingFile
        {
          Date = show.Datum.ToString(DateFormat, CultureInfo.InvariantCulture),
          Time = show.Time.ToString(TimeFormat, CultureInfo.InvariantCulture),
          Hall = show.Zaal.Name,
          SwitchedOff = show.Zaal.SwitchedOffSections.ToList()
        }).ToList(),
        Groups = Groups.Select(group => new GroupFile
        {
          Name = group.Name,
          People = group.Personen,
          FrontRows = group.Firstrows,
          Morning = group.Morning,
          PreferredDates = group.Datum.Select(date => date.ToString(DateFormat, CultureInfo.InvariantCulture)).ToList(),
          PinnedTo = group.Pinned && group.Voorstelling != null ? group.Voorstelling.Start.ToString(MomentFormat, CultureInfo.InvariantCulture) : null
        }).ToList()
      };
      File.WriteAllText(path, JsonSerializer.Serialize(file, new JsonSerializerOptions { WriteIndented = true }));
    }

    public static Plan Load(string path)
    {
      var file = JsonSerializer.Deserialize<PlanFile>(File.ReadAllText(path));
      if (file == null || file.Version != FileVersion)
      {
        throw new InvalidDataException($"Dit is geen planbestand van versie {FileVersion}.");
      }

      var plan = new Plan { ShowName = file.ShowName ?? "" };
      foreach (var showing in file.Showings ?? new List<ShowingFile>())
      {
        var hall = Zaal.Load(showing.Hall);
        foreach (var section in showing.SwitchedOff ?? new List<string>())
        {
          hall.SwitchOff(section);
        }
        var date = ParseDate(showing.Date);
        var time = date + DateTime.ParseExact(showing.Time, TimeFormat, CultureInfo.InvariantCulture).TimeOfDay;
        plan.Shows.Add(new Voorstelling(time, hall, date));
      }
      foreach (var group in file.Groups ?? new List<GroupFile>())
      {
        var dates = (group.PreferredDates ?? new List<string>()).Select(ParseDate).ToList();
        var loaded = new Group(group.Name, group.People, group.FrontRows, group.Morning, dates);
        // A pin on a showing that is not in the file is dropped.
        loaded.Voorstelling = group.PinnedTo == null ? null : plan.Shows.FirstOrDefault(show => show.Start.ToString(MomentFormat, CultureInfo.InvariantCulture) == group.PinnedTo);
        loaded.Pinned = loaded.Voorstelling != null;
        plan.Groups.Add(loaded);
      }
      return plan;
    }

    private static DateTime ParseDate(string date)
    {
      return DateTime.ParseExact(date, DateFormat, CultureInfo.InvariantCulture);
    }

    // The shape of the JSON file.
    private sealed class PlanFile
    {
      public int Version { get; set; }
      public string ShowName { get; set; }
      public List<ShowingFile> Showings { get; set; }
      public List<GroupFile> Groups { get; set; }
    }

    private sealed class ShowingFile
    {
      public string Date { get; set; }
      public string Time { get; set; }
      public string Hall { get; set; }
      public List<string> SwitchedOff { get; set; }
    }

    private sealed class GroupFile
    {
      public string Name { get; set; }
      public int People { get; set; }
      public bool FrontRows { get; set; }
      public bool Morning { get; set; }
      public List<string> PreferredDates { get; set; }

      // Start of the showing the group is pinned to, or null when it is not pinned.
      [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
      public string PinnedTo { get; set; }
    }
  }
}
