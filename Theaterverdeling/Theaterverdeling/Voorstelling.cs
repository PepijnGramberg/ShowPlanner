using System;
using System.Collections.Generic;
using System.Linq;
using PdfSharpCore.Drawing;

namespace Theaterverdeling
{
  public class Voorstelling
  {
    public DateTime Time { get; set; }
    public Zaal Zaal { get; set; }
    public DateTime Datum { get; set; }
    public int Freeseats => Zaal.Seats - Groups.Sum(group => group.Personen);
    public List<Group> Groups { get; set; } = new List<Group>();

    public Voorstelling(DateTime time, Zaal zaal, DateTime datum)
    {
      Zaal = zaal;
      Time = time;
      Datum = datum;
    }

    public void Assignseats()
    {
      Zaal.Groups = Groups;
      Zaal.AssignSeats();
    }

    internal void Draw(XGraphics gr)
    {
      //todo data tijd drawing
      Zaal.Draw(gr);
    }
  }
}
