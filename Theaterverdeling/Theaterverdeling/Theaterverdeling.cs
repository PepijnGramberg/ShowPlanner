using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;


namespace Theaterverdeling
{
  public partial class Theaterverdeling : Form
  {
    public Theaterverdeling()
    {
      InitializeComponent();
      Icon = AppIcon.Value;
      // Columns and tiles are sized in pixels and do not scale with the font, so they are fitted here.
      foreach (var column in Groupviewer.Columns.Cast<ColumnHeader>().Skip(1))
      {
        column.Width = TextRenderer.MeasureText(column.Text, Groupviewer.Font).Width + 14;
      }
      GroupviewerResize(this, EventArgs.Empty);
      Voorstellingviewer.TileSize = new System.Drawing.Size(Voorstellingviewer.ClientSize.Width - 8, Voorstellingviewer.Font.Height * 2 + 6);
    }

    private const string NoShowing = "Voeg eerst een voorstelling toe: de datum en de tijd waarop gespeeld wordt.";

    private Plan _plan = new Plan();

    // True when the plan on screen has changes that are not in a file.
    private bool _changed;

    // True while a plan is put on screen by the form itself, so that is not taken for a change.
    private bool _filling;

    // Asks a yes or no question. Replaced in tests, where nobody can answer a message box.
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Func<string, bool> Confirm { get; set; } =
      question => MessageBox.Show(question, "Groepsindeling", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

    // Tells the user something. Replaced in tests, where nobody can close a message box.
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<string> Tell { get; set; } = text => MessageBox.Show(text, "Groepsindeling");

    // True when the plan on screen may be thrown away: it is saved, or the user says so.
    private bool MayDiscard(string question)
    {
      return !_changed || Confirm("Het plan is gewijzigd en nog niet opgeslagen. " + question);
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool EnableWindow(IntPtr window, bool enable);

    // Shows a dialog in the middle of this window, and keeps this window from being used until the
    // dialog is closed. That is what ShowDialog does, but ShowDialog only returns when the dialog
    // closes, so a test could not fill the dialog in.
    private void OpenDialog(Form dialog)
    {
      dialog.StartPosition = FormStartPosition.Manual;
      dialog.Location = new System.Drawing.Point(
        Left + (Width - dialog.Width) / 2,
        Top + (Height - dialog.Height) / 2);
      dialog.FormClosed += (sender, e) =>
      {
        EnableWindow(Handle, true);
        Activate();
      };
      EnableWindow(Handle, false);
      dialog.Show(this);
    }

    // The preview of the last distribution, while it is open.
    private PreviewForm _preview;

    // Divides the groups over the showings and shows the outcome in the preview window,
    // from where it can be exported to PDF.
    private void Divideshows(object sender, EventArgs eventArgs)
    {
      var problem = _plan.Shows.Count == 0 ? NoShowing
        : _plan.Groups.Count == 0 ? "Voeg eerst de groepen toe."
        : _plan.CheckSeats();
      if (problem != null)
      {
        Tell(problem);
        return;
      }

      Hintlabel.Text = "";
      _plan.ShowName = voorstellingnaambox.Text;
      var unplaced = _plan.Distribute();
      Groupviewerupdate();
      Voorstellingviewerupdate();
      if (_plan.Unpinned.Count > 0)
      {
        // The pins are part of the plan file.
        _changed = true;
        Tell(_plan.DescribeUnpinned());
      }
      if (unplaced.Count > 0)
      {
        Tell(_plan.DescribeUnplaced(unplaced));
      }

      ClosePreview();
      _preview = new PreviewForm(_plan);
      // Pins and hand-chosen seats are part of the plan file.
      _preview.Corrected += (sender, e) => _changed = true;
      _preview.Show();
    }

    // The preview shows the last distribution, so it goes away as soon as the plan changes.
    private void ClosePreview()
    {
      _preview?.Close();
      _preview = null;
    }

    // The plan on screen was changed by the user: it is no longer what is in the file,
    // and the preview is out of date.
    private void PlanChanged()
    {
      _changed = true;
      if (_preview != null && _preview.Visible)
      {
        Hintlabel.Text = "De indeling is gesloten, want het plan is gewijzigd. Klik op \"Verdeel groepen\" voor de nieuwe indeling.";
      }
      ClosePreview();
    }

    private void ShowNameChanged(object sender, EventArgs e)
    {
      if (!_filling)
      {
        PlanChanged();
      }
    }

    // Closing the window with changes that are not saved asks first.
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      if (e.CloseReason == CloseReason.UserClosing && !(MayDiscard("Toch afsluiten?") && CloseHallEditors()))
      {
        e.Cancel = true;
      }
      base.OnFormClosing(e);
    }

    // Closes the hall editors, which ask first when their hall is not saved. The app ends with this
    // window, so they would not get to ask otherwise. False when an editor stays open.
    private static bool CloseHallEditors()
    {
      foreach (var editor in Application.OpenForms.OfType<EditorForm>().ToList())
      {
        editor.Close();
        if (!editor.IsDisposed)
        {
          return false;
        }
      }
      return true;
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
      OpenDialog(addshowform);
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
        PlanChanged();
        Updatestoelen();
      }
    }
    void Voorstellingviewerupdate()
    {
      Voorstellingviewer.Items.Clear();
      foreach (var show in _plan.Shows)
      {
        // The list is narrow, so the full text is also shown as tooltip.
        Voorstellingviewer.Items.Add(_plan.Describe(show)).ToolTipText = _plan.Describe(show);
      }
    }
    #endregion
    private void Updatestoelen()
    {
      var personen = _plan.Groups.Sum(group => group.Personen);
      var stoelen = _plan.Shows.Sum(show => show.Zaal.Seats);
      Seatlabel.Text = $"{personen} personen, {stoelen} stoelen";
      Seatlabel.ForeColor = personen > stoelen ? System.Drawing.Color.Firebrick : System.Drawing.SystemColors.ControlText;
    }
    #region Groepen
    private void AddgroupClick(object sender, EventArgs e)
    {
      if (_plan.Shows.Count == 0)
        Tell(NoShowing);
      else
      {
        OpenGroupDialog(null);
      }
    }

    private void BulkgroupClick(object sender, EventArgs e)
    {
      if (_plan.Shows.Count == 0)
      {
        Tell(NoShowing);
        return;
      }
      var bulkgroupform = new Bulkgroupform(_plan);
      bulkgroupform.Addinggroups += BulkgroupformAddinggroups;
      OpenDialog(bulkgroupform);
    }

    void BulkgroupformAddinggroups(object sender, EventArgs e)
    {
      if (sender is Bulkgroupform bulkGroupForm)
      {
        _plan.Groups.AddRange(bulkGroupForm.Groups);
        Groupviewerupdate();
        PlanChanged();
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
      OpenDialog(addgroupform);
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
        PlanChanged();
        Updatestoelen();
      }
    }

    // The name column takes the room the other columns leave, so a wider window shows longer names.
    private void GroupviewerResize(object sender, EventArgs e)
    {
      // The list gets its size before its columns while the window is built.
      if (Groupviewer.Columns.Count == 0)
      {
        return;
      }
      var others = Groupviewer.Columns.Cast<ColumnHeader>().Skip(1).Sum(column => column.Width);
      Groupviewer.Columns[0].Width = Math.Max(80, Groupviewer.ClientSize.Width - others);
    }

    private void Groupviewerupdate()
    {
      Groupviewer.Items.Clear();
      foreach (var group in _plan.Groups)
      {
        var item = Groupviewer.Items.Add(group.Name);
        item.SubItems.Add(group.Personen.ToString());
        item.SubItems.Add(group.Firstrows ? "ja" : "");
        item.SubItems.Add(group.Morning ? "ja" : "");
      }
    }

    #endregion

    #region ButtonClicks
    private void ResetButtonClick(object sender, EventArgs e)
    {
      if (!MayDiscard("Toch alles wissen?"))
      {
        return;
      }
      _plan = new Plan();
      ClosePreview();
      Hintlabel.Text = "";
      Groupviewer.Items.Clear();
      Voorstellingviewer.Items.Clear();
      _filling = true;
      voorstellingnaambox.Clear();
      _filling = false;
      _changed = false;
      Updatestoelen();
    }

    private void RemoveshowbuttonClick(object sender, EventArgs e)
    {
      var indexes = new int[Voorstellingviewer.SelectedIndices.Count];
      Voorstellingviewer.SelectedIndices.CopyTo(indexes, 0);
      var question = indexes.Length == 1
        ? $"De voorstelling van {_plan.Describe(_plan.Shows[indexes[0]])} verwijderen?"
        : $"{indexes.Length} voorstellingen verwijderen?";
      if (indexes.Length == 0 || !Confirm(question))
      {
        return;
      }
      for (var i = 0; i < indexes.Length; i++)
      {
        _plan.Shows.RemoveAt(indexes[i]);
        for (var j = i; j < indexes.Length; j++)
          indexes[j]--;
      }
      Voorstellingviewerupdate();
      PlanChanged();
      Updatestoelen();
    }

    private void RemoveGroupbut_Click(object sender, EventArgs e)
    {
      var indexes = new int[Groupviewer.SelectedIndices.Count];
      Groupviewer.SelectedIndices.CopyTo(indexes, 0);
      var question = indexes.Length == 1
        ? $"De groep \"{_plan.Groups[indexes[0]].Name}\" verwijderen?"
        : $"{indexes.Length} groepen verwijderen?";
      if (indexes.Length == 0 || !Confirm(question))
      {
        return;
      }
      for (var i = 0; i < indexes.Length; i++)
      {
        _plan.Groups.RemoveAt(indexes[i]);
        for (var j = i; j < indexes.Length; j++)
          indexes[j]--;
      }
      Groupviewerupdate();
      PlanChanged();
      Updatestoelen();
    }

    // Opens the window to make or change a hall. A changed hall counts for showings that are
    // added afterwards and for plans that are loaded afterwards.
    private void HallsbuttonClick(object sender, EventArgs e)
    {
      new EditorForm().Show();
    }

    private const string PlanFileFilter = "Theaterverdeling plan (*.json)|*.json";

    // Writes the plan that is on screen to a file.
    public void SavePlan(string path)
    {
      _plan.ShowName = voorstellingnaambox.Text;
      _plan.Save(path);
      _changed = false;
    }

    // Replaces the plan on screen with the plan in a file.
    // When the file cannot be read, the plan on screen stays as it was.
    public void LoadPlan(string path)
    {
      var plan = Plan.Load(path);
      _filling = true;
      voorstellingnaambox.Text = plan.ShowName;
      _filling = false;
      _plan = plan;
      _changed = false;
      Voorstellingviewerupdate();
      Groupviewerupdate();
      ClosePreview();
      Updatestoelen();
      // The file holds the plan, not the seating: that is made again from the plan.
      Hintlabel.Text = _plan.Shows.Count > 0 && _plan.Groups.Count > 0
        ? "Het plan is geladen. Klik op \"Verdeel groepen\" om de indeling weer te zien."
        : "";
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
        Tell($"Opslaan is niet gelukt: {ex.Message}");
      }
    }

    private void LoadButton_Click(object sender, EventArgs e)
    {
      if (!MayDiscard("Toch een ander plan laden?"))
      {
        return;
      }
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
        Tell($"Laden is niet gelukt: {ex.Message}");
      }
    }
    #endregion
  }
}
