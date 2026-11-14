using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace Theaterverdeling
{
  public partial class Theaterverdeling : Form
  {
    public Theaterverdeling()
    {
      InitializeComponent();
    }

    private Plan _plan = new Plan();

    // The preview of the last distribution, while it is open.
    private PreviewForm _preview;

    // Divides the groups over the showings and shows the outcome in the preview window,
    // from where it can be exported to PDF.
    private void Divideshows(object sender, EventArgs eventArgs)
    {
      if (_plan.Groups.Sum(group => group.Personen) > _plan.Shows.Sum(show => show.Zaal.Seats))
      {
        MessageBox.Show("Te weinig stoelen");
        return;
      }

      _plan.ShowName = voorstellingnaambox.Text;
      var unplaced = _plan.Distribute();
      Groupviewerupdate();
      Voorstellingviewerupdate();
      if (unplaced.Count > 0)
      {
        MessageBox.Show("Niet ingedeeld:" + Environment.NewLine + string.Join(Environment.NewLine, unplaced.Select(group => group.Name)));
      }

      ClosePreview();
      _preview = new PreviewForm(_plan);
      _preview.Show();
    }

    // The preview shows the last distribution, so it goes away as soon as the plan changes.
    private void ClosePreview()
    {
      _preview?.Close();
      _preview = null;
    }

    private void ShowNameChanged(object sender, EventArgs e)
    {
      ClosePreview();
    }

    #region Shows
    private void AddshowClick(object sender, EventArgs e)
    {
      OpenShowDialog(null);
    }

    private void EditshowClick(object sender, EventArgs e)
    {
      if (Voorstellingviewer.SelectedIndices.Count > 0)
      {
        OpenShowDialog(_plan.Shows[Voorstellingviewer.SelectedIndices[0]]);
      }
    }

    // Opens the showing dialog, to add a showing or to change the given one.
    private void OpenShowDialog(Voorstelling editing)
    {
      var addshowform = new Addshowform(_plan, editing);
      addshowform.Addingshow += AddshowformAddingshow;
      addshowform.Show();
    }

    void AddshowformAddingshow(object sender, EventArgs e)
    {
      if (sender is Addshowform addShowForm)
      {
        if (addShowForm.Editing != null)
        {
          _plan.ReplaceShow(addShowForm.Editing, addShowForm.show);
        }
        else
        {
          _plan.Shows.Add(addShowForm.show);
        }
        Voorstellingviewerupdate();
        ClosePreview();
        Updatestoelen();
      }
    }
    void Voorstellingviewerupdate()
    {
      Voorstellingviewer.Items.Clear();
      foreach (var show in _plan.Shows)
      {
        // The list is narrow, so the full text is also shown as tooltip.
        Voorstellingviewer.Items.Add(show.Description).ToolTipText = show.Description;
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
        OpenGroupDialog(null);
      }
    }

    private void BulkgroupClick(object sender, EventArgs e)
    {
      if (_plan.Shows.Count == 0)
      {
        MessageBox.Show("Voeg eerst de voorstelling toe");
        return;
      }
      var bulkgroupform = new Bulkgroupform(_plan);
      bulkgroupform.Addinggroups += BulkgroupformAddinggroups;
      bulkgroupform.Show();
    }

    void BulkgroupformAddinggroups(object sender, EventArgs e)
    {
      if (sender is Bulkgroupform bulkGroupForm)
      {
        _plan.Groups.AddRange(bulkGroupForm.Groups);
        Groupviewerupdate();
        ClosePreview();
        Updatestoelen();
      }
    }

    private void EditgroupClick(object sender, EventArgs e)
    {
      if (Groupviewer.SelectedIndices.Count > 0)
      {
        OpenGroupDialog(_plan.Groups[Groupviewer.SelectedIndices[0]]);
      }
    }

    // Opens the group dialog, to add a group or to change the given one.
    private void OpenGroupDialog(Group editing)
    {
      var addgroupform = new Addgroupform(_plan, editing);
      addgroupform.Addinggroup += AddgroupformAddinggroup;
      addgroupform.Show();
    }

    void AddgroupformAddinggroup(object sender, EventArgs e)
    {
      if (sender is Addgroupform addGroupForm)
      {
        if (addGroupForm.Editing != null)
        {
          _plan.ReplaceGroup(addGroupForm.Editing, addGroupForm.Group);
        }
        else
        {
          _plan.Groups.Add(addGroupForm.Group);
        }
        Groupviewerupdate();
        ClosePreview();
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
      ClosePreview();
      Groupviewer.Items.Clear();
      Voorstellingviewer.Items.Clear();
      voorstellingnaambox.Clear();
      Updatestoelen();
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
      ClosePreview();
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
      ClosePreview();
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
      ClosePreview();
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
