using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Xunit;

namespace Theaterverdeling.Tests
{
  public class PlanFileTests
  {
    [Fact]
    public void Saved_plan_loads_back_with_its_show_name()
    {
      var plan = new Plan { ShowName = "Kikker is jarig" };

      var loaded = SaveAndLoad(plan);

      Assert.Equal("Kikker is jarig", loaded.ShowName);
    }

    [Fact]
    public void Saved_plan_loads_back_with_the_date_and_time_of_each_showing()
    {
      var plan = new Plan();
      plan.Shows.Add(Showing(new DateTime(2026, 3, 14), 10, 30));
      plan.Shows.Add(Showing(new DateTime(2026, 3, 15), 13, 0));

      var loaded = SaveAndLoad(plan);

      Assert.Equal(
        new[] { new DateTime(2026, 3, 14, 10, 30, 0), new DateTime(2026, 3, 15, 13, 0, 0) },
        loaded.Shows.Select(show => show.Datum + show.Time.TimeOfDay));
    }

    [Fact]
    public void Saved_plan_loads_back_with_the_hall_of_each_showing()
    {
      var plan = new Plan();
      plan.Shows.Add(Showing(new DateTime(2026, 3, 14), 10, 30));

      var loaded = SaveAndLoad(plan);

      Assert.Equal("Lampegiet", loaded.Shows.Single().Zaal.Name);
      Assert.Equal(550, loaded.Shows.Single().Zaal.Seats);
    }

    [Fact]
    public void Saved_plan_loads_back_with_the_switched_off_sections_of_each_showing()
    {
      var plan = new Plan();
      plan.Shows.Add(Showing(new DateTime(2026, 3, 14), 10, 30, "Balkon"));

      var loaded = SaveAndLoad(plan);

      Assert.Equal(new[] { "Balkon" }, loaded.Shows.Single().Zaal.SwitchedOffSections);
    }

    [Fact]
    public void Saved_plan_loads_back_with_its_groups_and_their_wishes()
    {
      var plan = new Plan();
      plan.Groups.Add(new Group("De Regenboog", 28, true, false, new List<DateTime> { new DateTime(2026, 3, 14), new DateTime(2026, 3, 15) }));
      plan.Groups.Add(new Group("Het Kompas", 31, false, true, new List<DateTime>()));

      var loaded = SaveAndLoad(plan);

      Assert.Equal(new[] { "De Regenboog", "Het Kompas" }, loaded.Groups.Select(group => group.Name));
      Assert.Equal(new[] { 28, 31 }, loaded.Groups.Select(group => group.Personen));
      Assert.Equal(new[] { true, false }, loaded.Groups.Select(group => group.Firstrows));
      Assert.Equal(new[] { false, true }, loaded.Groups.Select(group => group.Morning));
      Assert.Equal(new[] { new DateTime(2026, 3, 14), new DateTime(2026, 3, 15) }, loaded.Groups[0].Datum);
      Assert.Empty(loaded.Groups[1].Datum);
    }

    [Fact]
    public void Plan_saved_with_Dutch_settings_loads_with_American_settings()
    {
      var plan = new Plan();
      plan.Shows.Add(Showing(new DateTime(2026, 3, 14), 14, 30));
      var path = NewPlanPath();

      CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("nl-NL");
      plan.Save(path);
      CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
      var loaded = Plan.Load(path);

      var show = loaded.Shows.Single();
      Assert.Equal(new DateTime(2026, 3, 14, 14, 30, 0), show.Datum + show.Time.TimeOfDay);
    }

    [Fact]
    public void File_of_another_version_is_refused()
    {
      var path = NewPlanPath();
      File.WriteAllText(path, "{ \"Version\": 2, \"ShowName\": \"Later\" }");

      Assert.Throws<InvalidDataException>(() => Plan.Load(path));
    }

    private static Plan SaveAndLoad(Plan plan)
    {
      var path = NewPlanPath();
      plan.Save(path);
      return Plan.Load(path);
    }

    private static Voorstelling Showing(DateTime date, int hour, int minute, params string[] switchedOff)
    {
      var hall = Zaal.Load("Lampegiet");
      foreach (var section in switchedOff)
      {
        hall.SwitchOff(section);
      }
      return new Voorstelling(DateTime.Today.AddHours(hour).AddMinutes(minute), hall, date);
    }

    private static string NewPlanPath()
    {
      var folder = Path.Combine(AppContext.BaseDirectory, "testplans");
      Directory.CreateDirectory(folder);
      return Path.Combine(folder, Guid.NewGuid() + ".json");
    }
  }
}
