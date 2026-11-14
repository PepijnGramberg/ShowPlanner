using System;
using System.Linq;
using System.Windows.Forms;

namespace Theaterverdeling
{
  // Dialog to add a showing to the plan, or to change a showing that is already in it.
  public partial class Addshowform : Form
  {
    // The only hall for now. Becomes a choice in this dialog once there are more hall files.
    private const string HallName = "Lampegiet";

    private readonly Plan _plan;

    public delegate void AddingshowEventHandler(object sender, EventArgs e);

    public Addshowform(Plan plan, Voorstelling editing = null)
    {
      InitializeComponent();
      _plan = plan;
      Editing = editing;
      foreach (var section in Zaal.Load(HallName).Sections)
      {
        Sectionsoff.Items.Add(section);
      }

      if (editing != null)
      {
        Text = "Voorstelling wijzigen";
        Addshow.Text = "Wijzigen";
        Datumkeuze.Value = editing.Datum;
        Time.Value = DateTime.Today + editing.Time.TimeOfDay;
        for (var i = 0; i < Sectionsoff.Items.Count; i++)
        {
          Sectionsoff.SetItemChecked(i, editing.Zaal.SwitchedOffSections.Contains((string)Sectionsoff.Items[i]));
        }
      }
    }

    private void AddshowClick(object sender, EventArgs e)
    {
      var error = _plan.CheckShowing(Datumkeuze.Value.Date, Time.Value, Editing);
      if (error != null)
      {
        Errorlabel.Text = error;
        return;
      }

      var zaal = Zaal.Load(HallName);
      foreach (string section in Sectionsoff.CheckedItems)
      {
        zaal.SwitchOff(section);
      }
      show = new Voorstelling(Time.Value, zaal, Datumkeuze.Value.Date);
      Addingshow?.Invoke(this, EventArgs.Empty);
      Close();
    }
    public event AddingshowEventHandler Addingshow;

    // The showing this dialog changes, or null when it adds a new one.
    public Voorstelling Editing { get; }

    private void CancelButtonClick(object sender, EventArgs e)
    {
      Close();
    }
  }
}
