using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Theaterverdeling
{
  // Window to make a hall or change one: its rows in a grid, next to a seat map that can be clicked.
  // A hall is saved as a file in the halls folder, from where the showings load it.
  public partial class EditorForm : Form
  {
    private const string NewHall = "(nieuwe zaal)";

    private HallDraft _draft = new HallDraft();

    // Name of the hall that was opened or last saved, or null for a new hall.
    private string _openedName;

    // True while the grid is filled by the form itself, so that is not taken for typing.
    private bool _filling;
    private bool _changed;

    public EditorForm()
    {
      InitializeComponent();
      Icon = AppIcon.Value;
      AddColumns();
      FillPicker(null);
    }

    // Asks a yes or no question. Replaced in tests, where nobody can answer a message box.
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public Func<string, bool> Confirm { get; set; } =
      question => MessageBox.Show(question, "Zaal", MessageBoxButtons.YesNo) == DialogResult.Yes;

    // Raised after the hall is written to the halls folder.
    public event EventHandler HallSaved;

    // Opens a hall from the halls folder in the editor.
    public void OpenHall(string name)
    {
      Hallpicker.SelectedItem = name;
    }

    // Checks the hall and writes it to the halls folder. Returns false, with a message on screen, when it cannot be saved.
    public bool SaveHall()
    {
      Rowsgrid.EndEdit();
      ShowMessages();
      var name = Hallname.Text.Trim();
      var problem = _draft.CheckHall(name);
      if (problem != null)
      {
        ShowStatus(problem, true);
        return false;
      }
      var replacesOther = Zaal.Exists(name) && !string.Equals(name, _openedName, StringComparison.OrdinalIgnoreCase);
      if (replacesOther && !Confirm($"Er is al een zaal \"{name}\". Overschrijven?"))
      {
        return false;
      }

      try
      {
        _draft.ToHall().Save(name);
      }
      catch (Exception ex)
      {
        ShowStatus($"Opslaan is niet gelukt: {ex.Message}", true);
        return false;
      }
      _changed = false;
      FillPicker(name);
      ShowStatus("Opgeslagen.", false);
      HallSaved?.Invoke(this, EventArgs.Empty);
      return true;
    }

    private void AddColumns()
    {
      Rowsgrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Soort", HeaderText = "Soort", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, ReadOnly = true });
      Rowsgrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Rij", HeaderText = "Rij", MinimumWidth = 40, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
      Rowsgrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Stoelen", HeaderText = "Stoelen", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
      Rowsgrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Deel", HeaderText = "Deel", Width = 90 });
      Rowsgrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nietgebruikt", HeaderText = "Niet gebruikt", Width = 90 });
      var message = new DataGridViewTextBoxColumn
      {
        Name = "Melding",
        HeaderText = "Melding",
        ReadOnly = true,
        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
        MinimumWidth = 120
      };
      message.DefaultCellStyle.ForeColor = Color.Firebrick;
      Rowsgrid.Columns.Add(message);
      foreach (DataGridViewColumn column in Rowsgrid.Columns)
      {
        column.SortMode = DataGridViewColumnSortMode.NotSortable;
      }
    }

    // Lists the new hall and the halls in the halls folder, and chooses the given one without loading it again.
    private void FillPicker(string chosen)
    {
      _filling = true;
      Hallpicker.Items.Clear();
      Hallpicker.Items.Add(NewHall);
      foreach (var name in Zaal.HallNames())
      {
        Hallpicker.Items.Add(name);
      }
      var index = chosen == null ? 0 : Hallpicker.FindStringExact(chosen);
      Hallpicker.SelectedIndex = Math.Max(index, 0);
      _openedName = Hallpicker.SelectedIndex == 0 ? null : (string)Hallpicker.SelectedItem;
      _filling = false;
    }

    private void HallpickerSelectedIndexChanged(object sender, EventArgs e)
    {
      if (_filling)
      {
        return;
      }
      var name = Hallpicker.SelectedIndex <= 0 ? null : (string)Hallpicker.SelectedItem;
      if (_changed && !Confirm("De wijzigingen in deze zaal zijn niet opgeslagen. Toch een andere zaal openen?"))
      {
        FillPicker(_openedName);
        return;
      }

      HallDraft draft;
      try
      {
        draft = name == null ? new HallDraft() : HallDraft.FromHall(Zaal.Load(name));
      }
      catch (Exception ex)
      {
        FillPicker(_openedName);
        ShowStatus($"Openen is niet gelukt: {ex.Message}", true);
        return;
      }
      _draft = draft;
      _openedName = name;
      Hallname.Text = name ?? "";
      _changed = false;
      ShowStatus("", false);
      FillGrid(0);
    }

    // Shows the lines of the hall in the grid and puts the cursor on the given line.
    private void FillGrid(int select)
    {
      _filling = true;
      Rowsgrid.Rows.Clear();
      foreach (var row in _draft.Rows)
      {
        var index = row.IsWalkway
          ? Rowsgrid.Rows.Add("Looppad")
          : Rowsgrid.Rows.Add("Rij", row.RowNr, row.SeatCount, row.Section, row.Unusable);
        if (row.IsWalkway)
        {
          foreach (var column in new[] { "Rij", "Stoelen", "Deel", "Nietgebruikt" })
          {
            Rowsgrid.Rows[index].Cells[column].ReadOnly = true;
          }
        }
      }
      _filling = false;
      SelectLine(select);
      ShowHall();
    }

    private void SelectLine(int index)
    {
      if (index >= 0 && index < Rowsgrid.Rows.Count)
      {
        var column = Rowsgrid.CurrentCell?.ColumnIndex ?? 0;
        Rowsgrid.CurrentCell = Rowsgrid.Rows[index].Cells[column];
      }
      Seatmap.SelectedRow = CurrentLine;
    }

    private int CurrentLine => Rowsgrid.CurrentCell?.RowIndex ?? -1;

    // Shows the hall as it is now: the messages per line, the seat map and the number of seats.
    private void ShowHall()
    {
      ShowMessages();
      var hall = _draft.ToHall();
      Seatmap.Hall = hall;
      Seatmap.SelectedRow = CurrentLine;
      var chairs = hall.Rows.Sum(row => row.TotalSeatCount);
      Countlabel.Text = $"{chairs} stoelen, {hall.Seats} te gebruiken";
    }

    private void ShowMessages()
    {
      var messages = _draft.Check();
      _filling = true;
      for (var i = 0; i < messages.Count; i++)
      {
        Rowsgrid.Rows[i].Cells["Melding"].Value = messages[i];
      }
      _filling = false;
    }

    private void ShowStatus(string text, bool problem)
    {
      Statuslabel.ForeColor = problem ? Color.Firebrick : SystemColors.ControlText;
      Statuslabel.Text = text;
    }

    private void Changed()
    {
      _changed = true;
      ShowStatus("", false);
    }

    // A cell was typed in: take it over in the hall.
    private void RowsgridCellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      if (_filling || e.RowIndex < 0 || e.RowIndex >= _draft.Rows.Count)
      {
        return;
      }
      var row = _draft.Rows[e.RowIndex];
      var text = Convert.ToString(Rowsgrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) ?? "";
      switch (Rowsgrid.Columns[e.ColumnIndex].Name)
      {
        case "Rij": row.RowNr = text; break;
        case "Stoelen": row.SeatCount = text; break;
        case "Deel": row.Section = text; break;
        case "Nietgebruikt": row.Unusable = text; break;
        default: return;
      }
      Changed();
      ShowHall();
    }

    private void RowsgridCurrentCellChanged(object sender, EventArgs e)
    {
      if (!_filling)
      {
        Seatmap.SelectedRow = CurrentLine;
      }
    }

    // A click on a seat switches it between used and not used. Any click on a row chooses that row.
    private void SeatmapSeatMapClick(object sender, SeatMapClickEventArgs e)
    {
      Rowsgrid.EndEdit();
      var index = e.Hit.RowIndex;
      if (index >= _draft.Rows.Count)
      {
        return;
      }
      SelectLine(index);
      if (e.Hit.OnSeat)
      {
        var row = _draft.Rows[index];
        row.ToggleSeat(e.Hit.SeatIndex + 1);
        _filling = true;
        Rowsgrid.Rows[index].Cells["Nietgebruikt"].Value = row.Unusable;
        _filling = false;
        Changed();
        ShowHall();
      }
    }

    private void AddrowbuttonClick(object sender, EventArgs e)
    {
      Rowsgrid.EndEdit();
      Changed();
      FillGrid(_draft.AddRow(CurrentLine));
    }

    private void AddwalkwaybuttonClick(object sender, EventArgs e)
    {
      Rowsgrid.EndEdit();
      Changed();
      FillGrid(_draft.AddWalkway(CurrentLine));
    }

    private void RemoverowbuttonClick(object sender, EventArgs e)
    {
      Rowsgrid.EndEdit();
      var index = CurrentLine;
      if (index < 0)
      {
        return;
      }
      _draft.Rows.RemoveAt(index);
      Changed();
      FillGrid(Math.Min(index, _draft.Rows.Count - 1));
    }

    private void UpbuttonClick(object sender, EventArgs e)
    {
      MoveLine(-1);
    }

    private void DownbuttonClick(object sender, EventArgs e)
    {
      MoveLine(1);
    }

    private void MoveLine(int direction)
    {
      Rowsgrid.EndEdit();
      var index = CurrentLine;
      var moved = _draft.Move(index, direction);
      if (moved != index)
      {
        Changed();
        FillGrid(moved);
      }
    }

    private void HallnameTextChanged(object sender, EventArgs e)
    {
      if (!_filling && Hallname.Text != (_openedName ?? ""))
      {
        Changed();
      }
    }

    private void SavebuttonClick(object sender, EventArgs e)
    {
      SaveHall();
    }

    private void EditorFormFormClosing(object sender, FormClosingEventArgs e)
    {
      if (_changed && e.CloseReason == CloseReason.UserClosing
          && !Confirm("De wijzigingen in deze zaal zijn niet opgeslagen. Toch sluiten?"))
      {
        e.Cancel = true;
      }
    }
  }
}
