using System.Drawing;
using System.Linq;
using Xunit;
using static Theaterverdeling.Tests.TestHalls;

namespace Theaterverdeling.Tests
{
  public class SeatMapTests
  {
    [Fact]
    public void Point_on_a_seat_gives_its_row_and_seat()
    {
      var hall = Hall(Row(1, 4), Row(2, 6));
      var layout = new SeatMapLayout(hall.Rows, 800, 500);

      var hit = layout.HitTest(Centre(layout.SeatBounds(1, 4)));

      Assert.Equal(1, hit.RowIndex);
      Assert.Equal(4, hit.SeatIndex);
      Assert.Same(((ChairRow)hall.Rows[1]).Seats[4], hit.Seat);
    }

    [Fact]
    public void Point_on_a_row_number_gives_the_row_without_a_seat()
    {
      var hall = Hall(Row(1, 4), Row(2, 6));
      var layout = new SeatMapLayout(hall.Rows, 800, 500);

      var hit = layout.HitTest(Centre(layout.RowNumberBounds(0, true)));

      Assert.Equal(0, hit.RowIndex);
      Assert.False(hit.OnSeat);
      Assert.Null(hit.Seat);
    }

    [Fact]
    public void Point_on_a_walkway_gives_the_walkway()
    {
      var hall = Hall(Row(1, 4), Walkway, Row(2, 4));
      var layout = new SeatMapLayout(hall.Rows, 800, 500);

      var hit = layout.HitTest(Centre(layout.RowBounds(1)));

      Assert.IsType<WalkWay>(hit.Row);
      Assert.False(hit.OnSeat);
    }

    [Fact]
    public void Point_below_the_last_row_hits_nothing()
    {
      var hall = Hall(Row(1, 4));
      var layout = new SeatMapLayout(hall.Rows, 800, 500);

      Assert.Null(layout.HitTest(new PointF(400, layout.Height + 1)));
    }

    [Fact]
    public void Seats_are_drawn_where_the_layout_says_they_are()
    {
      var hall = Hall(Row(1, 4, true, 2), Row(2, 6));
      var drawing = new RecordingDrawingContext();

      var layout = hall.DrawMap(drawing, new RectangleF(20, 76, 555, 500));

      var closed = drawing.Fills.Single(fill => fill.Color == Zaal.ClosedSeatColor).Rect;
      var expected = layout.SeatBounds(0, 1);
      expected.Offset(20, 76);
      Assert.Equal(expected, closed);
    }

    [Fact]
    public void Hall_without_rows_draws_nothing()
    {
      var drawing = new RecordingDrawingContext();

      new HallDraft().ToHall().DrawMap(drawing, new RectangleF(0, 0, 555, 500));

      Assert.Empty(drawing.Texts);
    }

    private static PointF Centre(RectangleF rect)
    {
      return new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
    }
  }
}
