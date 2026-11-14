using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Xunit;
using static Theaterverdeling.Tests.FormDriver;
using static Theaterverdeling.Tests.TestHalls;

namespace Theaterverdeling.Tests
{
  // Form tests share the list of open windows, so they must not run side by side.
  [Collection("Forms")]
  public class HallChoiceTests
  {
    private static readonly DateTime Saturday = new DateTime(2026, 3, 14);
    private static readonly DateTime Sunday = new DateTime(2026, 3, 15);

    [Fact]
    public void Dialog_offers_the_halls_in_the_halls_folder()
    {
      WithSecondHall(name => OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();

        var dialog = OpenDialog(main);

        var halls = Find<ComboBox>(dialog, "Hallchoice");
        Assert.Contains("Lampegiet", halls.Items.Cast<string>());
        Assert.Contains(name, halls.Items.Cast<string>());
      }));
    }

    [Fact]
    public void Choosing_a_hall_lists_its_sections()
    {
      WithSecondHall(name => OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        var dialog = OpenDialog(main);

        Find<ComboBox>(dialog, "Hallchoice").SelectedItem = name;

        Assert.Equal(new[] { "Achter" }, Find<CheckedListBox>(dialog, "Sectionsoff").Items.Cast<string>());
      }));
    }

    [Fact]
    public void Showing_is_added_in_the_chosen_hall_and_the_lists_name_the_hall()
    {
      WithSecondHall(name => OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        AddShowing(main, Saturday, Saturday.AddHours(10));
        var dialog = OpenDialog(main);
        Find<DateTimePicker>(dialog, "Datumkeuze").Value = Sunday;
        Find<DateTimePicker>(dialog, "Time").Value = Sunday.AddHours(10);
        Find<ComboBox>(dialog, "Hallchoice").SelectedItem = name;

        Find<Button>(dialog, "Addshow").PerformClick();

        Assert.Equal("0/560 stoelen", Find<Label>(main, "Seatlabel").Text);
        Assert.Equal(
          new[] { "14-03-2026 10:00 Lampegiet", "15-03-2026 10:00 " + name },
          Find<ListView>(main, "Voorstellingviewer").Items.Cast<ListViewItem>().Select(item => item.Text));
      }));
    }

    [Fact]
    public void New_showing_starts_in_the_hall_of_the_last_showing()
    {
      WithSecondHall(name => OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        var first = OpenDialog(main);
        Find<ComboBox>(first, "Hallchoice").SelectedItem = name;
        Find<Button>(first, "Addshow").PerformClick();

        var second = OpenDialog(main);

        Assert.Equal(name, Find<ComboBox>(second, "Hallchoice").SelectedItem);
      }));
    }

    [Fact]
    public void Plan_with_two_halls_is_saved_and_loaded_with_both()
    {
      WithSecondHall(name =>
      {
        var plan = new Plan();
        plan.Shows.Add(new Voorstelling(Saturday.AddHours(10), Zaal.Load("Lampegiet"), Saturday));
        plan.Shows.Add(new Voorstelling(Sunday.AddHours(10), Zaal.Load(name), Sunday));
        var path = Path.Combine(AppContext.BaseDirectory, Guid.NewGuid() + ".json");
        try
        {
          plan.Save(path);

          var loaded = Plan.Load(path);

          Assert.Equal(new[] { "Lampegiet", name }, loaded.Shows.Select(show => show.Zaal.Name));
          Assert.Equal(10, loaded.Shows[1].Zaal.Seats);
        }
        finally
        {
          File.Delete(path);
        }
      });
    }

    private static Addshowform OpenDialog(Form main)
    {
      Find<Button>(main, "Addshow").PerformClick();
      return Application.OpenForms.OfType<Addshowform>().Last();
    }

    // Runs a test with a small second hall in the halls folder: one free row and one row in section "Achter".
    private static void WithSecondHall(Action<string> test)
    {
      var name = "Testzaal " + Guid.NewGuid().ToString("N");
      Hall(Row(1, 6), SectionRow(2, 4, "Achter")).Save(name);
      try
      {
        test(name);
      }
      finally
      {
        File.Delete(Path.Combine(Zaal.HallsFolder, name + ".json"));
      }
    }
  }
}
