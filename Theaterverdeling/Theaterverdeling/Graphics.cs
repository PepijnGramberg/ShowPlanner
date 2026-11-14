using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;

namespace Theaterverdeling
{
  // Drawing abstraction interface
  public interface IDrawingContext
  {
    float PageWidth { get; }
    float PageHeight { get; }
    void DrawRectangles(IReadOnlyCollection<RectangleF> rects);
    void FillRectangle(RectangleF rect, Color color);

    // Centred in the rectangle.
    void DrawString(string text, RectangleF rect);

    // Left aligned and vertically centred in the rectangle.
    void DrawText(string text, RectangleF rect);

    void DrawLine(PointF from, PointF to);
    IDrawingContainer BeginContainer(RectangleF destRect, RectangleF srcRect);
  }

  // Container interface for saving graphics state
  public interface IDrawingContainer : IDisposable
  {
    void End();
  }

  // XGraphics implementation of IDrawingContext
  public class XGraphicsDrawingContext : IDrawingContext
  {
    private readonly XGraphics _graphics;
    private readonly XFont _font = new XFont("Verdana", 10);

    public XGraphicsDrawingContext(XGraphics graphics)
    {
      _graphics = graphics;
    }

    public float PageWidth => (float)_graphics.PageSize.Width;
    public float PageHeight => (float)_graphics.PageSize.Height;

    public void DrawRectangles(IReadOnlyCollection<RectangleF> rects)
    {
      var xRects = rects.Select(r => new XRect(r.X, r.Y, r.Width, r.Height)).ToArray();
      _graphics.DrawRectangles(XPens.Black, xRects);
    }

    public void FillRectangle(RectangleF rect, Color color)
    {
      _graphics.DrawRectangle(new XSolidBrush(XColor.FromArgb(color.ToArgb())), new XRect(rect.X, rect.Y, rect.Width, rect.Height));
    }

    public void DrawString(string text, RectangleF rect)
    {
      _graphics.DrawString(text, _font, XBrushes.Black,
                          new XRect(rect.X, rect.Y, rect.Width, rect.Height),
                          XStringFormats.Center);
    }

    public void DrawText(string text, RectangleF rect)
    {
      _graphics.DrawString(text, _font, XBrushes.Black,
                          new XRect(rect.X, rect.Y, rect.Width, rect.Height),
                          XStringFormats.CenterLeft);
    }

    public void DrawLine(PointF from, PointF to)
    {
      _graphics.DrawLine(XPens.Black, from.X, from.Y, to.X, to.Y);
    }

    public IDrawingContainer BeginContainer(RectangleF destRect, RectangleF srcRect)
    {
      return new XGraphicsContainer(_graphics,
                                   new XRect(destRect.X, destRect.Y, destRect.Width, destRect.Height),
                                   new XRect(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height));
    }

    private class XGraphicsContainer : IDrawingContainer
    {
      private readonly XGraphics _graphics;
      private readonly PdfSharpCore.Drawing.XGraphicsContainer _container;

      public XGraphicsContainer(XGraphics graphics, XRect destRect, XRect srcRect)
      {
        _graphics = graphics;
        _container = _graphics.BeginContainer(destRect, srcRect, XGraphicsUnit.Point);
      }

      public void End()
      {
        _graphics.EndContainer(_container);
      }

      public void Dispose()
      {
        End();
      }
    }
  }

  // GDI+ Graphics implementation of IDrawingContext, for drawing a page on screen.
  // The page is measured in points, like the PDF page; scale the Graphics to fit it where it is shown.
  public class GdiDrawingContext : IDrawingContext
  {
    private readonly Graphics _graphics;
    private readonly SizeF _pageSize;
    // Sized in the units of the page, so the text scales along with the Graphics.
    // One font and one pen for every context: a context is made for each paint, and nothing disposes it.
    private static readonly Font _font = new Font("Verdana", 10, GraphicsUnit.World);
    private static readonly Pen _pen = new Pen(Color.Black, 0.5f);

    public GdiDrawingContext(Graphics graphics, SizeF pageSize)
    {
      _graphics = graphics;
      _pageSize = pageSize;
    }

    public float PageWidth => _pageSize.Width;
    public float PageHeight => _pageSize.Height;

    public void DrawRectangles(IReadOnlyCollection<RectangleF> rects)
    {
      _graphics.DrawRectangles(_pen, rects.ToArray());
    }

    public void FillRectangle(RectangleF rect, Color color)
    {
      using var brush = new SolidBrush(color);
      _graphics.FillRectangle(brush, rect);
    }

    public void DrawString(string text, RectangleF rect)
    {
      using var format = Format(StringAlignment.Center);
      _graphics.DrawString(text, _font, Brushes.Black, rect, format);
    }

    public void DrawText(string text, RectangleF rect)
    {
      using var format = Format(StringAlignment.Near);
      _graphics.DrawString(text, _font, Brushes.Black, rect, format);
    }

    public void DrawLine(PointF from, PointF to)
    {
      _graphics.DrawLine(_pen, from, to);
    }

    public IDrawingContainer BeginContainer(RectangleF destRect, RectangleF srcRect)
    {
      return new GdiContainer(_graphics, destRect, srcRect);
    }

    // Text keeps to one line and is not cut off when it is a little wider than its box, like in the PDF.
    private static StringFormat Format(StringAlignment alignment)
    {
      return new StringFormat(StringFormat.GenericTypographic)
      {
        Alignment = alignment,
        LineAlignment = StringAlignment.Center,
        FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip
      };
    }

    private class GdiContainer : IDrawingContainer
    {
      private readonly Graphics _graphics;
      private readonly GraphicsState _state;

      public GdiContainer(Graphics graphics, RectangleF destRect, RectangleF srcRect)
      {
        _graphics = graphics;
        _state = _graphics.Save();

        // Calculate transform to map srcRect to destRect
        float scaleX = destRect.Width / srcRect.Width;
        float scaleY = destRect.Height / srcRect.Height;

        _graphics.TranslateTransform(destRect.X - srcRect.X * scaleX, destRect.Y - srcRect.Y * scaleY);
        _graphics.ScaleTransform(scaleX, scaleY);
      }

      public void End()
      {
        _graphics.Restore(_state);
      }

      public void Dispose()
      {
        End();
      }
    }
  }

}
