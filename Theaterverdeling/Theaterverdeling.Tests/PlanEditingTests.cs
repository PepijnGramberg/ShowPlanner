using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Theaterverdeling.Tests.TestHalls;

namespace Theaterverdeling.Tests
{
  public class PlanEditingTests
  {
    private static readonly DateTime Monday = new DateTime(2026, 3, 16);
    private static readonly DateTime Tuesday = new DateTime(2026, 3, 17);

    [Fact]
    public void Group_that_meets_all_rules_is_accepted()
    {
      var plan = PlanWith(Showing(Monday, 10, 30, seats: 10));

      Assert.Null(plan.CheckGroup("De Regenboog", 10));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Group_needs_a_name(string name)
    {
      var plan = PlanWith(Showing(Monday, 10, 30, seats: 10));

      Assert.Equal("Vul een groepsnaam in.", plan.CheckGroup(name, 5));
    }

    [Fact]
    public void Group_name_must_be_unique_whatever_the_capitals()
    {
      var plan = PlanWith(Showing(Monday, 10, 30, seats: 10));
      plan.Groups.Add(Group("De Regenboog", 5));

      Assert.Equal("Er is al een groep met de naam \"de regenboog\".", plan.CheckGroup(" de regenboog ", 5));
    }

    [Fact]
    public void Group_that_is_being_edited_may_keep_its_name()
    {
      var plan = PlanWith(Showing(Monday, 10, 30, seats: 10));
      var existing = Group("De Regenboog", 5);
      plan.Groups.Add(existing);

      Assert.Null(plan.CheckGroup("De Regenboog", 6, existing));
    }

    [Fact]
    public void Group_must_fit_in_the_largest_showing()
    {
      var plan = PlanWith(Showing(Monday, 10, 30, seats: 10), Showing(Tuesday, 10, 30, seats: 12));

      Assert.Equal("De groep past in geen enkele voorstelling. De grootste heeft 12 stoelen.", plan.CheckGroup("Groot", 13));
    }

    [Fact]
    public void Showing_at_a_free_moment_is_accepted()
    {
      var plan = PlanWith(Showing(Monday, 10, 30, seats: 10));

      Assert.Null(plan.CheckShowing(Monday, Monday.AddHours(13)));
    }

    [Fact]
    public void Two_showings_cannot_start_in_the_same_minute()
    {
      var plan = PlanWith(Showing(Monday, 10, 30, seats: 10));

      Assert.Equal("Er is al een voorstelling op dat moment.", plan.CheckShowing(Monday, DateTime.Today.AddHours(10).AddMinutes(30).AddSeconds(45)));
    }

    [Fact]
    public void Showing_that_is_being_edited_may_keep_its_moment()
    {
      var existing = Showing(Monday, 10, 30, seats: 10);
      var plan = PlanWith(existing);

      Assert.Null(plan.CheckShowing(Monday, DateTime.Today.AddHours(10).AddMinutes(30), existing));
    }

    [Fact]
    public void Replaced_group_keeps_its_place_in_the_list()
    {
      var plan = PlanWith(Showing(Monday, 10, 30, seats: 10));
      var first = Group("A", 2);
      var second = Group("B", 2);
      plan.Groups.AddRange(new[] { first, second });
      var changed = Group("A", 3);

      plan.ReplaceGroup(first, changed);

      Assert.Equal(new[] { changed, second }, plan.Groups);
    }

    [Fact]
    public void Replaced_showing_keeps_its_place_in_the_list()
    {
      var first = Showing(Monday, 10, 30, seats: 10);
      var second = Showing(Tuesday, 10, 30, seats: 10);
      var plan = PlanWith(first, second);
      var changed = Showing(Monday, 11, 0, seats: 10);

      plan.ReplaceShow(first, changed);

      Assert.Equal(new[] { changed, second }, plan.Shows);
    }

    [Fact]
    public void Replacement_of_a_group_that_was_removed_meanwhile_is_added()
    {
      var plan = PlanWith(Showing(Monday, 10, 30, seats: 10));
      var changed = Group("A", 3);

      plan.ReplaceGroup(Group("A", 2), changed);

      Assert.Equal(new[] { changed }, plan.Groups);
    }

    [Fact]
    public void Replacement_of_a_showing_that_was_removed_meanwhile_is_added()
    {
      var plan = PlanWith();
      var changed = Showing(Monday, 11, 0, seats: 10);

      plan.ReplaceShow(Showing(Monday, 10, 30, seats: 10), changed);

      Assert.Equal(new[] { changed }, plan.Shows);
    }

    [Fact]
    public void Groups_follow_a_showing_that_moves_to_another_date()
    {
      var monday = Showing(Monday, 10, 30, seats: 10);
      var plan = PlanWith(monday);
      var a = new Group("A", 2, false, false, new List<DateTime> { Monday });
      plan.Groups.Add(a);

      plan.ReplaceShow(monday, Showing(Tuesday, 10, 30, seats: 10));

      Assert.Equal(new[] { Tuesday }, a.Datum);
    }

    [Fact]
    public void Groups_keep_their_date_when_another_showing_is_still_on_it()
    {
      var morning = Showing(Monday, 10, 30, seats: 10);
      var afternoon = Showing(Monday, 14, 0, seats: 10);
      var plan = PlanWith(morning, afternoon);
      var a = new Group("A", 2, false, false, new List<DateTime> { Monday });
      plan.Groups.Add(a);

      plan.ReplaceShow(morning, Showing(Tuesday, 10, 30, seats: 10));

      Assert.Equal(new[] { Monday }, a.Datum);
    }

    private static Voorstelling Showing(DateTime date, int hour, int minute, int seats)
    {
      return new Voorstelling(DateTime.Today.AddHours(hour).AddMinutes(minute), Hall(Row(1, seats)), date);
    }

    private static Plan PlanWith(params Voorstelling[] shows)
    {
      var plan = new Plan();
      plan.Shows.AddRange(shows);
      return plan;
    }
  }
}
