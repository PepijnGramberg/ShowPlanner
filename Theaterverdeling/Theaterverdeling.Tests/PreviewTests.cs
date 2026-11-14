using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PdfSharpCore.Pdf.IO;
using Xunit;
using static Theaterverdeling.Tests.FormDriver;

namespace Theaterverdeling.Tests
{
  // Form tests share the list of open windows, so they must not run side by side.
  [Collection("Forms")]
  public class PreviewTests
  {
    private static readonly DateTime Saturday = new DateTime(2026, 3, 14);
    private static readonly DateTime Sunday = new DateTime(2026, 3, 15);

    [Fact]
    public void Distributing_opens_the_preview_on_the_first_showing()
    {
      OnUiThread(() =>
      {
        using var main = PlanOnScreen();

        Find<Button>(main, "Dividebutton").PerformClick();

        var preview = Application.OpenForms.OfType<PreviewForm>().Single();
        var showings = Find<ListBox>(preview, "Showingslist");
        Assert.Equal(new[] { "14-03-2026 10:30", "15-03-2026 13:00" }, showings.Items.Cast<string>());
        Assert.Equal(0, showings.SelectedIndex);
        Assert.NotNull(Find<PictureBox>(preview, "Pagebox").Image);
      });
    }

    [Fact]
    public void Preview_shows_the_page_of_the_showing_that_is_chosen()
    {
      OnUiThread(() =>
      {
        using var main = PlanOnScreen();
        Find<Button>(main, "Dividebutton").PerformClick();
        var preview = Application.OpenForms.OfType<PreviewForm>().Single();
        var page = Find<PictureBox>(preview, "Pagebox");

        // The only group sits in the first showing.
        Assert.True(Contains((Bitmap)page.Image, Zaal.GroupColor(0)));

        Find<ListBox>(preview, "Showingslist").SelectedIndex = 1;

        Assert.False(Contains((Bitmap)page.Image, Zaal.GroupColor(0)));
      });
    }

    [Fact]
    public void Distributing_again_replaces_the_preview()
    {
      OnUiThread(() =>
      {
        using var main = PlanOnScreen();

        Find<Button>(main, "Dividebutton").PerformClick();
        Find<Button>(main, "Dividebutton").PerformClick();

        Assert.Single(Application.OpenForms.OfType<PreviewForm>());
      });
    }

    [Fact]
    public void Changing_the_plan_closes_the_preview_because_it_is_out_of_date()
    {
      OnUiThread(() =>
      {
        using var main = PlanOnScreen();
        Find<Button>(main, "Dividebutton").PerformClick();

        AddGroup(main, "Het Kompas", 31);

        Assert.Empty(Application.OpenForms.OfType<PreviewForm>());
      });
    }

    [Fact]
    public void Changing_the_show_name_closes_the_preview_because_it_is_out_of_date()
    {
      OnUiThread(() =>
      {
        using var main = PlanOnScreen();
        Find<Button>(main, "Dividebutton").PerformClick();

        Find<TextBox>(main, "voorstellingnaambox").Text = "Kikker in de kou";

        Assert.Empty(Application.OpenForms.OfType<PreviewForm>());
      });
    }

    [Fact]
    public void Preview_exports_the_showings_to_pdf()
    {
      OnUiThread(() =>
      {
        using var main = PlanOnScreen();
        Find<Button>(main, "Dividebutton").PerformClick();
        var preview = Application.OpenForms.OfType<PreviewForm>().Single();
        var folder = Path.Combine(AppContext.BaseDirectory, "testpdfs", Guid.NewGuid().ToString());
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, "Indeling.pdf");

        preview.ExportPdf(path);

        using var document = PdfReader.Open(path, PdfDocumentOpenMode.InformationOnly);
        Assert.Equal(2, document.PageCount);
        Assert.True(File.Exists(Path.Combine(folder, "Indeling Voorstelling 2.pdf")));
      });
    }

    // Two showings, entered latest first, and one group.
    private static Theaterverdeling PlanOnScreen()
    {
      var main = new Theaterverdeling();
      main.Show();
      Find<TextBox>(main, "voorstellingnaambox").Text = "Kikker is jarig";
      AddShowing(main, Sunday, Sunday.AddHours(13));
      AddShowing(main, Saturday, Saturday.AddHours(10).AddMinutes(30));
      AddGroup(main, "De Regenboog", 28);
      return main;
    }

    private static bool Contains(Bitmap image, Color colour)
    {
      // Seats are far larger than three pixels, so every third pixel is enough.
      for (var y = 0; y < image.Height; y += 3)
      {
        for (var x = 0; x < image.Width; x += 3)
        {
          if (image.GetPixel(x, y).ToArgb() == colour.ToArgb())
          {
            return true;
          }
        }
      }
      return false;
    }
  }
}
