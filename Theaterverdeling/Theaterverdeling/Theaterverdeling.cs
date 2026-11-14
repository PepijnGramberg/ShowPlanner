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

    private List<Voorstelling> _shows = new List<Voorstelling>();
    private List<Group> _groups = new List<Group>();

    /// <summary>
    /// Assigns groups to shows and generates PDF documents with the seating arrangements.
    /// Ensures proper disposal of all resources.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="eventArgs">Event arguments.</param>
    private void Divideshows(object sender, EventArgs eventArgs)
    {
      if (_groups.Sum(group => group.Personen) > _shows.Sum(show => show.Zaal.Seats))
        MessageBox.Show("Te weinig stoelen");
      else
      {
        foreach (var voorstelling in _shows)
        {
          voorstelling.Groups = new List<Group>();
        }
        _groups = Groupmergesort(_groups);
        Groupviewerupdate();
        foreach (var gGroup in _groups)
        {
          var found = false;
          _shows.Sort(Zaalsort);
          foreach (var voorstelling in _shows)
          {
            if (((gGroup.Morning && voorstelling.Time.Hour < 12) ||
                 !gGroup.Morning) &&
                voorstelling.Freeseats >= gGroup.Personen &&
                gGroup.Datum.Any(date => date == voorstelling.Datum) && !found)
            {
              gGroup.Voorstelling = voorstelling;
              voorstelling.Groups.Add(gGroup);
              found = true;
            }
          }
          if (!found)
            foreach (var voorstelling in _shows)
            {
              if (((gGroup.Morning && voorstelling.Time.Hour < 12) ||
               !gGroup.Morning) &&
                  voorstelling.Freeseats >= gGroup.Personen && !found)
              {
                gGroup.Voorstelling = voorstelling;
                voorstelling.Groups.Add(gGroup);
                found = true;
              }
            }
          if (!found)
          {
            MessageBox.Show($"De groep \"{gGroup.Name}\" is niet ingedeeld!");
          }
        }

        foreach (var voorstelling in _shows)
        {
          voorstelling.Assignseats();
        }
        _shows.Sort(Zaaldatesort);

        // Proper resource management for PDF documents
        using var pdf = new PdfDocument();
        var pdfs = new List<PdfDocument>();

        foreach (var voorstelling in _shows)
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
            // Log exception instead of swallowing it
            MessageBox.Show($"Error creating PDF for show on {voorstelling.Datum.ToShortDateString()}: {ex.Message}");
          }
        }

        // When creating a save dialog, wrap with using
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
                using (pdfs[i])
                {
                  pdfs[i].Save(saveFileDialog.FileName.Substring(0, saveFileDialog.FileName.Length - 4) +
                           " Voorstelling " + (i + 1) + ".pdf");
                }
              }
            }
            catch (Exception ex)
            {
              MessageBox.Show($"Error saving PDF files: {ex.Message}");
            }
          }
        }

        // Clean up any remaining PDF documents in case of exceptions
        foreach (var pdfDoc in pdfs.Where(p => p != null))
        {
          try { pdfDoc.Dispose(); } catch { /* Ignore errors during cleanup */ }
        }

        Groupviewerupdate();
        Voorstellingviewerupdate();
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
        _shows.Add(show);
        Voorstellingviewerupdate();

        Updatestoelen();
      }
    }
    void Voorstellingviewerupdate()
    {
      Voorstellingviewer.Items.Clear();
      foreach (var show in _shows)
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
    private int Zaalsort(Voorstelling x, Voorstelling y) => x.Freeseats < y.Freeseats ? 1 : 0;
    private int Zaaldatesort(Voorstelling x, Voorstelling y)
    {
      if (x.Datum.Date < y.Datum.Date)
        return 0;
      if (x.Datum.Date > y.Datum.Date)
        return 1;
      return x.Time.TimeOfDay < y.Time.TimeOfDay ? 0 : 1;
    }
    #endregion
    private void Updatestoelen()
    {
      var personen = _groups.Sum(group => group.Personen);
      var stoelen = _shows.Sum(show => show.Zaal.Seats);
      Seatlabel.Text = $"{personen}/{stoelen} stoelen";
    }
    #region Groepen
    private void AddgroupClick(object sender, EventArgs e)
    {
      if (_shows.Count == 0)
        MessageBox.Show("Voeg eerst de voorstelling toe");
      else
      {
        var data = _shows.Select(voorstelling => voorstelling.Datum).ToList();
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
        _groups.Add(group);
        Groupviewerupdate();
        Updatestoelen();
      }
    }

    private void Groupviewerupdate()
    {
      Groupviewer.Items.Clear();
      foreach (var group in _groups)
      {
        var text = $"{group.Name} {group.Personen}{(group.Morning ? "  O" : "")}{(group.Firstrows ? " V" : "")}";
        Groupviewer.Items.Add(text);
      }
    }
    private List<Group> Groupmergesort(List<Group> groups)
    {
      if (groups.Count < 2)
        return groups;
      var group1 = Groupmergesort(groups.GetRange(0, groups.Count / 2));
      var group2 = Groupmergesort(groups.GetRange(groups.Count / 2, groups.Count - (groups.Count / 2)));
      var returngroup = new List<Group>();
      while (group1.Count > 0 || group2.Count > 0)
      {
        if (group2.Count == 0 || (group1.Count > 0 && Groupsort(group1[0], group2[0]) == 1))
        {
          returngroup.Add(group1[0]);
          group1.RemoveAt(0);
        }
        else
        {
          returngroup.Add(group2[0]);
          group2.RemoveAt(0);
        }
      }
      return returngroup;

    }
    private int Groupsort(Group x, Group y)
    {
      if (y.Firstrows && !x.Firstrows) return 0;
      if (x.Firstrows && !y.Firstrows) return 1;
      if (y.Morning && !x.Morning) return 0;
      if (x.Morning && !y.Morning) return 1;
      return x.Personen > y.Personen ? 1 : 0;
    }

    #endregion

    #region ButtonClicks
    private void ResetButtonClick(object sender, EventArgs e)
    {
      _groups = new List<Group>();
      _shows = new List<Voorstelling>();
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
        _shows.RemoveAt(indexes[i]);
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
        _groups.RemoveAt(indexes[i]);
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
      var plan = new Plan { ShowName = voorstellingnaambox.Text };
      plan.Shows.AddRange(_shows);
      plan.Groups.AddRange(_groups);
      plan.Save(path);
    }

    // Replaces the plan on screen with the plan in a file.
    // When the file cannot be read, the plan on screen stays as it was.
    public void LoadPlan(string path)
    {
      var plan = Plan.Load(path);
      voorstellingnaambox.Text = plan.ShowName;
      _shows = plan.Shows;
      _groups = plan.Groups;
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
