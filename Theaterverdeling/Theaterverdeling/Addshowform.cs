using System;
using System.Linq;
using System.Windows.Forms;

namespace Theaterverdeling
{
  // Dialog to add a showing to the plan, or to change a showing that is already in it.
  public partial class Addshowform : Form
  {
    private readonly Plan _plan;

    public delegate void AddingshowEventHandler(object sender, EventArgs e);

    public Addshowform(Plan plan, Voorstelling editing = null)
    {
      InitializeComponent();
      Icon = AppIcon.Value;
      _plan = plan;
      Editing = editing;
      // Most school showings are in the morning, and nobody plans one to the second.
      Time.Value = DateTime.Today.AddHours(10);
      foreach (var name in Zaal.HallNames())
      {
        Hallchoice.Items.Add(name);
      }
      // A new showing starts as the last showing of the plan: its hall, its date and the sections
      // that are not used. Showings of one show are mostly alike, so only the time is left to set.
      var example = editing ?? plan.Shows.LastOrDefault();
      var hall = example?.Zaal.Name;
      var index = hall == null ? -1 : Hallchoice.FindStringExact(hall);
      if (Hallchoice.Items.Count > 0)
      {
        // Choosing the hall lists its sections.
        Hallchoice.SelectedIndex = Math.Max(index, 0);
      }

      if (example != null)
      {
        Datumkeuze.Value = example.Datum;
        for (var i = 0; i < Sectionsoff.Items.Count; i++)
        {
          Sectionsoff.SetItemChecked(i, example.Zaal.SwitchedOffSections.Contains((string)Sectionsoff.Items[i]));
        }
      }
      if (editing != null)
      {
        Text = "Voorstelling wijzigen";
        Addshow.Text = "Wijzigen";
        Time.Value = DateTime.Today + editing.Time.TimeOfDay;
      }
    }

    // Lists the sections of the chosen hall that can be switched off. Sections that were ticked
    // and that the newly chosen hall also has stay ticked.
    private void HallchoiceSelectedIndexChanged(object sender, EventArgs e)
    {
      var ticked = Sectionsoff.CheckedItems.Cast<string>().ToList();
      Sectionsoff.Items.Clear();
      Errorlabel.Text = "";
      try
      {
        foreach (var section in Zaal.Load((string)Hallchoice.SelectedItem).Sections)
        {
          Sectionsoff.Items.Add(section, ticked.Contains(section));
        }
      }
      catch (Exception ex)
      {
        Errorlabel.Text = $"De zaal kan niet worden geopend: {ex.Message}";
      }
    }

    private void AddshowClick(object sender, EventArgs e)
    {
      if (Hallchoice.SelectedItem == null)
      {
        Errorlabel.Text = "Er is geen zaal. Maak eerst een zaal met \"Zalen beheren\".";
        return;
      }
      var error = _plan.CheckShowing(Datumkeuze.Value.Date, Time.Value, Editing);
      if (error != null)
      {
        Errorlabel.Text = error;
        return;
      }

      Zaal zaal;
      try
      {
        zaal = Zaal.Load((string)Hallchoice.SelectedItem);
      }
      catch (Exception ex)
      {
        Errorlabel.Text = $"De zaal kan niet worden geopend: {ex.Message}";
        return;
      }
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
