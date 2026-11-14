using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Theaterverdeling
{
  // Draws the page of a showing as an image, with the same drawing code as the PDF.
  public static class PagePreview
  {
    // An A4 page in points, the size of a PDF page.
    public static readonly SizeF PageSize = new SizeF(595, 842);

    // Scale 1 gives one pixel per point; a higher scale gives a sharper image.
    public static Bitmap Render(Voorstelling show, string showName, float scale)
    {
      var image = new Bitmap((int)(PageSize.Width * scale), (int)(PageSize.Height * scale));
      using var graphics = Graphics.FromImage(image);
      graphics.Clear(Color.White);
      graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
      graphics.SmoothingMode = SmoothingMode.None;
      graphics.ScaleTransform(scale, scale);
      show.Draw(new GdiDrawingContext(graphics, PageSize), showName);
      return image;
    }
  }
}
