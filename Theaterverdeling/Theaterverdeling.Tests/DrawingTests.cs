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
      Assert.Contains("Rij 1 Stoel 3 t/m 4", texts);
      Assert.Contains("B", texts);
      Assert.Contains("Rij 1 Stoel 1 t/m 2", texts);
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

    private static float LegendTop(RecordingDrawingContext drawing)
    {
      return drawing.Texts.First(text => text.Text.StartsWith("Rij ")).Rect.Top;
    }
  }
}
