using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Theaterverdeling.Tests.TestHalls;

namespace Theaterverdeling.Tests
{
  public class SeatFillingTests
  {
    [Fact]
    public void First_group_is_seated_from_the_right_edge_of_the_front_row()
    {
      var hall = Hall(Row(1, 10));
      var a = Group("A", 4);

      Seat(hall, a);

      Assert.Equal("rij 1: 7-10", Describe(a));
    }

    [Fact]
    public void Group_that_does_not_fit_in_its_half_row_continues_in_the_next_row()
    {
      var hall = Hall(Row(1, 10), Row(2, 10));
      var a = Group("A", 4);
      var b = Group("B", 4);

      Seat(hall, a, b);

      Assert.Equal("rij 1: 6-6, rij 2: 8-10", Describe(b));
    }

    [Fact]
    public void Left_side_is_used_once_the_right_side_is_a_row_ahead()
    {
      var hall = Hall(Row(1, 10), Row(2, 10));
      var a = Group("A", 4);
      var b = Group("B", 4);
      var c = Group("C", 3);

      Seat(hall, a, b, c);

      Assert.Equal("rij 1: 1-3", Describe(c));
    }

    [Fact]
    public void Group_continues_on_the_other_side_when_its_side_is_full()
    {
      var hall = Hall(Row(1, 4));
      var a = Group("A", 3);

      Seat(hall, a);

      Assert.Equal("rij 1: 1-1, rij 1: 3-4", Describe(a));
    }

    [Fact]
    public void Walkway_between_rows_is_skipped()
    {
      var hall = Hall(Row(1, 2), Walkway, Row(2, 2));
      var a = Group("A", 2);

      Seat(hall, a);

      Assert.Equal("rij 1: 2-2, rij 2: 2-2", Describe(a));
    }

    [Fact]
    public void Row_that_is_not_in_use_is_skipped()
    {
      var hall = Hall(Row(1, 4, inUse: false), Row(2, 4));
      var a = Group("A", 2);

      Seat(hall, a);

      Assert.Equal("rij 2: 3-4", Describe(a));
    }

    [Fact]
    public void Unusable_seats_are_not_given_to_a_group()
    {
      var hall = Hall(Row(1, 10, true, 9, 10));
      var a = Group("A", 2);

      Seat(hall, a);

      Assert.Equal("rij 1: 7-8", Describe(a));
    }

    [Fact]
    public void Unusable_seats_do_not_count_as_capacity()
    {
      var hall = Hall(Row(1, 10, true, 9, 10));

      Assert.Equal(8, hall.Seats);
    }

    [Fact]
    public void Unusable_seats_survive_saving_and_loading_the_hall()
    {
      var hall = Hall(Row(1, 10, true, 9, 10));

      var reloaded = Zaal.FromJson<Lampegiet>(hall.ToJson());

      Assert.Equal(8, reloaded.Seats);
    }

    [Fact]
    public void More_people_than_usable_seats_is_an_error()
    {
      var hall = Hall(Row(1, 4));
      var a = Group("A", 5);

      Assert.Throws<InvalidOperationException>(() => Seat(hall, a));
    }

    [Fact]
    public void Seating_again_starts_from_an_empty_hall()
    {
      var hall = Hall(Row(1, 4));
      Seat(hall, Group("A", 4));
      var b = Group("B", 2);

      Seat(hall, b);

      Assert.Equal("rij 1: 3-4", Describe(b));
    }

    [Fact]
    public void Full_Lampegiet_gives_every_person_a_seat()
    {
      var hall = Lampegiet.Load();
      var groups = Enumerable.Range(1, 10).Select(number => Group("G" + number, 55)).ToArray();

      Seat(hall, groups);

      Assert.All(groups, group => Assert.Equal(55, group.Seats.Sum(range => range.EndSeat - range.StartSeat + 1)));
    }
  }
}
