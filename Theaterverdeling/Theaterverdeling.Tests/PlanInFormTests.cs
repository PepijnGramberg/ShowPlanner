using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Xunit;
using static Theaterverdeling.Tests.FormDriver;

namespace Theaterverdeling.Tests
{
  // Form tests share the list of open windows, so they must not run side by side.
  [Collection("Forms")]
  public class PlanInFormTests
  {
    private static readonly DateTime Day = new DateTime(2026, 3, 14);
    private static readonly DateTime HalfPastTen = new DateTime(2026, 3, 14, 10, 30, 0);

    [Fact]
    public void Loading_a_plan_shows_its_show_name_showings_and_groups()
    {
      OnUiThread(() =>
      {
        var path = SavedPlan();
        using var main = new Theaterverdeling();
        main.Show();

        main.LoadPlan(path);

        Assert.Equal("Kikker is jarig", Find<TextBox>(main, "voorstellingnaambox").Text);
        Assert.Equal(new[] { "14-03-2026 10:30 (Balkon uit)" }, Texts(main, "Voorstellingviewer"));
        Assert.Equal(new[] { "De Regenboog 28" }, Texts(main, "Groupviewer"));
        Assert.Equal("28/363 stoelen", Find<Label>(main, "Seatlabel").Text);
      });
    }

    [Fact]
    public void Loading_a_plan_replaces_what_was_on_screen()
    {
      OnUiThread(() =>
      {
        var path = SavedPlan();
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, new DateTime(2026, 5, 1), new DateTime(2026, 5, 1, 9, 0, 0));
        AddGroup(main, "Oude groep", 12);

        main.LoadPlan(path);
        main.LoadPlan(path);

        Assert.Equal(new[] { "14-03-2026 10:30 (Balkon uit)" }, Texts(main, "Voorstellingviewer"));
        Assert.Equal(new[] { "De Regenboog 28" }, Texts(main, "Groupviewer"));
      });
    }

    [Fact]
    public void Saving_writes_the_plan_that_is_on_screen()
    {
      OnUiThread(() =>
      {
        var path = NewPlanPath();
        using var main = new Theaterverdeling();
        main.Show();
        Find<TextBox>(main, "voorstellingnaambox").Text = "Kikker is jarig";
        AddShowing(main, Day, HalfPastTen, "Balkon");
        AddGroup(main, "De Regenboog", 28);

        main.SavePlan(path);

        var saved = Plan.Load(path);
        Assert.Equal("Kikker is jarig", saved.ShowName);
        Assert.Equal(HalfPastTen, saved.Shows.Single().Datum + saved.Shows.Single().Time.TimeOfDay);
        Assert.Equal(new[] { "Balkon" }, saved.Shows.Single().Zaal.SwitchedOffSections);
        Assert.Equal("De Regenboog", saved.Groups.Single().Name);
        Assert.Equal(28, saved.Groups.Single().Personen);
        Assert.Equal(new[] { Day }, saved.Groups.Single().Datum);
      });
    }

    [Fact]
    public void File_that_cannot_be_loaded_leaves_the_plan_on_screen_untouched()
    {
      OnUiThread(() =>
      {
        var path = NewPlanPath();
        File.WriteAllText(path, "dit is geen plan");
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Day, HalfPastTen);
        AddGroup(main, "De Regenboog", 28);

        Assert.ThrowsAny<Exception>(() => main.LoadPlan(path));

        Assert.Equal(new[] { "14-03-2026 10:30" }, Texts(main, "Voorstellingviewer"));
        Assert.Equal(new[] { "De Regenboog 28" }, Texts(main, "Groupviewer"));
      });
    }

    private static string SavedPlan()
    {
      var hall = Zaal.Load("Lampegiet");
      hall.SwitchOff("Balkon");
      var plan = new Plan { ShowName = "Kikker is jarig" };
      plan.Shows.Add(new Voorstelling(HalfPastTen, hall, Day));
      plan.Groups.Add(new Group("De Regenboog", 28, false, false, new List<DateTime> { Day }));
      var path = NewPlanPath();
      plan.Save(path);
      return path;
    }

    private static List<string> Texts(Control main, string listName)
    {
      return Find<ListView>(main, listName).Items.Cast<ListViewItem>().Select(item => item.Text).ToList();
    }

    private static string NewPlanPath()
    {
      var folder = Path.Combine(AppContext.BaseDirectory, "testplans");
      Directory.CreateDirectory(folder);
      return Path.Combine(folder, Guid.NewGuid() + ".json");
    }
  }
}
