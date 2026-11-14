using System;
using System.Globalization;
using System.Linq;
using Xunit;
using static Theaterverdeling.Tests.TestHalls;

namespace Theaterverdeling.Tests
{
  public class DrawingTests
  {
    [Fact]
    public void Seats_of_a_group_share_one_colour_that_differs_from_other_groups()
    {
      var hall = Hall(Row(1, 4));
      Seat(hall, Group("A", 2), Group("B", 2));
      var drawing = new RecordingDrawingContext();

      hall.Draw(drawing);

      // Seats are drawn left to right: B sits on 1-2, A on 3-4.
      var seatColours = drawing.Fills.Take(4).Select(fill => fill.Color).ToList();
      Assert.Equal(seatColours[0], seatColours[1]);
      Assert.Equal(seatColours[2], seatColours[3]);
      Assert.NotEqual(seatColours[0], seatColours[2]);
    }

    [Fact]
    public void Free_seats_are_not_coloured()
    {
      var hall = Hall(Row(1, 4), Row(2, 4));
      Seat(hall, Group("A", 2));
      var drawing = new RecordingDrawingContext();

      hall.Draw(drawing);

      var seatFills = drawing.Fills.Where(fill => fill.Rect.Top < LegendTop(drawing));
      Assert.Equal(2, seatFills.Count());
    }

    [Fact]
    public void Seats_of_a_switched_off_section_are_marked_as_closed()
    {
      var hall = Hall(Row(1, 4), SectionRow(2, 4, "Balkon"));
      hall.SwitchOff("Balkon");
      Seat(hall, Group("A", 2));
      var drawing = new RecordingDrawingContext();

      hall.Draw(drawing);

      Assert.Equal(4, drawing.Fills.Count(fill => fill.Color == Zaal.ClosedSeatColor));
    }

    [Fact]
    public void Seats_that_are_never_given_to_a_group_are_marked_as_closed()
    {
      var hall = Hall(Row(1, 4, true, 1));
      Seat(hall, Group("A", 2));
      var drawing = new RecordingDrawingContext();

      hall.Draw(drawing);

      Assert.Equal(1, drawing.Fills.Count(fill => fill.Color == Zaal.ClosedSeatColor));
    }

    [Fact]
    public void Stage_is_labelled_above_the_first_row()
    {
      var hall = Hall(Row(1, 4));
      Seat(hall, Group("A", 2));
      var drawing = new RecordingDrawingContext();

      hall.Draw(drawing);

      var stage = drawing.Texts.Single(text => text.Text == "Podium");
      Assert.True(stage.Rect.Bottom <= drawing.Fills.Min(fill => fill.Rect.Top));
    }

    [Fact]
    public void Legend_lists_the_seat_ranges_of_each_group()
    {
      var hall = Hall(Row(1, 4));
      Seat(hall, Group("A", 2), Group("B", 2));
      var drawing = new RecordingDrawingContext();

      hall.Draw(drawing);

      var texts = drawing.Texts.Select(text => text.Text).ToList();
      Assert.Contains("A", texts);
      Assert.Contains("rij 1: 3-4", texts);
      Assert.Contains("B", texts);
      Assert.Contains("rij 1: 1-2", texts);
    }

    [Fact]
    public void Single_seat_is_listed_without_a_range()
    {
      var hall = Hall(Row(1, 4));
      Seat(hall, Group("A", 3));
      var drawing = new RecordingDrawingContext();

      hall.Draw(drawing);

      var texts = drawing.Texts.Select(text => text.Text).ToList();
      Assert.Contains("rij 1: 1", texts);
      Assert.Contains("rij 1: 3-4", texts);
    }

    [Fact]
    public void Group_with_many_rows_continues_on_the_next_legend_line()
    {
      var hall = Hall(Row(1, 2), Row(2, 2), Row(3, 2), Row(4, 2), Row(5, 2), Row(6, 2));
      Seat(hall, Group("A", 6));
      var drawing = new RecordingDrawingContext();

      hall.Draw(drawing);

      // Six rows, one seat each: four on the first legend line, two on the next.
      var first = drawing.Texts.Single(text => text.Text == "rij 1: 2").Rect;
      var fourth = drawing.Texts.Single(text => text.Text == "rij 4: 2").Rect;
      var fifth = drawing.Texts.Single(text => text.Text == "rij 5: 2").Rect;
      Assert.Equal(first.Top, fourth.Top);
      Assert.True(fifth.Top > fourth.Top);
      Assert.Equal(first.Left, fifth.Left);
    }

    [Fact]
    public void Legend_of_a_full_hall_with_many_groups_stays_on_the_page()
    {
      var hall = Zaal.Load("Lampegiet");
      var groups = Enumerable.Range(1, 22).Select(number => Group("Groep " + number, 25)).ToArray();
      Seat(hall, groups);
      var drawing = new RecordingDrawingContext();

      hall.Draw(drawing);

      Assert.All(groups, group => Assert.Contains(drawing.Texts, text => text.Text == group.Name));
      Assert.All(drawing.Texts, text => Assert.True(text.Rect.Bottom <= drawing.PageHeight, $"\"{text.Text}\" is below the page"));
      Assert.All(drawing.Texts, text => Assert.True(text.Rect.Right <= drawing.PageWidth, $"\"{text.Text}\" is right of the page"));
    }

    [Fact]
    public void Legend_marks_each_group_with_the_colour_of_its_seats()
    {
      var hall = Hall(Row(1, 4));
      Seat(hall, Group("A", 2), Group("B", 2));
      var drawing = new RecordingDrawingContext();

      hall.Draw(drawing);

      // Four seat fills (B, B, A, A) followed by one legend marker per group, in group order.
      var colours = drawing.Fills.Select(fill => fill.Color).ToList();
      Assert.Equal(6, colours.Count);
      Assert.Equal(colours[2], colours[4]);
      Assert.Equal(colours[0], colours[5]);
    }

    [Fact]
    public void Legend_starts_below_the_last_row()
    {
      var hall = Hall(Row(1, 4), Row(2, 4));
      Seat(hall, Group("A", 6));
      var drawing = new RecordingDrawingContext();

      hall.Draw(drawing);

      var lastRowBottom = drawing.Texts.Where(text => text.Text == "2").Max(text => text.Rect.Bottom);
      Assert.True(LegendTop(drawing) >= lastRowBottom);
    }

    [Fact]
    public void Legend_separates_groups_with_a_line()
    {
      var hall = Hall(Row(1, 4));
      Seat(hall, Group("A", 2), Group("B", 2));
      var drawing = new RecordingDrawingContext();

      hall.Draw(drawing);

      Assert.Single(drawing.Lines);
    }

    [Fact]
    public void Showing_is_titled_with_show_name_date_and_time()
    {
      CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("nl-NL");
      var show = new Voorstelling(new DateTime(2026, 3, 14, 10, 30, 0), Hall(Row(1, 4)), new DateTime(2026, 3, 14));
      show.Groups.Add(Group("A", 2));
      show.Assignseats();
      var drawing = new RecordingDrawingContext();

      show.Draw(drawing, "Kikker is jarig");

      var texts = drawing.Texts.Select(text => text.Text).ToList();
      Assert.Contains("Kikker is jarig", texts);
      Assert.Contains("14-03-2026 10:30", texts);
    }

    [Fact]
    public void Title_of_a_showing_names_its_hall()
    {
      CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("nl-NL");
      var show = new Voorstelling(new DateTime(2026, 3, 14, 10, 30, 0), Zaal.Load("Lampegiet"), new DateTime(2026, 3, 14));
      show.Assignseats();
      var drawing = new RecordingDrawingContext();

      show.Draw(drawing, "Kikker is jarig");

      Assert.Contains("14-03-2026 10:30, Lampegiet", drawing.Texts.Select(text => text.Text));
    }

    private static float LegendTop(RecordingDrawingContext drawing)
    {
      return drawing.Texts.First(text => text.Text.StartsWith("rij ")).Rect.Top;
    }
  }
}
