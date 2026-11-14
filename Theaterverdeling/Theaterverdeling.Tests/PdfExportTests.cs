using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfSharpCore.Pdf.IO;
using Xunit;

namespace Theaterverdeling.Tests
{
  public class PdfExportTests
  {
    [Fact]
    public void Export_writes_one_file_with_a_page_for_every_showing()
    {
      var path = NewPdfPath();

      PdfExport.Write(DistributedPlan(), path);

      Assert.Equal(2, PageCount(path));
    }

    [Fact]
    public void Export_writes_a_separate_file_for_every_showing_next_to_it()
    {
      var path = NewPdfPath();

      PdfExport.Write(DistributedPlan(), path);

      var withoutExtension = path.Substring(0, path.Length - ".pdf".Length);
      Assert.Equal(1, PageCount(withoutExtension + " Voorstelling 1.pdf"));
      Assert.Equal(1, PageCount(withoutExtension + " Voorstelling 2.pdf"));
    }

    [Fact]
    public void Export_tells_which_files_it_wrote()
    {
      var path = NewPdfPath();

      var written = PdfExport.Write(DistributedPlan(), path);

      Assert.Equal(3, written.Count);
      Assert.Equal(path, written[0]);
      Assert.All(written, file => Assert.True(File.Exists(file)));
    }

    private static Plan DistributedPlan()
    {
      var monday = new DateTime(2026, 3, 16);
      var tuesday = new DateTime(2026, 3, 17);
      var plan = new Plan { ShowName = "Kikker is jarig" };
      plan.Shows.Add(new Voorstelling(monday.AddHours(10), Zaal.Load("Lampegiet"), monday));
      plan.Shows.Add(new Voorstelling(tuesday.AddHours(10), Zaal.Load("Lampegiet"), tuesday));
      plan.Groups.Add(new Group("De Regenboog", 28, false, false, new List<DateTime> { monday }));
      plan.Groups.Add(new Group("Het Kompas", 31, false, false, new List<DateTime> { tuesday }));
      plan.Distribute();
      return plan;
    }

    private static int PageCount(string path)
    {
      using var document = PdfReader.Open(path, PdfDocumentOpenMode.InformationOnly);
      return document.PageCount;
    }

    private static string NewPdfPath()
    {
      var folder = Path.Combine(AppContext.BaseDirectory, "testpdfs", Guid.NewGuid().ToString());
      Directory.CreateDirectory(folder);
      return Path.Combine(folder, "Indeling.pdf");
    }
  }
}
