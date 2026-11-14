using System;
using System.Drawing;
using System.Linq;
using Xunit;
using static Theaterverdeling.Tests.TestHalls;

namespace Theaterverdeling.Tests
{
  // A group is recognised by its colour and, from the thirteenth group on, by dark stripes on its seats.
  public class GroupColourTests
  {
    [Fact]
    public void First_twelve_groups_get_clearly_different_colours_without_stripes()
    {
      var colours = Enumerable.Range(0, 12).Select(Zaal.GroupColor).ToList();

      var closest = (
        from first in Enumerable.Range(0, 12)
        from second in Enumerable.Range(first + 1, 12 - first - 1)
        select Distance(colours[first], colours[second])).Min();
      Assert.True(closest >= 50, $"Two groups have nearly the same colour (distance {closest:0}).");
      Assert.All(Enumerable.Range(0, 12), index => Assert.Equal(0, Zaal.GroupStripes(index)));
    }

    [Fact]
    public void Thirty_six_groups_in_one_showing_are_all_told_apart_by_colour_or_stripes()
    {
      var looks = Enumerable.Range(0, 36).Select(index => (Zaal.GroupColor(index), Zaal.GroupStripes(index))).ToList();

      Assert.Equal(36, looks.Distinct().Count());
    }

    [Fact]
    public void Groups_that_are_seated_after_each_other_get_very_different_colours()
    {
      var colours = Enumerable.Range(0, 36).Select(Zaal.GroupColor).ToList();

      var closest = Enumerable.Range(0, 35).Min(index => Distance(colours[index], colours[index + 1]));
      Assert.True(closest >= 90, $"Two neighbours have similar colours (distance {closest:0}).");
    }

    [Fact]
    public void Every_colour_is_light_enough_to_read_black_seat_numbers_on()
    {
      var lowest = Enumerable.Range(0, 12).Min(index => ContrastWithBlack(Zaal.GroupColor(index)));

      Assert.True(lowest >= 4.5, $"A colour is too dark for black text (contrast {lowest:0.0}).");
    }

    [Fact]
    public void Thirteenth_group_has_a_stripe_on_its_seats_and_on_its_legend_marker()
    {
      var hall = Hall(Row(1, 26));
      var groups = Enumerable.Range(1, 13).Select(number => Group("G" + number, 2)).ToArray();
      Seat(hall, groups);
      var drawing = new RecordingDrawingContext();

      hall.Draw(drawing);

      // Two seats and one legend marker carry the stripe of the thirteenth group.
      var stripes = drawing.Fills.Where(fill => fill.Color == Zaal.GroupStripeColor(12)).ToList();
      Assert.Equal(3, stripes.Count);
      // No stripe for the first twelve groups: 26 seats, 13 markers and those 3 stripes are all the fills.
      Assert.Equal(26 + 13 + 3, drawing.Fills.Count);
    }

    private static double Distance(Color a, Color b)
    {
      return Math.Sqrt(Math.Pow(a.R - b.R, 2) + Math.Pow(a.G - b.G, 2) + Math.Pow(a.B - b.B, 2));
    }

    // Contrast ratio as defined by WCAG; 4.5 is its minimum for normal text.
    private static double ContrastWithBlack(Color colour)
    {
      double Linear(byte channel)
      {
        var value = channel / 255.0;
        return value <= 0.03928 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
      }
      var luminance = 0.2126 * Linear(colour.R) + 0.7152 * Linear(colour.G) + 0.0722 * Linear(colour.B);
      return (luminance + 0.05) / 0.05;
    }
  }
}
