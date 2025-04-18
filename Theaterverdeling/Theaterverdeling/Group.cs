using System;
using System.Collections.Generic;


namespace Theaterverdeling
{
  public class Group
  {
    bool _morning;
    List<DateTime> _datum;
    int _personen;
    bool _firstrows;
    private string _name;
    public string Name { get { return _name; } }
    public bool Morning { get { return _morning; } }
    public List<DateTime> Datum { get { return _datum; } }
    public int Personen { get { return _personen; } }
    public bool Firstrows { get { return _firstrows; } }
    public Voorstelling Voorstelling;
    public List<Tuple<int, int, int>> Seats = new List<Tuple<int, int, int>>();

    public Group(string naam, int personen, bool first, bool morning, List<DateTime> datum)
    {
      _morning = morning;
      _personen = personen;
      _datum = datum;
      _firstrows = first;
      _name = naam;
    }
  }
}
