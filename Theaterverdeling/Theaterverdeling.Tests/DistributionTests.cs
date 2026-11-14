using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Theaterverdeling.Tests.TestHalls;

namespace Theaterverdeling.Tests
{
  public class DistributionTests
  {
    private static readonly DateTime Monday = new DateTime(2026, 3, 16);
    private static readonly DateTime Tuesday = new DateTime(2026, 3, 17);

    [Fact]
    public void Group_goes_to_a_showing_on_a_date_it_prefers()
    {
      var monday = Showing(Monday, 10, seats: 10);
      var tuesday = Showing(Tuesday, 10, seats: 10);
      var a = Wish("A", 4, Tuesday);

      Plan(monday, tuesday).With(a).Distribute();

      Assert.Same(tuesday, a.Voorstelling);
      Assert.Equal(new[] { a }, tuesday.Groups);
      Assert.Empty(monday.Groups);
    }

    [Fact]
    public void Group_goes_to_another_date_when_its_preferred_showing_is_full()
    {
      var monday = Showing(Monday, 10, seats: 10);
      var tuesday = Showing(Tuesday, 10, seats: 4);
      var a = Wish("A", 4, Tuesday);
      var b = Wish("B", 3, Tuesday);

      Plan(monday, tuesday).With(a, b).Distribute();

      Assert.Same(tuesday, a.Voorstelling);
      Assert.Same(monday, b.Voorstelling);
    }

    [Fact]
    public void Morning_group_goes_to_a_showing_before_noon()
    {
      var afternoon = Showing(Monday, 14, seats: 10);
      var morning = Showing(Monday, 10, seats: 6);
      var a = Wish("A", 4, Monday, morning: true);

      Plan(afternoon, morning).With(a).Distribute();

      Assert.Same(morning, a.Voorstelling);
    }

    [Fact]
    public void Group_that_fits_nowhere_is_reported_and_has_no_showing()
    {
      var afternoon = Showing(Monday, 14, seats: 10);
      var morningGroup = Wish("Ochtend", 4, Monday, morning: true);
      var tooLarge = Wish("Groot", 11, Monday);

      var unplaced = Plan(afternoon).With(morningGroup, tooLarge).Distribute();

      Assert.Equal(new[] { "Groot", "Ochtend" }, unplaced.Select(group => group.Name).OrderBy(name => name));
      Assert.Null(morningGroup.Voorstelling);
      Assert.Null(tooLarge.Voorstelling);
    }

    [Fact]
    public void Group_goes_to_the_preferred_showing_with_the_most_free_seats()
    {
      var small = Showing(Monday, 10, seats: 10);
      var large = Showing(Tuesday, 10, seats: 20);
      var a = Wish("A", 5, Monday, Tuesday);

      Plan(small, large).With(a).Distribute();

      Assert.Same(large, a.Voorstelling);
    }

    [Fact]
    public void With_equal_room_the_earlier_showing_is_used()
    {
      var tuesday = Showing(Tuesday, 10, seats: 10);
      var monday = Showing(Monday, 10, seats: 10);
      var a = Wish("A", 5, Monday, Tuesday);

      Plan(tuesday, monday).With(a).Distribute();

      Assert.Same(monday, a.Voorstelling);
    }

    [Fact]
    public void Front_row_groups_are_placed_first_then_morning_groups_then_larger_groups()
    {
      var show = Showing(Monday, 10, seats: 30);
      var small = Wish("klein", 3, Monday);
      var large = Wish("groot", 8, Monday);
      var morning = Wish("ochtend", 4, Monday, morning: true);
      var front = new Group("voor", 2, true, false, new List<DateTime> { Monday });

      Plan(show).With(small, large, morning, front).Distribute();

      Assert.Equal(new[] { "voor", "ochtend", "groot", "klein" }, show.Groups.Select(group => group.Name));
    }

    [Fact]
    public void Placed_groups_get_their_seats()
    {
      var show = Showing(Monday, 10, seats: 10);
      var a = Wish("A", 4, Monday);

      Plan(show).With(a).Distribute();

      Assert.Equal("rij 1: 7-10", Describe(a));
    }

    [Fact]
    public void Distributing_again_starts_over()
    {
      var show = Showing(Monday, 10, seats: 10);
      var a = Wish("A", 4, Monday);
      var plan = Plan(show).With(a);

      plan.Distribute();
      plan.Distribute();

      Assert.Equal(new[] { a }, show.Groups);
    }

    [Fact]
    public void Group_that_no_longer_fits_loses_its_showing_and_seats()
    {
      var show = Showing(Monday, 10, seats: 10);
      var a = Wish("A", 4, Monday);
      var plan = Plan(show).With(a);
      plan.Distribute();
      Assert.Same(show, a.Voorstelling);
      plan.Shows.Clear();

      plan.Distribute();

      Assert.Null(a.Voorstelling);
      Assert.Empty(a.Seats);
    }

    [Fact]
    public void Showings_are_in_order_of_date_and_time_after_distributing()
    {
      var tuesday = Showing(Tuesday, 10, seats: 10);
      var mondayAfternoon = Showing(Monday, 14, seats: 10);
      var mondayMorning = Showing(Monday, 10, seats: 10);
      var plan = Plan(tuesday, mondayAfternoon, mondayMorning);

      plan.Distribute();

      Assert.Equal(new[] { mondayMorning, mondayAfternoon, tuesday }, plan.Shows);
    }

    private static Voorstelling Showing(DateTime date, int hour, int seats)
    {
      return new Voorstelling(DateTime.Today.AddHours(hour), Hall(Row(1, seats)), date);
    }

    private static Group Wish(string name, int people, DateTime date, bool morning = false)
    {
      return new Group(name, people, false, morning, new List<DateTime> { date });
    }

    private static Group Wish(string name, int people, DateTime first, DateTime second)
    {
      return new Group(name, people, false, false, new List<DateTime> { first, second });
    }

    private static Plan Plan(params Voorstelling[] shows)
    {
      var plan = new Plan();
      plan.Shows.AddRange(shows);
      return plan;
    }
  }

  internal static class PlanTestExtensions
  {
    public static Plan With(this Plan plan, params Group[] groups)
    {
      plan.Groups.AddRange(groups);
      return plan;
    }
  }
}
