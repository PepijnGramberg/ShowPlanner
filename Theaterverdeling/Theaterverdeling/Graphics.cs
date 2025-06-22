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
    void DrawRectangles(IReadOnlyCollection<RectangleF> rects);
    void FillRectangle(RectangleF rect, Color color);

    void DrawString(string text, RectangleF rect);
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
    private readonly XFont _font = new XFont("Verdana", 11);

    public XGraphicsDrawingContext(XGraphics graphics)
    {
      _graphics = graphics;
    }

    public float PageWidth => (float)_graphics.PageSize.Width;

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

  // GDI+ Graphics implementation of IDrawingContext
  public class GdiDrawingContext : IDrawingContext
  {
    private readonly Graphics _graphics;

    public GdiDrawingContext(Graphics graphics)
    {
      _graphics = graphics;
    }

    public float PageWidth => _graphics.VisibleClipBounds.Width;

    public void DrawRectangles(IReadOnlyCollection<RectangleF> rects)
    {
        _graphics.DrawRectangles(Pens.Black, rects.ToArray());
    }

    public void FillRectangle(RectangleF rect, Color color)
    {
        _graphics.FillRectangle(new SolidBrush(color),rect);
    }

    public void DrawString(string text, RectangleF rect)
    {
      _graphics.DrawString(text, new Font("Verdana", 11), Brushes.Black, rect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
    }

    public IDrawingContainer BeginContainer(RectangleF destRect, RectangleF srcRect)
    {
      return new GdiContainer(_graphics, destRect, srcRect);
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
