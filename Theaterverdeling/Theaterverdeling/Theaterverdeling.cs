using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System.IO;


namespace Theaterverdeling
{
  public partial class Theaterverdeling : Form
  {
    public Theaterverdeling()
    {
      InitializeComponent();
    }

    private Plan _plan = new Plan();

    // Divides the groups over the showings and writes the seating to PDF files:
    // one file with all showings and one file per showing.
    private void Divideshows(object sender, EventArgs eventArgs)
    {
      if (_plan.Groups.Sum(group => group.Personen) > _plan.Shows.Sum(show => show.Zaal.Seats))
      {
        MessageBox.Show("Te weinig stoelen");
        return;
      }

      var unplaced = _plan.Distribute();
      Groupviewerupdate();
      Voorstellingviewerupdate();
      if (unplaced.Count > 0)
      {
        MessageBox.Show("Niet ingedeeld:" + Environment.NewLine + string.Join(Environment.NewLine, unplaced.Select(group => group.Name)));
      }

      using var pdf = new PdfDocument();
      var pdfs = new List<PdfDocument>();

      foreach (var voorstelling in _plan.Shows)
      {
        try
        {
          var page = new PdfPage();
          pdf.AddPage(page);
          using var gr = XGraphics.FromPdfPage(page);
          voorstelling.Draw(gr, voorstellingnaambox.Text);

          var pdf1 = new PdfDocument();
          pdfs.Add(pdf1);
          var page1 = new PdfPage();
          pdf1.AddPage(page1);
          using var gr1 = XGraphics.FromPdfPage(page1);
          voorstelling.Draw(gr1, voorstellingnaambox.Text);
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Error creating PDF for show on {voorstelling.Datum.ToShortDateString()}: {ex.Message}");
        }
      }

      using (var saveFileDialog = new SaveFileDialog())
      {
        saveFileDialog.Filter = "pdf (*.pdf)|*.pdf";
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
          try
          {
            pdf.Save(saveFileDialog.FileName);

            for (var i = 0; i < pdfs.Count; i++)
            {
              pdfs[i].Save(saveFileDialog.FileName.Substring(0, saveFileDialog.FileName.Length - 4) +
                           " Voorstelling " + (i + 1) + ".pdf");
            }
          }
          catch (Exception ex)
          {
            MessageBox.Show($"Error saving PDF files: {ex.Message}");
          }
        }
      }

      foreach (var pdfDoc in pdfs)
      {
        pdfDoc.Dispose();
      }
    }

    #region Shows
    private void AddshowClick(object sender, EventArgs e)
    {
      var addshowform = new Addshowform();
      addshowform.Show();
      addshowform.Addingshow += AddshowformAddingshow;
    }

    void AddshowformAddingshow(object sender, EventArgs e)
    {
      if (sender is Addshowform addShowForm)
      {
        var show = addShowForm.show;
        _plan.Shows.Add(show);
        Voorstellingviewerupdate();

        Updatestoelen();
      }
    }
    void Voorstellingviewerupdate()
    {
      Voorstellingviewer.Items.Clear();
      foreach (var show in _plan.Shows)
      {
        var text = $"{show.Datum.ToShortDateString()} {show.Time.ToShortTimeString()}";
        var switchedOff = show.Zaal.SwitchedOffSections;
        if (switchedOff.Count > 0)
        {
          text += $" ({string.Join(", ", switchedOff)} uit)";
        }
        // The list is narrow, so the full text is also shown as tooltip.
        Voorstellingviewer.Items.Add(text).ToolTipText = text;
      }
    }
    #endregion
    private void Updatestoelen()
    {
      var personen = _plan.Groups.Sum(group => group.Personen);
      var stoelen = _plan.Shows.Sum(show => show.Zaal.Seats);
      Seatlabel.Text = $"{personen}/{stoelen} stoelen";
    }
    #region Groepen
    private void AddgroupClick(object sender, EventArgs e)
    {
      if (_plan.Shows.Count == 0)
        MessageBox.Show("Voeg eerst de voorstelling toe");
      else
      {
        var data = _plan.Shows.Select(voorstelling => voorstelling.Datum).ToList();
        var addgroupform = new Addgroupform(data);
        addgroupform.Show();
        addgroupform.Addinggroup += AddgroupformAddinggroup;
      }
    }

    void AddgroupformAddinggroup(object sender, EventArgs e)
    {
      if (sender is Addgroupform addGroupForm)
      {
        var group = addGroupForm.Group;
        _plan.Groups.Add(group);
        Groupviewerupdate();
        Updatestoelen();
      }
    }

    private void Groupviewerupdate()
    {
      Groupviewer.Items.Clear();
      foreach (var group in _plan.Groups)
      {
        var text = $"{group.Name} {group.Personen}{(group.Morning ? "  O" : "")}{(group.Firstrows ? " V" : "")}";
        Groupviewer.Items.Add(text);
      }
    }

    #endregion

    #region ButtonClicks
    private void ResetButtonClick(object sender, EventArgs e)
    {
      _plan = new Plan();
      Groupviewer.Items.Clear();
      Voorstellingviewer.Items.Clear();
      voorstellingnaambox.Clear();
    }

    private void RemoveshowbuttonClick(object sender, EventArgs e)
    {

      var indexes = new int[Voorstellingviewer.SelectedIndices.Count];
      Voorstellingviewer.SelectedIndices.CopyTo(indexes, 0);
      for (var i = 0; i < indexes.Length; i++)
      {
        _plan.Shows.RemoveAt(indexes[i]);
        for (var j = i; j < indexes.Length; j++)
          indexes[j]--;
      }
      Voorstellingviewerupdate();
      Updatestoelen();
    }

    private void RemoveGroupbut_Click(object sender, EventArgs e)
    {
      var indexes = new int[Groupviewer.SelectedIndices.Count];
      Groupviewer.SelectedIndices.CopyTo(indexes, 0);
      for (var i = 0; i < indexes.Length; i++)
      {
        _plan.Groups.RemoveAt(indexes[i]);
        for (var j = i; j < indexes.Length; j++)
          indexes[j]--;
      }
      Groupviewerupdate();
      Updatestoelen();
    }

    private const string PlanFileFilter = "Theaterverdeling plan (*.json)|*.json";

    // Writes the plan that is on screen to a file.
    public void SavePlan(string path)
    {
      _plan.ShowName = voorstellingnaambox.Text;
      _plan.Save(path);
    }

    // Replaces the plan on screen with the plan in a file.
    // When the file cannot be read, the plan on screen stays as it was.
    public void LoadPlan(string path)
    {
      var plan = Plan.Load(path);
      voorstellingnaambox.Text = plan.ShowName;
      _plan = plan;
      Voorstellingviewerupdate();
      Groupviewerupdate();
      Updatestoelen();
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
      using var dialog = new SaveFileDialog { Filter = PlanFileFilter };
      if (dialog.ShowDialog() != DialogResult.OK)
      {
        return;
      }
      try
      {
        SavePlan(dialog.FileName);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Opslaan is niet gelukt: {ex.Message}");
      }
    }

    private void LoadButton_Click(object sender, EventArgs e)
    {
      using var dialog = new OpenFileDialog { Filter = PlanFileFilter };
      if (dialog.ShowDialog() != DialogResult.OK)
      {
        return;
      }
      try
      {
        LoadPlan(dialog.FileName);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Laden is niet gelukt: {ex.Message}");
      }
    }
    #endregion
  }
}
