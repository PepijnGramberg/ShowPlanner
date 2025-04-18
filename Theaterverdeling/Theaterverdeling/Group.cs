using System;
using System.Collections.Generic;

namespace Theaterverdeling
{
    public class Group
    {
        public string Name { get; }
        public bool Morning { get; }
        public List<DateTime> Datum { get; }
        public int Personen { get; }
        public bool Firstrows { get; }
        public Voorstelling Voorstelling { get; set; }
        public List<Tuple<int,int,int>> Seats { get; set; } = new List<Tuple<int,int,int>>();
        
        public Group(string naam, int personen, bool first, bool morning, List<DateTime> datum)
        {
            Morning = morning;
            Personen = personen;
            Datum = datum;
            Firstrows = first;
            Name = naam;
        }
    }
}
