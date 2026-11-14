using System;
using System.Windows.Forms;

namespace Theaterverdeling
{
  // Shows the pages of a distributed plan on screen, one showing at a time, and exports them to PDF.
  public partial class PreviewForm : Form
  {
    // Pixels per point of the page image. Two keeps the text sharp when the window is large.
    private const float ImageScale = 2;

    private readonly Plan _plan;

    public PreviewForm(Plan plan)
    {
      InitializeComponent();
      _plan = plan;
      foreach (var show in plan.Shows)
      {
        Showingslist.Items.Add(plan.Describe(show));
      }
      if (Showingslist.Items.Count > 0)
      {
        Showingslist.SelectedIndex = 0;
      }
    }

    // Writes the pages to PDF: one file with all showings and one file per showing.
    public void ExportPdf(string path)
    {
      PdfExport.Write(_plan, path);
    }

    private void ShowingslistSelectedIndexChanged(object sender, EventArgs e)
    {
      var previous = Pagebox.Image;
      Pagebox.Image = Showingslist.SelectedIndex < 0
        ? null
        : PagePreview.Render(_plan.Shows[Showingslist.SelectedIndex], _plan.ShowName, ImageScale);
      previous?.Dispose();
    }

    private void ExportbuttonClick(object sender, EventArgs e)
    {
      using var dialog = new SaveFileDialog { Filter = "pdf (*.pdf)|*.pdf" };
      if (dialog.ShowDialog() != DialogResult.OK)
      {
        return;
      }
      try
      {
        ExportPdf(dialog.FileName);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Exporteren is niet gelukt: {ex.Message}");
      }
    }
  }
}
