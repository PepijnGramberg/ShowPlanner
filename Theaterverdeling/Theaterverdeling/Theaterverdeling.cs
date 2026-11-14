using System;
using System.Collections.Generic;
using System.Globalization;
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

    /// <summary>
    /// Saves the current state of shows and groups to a file.
    /// Ensures proper disposal of file resources.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void SaveButton_Click(object sender, EventArgs e)
    {
      using (var saveFileDialog = new SaveFileDialog { Filter = "WETbestand (*.wet)|*.wet" })
      {
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
          try
          {
            using var writer = new StreamWriter(saveFileDialog.FileName);
            writer.WriteLine("#THEATERVERDEELFILE#");
            writer.WriteLine(voorstellingnaambox.Text);
            writer.WriteLine("#VOORSTELLING#");
            foreach (var voorstelling in _shows)
            {
             //todo  writer.WriteLine(voorstelling.Zaal.Balconyoff);
              writer.WriteLine(voorstelling.Datum.ToString());
              writer.WriteLine(voorstelling.Time.ToString());
            }
            writer.WriteLine("#GROEPEN#");
            foreach (var group in _groups)
            {
              writer.WriteLine(group.Name);
              writer.WriteLine(group.Personen);
              writer.WriteLine(group.Firstrows);
              writer.WriteLine(group.Morning);
              foreach (var data in group.Datum)
              {
                writer.WriteLine(data.Date);
              }
            }
            writer.WriteLine("#END#");
          }
          catch (Exception ex)
          {
            MessageBox.Show($"Error saving file: {ex.Message}");
          }
        }
      }
    }

    /// <summary>
    /// Loads shows and groups from a file.
    /// Ensures proper disposal of file resources.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void LoadButton_Click(object sender, EventArgs e)
    {
      using (var openFileDialog = new OpenFileDialog { Filter = "WETbestand (*.wet)|*.wet" })
      {
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
          try
          {
            using var reader = new StreamReader(openFileDialog.FileName);
            var line = reader.ReadLine();
            if (line != "#THEATERVERDEELFILE#")
              MessageBox.Show("Corrupted File");
            else
            {
              voorstellingnaambox.Text = reader.ReadLine();
              reader.ReadLine();
              line = reader.ReadLine();
              _shows.Clear();
              while (line != "#GROEPEN#")
              {
                bool balcony = false;
                if (line == "True")
                {
                  balcony = true;
                  line = reader.ReadLine();
                }
                if (line == "False")
                  line = reader.ReadLine();
                var date = DateTime.ParseExact(line, "d-M-yyyy H:mm:ss", CultureInfo.InvariantCulture,
                                               DateTimeStyles.AllowLeadingWhite);
                line = reader.ReadLine();
                var time = DateTime.ParseExact(line, "d-M-yyyy H:mm:ss", CultureInfo.InvariantCulture,
                                               DateTimeStyles.AllowLeadingWhite);
               // todo _shows.Add(new Voorstelling(time, hall, date));
                line = reader.ReadLine();
              }
              line = reader.ReadLine();
              while (line != "#END#")
              {
                var name = line;
                var pers = int.Parse(reader.ReadLine());
                var first = bool.Parse(reader.ReadLine());
                var morn = bool.Parse(reader.ReadLine());
                var temp = new List<DateTime>();
                DateTime dt;
                line = reader.ReadLine();
                while (DateTime.TryParseExact(line, "d-M-yyyy H:mm:ss", CultureInfo.InvariantCulture,
                                              DateTimeStyles.AllowLeadingWhite, out dt))
                {
                  temp.Add(dt);
                  line = reader.ReadLine();
                }
                _groups.Add(new Group(name, pers, first, morn, temp));
                // line = reader.ReadLine();
              }

              Voorstellingviewerupdate();
              Groupviewerupdate();
              Updatestoelen();
            }
          }
          catch (Exception ex)
          {
            MessageBox.Show($"Error loading file: {ex.Message}");
          }
        }
      }
    }

  }
  #endregion
}

