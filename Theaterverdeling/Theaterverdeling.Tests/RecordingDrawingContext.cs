using System;
using System.Collections.Generic;
using System.Drawing;

namespace Theaterverdeling.Tests
{
  // Drawing surface that remembers what was drawn, in page coordinates.
  internal sealed class RecordingDrawingContext : IDrawingContext
  {
    private PointF _offset;

    public List<(RectangleF Rect, Color Color)> Fills { get; } = new List<(RectangleF, Color)>();
    public List<(string Text, RectangleF Rect)> Texts { get; } = new List<(string, RectangleF)>();
    public List<(PointF From, PointF To)> Lines { get; } = new List<(PointF, PointF)>();

    public float PageWidth => 595;

    public float PageHeight => 842;

    public void DrawRectangles(IReadOnlyCollection<RectangleF> rects)
    {
    }

    public void FillRectangle(RectangleF rect, Color color)
    {
      Fills.Add((OnPage(rect), color));
    }

    public void DrawString(string text, RectangleF rect)
    {
      Texts.Add((text, OnPage(rect)));
    }

    public void DrawText(string text, RectangleF rect)
    {
      Texts.Add((text, OnPage(rect)));
    }

    public void DrawLine(PointF from, PointF to)
    {
      Lines.Add((new PointF(from.X + _offset.X, from.Y + _offset.Y), new PointF(to.X + _offset.X, to.Y + _offset.Y)));
    }

    public IDrawingContainer BeginContainer(RectangleF destRect, RectangleF srcRect)
    {
      var previous = _offset;
      _offset = new PointF(previous.X + destRect.X - srcRect.X, previous.Y + destRect.Y - srcRect.Y);
      return new Container(() => _offset = previous);
    }

    private RectangleF OnPage(RectangleF rect)
    {
      return new RectangleF(rect.X + _offset.X, rect.Y + _offset.Y, rect.Width, rect.Height);
    }

    private sealed class Container : IDrawingContainer
    {
      private readonly Action _end;

      public Container(Action end)
      {
        _end = end;
      }

      public void End()
      {
        _end();
      }

      public void Dispose()
      {
        End();
      }
    }
  }
}
