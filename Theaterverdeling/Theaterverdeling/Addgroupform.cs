using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Theaterverdeling
{
  public partial class Addgroupform : Form
  {
    public Addgroupform(IEnumerable<DateTime> data)
    {
      InitializeComponent();
      foreach (var datum in data)
        if (!Datumvoorkeurenbox.Items.Contains(datum))
          Datumvoorkeurenbox.Items.Add(datum);
    }

    private void CancelbuttonClick(object sender, EventArgs e)
    {
      Close();
    }

    private void AddgroupbuttonClick(object sender, EventArgs e)
    {
      if (Groepnaam.Text == "")
      {
        MessageBox.Show(@"Vul een groepsnaam in.");
      }
      else
      {
        var data = Datumvoorkeurenbox.CheckedItems.Cast<DateTime>().ToList();
        var time = Morningcheck.Checked;
        Group = new Group(Groepnaam.Text, (int)Groepsgrootte.Value, Firstrowcheck.Checked, time, data);
        Addinggroup(this, new EventArgs());
        Close();
      }
    }
    public delegate void AddinggroupEventHandler(object sender, EventArgs e);
    public event AddinggroupEventHandler Addinggroup;
    public Group Group;
  }

}
