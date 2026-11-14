using System;
using System.Linq;
using System.Windows.Forms;

namespace Theaterverdeling
{
  // Dialog to add a group to the plan, or to change a group that is already in it.
  public partial class Addgroupform : Form
  {
    private readonly Plan _plan;

    public Addgroupform(Plan plan, Group editing = null)
    {
      InitializeComponent();
      _plan = plan;
      Editing = editing;
      foreach (var datum in plan.Shows.Select(show => show.Datum).Distinct().OrderBy(datum => datum))
      {
        Datumvoorkeurenbox.Items.Add(datum);
      }

      if (editing != null)
      {
        Text = "Groep wijzigen";
        Addgroupbutton.Text = "Groep wijzigen";
        Groepnaam.Text = editing.Name;
        Groepsgrootte.Value = Math.Min(Math.Max(editing.Personen, Groepsgrootte.Minimum), Groepsgrootte.Maximum);
        Firstrowcheck.Checked = editing.Firstrows;
        Morningcheck.Checked = editing.Morning;
        for (var i = 0; i < Datumvoorkeurenbox.Items.Count; i++)
        {
          Datumvoorkeurenbox.SetItemChecked(i, editing.Datum.Contains((DateTime)Datumvoorkeurenbox.Items[i]));
        }
      }
    }

    private void CancelbuttonClick(object sender, EventArgs e)
    {
      Close();
    }

    private void AddgroupbuttonClick(object sender, EventArgs e)
    {
      var data = Datumvoorkeurenbox.CheckedItems.Cast<DateTime>().ToList();
      var error = _plan.CheckGroup(Groepnaam.Text, (int)Groepsgrootte.Value, Editing);
      if (error != null)
      {
        Errorlabel.Text = error;
        return;
      }

      Group = new Group(Groepnaam.Text.Trim(), (int)Groepsgrootte.Value, Firstrowcheck.Checked, Morningcheck.Checked, data);
      Addinggroup?.Invoke(this, EventArgs.Empty);
      Close();
    }

    public delegate void AddinggroupEventHandler(object sender, EventArgs e);
    public event AddinggroupEventHandler Addinggroup;

    // The group made in this dialog, set when it is accepted.
    public Group Group;

    // The group this dialog changes, or null when it adds a new one.
    public Group Editing { get; }
  }
}
