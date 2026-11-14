using System;
using System.Windows.Forms;

namespace Theaterverdeling
{
  public partial class Addshowform : Form
  {
    public delegate void AddingshowEventHandler(object sender, EventArgs e);
    public Addshowform()
    {
      InitializeComponent();
      foreach (var section in Lampegiet.Load().Sections)
      {
        Sectionsoff.Items.Add(section);
      }
    }

    private void AddshowClick(object sender, EventArgs e)
    {
      var time = Time.Value;
      var zaal = Lampegiet.Load();
      foreach (string section in Sectionsoff.CheckedItems)
      {
        zaal.SwitchOff(section);
      }
      show = new Voorstelling(time, zaal, Datumkeuze.Value.Date);
      Addingshow(this, new EventArgs());
      Close();
    }
    public event AddingshowEventHandler Addingshow;

    private void CancelButtonClick(object sender, EventArgs e)
    {
      Close();
    }


  }
}
