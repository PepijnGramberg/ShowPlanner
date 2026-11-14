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

    private const string GoodShowing = "{\"Date\":\"2026-03-16\",\"Time\":\"10:00:00\",\"Hall\":\"Lampegiet\"}";

    [Theory]
    [InlineData("{\"Date\":\"2026-03-16\",\"Time\":\"10:00:00\"}", "", "een voorstelling heeft geen zaal")]
    [InlineData("{\"Date\":\"2026-03-16\",\"Hall\":\"Lampegiet\"}", "", "een voorstelling heeft geen geldige datum of tijd")]
    [InlineData(GoodShowing + "," + GoodShowing, "", "er zijn twee voorstellingen op 2026-03-16 10:00")]
    [InlineData(GoodShowing, "{\"People\":28}", "een groep heeft geen naam")]
    [InlineData(GoodShowing, "{\"Name\":\"De Regenboog\",\"People\":0}", "de groep \"De Regenboog\" heeft geen geldig aantal personen")]
    [InlineData(GoodShowing, "{\"Name\":\"De Regenboog\",\"People\":28},{\"Name\":\"de regenboog\",\"People\":20}", "de groep \"de regenboog\" staat er twee keer in")]
    [InlineData(GoodShowing, "{\"Name\":\"De Regenboog\",\"People\":28,\"PreferredDates\":[\"maandag\"]}", "de groep \"De Regenboog\" heeft een ongeldige voorkeursdatum")]
    public void File_with_what_the_dialogs_refuse_is_refused_with_the_reason(string showings, string groups, string reason)
    {
      var path = NewPlanPath();
      File.WriteAllText(path, "{\"Version\":1,\"Showings\":[" + showings + "],\"Groups\":[" + groups + "]}");

      var error = Assert.Throws<InvalidDataException>(() => Plan.Load(path));

      Assert.Equal($"Het planbestand klopt niet: {reason}.", error.Message);
    }

    [Fact]
    public void File_that_is_no_plan_is_refused_in_Dutch()
    {
      var path = NewPlanPath();
      File.WriteAllText(path, "dit is geen json");

      var error = Assert.Throws<InvalidDataException>(() => Plan.Load(path));

      Assert.Equal("Dit is geen planbestand: het kan niet worden gelezen.", error.Message);
    }

    [Fact]
    public void Plan_with_a_hall_that_is_not_there_says_which_hall_is_missing()
    {
      var path = NewPlanPath();
      File.WriteAllText(path, "{\"Version\":1,\"Showings\":[{\"Date\":\"2026-03-16\",\"Time\":\"10:00:00\",\"Hall\":\"Schouwburg\"}]}");

      var error = Assert.Throws<FileNotFoundException>(() => Plan.Load(path));

      Assert.Equal("De zaal \"Schouwburg\" staat niet in de map Zalen naast het programma. Zet het zaalbestand in die map, of maak de zaal met \"Zalen beheren\".", error.Message);
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
