using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Theaterverdeling
{
  // Dialog to add many groups at once in a grid. Rows can be typed or pasted from a spreadsheet.
  public partial class Bulkgroupform : Form
  {
    private readonly Plan _plan;
    private readonly List<DateTime> _dates;

    public Bulkgroupform(Plan plan)
    {
      InitializeComponent();
      _plan = plan;
      _dates = plan.Shows.Select(show => show.Datum).Distinct().OrderBy(datum => datum).ToList();
      AddColumns();
    }

    // The groups that were accepted by the last click on "Toevoegen".
    public List<Group> Groups { get; private set; } = new List<Group>();

    public event EventHandler Addinggroups;

    // Adds a row for every pasted line, with its name and size. The ticks are set in the grid.
    public void PasteText(string text)
    {
      foreach (var entry in GroupEntry.FromPastedText(text))
      {
        Groupgrid.Rows.Add(entry.Name, entry.People);
      }
    }

    private void AddColumns()
    {
      Groupgrid.Columns.Add(new DataGridViewTextBoxColumn
      {
        Name = "Naam",
        HeaderText = "Naam",
        Width = 200
      });
      Groupgrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Aantal", HeaderText = "Aantal", Width = 55 });
      Groupgrid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Vooraan", HeaderText = "Vooraan", Width = 60 });
      Groupgrid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Ochtend", HeaderText = "Ochtend", Width = 60 });
      for (var i = 0; i < _dates.Count; i++)
      {
        Groupgrid.Columns.Add(new DataGridViewCheckBoxColumn { Name = DateColumn(i), HeaderText = _dates[i].ToShortDateString(), Width = 75 });
      }
      var message = new DataGridViewTextBoxColumn
      {
        Name = "Melding",
        HeaderText = "Melding",
        ReadOnly = true,
        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
        MinimumWidth = 250
      };
      message.DefaultCellStyle.ForeColor = Color.Firebrick;
      Groupgrid.Columns.Add(message);
    }

    private static string DateColumn(int index)
    {
      return "Datum" + index;
    }

    // Adds the rows that pass the checks to the plan and takes them out of the grid.
    // A row with a problem stays, with its message. The dialog closes when no row is left.
    private void AddbuttonClick(object sender, EventArgs e)
    {
      Groupgrid.EndEdit();
      var rows = Groupgrid.Rows.Cast<DataGridViewRow>().Where(row => !row.IsNewRow).ToList();
      var entries = rows.Select(ToEntry).ToList();
      var messages = _plan.CheckNewGroups(entries);

      Groups = new List<Group>();
      for (var i = 0; i < rows.Count; i++)
      {
        if (messages[i] == null)
        {
          Groups.Add(entries[i].ToGroup());
          Groupgrid.Rows.Remove(rows[i]);
        }
        else
        {
          rows[i].Cells["Melding"].Value = messages[i];
        }
      }

      if (Groups.Count > 0)
      {
        Addinggroups?.Invoke(this, EventArgs.Empty);
      }
      if (Groups.Count == rows.Count)
      {
        Close();
      }
    }

    private GroupEntry ToEntry(DataGridViewRow row)
    {
      return new GroupEntry
      {
        Name = Convert.ToString(row.Cells["Naam"].Value),
        People = Convert.ToString(row.Cells["Aantal"].Value),
        FrontRows = IsTicked(row, "Vooraan"),
        Morning = IsTicked(row, "Ochtend"),
        Dates = _dates.Where((datum, index) => IsTicked(row, DateColumn(index))).ToList()
      };
    }

    private static bool IsTicked(DataGridViewRow row, string column)
    {
      return true.Equals(row.Cells[column].Value);
    }

    private void PastebuttonClick(object sender, EventArgs e)
    {
      PasteText(Clipboard.GetText());
    }

    // Ctrl+V on the grid pastes rows. While a cell is being typed in, Ctrl+V pastes into that cell as usual.
    private void GroupgridKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Control && e.KeyCode == Keys.V)
      {
        PasteText(Clipboard.GetText());
        e.Handled = true;
      }
    }

    private void ClosebuttonClick(object sender, EventArgs e)
    {
      Close();
    }
  }
}
