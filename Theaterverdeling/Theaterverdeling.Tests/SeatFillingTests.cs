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
      var hall = Hall(Row(1, 10), Row(2, 10), Row(3, 10));
      var a = Group("A", 3);
      var b = Group("B", 9);

      Seat(hall, a, b);

      Assert.Equal("rij 1: 6-7, rij 2: 6-10, rij 3: 9-10", Describe(b));
    }

    [Fact]
    public void Last_people_of_a_group_sit_across_the_middle_instead_of_starting_a_new_row()
    {
      var hall = Hall(Row(1, 10), Row(2, 10), Row(3, 10), Row(4, 10));
      var a = Group("A", 14);
      var b = Group("B", 16);

      Seat(hall, a, b);

      // A leaves one seat free next to the middle of row 3. The sixteenth of B sits there, not alone in row 4.
      Assert.Equal("rij 1: 6-10, rij 2: 6-10, rij 3: 7-10", Describe(a));
      Assert.Equal("rij 1: 1-5, rij 2: 1-5, rij 3: 1-6", Describe(b));
    }

    [Fact]
    public void Group_that_fits_in_one_row_across_the_middle_does_not_start_a_new_row()
    {
      var hall = Hall(Row(1, 10), Row(2, 10));
      var a = Group("A", 3);
      var b = Group("B", 4);

      Seat(hall, a, b);

      Assert.Equal("rij 1: 4-7", Describe(b));
    }

    [Fact]
    public void Nobody_sits_alone_in_the_first_row_of_a_group()
    {
      var hall = Hall(Row(1, 10), Row(2, 10));
      var a = Group("A", 4);
      var b = Group("B", 4);

      Seat(hall, a, b);

      // One seat is left in the half row of A, so B starts in row 2.
      Assert.Equal("rij 2: 7-10", Describe(b));
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
    public void Nobody_sits_alone_in_the_last_row_of_a_group()
    {
      var hall = Hall(Row(1, 10), Row(2, 10), Row(3, 10));
      var a = Group("A", 6);

      Seat(hall, Group("X", 5), a);

      // Five fit in the half row, but then the sixth would sit alone in row 3. The left side has not
      // reached row 2 yet, so the sixth does not sit across the middle either.
      Assert.Equal("rij 2: 7-10, rij 3: 9-10", Describe(a));
    }

    [Fact]
    public void Seat_that_is_left_open_goes_to_a_later_group_that_fits_in_it()
    {
      var hall = Hall(Row(1, 10), Row(2, 10), Row(3, 10));
      var a = Group("A", 6);
      var b = Group("B", 2);
      var c = Group("C", 1);

      Seat(hall, Group("X", 5), a, b, c);

      Assert.Equal("rij 1: 1-2", Describe(b));
      Assert.Equal("rij 2: 6-6", Describe(c));
    }

    [Fact]
    public void One_person_stays_in_the_last_row_when_a_neighbour_would_leave_somebody_alone_in_the_row_before()
    {
      var hall = Hall(Row(1, 4), Row(2, 4), Row(3, 4));
      var a = Group("A", 3);

      Seat(hall, Group("X", 2), a);

      Assert.Equal("rij 2: 3-4, rij 3: 4-4", Describe(a));
    }

    [Fact]
    public void Group_that_would_sit_on_both_sides_of_the_walkway_starts_behind_it()
    {
      var hall = Hall(Row(1, 10, true, 1, 2, 3, 4, 5), Walkway, Row(2, 10), Row(3, 10));
      var a = Group("A", 3);
      var b = Group("B", 4);

      Seat(hall, a, b);

      Assert.Equal("rij 1: 8-10", Describe(a));
      Assert.Equal("rij 2: 7-10", Describe(b));
    }

    [Fact]
    public void Seats_left_open_before_the_walkway_go_to_a_later_group_that_fits_in_them()
    {
      var hall = Hall(Row(1, 10, true, 1, 2, 3, 4, 5), Walkway, Row(2, 10), Row(3, 10));
      var a = Group("A", 3);
      var b = Group("B", 4);
      var c = Group("C", 2);

      Seat(hall, a, b, c);

      Assert.Equal("rij 1: 6-7", Describe(c));
    }

    [Fact]
    public void Group_that_does_not_fit_behind_the_walkway_sits_on_both_sides_of_it()
    {
      var hall = Hall(Row(1, 10, true, 1, 2, 3, 4, 5), Walkway, Row(2, 10));
      var a = Group("A", 3);
      var b = Group("B", 7);

      Seat(hall, a, b);

      Assert.Equal("rij 1: 6-7, rij 2: 6-10", Describe(b));
    }

    [Fact]
    public void Hall_that_is_too_full_for_the_rules_is_seated_row_by_row_with_every_seat_used()
    {
      var hall = Hall(Row(1, 10, true, 1, 2, 3, 4, 5), Walkway, Row(2, 10));
      var a = Group("A", 3);
      var b = Group("B", 4);
      var c = Group("C", 3);
      var d = Group("D", 5);

      Seat(hall, a, b, c, d);

      // With the rules B starts behind the walkway, and then D finds no place. So nobody skips the walkway.
      Assert.Equal("rij 1: 6-7, rij 2: 9-10", Describe(b));
      Assert.Equal(new[] { 3, 4, 3, 5 }, new[] { a, b, c, d }.Select(group => hall.SeatsOf(group)));
    }

    [Fact]
    public void Walkway_between_rows_is_skipped()
    {
      var hall = Hall(Row(1, 2, true, 1), Walkway, Row(2, 2));
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

      var reloaded = Zaal.FromJson(hall.ToJson());

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
      var hall = Zaal.Load("Lampegiet");
      var groups = Enumerable.Range(1, 10).Select(number => Group("G" + number, 55)).ToArray();

      Seat(hall, groups);

      Assert.All(groups, group => Assert.Equal(55, group.Seats.Sum(range => range.EndSeat - range.StartSeat + 1)));
    }

    [Fact]
    public void Nobody_sits_alone_in_seats_that_an_earlier_group_left_open()
    {
      var hall = Hall(Row(1, 10), Row(2, 10), Walkway, Row(3, 10), Row(4, 10), Row(5, 10), Row(6, 10));
      var c = Group("C", 3);

      // B leaves one seat open in row 1 and the right half of row 2, in front of the walkway.
      Seat(hall, Group("A", 4), Group("B", 12), c);

      Assert.Equal("rij 1: 1-3", Describe(c));
    }
  }
}
