using System;
using System.Collections.Generic;
using System.Drawing;

namespace Theaterverdeling.Tests
{
  // Drawing surface that remembers what was drawn, in page coordinates.
  internal sealed class RecordingDrawingContext : IDrawingContext
  {
    private PointF _offset;
    private SizeF _scale = new SizeF(1, 1);

    public List<(RectangleF Rect, Color Color)> Fills { get; } = new List<(RectangleF, Color)>();
    public List<(string Text, RectangleF Rect)> Texts { get; } = new List<(string, RectangleF)>();
    public List<(PointF From, PointF To)> Lines { get; } = new List<(PointF, PointF)>();

    // How much smaller than its normal size each text in Texts is drawn: 1 for the normal size.
    public List<float> TextScales { get; } = new List<float>();

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
      DrawText(text, rect);
    }

    public void DrawText(string text, RectangleF rect)
    {
      Texts.Add((text, OnPage(rect)));
      TextScales.Add(_scale.Height);
    }

    public void DrawLine(PointF from, PointF to)
    {
      Lines.Add((OnPage(from), OnPage(to)));
    }

    public IDrawingContainer BeginContainer(RectangleF destRect, RectangleF srcRect)
    {
      var offset = _offset;
      var scale = _scale;
      var x = destRect.Width / srcRect.Width;
      var y = destRect.Height / srcRect.Height;
      _offset = new PointF(offset.X + scale.Width * (destRect.X - srcRect.X * x), offset.Y + scale.Height * (destRect.Y - srcRect.Y * y));
      _scale = new SizeF(scale.Width * x, scale.Height * y);
      return new Container(() =>
      {
        _offset = offset;
        _scale = scale;
      });
    }

    private PointF OnPage(PointF point)
    {
      return new PointF(point.X * _scale.Width + _offset.X, point.Y * _scale.Height + _offset.Y);
    }

    private RectangleF OnPage(RectangleF rect)
    {
      return new RectangleF(OnPage(rect.Location), new SizeF(rect.Width * _scale.Width, rect.Height * _scale.Height));
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
