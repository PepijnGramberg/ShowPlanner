using System;
using System.Collections.Generic;
using System.Drawing;
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
  public class CorrectionTests
  {
    private static readonly DateTime Monday = new DateTime(2026, 3, 16);
    private static readonly DateTime Tuesday = new DateTime(2026, 3, 17);

    [Fact]
    public void Moved_group_sits_in_the_other_showing_and_is_pinned()
    {
      var monday = Showing(Monday, 10, seats: 10);
      var tuesday = Showing(Tuesday, 10, seats: 10);
      var a = Wish("A", 4, Monday);
      var b = Wish("B", 3, Monday);
      var plan = Plan(monday, tuesday).With(a, b);
      plan.Distribute();

      var problem = plan.Move(b, tuesday);

      Assert.Null(problem);
      Assert.Equal(new[] { a }, monday.Groups);
      Assert.Equal(new[] { b }, tuesday.Groups);
      Assert.Same(tuesday, b.Voorstelling);
      Assert.True(b.Pinned);
      Assert.Equal("rij 1: 8-10", Describe(b));
      Assert.Equal(6, monday.Freeseats);
    }

    [Fact]
    public void Group_does_not_move_to_a_showing_without_room()
    {
      var monday = Showing(Monday, 10, seats: 10);
      var tuesday = Showing(Tuesday, 10, seats: 2);
      var a = Wish("A", 4, Monday);
      var plan = Plan(monday, tuesday).With(a);
      plan.Distribute();

      var problem = plan.Move(a, tuesday);

      Assert.Equal("De groep past niet: er zijn 2 stoelen vrij.", problem);
      Assert.Same(monday, a.Voorstelling);
      Assert.False(a.Pinned);
    }

    [Fact]
    public void Morning_group_can_be_moved_to_an_afternoon_showing_by_hand()
    {
      var afternoon = Showing(Monday, 14, seats: 10);
      var a = Wish("A", 4, Monday, morning: true);
      var plan = Plan(afternoon).With(a);
      Assert.Equal(new[] { a }, plan.Distribute());

      Assert.Null(plan.Move(a, afternoon));

      Assert.Empty(plan.Unplaced);
      Assert.Equal(new[] { a }, afternoon.Groups);
    }

    [Fact]
    public void Pinned_group_stays_in_its_showing_when_the_groups_are_distributed_again()
    {
      var monday = Showing(Monday, 10, seats: 10);
      var tuesday = Showing(Tuesday, 10, seats: 10);
      var a = Wish("A", 4, Monday);
      var b = Wish("B", 3, Monday);
      var plan = Plan(monday, tuesday).With(a, b);
      plan.Distribute();
      plan.Move(a, tuesday);

      plan.Distribute();

      Assert.Same(tuesday, a.Voorstelling);
      Assert.Same(monday, b.Voorstelling);
    }

    [Fact]
    public void Group_that_is_let_go_is_distributed_by_its_wishes_again()
    {
      var monday = Showing(Monday, 10, seats: 10);
      var tuesday = Showing(Tuesday, 10, seats: 10);
      var a = Wish("A", 4, Monday);
      var plan = Plan(monday, tuesday).With(a);
      plan.Distribute();
      plan.Move(a, tuesday);

      a.Pinned = false;
      plan.Distribute();

      Assert.Same(monday, a.Voorstelling);
    }

    [Fact]
    public void Pin_is_dropped_when_its_showing_is_removed()
    {
      var monday = Showing(Monday, 10, seats: 10);
      var tuesday = Showing(Tuesday, 10, seats: 10);
      var a = Wish("A", 4, Monday);
      var plan = Plan(monday, tuesday).With(a);
      plan.Distribute();
      plan.Move(a, tuesday);
      plan.Shows.Remove(tuesday);

      var unplaced = plan.Distribute();

      Assert.Empty(unplaced);
      Assert.Same(monday, a.Voorstelling);
      Assert.False(a.Pinned);
    }

    [Fact]
    public void Pinned_groups_are_seated_in_the_same_order_as_the_others()
    {
      var monday = Showing(Monday, 10, seats: 10);
      var tuesday = Showing(Tuesday, 10, seats: 10);
      var large = Wish("Groot", 5, Tuesday);
      var small = Wish("Klein", 3, Monday);
      var plan = Plan(monday, tuesday).With(large, small);
      plan.Distribute();

      plan.Move(small, tuesday);
      plan.Distribute();

      // The pinned group is placed first, but the larger group is still seated first.
      Assert.Equal(new[] { large, small }, tuesday.Groups);
    }

    [Fact]
    public void Changed_group_and_changed_showing_keep_the_pin()
    {
      var monday = Showing(Monday, 10, seats: 10);
      var tuesday = Showing(Tuesday, 10, seats: 10);
      var a = Wish("A", 4, Monday);
      var plan = Plan(monday, tuesday).With(a);
      plan.Distribute();
      plan.Move(a, tuesday);

      var renamed = Wish("A2", 5, Monday);
      plan.ReplaceGroup(a, renamed);
      var later = Showing(Tuesday, 11, seats: 10);
      plan.ReplaceShow(tuesday, later);
      plan.Distribute();

      Assert.True(renamed.Pinned);
      Assert.Same(later, renamed.Voorstelling);
    }

    [Fact]
    public void Pins_are_saved_with_the_plan()
    {
      var plan = new Plan();
      plan.Shows.Add(new Voorstelling(Monday.AddHours(10), Zaal.Load("Lampegiet"), Monday));
      plan.Shows.Add(new Voorstelling(Tuesday.AddHours(10), Zaal.Load("Lampegiet"), Tuesday));
      var a = Wish("A", 4, Monday);
      var b = Wish("B", 3, Monday);
      plan.With(a, b).Distribute();
      plan.Move(a, plan.Shows[1]);
      var path = Path.Combine(AppContext.BaseDirectory, Guid.NewGuid() + ".json");
      try
      {
        plan.Save(path);

        var loaded = global::Theaterverdeling.Plan.Load(path);
        loaded.Distribute();

        Assert.Equal(new[] { true, false }, loaded.Groups.Select(group => group.Pinned));
        Assert.Same(loaded.Shows[1], loaded.Groups[0].Voorstelling);
        Assert.Same(loaded.Shows[0], loaded.Groups[1].Voorstelling);
      }
      finally
      {
        File.Delete(path);
      }
    }

    [Fact]
    public void Preview_lists_the_groups_of_the_chosen_showing()
    {
      OnUiThread(() =>
      {
        var preview = Preview(out _);

        Assert.Equal(new[] { "Het Kompas (31)", "De Regenboog (28)" }, Items(preview, "Grouplist"));
        Assert.Equal("491 van de 550 stoelen vrij", Find<Label>(preview, "Freelabel").Text);

        Find<ListBox>(preview, "Showingslist").SelectedIndex = 1;

        Assert.Empty(Items(preview, "Grouplist"));
      });
    }

    [Fact]
    public void Clicking_a_seat_chooses_the_group_that_sits_there()
    {
      OnUiThread(() =>
      {
        var preview = Preview(out _);
        var map = Find<SeatMapControl>(preview, "Seatmap");

        // Het Kompas is the largest group, so it is seated first: from the right end of row 1.
        map.ClickAt(Centre(map.SeatBounds(0, 20)));

        Assert.Equal("Het Kompas (31)", Find<ListBox>(preview, "Grouplist").SelectedItem);
        Assert.Equal("Het Kompas", map.HighlightedGroup.Name);
        Assert.StartsWith("rij 1: 12-21", Find<Label>(preview, "Seatslabel").Text);
      });
    }

    [Fact]
    public void Clicking_next_to_the_seats_chooses_no_group()
    {
      OnUiThread(() =>
      {
        var preview = Preview(out _);
        var map = Find<SeatMapControl>(preview, "Seatmap");
        Find<ListBox>(preview, "Grouplist").SelectedIndex = 0;
        var firstSeat = map.SeatBounds(19, 0);

        map.ClickAt(new Point(firstSeat.Left - firstSeat.Width / 2, firstSeat.Top + firstSeat.Height / 2));

        Assert.Equal(-1, Find<ListBox>(preview, "Grouplist").SelectedIndex);
        Assert.Null(map.HighlightedGroup);
        Assert.False(Find<Button>(preview, "Movebutton").Enabled);
      });
    }

    [Fact]
    public void Seat_of_the_chosen_group_is_moved_by_hand()
    {
      OnUiThread(() =>
      {
        var preview = Preview(out _);
        var map = Find<SeatMapControl>(preview, "Seatmap");
        var groups = Find<ListBox>(preview, "Grouplist");
        // Het Kompas sits on seats 12-21 of row 1.
        groups.SelectedIndex = 0;
        var kompas = map.HighlightedGroup;

        map.ClickAt(Centre(map.SeatBounds(0, 11)));

        Assert.Equal("Het Kompas (30 van 31 geplaatst)", groups.Items[0]);
        Assert.Equal("Kies nog 1 stoel voor Het Kompas.", Find<Label>(preview, "Messagelabel").Text);
        Assert.StartsWith("rij 1: 13-21", Find<Label>(preview, "Seatslabel").Text);

        // The last seat of the hall is free.
        map.ClickAt(Centre(map.SeatBounds(19, 33)));

        Assert.Equal("Het Kompas (31)", groups.Items[0]);
        Assert.Equal(0, groups.SelectedIndex);
        Assert.Equal("Het Kompas heeft weer 31 stoelen.", Find<Label>(preview, "Messagelabel").Text);
        Assert.EndsWith("rij 19: 34", Find<Label>(preview, "Seatslabel").Text);
        Assert.Same(kompas, ((ChairRow)map.Hall.Rows[19]).Seats[33].Group);
        Assert.Null(((ChairRow)map.Hall.Rows[0]).Seats[11].Group);
      });
    }

    [Fact]
    public void Group_with_all_its_seats_gets_no_extra_seat()
    {
      OnUiThread(() =>
      {
        var preview = Preview(out _);
        var map = Find<SeatMapControl>(preview, "Seatmap");
        Find<ListBox>(preview, "Grouplist").SelectedIndex = 0;

        map.ClickAt(Centre(map.SeatBounds(19, 33)));

        Assert.Equal(
          "Het Kompas heeft al 31 stoelen. Klik eerst op een stoel van de groep om die vrij te maken.",
          Find<Label>(preview, "Messagelabel").Text);
        Assert.Null(((ChairRow)map.Hall.Rows[19]).Seats[33].Group);
      });
    }

    [Fact]
    public void Seat_that_is_not_used_cannot_be_given_to_a_group()
    {
      var hall = Hall(Row(1, 4, true, 1), SectionRow(2, 4, "Balkon"));
      hall.SwitchOff("Balkon");
      var a = Group("A", 2);
      Seat(hall, a);
      var first = (ChairRow)hall.Rows[0];
      Assert.Null(hall.ChangeSeat(first, 3, a));

      Assert.Equal("Deze stoel wordt niet gebruikt.", hall.ChangeSeat(first, 0, a));
      Assert.Equal("Deze stoel wordt niet gebruikt.", hall.ChangeSeat((ChairRow)hall.Rows[1], 0, a));
      Assert.Equal(1, hall.SeatsOf(a));
    }

    [Fact]
    public void Seats_changed_by_hand_are_in_the_pdf_legend()
    {
      var hall = Hall(Row(1, 6));
      var a = Group("A", 2);
      Seat(hall, a);
      var row = (ChairRow)hall.Rows[0];

      hall.ChangeSeat(row, 5, a);
      hall.ChangeSeat(row, 0, a);

      Assert.Equal("rij 1: 1-1, rij 1: 5-5", Describe(a));
    }

    [Fact]
    public void Group_is_moved_to_the_showing_that_is_chosen_and_the_preview_follows_it()
    {
      OnUiThread(() =>
      {
        var preview = Preview(out _);
        Find<ListBox>(preview, "Grouplist").SelectedIndex = 1;
        Assert.Equal(new[] { "17-03-2026 10:00 (550 vrij)" }, Find<ComboBox>(preview, "Movechoice").Items.Cast<string>());

        Find<Button>(preview, "Movebutton").PerformClick();

        Assert.Equal(1, Find<ListBox>(preview, "Showingslist").SelectedIndex);
        Assert.Equal(new[] { "De Regenboog (28) - vast" }, Items(preview, "Grouplist"));
        Assert.Equal(0, Find<ListBox>(preview, "Grouplist").SelectedIndex);
        Assert.Equal("Losmaken", Find<Button>(preview, "Pinbutton").Text);
        Assert.Equal("522 van de 550 stoelen vrij", Find<Label>(preview, "Freelabel").Text);
      });
    }

    [Fact]
    public void Pinned_group_stays_where_it_is_when_the_main_window_distributes_again()
    {
      OnUiThread(() =>
      {
        var preview = Preview(out var main);
        Find<ListBox>(preview, "Grouplist").SelectedIndex = 1;
        Find<Button>(preview, "Movebutton").PerformClick();

        Find<Button>(main, "Dividebutton").PerformClick();

        var again = Application.OpenForms.OfType<PreviewForm>().Single();
        Assert.Equal(new[] { "Het Kompas (31)" }, Items(again, "Grouplist"));
        Find<ListBox>(again, "Showingslist").SelectedIndex = 1;
        Assert.Equal(new[] { "De Regenboog (28) - vast" }, Items(again, "Grouplist"));
      });
    }

    [Fact]
    public void Pin_button_pins_a_group_and_lets_it_go()
    {
      OnUiThread(() =>
      {
        var preview = Preview(out _);
        Find<ListBox>(preview, "Grouplist").SelectedIndex = 0;

        Find<Button>(preview, "Pinbutton").PerformClick();
        Assert.Equal(new[] { "Het Kompas (31) - vast", "De Regenboog (28)" }, Items(preview, "Grouplist"));

        Find<Button>(preview, "Pinbutton").PerformClick();
        Assert.Equal(new[] { "Het Kompas (31)", "De Regenboog (28)" }, Items(preview, "Grouplist"));
      });
    }

    [Fact]
    public void Groups_without_a_showing_are_listed_and_can_be_placed_by_hand()
    {
      OnUiThread(() =>
      {
        // A morning group with only an afternoon showing fits in no showing by its wishes.
        var plan = new Plan();
        plan.Shows.Add(new Voorstelling(Monday.AddHours(14), Zaal.Load("Lampegiet"), Monday));
        plan.Groups.Add(new Group("De Vroege Vogel", 20, false, true, new List<DateTime> { Monday }));
        plan.Distribute();
        var preview = new PreviewForm(plan);
        preview.Show();

        Assert.Equal(new[] { "16-03-2026 14:00", "Niet ingedeeld (1)" }, Items(preview, "Showingslist"));
        Find<ListBox>(preview, "Showingslist").SelectedIndex = 1;
        Find<ListBox>(preview, "Grouplist").SelectedIndex = 0;
        Assert.False(Find<Button>(preview, "Pinbutton").Enabled);
        Find<Button>(preview, "Movebutton").PerformClick();

        Assert.Equal(new[] { "16-03-2026 14:00" }, Items(preview, "Showingslist"));
        Assert.Equal(new[] { "De Vroege Vogel (20) - vast" }, Items(preview, "Grouplist"));
      });
    }

    // Two showings in the Lampegiet and two groups that both prefer the first one.
    private static PreviewForm Preview(out Theaterverdeling main)
    {
      main = new Theaterverdeling();
      main.Show();
      AddShowing(main, Monday, Monday.AddHours(10));
      AddShowing(main, Tuesday, Tuesday.AddHours(10));
      AddGroupOnFirstDate(main, "De Regenboog", 28);
      AddGroupOnFirstDate(main, "Het Kompas", 31);
      Find<Button>(main, "Dividebutton").PerformClick();
      return Application.OpenForms.OfType<PreviewForm>().Single();
    }

    private static void AddGroupOnFirstDate(Form main, string name, int people)
    {
      Find<Button>(main, "Addgroupbutton").PerformClick();
      var dialog = Application.OpenForms.OfType<Addgroupform>().Single();
      Find<TextBox>(dialog, "Groepnaam").Text = name;
      Find<NumericUpDown>(dialog, "Groepsgrootte").Value = people;
      Find<CheckedListBox>(dialog, "Datumvoorkeurenbox").SetItemChecked(0, true);
      Find<Button>(dialog, "Addgroupbutton").PerformClick();
    }

    private static string[] Items(Form form, string list)
    {
      return Find<ListBox>(form, list).Items.Cast<string>().ToArray();
    }

    private static Point Centre(Rectangle rect)
    {
      return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
    }

    private static Voorstelling Showing(DateTime date, int hour, int seats)
    {
      return new Voorstelling(DateTime.Today.AddHours(hour), Hall(Row(1, seats)), date);
    }

    private static Group Wish(string name, int people, DateTime date, bool morning = false)
    {
      return new Group(name, people, false, morning, new List<DateTime> { date });
    }

    private static Plan Plan(params Voorstelling[] shows)
    {
      var plan = new Plan();
      plan.Shows.AddRange(shows);
      return plan;
    }
  }
}
