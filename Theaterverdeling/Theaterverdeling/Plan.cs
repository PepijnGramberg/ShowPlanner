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

    public string ShowName { get; set; } = "";
    public List<Voorstelling> Shows { get; } = new List<Voorstelling>();
    public List<Group> Groups { get; } = new List<Group>();

    // Divides the groups over the showings and gives them their seats.
    // Returns the groups that fit in no showing.
    public List<Group> Distribute()
    {
      foreach (var show in Shows)
      {
        show.Groups = new List<Group>();
      }

      var unplaced = new List<Group>();
      // Front-row groups choose first, then morning groups, then the larger groups.
      var inOrder = Groups
        .OrderByDescending(group => group.Firstrows)
        .ThenByDescending(group => group.Morning)
        .ThenByDescending(group => group.Personen);
      foreach (var group in inOrder)
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
        show.Assignseats();
      }
      Shows.Sort((x, y) => x.Start.CompareTo(y.Start));
      return unplaced;
    }

    private static bool Fits(Group group, Voorstelling show)
    {
      var timeSuits = !group.Morning || show.Time.Hour < 12;
      return timeSuits && show.Freeseats >= group.Personen;
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
          PreferredDates = group.Datum.Select(date => date.ToString(DateFormat, CultureInfo.InvariantCulture)).ToList()
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
        plan.Groups.Add(new Group(group.Name, group.People, group.FrontRows, group.Morning, dates));
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
    }
  }
}
