using System.Collections.Generic;
using System.IO;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace Theaterverdeling
{
  // Writes the seating of a distributed plan to PDF files.
  public static class PdfExport
  {
    // Writes one file with a page for every showing to the given path, and next to it one file
    // per showing. Returns the files it wrote, the complete one first.
    public static List<string> Write(Plan plan, string path)
    {
      var written = new List<string>();

      using (var all = new PdfDocument())
      {
        foreach (var show in plan.Shows)
        {
          AddPage(all, show, plan.ShowName);
        }
        all.Save(path);
        written.Add(path);
      }

      var folder = Path.GetDirectoryName(path);
      var name = Path.GetFileNameWithoutExtension(path);
      for (var i = 0; i < plan.Shows.Count; i++)
      {
        using var single = new PdfDocument();
        AddPage(single, plan.Shows[i], plan.ShowName);
        var file = Path.Combine(folder, $"{name} Voorstelling {i + 1}.pdf");
        single.Save(file);
        written.Add(file);
      }
      return written;
    }

    private static void AddPage(PdfDocument document, Voorstelling show, string showName)
    {
      var page = new PdfPage();
      document.AddPage(page);
      using var graphics = XGraphics.FromPdfPage(page);
      show.Draw(new XGraphicsDrawingContext(graphics), showName);
    }
  }
}
