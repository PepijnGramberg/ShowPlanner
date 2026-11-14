using System.Collections.Generic;
using System.IO;
using System.Linq;
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

      // The files of an earlier export under this name go first, so no file stays behind of a
      // showing that is gone.
      foreach (var earlier in EarlierFiles(path))
      {
        File.Delete(earlier);
      }

      var folder = Path.GetDirectoryName(Path.GetFullPath(path));
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

    // The files per showing that an earlier export to this path left next to it.
    public static List<string> EarlierFiles(string path)
    {
      var folder = Path.GetDirectoryName(Path.GetFullPath(path));
      var start = Path.GetFileNameWithoutExtension(path) + " Voorstelling ";
      if (!Directory.Exists(folder))
      {
        return new List<string>();
      }
      return Directory.GetFiles(folder, start + "*.pdf")
        .Where(file => int.TryParse(Path.GetFileNameWithoutExtension(file).Substring(start.Length), out _))
        .ToList();
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
