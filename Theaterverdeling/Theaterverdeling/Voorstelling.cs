using System;
using System.Collections.Generic;
using System.Linq;
using PdfSharp.Drawing;

namespace Theaterverdeling
{
    public class Voorstelling
    {
        public DateTime Time;
        public Zaal Zaal;
        public DateTime Datum;
        public int Freeseats{get { return Zaal.Seats - Groups.Sum(group => group.Personen); }}
        public List<Group> Groups=new List<Group>(); 
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
