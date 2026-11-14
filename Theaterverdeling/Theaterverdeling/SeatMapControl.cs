using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Theaterverdeling
{
  // Seat map of a hall on screen that can be clicked. It is drawn by the same code as the PDF and
  // tells which row and seat a click lands on. What a click means is up to the window that shows it.
  public class SeatMapControl : Control
  {
    // The map is laid out this wide, in points like the PDF page, and then scaled to the control.
    // That keeps the numbers in the seats as readable as they are in the PDF.
    private const float MapWidth = 555;
    private const int Edge = 6;

    public static readonly Color SelectedRowColor = Color.FromArgb(205, 226, 255);

    private Zaal _hall;
    private int _selectedRow = -1;
    private Group _highlightedGroup;

    public SeatMapControl()
    {
      DoubleBuffered = true;
      ResizeRedraw = true;
      BackColor = Color.White;
    }

    // Raised when a row or a seat is clicked with the left mouse button.
    public event EventHandler<SeatMapClickEventArgs> SeatMapClick;

    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public Zaal Hall
    {
      get => _hall;
      set
      {
        _hall = value;
        Invalidate();
      }
    }

    // Position in the hall of the row that is marked, or -1 for none.
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public int SelectedRow
    {
      get => _selectedRow;
      set
      {
        _selectedRow = value;
        Invalidate();
      }
    }

    // The group whose seats get a thick outline, or null for none.
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public Group HighlightedGroup
    {
      get => _highlightedGroup;
      set
      {
        _highlightedGroup = value;
        Invalidate();
      }
    }

    // What is at a point of the control, or null when it is not on a row.
    public SeatMapHit HitTest(Point point)
    {
      var scale = MapScale;
      if (_hall == null || scale <= 0)
      {
        return null;
      }
      return Layout().HitTest(new PointF((point.X - Edge) / scale, (point.Y - Edge) / scale));
    }

    // Where a seat is drawn in the control.
    public Rectangle SeatBounds(int row, int seat)
    {
      var bounds = Layout().SeatBounds(row, seat);
      var scale = MapScale;
      return Rectangle.Round(new RectangleF(Edge + bounds.X * scale, Edge + bounds.Y * scale, bounds.Width * scale, bounds.Height * scale));
    }

    // Does what a click with the left mouse button on this point does.
    public void ClickAt(Point point)
    {
      var hit = HitTest(point);
      if (hit != null)
      {
        SeatMapClick?.Invoke(this, new SeatMapClickEventArgs(hit));
      }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      if (e.Button == MouseButtons.Left)
      {
        ClickAt(e.Location);
      }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      Cursor = HitTest(e.Location)?.OnSeat == true ? Cursors.Hand : Cursors.Default;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      var scale = MapScale;
      if (_hall == null || _hall.Rows.Count == 0 || scale <= 0)
      {
        return;
      }

      var graphics = e.Graphics;
      graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
      graphics.SmoothingMode = SmoothingMode.None;
      graphics.TranslateTransform(Edge, Edge);
      graphics.ScaleTransform(scale, scale);

      var layout = Layout();
      if (_selectedRow >= 0 && _selectedRow < _hall.Rows.Count)
      {
        using var brush = new SolidBrush(SelectedRowColor);
        graphics.FillRectangle(brush, layout.RowBounds(_selectedRow));
      }
      var size = new SizeF(MapWidth, MapHeight);
      _hall.DrawMap(new GdiDrawingContext(graphics, size), new RectangleF(PointF.Empty, size));

      if (_highlightedGroup != null)
      {
        using var pen = new Pen(Color.Black, 2f);
        for (var row = 0; row < _hall.Rows.Count; row++)
        {
          if (_hall.Rows[row] is ChairRow chairRow)
          {
            for (var seat = 0; seat < chairRow.Seats.Length; seat++)
            {
              if (chairRow.Seats[seat].Group == _highlightedGroup)
              {
                var bounds = layout.SeatBounds(row, seat);
                graphics.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width, bounds.Height);
              }
            }
          }
        }
      }
    }

    // Pixels per point of the map.
    private float MapScale => (ClientSize.Width - 2 * Edge) / MapWidth;

    // Height of the map in points: what is left of the control at this scale.
    private float MapHeight => (ClientSize.Height - 2 * Edge) / MapScale;

    private new SeatMapLayout Layout()
    {
      return new SeatMapLayout(_hall.Rows, MapWidth, MapHeight);
    }
  }

  public sealed class SeatMapClickEventArgs : EventArgs
  {
    public SeatMapClickEventArgs(SeatMapHit hit)
    {
      Hit = hit;
    }

    public SeatMapHit Hit { get; }
  }
}
