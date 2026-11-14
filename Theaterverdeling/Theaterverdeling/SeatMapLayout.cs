using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Theaterverdeling
{
  // Where the rows and seats of a hall are on a seat map of a given size. The drawing code and the
  // clickable seat map on screen both use it, so a click lands on the seat that is drawn there.
  // Rows run from the stage at the top to the back; every row is centred, with room for the row
  // number on both sides.
  public sealed class SeatMapLayout
  {
    private readonly IReadOnlyList<Row> _rows;
    private readonly float _width;

    public SeatMapLayout(IReadOnlyList<Row> rows, float width, float height)
    {
      _rows = rows;
      _width = width;
      var widest = rows.Count == 0 ? 0 : rows.Max(row => row.TotalSeatCount);
      // Two extra for the row number on both sides.
      SeatSize = Math.Min(width / (widest + 2), height / Math.Max(rows.Count, 1));
    }

    // Seats are squares of this size.
    public float SeatSize { get; }

    // Height the rows take together.
    public float Height => _rows.Count * SeatSize;

    // The full width of the map at the height of a row.
    public RectangleF RowBounds(int row)
    {
      return new RectangleF(0, row * SeatSize, _width, SeatSize);
    }

    public RectangleF SeatBounds(int row, int seat)
    {
      return new RectangleF(RowStart(row) + (seat + 1) * SeatSize, row * SeatSize, SeatSize, SeatSize);
    }

    // The place of the row number, left or right of the seats.
    public RectangleF RowNumberBounds(int row, bool right)
    {
      var position = right ? _rows[row].TotalSeatCount + 1 : 0;
      return new RectangleF(RowStart(row) + position * SeatSize, row * SeatSize, SeatSize, SeatSize);
    }

    // The row and seat at a point of the map, or null when the point is above or below the rows.
    public SeatMapHit HitTest(PointF point)
    {
      if (SeatSize <= 0 || point.X < 0 || point.X >= _width || point.Y < 0 || point.Y >= Height)
      {
        return null;
      }
      var rowIndex = Math.Min((int)(point.Y / SeatSize), _rows.Count - 1);
      var seatIndex = (int)Math.Floor((point.X - RowStart(rowIndex)) / SeatSize) - 1;
      var chairRow = _rows[rowIndex] as ChairRow;
      var onSeat = chairRow != null && seatIndex >= 0 && seatIndex < chairRow.Seats.Length;
      return new SeatMapHit(rowIndex, _rows[rowIndex], onSeat ? seatIndex : -1);
    }

    // The row with its two row numbers is centred on the map.
    private float RowStart(int row)
    {
      return (_width - (_rows[row].TotalSeatCount + 2) * SeatSize) / 2;
    }
  }

  // What is at a point of the seat map: always a row, and a seat when the point is on one.
  public sealed class SeatMapHit
  {
    public SeatMapHit(int rowIndex, Row row, int seatIndex)
    {
      RowIndex = rowIndex;
      Row = row;
      SeatIndex = seatIndex;
    }

    // Position of the row in the hall, counted from the stage and including walkways.
    public int RowIndex { get; }
    public Row Row { get; }

    // Position of the seat in its row, starting at 0, or -1 when the point is next to the seats.
    public int SeatIndex { get; }

    public bool OnSeat => SeatIndex >= 0;

    // The seat at the point, or null. Its group tells who sits there after a distribution.
    public Seat Seat => OnSeat ? ((ChairRow)Row).Seats[SeatIndex] : null;
  }
}
