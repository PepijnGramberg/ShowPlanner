using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Theaterverdeling
{
  // Shows the outcome of a distribution on screen, one showing at a time, and exports it to PDF.
  // A group can be chosen on the seat map or in the list, moved to another showing and pinned there.
  public partial class PreviewForm : Form
  {
    private readonly Plan _plan;

    // The groups in the list, in the order of the list.
    private List<Group> _groups = new List<Group>();

    // The showings in the "Verplaats naar" list, in the order of that list.
    private List<Voorstelling> _targets = new List<Voorstelling>();

    // True while the lists are filled by the form itself, so that is not taken for a choice.
    private bool _filling;

    // Raised when a group is moved, pinned or let go, or gets other seats: what the plan file keeps of a distribution.
    public event EventHandler Corrected;

    public PreviewForm(Plan plan)
    {
      InitializeComponent();
      _plan = plan;
      if (plan.ShowName != "")
      {
        Text = $"Indeling - {plan.ShowName}";
      }
      // What the buttons do is not in their short names.
      var tips = new ToolTip();
      tips.SetToolTip(Pinbutton, "Een vastgezette groep blijft in deze voorstelling als de groepen opnieuw worden verdeeld.");
      tips.SetToolTip(Movebutton, "Zet de gekozen groep in de voorstelling hierboven en zet hem daar vast.");
      tips.SetToolTip(Autoseatbutton, "Laat de zelf gekozen stoelen los: de groep krijgt weer stoelen volgens de regels.");
      ShowPlan(null);
    }

    // Writes the pages to PDF: one file with all showings and one file per showing.
    public void ExportPdf(string path)
    {
      PdfExport.Write(_plan, path);
      Messagelabel.ForeColor = SystemColors.ControlText;
      Messagelabel.Text = $"Opgeslagen als {System.IO.Path.GetFileName(path)}, met een PDF per voorstelling ernaast.";
    }

    // Asks a yes or no question. Replaced in tests, where nobody can answer a message box.
    [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
    public Func<string, bool> Confirm { get; set; } =
      question => MessageBox.Show(question, "Indeling", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

    // The showing that is chosen in the list, or null when the groups without a showing are chosen.
    private Voorstelling CurrentShow =>
      Showingslist.SelectedIndex >= 0 && Showingslist.SelectedIndex < _plan.Shows.Count ? _plan.Shows[Showingslist.SelectedIndex] : null;

    private Group CurrentGroup => Grouplist.SelectedIndex >= 0 ? _groups[Grouplist.SelectedIndex] : null;

    // Fills the list of showings and opens the showing of the given group, or else the showing that was open.
    private void ShowPlan(Group group)
    {
      var unplaced = _plan.Unplaced;
      var index = Math.Max(Showingslist.SelectedIndex, 0);
      if (group != null)
      {
        var show = _plan.Shows.FindIndex(candidate => candidate.Groups.Contains(group));
        index = show >= 0 ? show : _plan.Shows.Count;
      }

      _filling = true;
      Showingslist.Items.Clear();
      foreach (var show in _plan.Shows)
      {
        Showingslist.Items.Add(_plan.Describe(show));
      }
      if (unplaced.Count > 0)
      {
        Showingslist.Items.Add($"Niet ingedeeld ({unplaced.Count})");
      }
      if (Showingslist.Items.Count > 0)
      {
        Showingslist.SelectedIndex = Math.Min(index, Showingslist.Items.Count - 1);
      }
      _filling = false;
      ShowShowing(group);
    }

    // Shows the seat map and the groups of the chosen showing, and chooses the given group in it.
    private void ShowShowing(Group group)
    {
      var show = CurrentShow;
      _groups = show != null ? show.Groups.ToList() : _plan.Unplaced;
      Seatmap.Hall = show?.Zaal;
      Freelabel.Text = show == null ? "" : $"{show.Freeseats} van de {show.Zaal.Seats} stoelen vrij";

      _filling = true;
      Grouplist.Items.Clear();
      foreach (var entry in _groups)
      {
        Grouplist.Items.Add(GroupText(entry));
      }
      Grouplist.SelectedIndex = group == null ? -1 : _groups.IndexOf(group);
      _filling = false;
      ShowGroup();
    }

    // Shows what can be done with the chosen group: where it sits and where it can move to.
    private void ShowGroup()
    {
      var group = CurrentGroup;
      Seatmap.HighlightedGroup = group;
      Seatslabel.Text = group == null
        ? "Kies een groep in de lijst of op de plattegrond. Daarna maakt een klik op een stoel van de groep die stoel vrij, en geeft een klik op een vrije stoel die aan de groep. De groep houdt die stoelen."
        : string.Join(", ", group.Seats.Select(range => range.StartSeat == range.EndSeat
            ? $"rij {range.Row}: {range.StartSeat}"
            : $"rij {range.Row}: {range.StartSeat}-{range.EndSeat}"));

      _targets = group == null
        ? new List<Voorstelling>()
        : _plan.Shows.Where(show => show != CurrentShow && show.Freeseats >= group.Personen).ToList();
      Movechoice.Items.Clear();
      foreach (var target in _targets)
      {
        Movechoice.Items.Add($"{_plan.Describe(target)} ({target.Freeseats} vrij)");
      }
      if (_targets.Count > 0)
      {
        Movechoice.SelectedIndex = 0;
      }
      Movechoice.Enabled = _targets.Count > 0;
      Movebutton.Enabled = _targets.Count > 0;
      // A group without a showing has nothing to be pinned to.
      Pinbutton.Enabled = group != null && CurrentShow != null;
      Pinbutton.Text = group != null && group.Pinned ? "Losmaken" : "Vastzetten";
      Autoseatbutton.Enabled = group != null && group.HandSeated;
    }

    private void ShowingslistSelectedIndexChanged(object sender, EventArgs e)
    {
      if (!_filling)
      {
        Messagelabel.Text = "";
        ShowShowing(null);
      }
    }

    private void GrouplistSelectedIndexChanged(object sender, EventArgs e)
    {
      if (!_filling)
      {
        Messagelabel.Text = "";
        ShowGroup();
      }
    }

    // A click on a seat of another group chooses that group. With a group chosen, a click on one of
    // its seats makes that seat free and a click on a free seat gives it to the group, so single
    // seats can be changed by hand. A click next to the seats chooses no group.
    private void SeatmapSeatMapClick(object sender, SeatMapClickEventArgs e)
    {
      var chosen = CurrentGroup;
      var owner = e.Hit.Seat?.Group;
      if (!e.Hit.OnSeat || (owner != null && owner != chosen))
      {
        Grouplist.SelectedIndex = owner == null ? -1 : _groups.IndexOf(owner);
        return;
      }
      if (chosen == null || CurrentShow == null)
      {
        return;
      }

      var hall = CurrentShow.Zaal;
      var problem = hall.ChangeSeat((ChairRow)e.Hit.Row, e.Hit.SeatIndex, chosen);
      if (problem != null)
      {
        Messagelabel.ForeColor = Color.Firebrick;
        Messagelabel.Text = problem;
        return;
      }
      // Seats that are chosen by hand only count in this showing, so the group stays in it.
      chosen.Pinned = true;
      Corrected?.Invoke(this, EventArgs.Empty);
      _filling = true;
      Grouplist.Items[Grouplist.SelectedIndex] = GroupText(chosen);
      _filling = false;
      ShowGroup();
      Seatmap.Invalidate();
      var missing = chosen.Personen - hall.SeatsOf(chosen);
      Messagelabel.ForeColor = missing > 0 ? Color.Firebrick : SystemColors.ControlText;
      Messagelabel.Text = missing > 0
        ? $"Kies nog {missing} {(missing == 1 ? "stoel" : "stoelen")} voor {chosen.Name}."
        : $"{chosen.Name} heeft weer {chosen.Personen} stoelen.";
    }

    // How a group is named in the list: its size, whether it is pinned, and how many of its people
    // have a seat when seats were taken away by hand.
    private string GroupText(Group group)
    {
      var seated = CurrentShow?.Zaal.SeatsOf(group) ?? group.Personen;
      var size = seated == group.Personen ? $"{group.Personen}" : $"{seated} van {group.Personen} geplaatst";
      var state = group.HandSeated ? " - vast, eigen stoelen" : group.Pinned ? " - vast" : "";
      return $"{group.Name} ({size}){state}";
    }

    // Moves the chosen group to the chosen showing, pins it there and opens that showing.
    private void MovebuttonClick(object sender, EventArgs e)
    {
      var group = CurrentGroup;
      if (group == null || Movechoice.SelectedIndex < 0)
      {
        return;
      }
      var target = _targets[Movechoice.SelectedIndex];
      var problem = _plan.Move(group, target);
      if (problem != null)
      {
        Messagelabel.ForeColor = Color.Firebrick;
        Messagelabel.Text = problem;
        return;
      }
      Corrected?.Invoke(this, EventArgs.Empty);
      ShowPlan(group);
      Messagelabel.ForeColor = SystemColors.ControlText;
      Messagelabel.Text = $"{group.Name} is verplaatst en staat vast in deze voorstelling.";
    }

    // Pins the chosen group to its showing, or lets it go again. A pinned group stays in its showing at the next distribution.
    private void PinbuttonClick(object sender, EventArgs e)
    {
      var group = CurrentGroup;
      if (group == null || CurrentShow == null)
      {
        return;
      }
      group.Pinned = !group.Pinned;
      Corrected?.Invoke(this, EventArgs.Empty);
      if (!group.Pinned && group.HandSeated)
      {
        // Seats that were chosen by hand belong to this showing, so they go when the group is let go.
        group.HandSeated = false;
        CurrentShow.Assignseats();
      }
      ShowShowing(group);
      Messagelabel.ForeColor = SystemColors.ControlText;
      Messagelabel.Text = group.Pinned
        ? $"{group.Name} blijft bij opnieuw verdelen in deze voorstelling."
        : $"{group.Name} wordt bij opnieuw verdelen weer vrij ingedeeld.";
    }

    // Lets go of the seats that were chosen by hand: the group is seated by the rules again.
    private void AutoseatbuttonClick(object sender, EventArgs e)
    {
      var group = CurrentGroup;
      if (group == null || CurrentShow == null)
      {
        return;
      }
      Corrected?.Invoke(this, EventArgs.Empty);
      group.HandSeated = false;
      CurrentShow.Assignseats();
      ShowShowing(group);
      Messagelabel.ForeColor = SystemColors.ControlText;
      Messagelabel.Text = $"{group.Name} is weer automatisch geplaatst.";
    }

    private void ExportbuttonClick(object sender, EventArgs e)
    {
      using var dialog = new SaveFileDialog { Filter = "pdf (*.pdf)|*.pdf" };
      if (dialog.ShowDialog() != DialogResult.OK)
      {
        return;
      }
      try
      {
        ExportPdf(dialog.FileName);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Exporteren is niet gelukt: {ex.Message}");
        return;
      }
      if (Confirm($"De PDF is opgeslagen in {System.IO.Path.GetDirectoryName(dialog.FileName)}. Nu openen?"))
      {
        try
        {
          System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(dialog.FileName) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Openen is niet gelukt: {ex.Message}");
        }
      }
    }
  }
}
