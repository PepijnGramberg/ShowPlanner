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

    // True when the group was put in its showing by hand. A distribution leaves it there.
    public bool Pinned;

    // True when seats of the group were changed by hand. The group then keeps exactly the seats it
    // has when its showing is seated again.
    public bool HandSeated;
    public List<SeatRange> Seats = new List<SeatRange>();

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
