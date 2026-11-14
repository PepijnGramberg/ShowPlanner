using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Xunit;
using static Theaterverdeling.Tests.FormDriver;
using static Theaterverdeling.Tests.TestHalls;

namespace Theaterverdeling.Tests
{
  // Form tests share the list of open windows, so they must not run side by side.
  [Collection("Forms")]
  public class DistributionMessageTests
  {
    private static readonly DateTime Monday = new DateTime(2026, 3, 16);

    [Fact]
    public void Too_few_seats_is_said_with_the_numbers()
    {
      var plan = new Plan();
      plan.Shows.Add(Showing(10, seats: 10));
      plan.Groups.Add(Group("A", 8));
      plan.Groups.Add(Group("B", 5));

      Assert.Equal(
        "Er zijn te weinig stoelen: 13 personen en 10 stoelen. Voeg een voorstelling toe of maak een groep kleiner.",
        plan.CheckSeats());
    }

    [Fact]
    public void Enough_seats_gives_no_message()
    {
      var plan = new Plan();
      plan.Shows.Add(Showing(10, seats: 10));
      plan.Groups.Add(Group("A", 10));

      Assert.Null(plan.CheckSeats());
    }

    [Fact]
    public void Morning_group_without_a_morning_showing_is_told_so()
    {
      var plan = new Plan();
      plan.Shows.Add(Showing(14, seats: 10));
      var a = new Group("A", 4, false, true, new List<DateTime>());
      plan.Groups.Add(a);
      plan.Distribute();

      Assert.Equal("wil 's ochtends, maar er is geen voorstelling voor 12:00", plan.WhyUnplaced(a));
    }

    [Fact]
    public void Morning_group_that_finds_the_morning_full_is_told_so()
    {
      var plan = new Plan();
      plan.Shows.Add(Showing(10, seats: 6));
      plan.Shows.Add(Showing(14, seats: 10));
      var a = new Group("A", 5, false, true, new List<DateTime>());
      var b = new Group("B", 4, false, true, new List<DateTime>());
      plan.Groups.Add(a);
      plan.Groups.Add(b);

      var unplaced = plan.Distribute();

      Assert.Equal(new[] { b }, unplaced);
      Assert.Equal("wil 's ochtends, maar geen ochtendvoorstelling heeft nog 4 stoelen vrij", plan.WhyUnplaced(b));
    }

    [Fact]
    public void Group_that_fits_nowhere_is_told_how_many_seats_it_needs()
    {
      var plan = new Plan();
      plan.Shows.Add(Showing(10, seats: 6));
      plan.Shows.Add(Showing(14, seats: 6));
      var a = Group("A", 4);
      var b = Group("B", 4);
      var c = Group("C", 4);
      plan.Groups.AddRange(new[] { a, b, c });

      var unplaced = plan.Distribute();

      Assert.Equal(new[] { c }, unplaced);
      Assert.Equal(
        "Niet ingedeeld:" + Environment.NewLine
        + "- C (4): geen voorstelling heeft nog 4 stoelen vrij" + Environment.NewLine + Environment.NewLine
        + "Deze groepen staan in de indeling onder \"Niet ingedeeld\". Voeg een voorstelling toe, of verplaats ze daar met de hand.",
        plan.DescribeUnplaced(unplaced));
    }

    [Fact]
    public void Distributing_with_too_few_seats_tells_the_numbers_and_opens_no_preview()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        var told = new List<string>();
        main.Tell = told.Add;
        AddShowing(main, Monday, Monday.AddHours(10));
        AddGroup(main, "De Regenboog", 300);
        AddGroup(main, "Het Kompas", 300);

        Find<Button>(main, "Dividebutton").PerformClick();

        Assert.Equal(new[] { "Er zijn te weinig stoelen: 600 personen en 550 stoelen. Voeg een voorstelling toe of maak een groep kleiner." }, told);
        Assert.Empty(Application.OpenForms.OfType<PreviewForm>());
      });
    }

    [Fact]
    public void Distributing_an_empty_plan_says_what_to_add_first()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        var told = new List<string>();
        main.Tell = told.Add;

        Find<Button>(main, "Dividebutton").PerformClick();
        AddShowing(main, Monday, Monday.AddHours(10));
        Find<Button>(main, "Dividebutton").PerformClick();

        Assert.Equal(
          new[] { "Voeg eerst een voorstelling toe: de datum en de tijd waarop gespeeld wordt.", "Voeg eerst de groepen toe." },
          told);
        Assert.Empty(Application.OpenForms.OfType<PreviewForm>());
      });
    }

    [Fact]
    public void Group_that_loses_its_pin_is_named()
    {
      var plan = new Plan();
      plan.Shows.Add(Showing(11, 10));
      var group = Group("De Regenboog", 5);
      group.Voorstelling = Showing(10, 10);
      group.Pinned = true;
      plan.Groups.Add(group);

      plan.Distribute();

      Assert.Equal(
        "Niet meer vastgezet, want de voorstelling is weg of heeft geen plaats meer: De Regenboog. Deze groepen zijn opnieuw ingedeeld.",
        plan.DescribeUnpinned());
    }

    [Fact]
    public void Distributing_tells_which_groups_lost_their_pin()
    {
      OnUiThread(() =>
      {
        using var main = new Theaterverdeling();
        main.Show();
        var told = new List<string>();
        main.Tell = told.Add;
        main.Confirm = question => true;
        AddShowing(main, Monday, Monday.AddHours(10));
        AddGroup(main, "De Regenboog", 28);
        Find<Button>(main, "Dividebutton").PerformClick();
        var preview = Application.OpenForms.OfType<PreviewForm>().Single();
        Find<ListBox>(preview, "Grouplist").SelectedIndex = 0;
        Find<Button>(preview, "Pinbutton").PerformClick();

        Find<ListView>(main, "Voorstellingviewer").Items[0].Selected = true;
        Find<Button>(main, "Removeshowbutton").PerformClick();
        AddShowing(main, Monday, Monday.AddHours(11));
        Find<Button>(main, "Dividebutton").PerformClick();

        Assert.Equal(
          new[] { "Niet meer vastgezet, want de voorstelling is weg of heeft geen plaats meer: De Regenboog. Deze groepen zijn opnieuw ingedeeld." },
          told);
      });
    }

    private static Voorstelling Showing(int hour, int seats)
    {
      return new Voorstelling(DateTime.Today.AddHours(hour), Hall(Row(1, seats)), Monday);
    }
  }
}
