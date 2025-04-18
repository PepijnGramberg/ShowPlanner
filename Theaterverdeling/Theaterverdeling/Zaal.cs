using System;
using System.Collections.Generic;
using PdfSharpCore.Drawing;

namespace Theaterverdeling
{
  public abstract class Zaal
  {
    public bool Frontseatsoff { get; set; } = false;
    public bool Balconyoff { get; set; } = false;
    protected int _seats;
    public int Seats => _seats;
    public List<Group> Groups { get; set; }
    protected List<int>[] Places;
    public virtual void AssignSeats()
    {
      CreatePlaces();
      FillPlaces();
    }

    public abstract void Draw(XGraphics gr);
    protected abstract void CreatePlaces();
    public abstract void Frontseatchange(bool fso);

    protected void FillPlaces()
    {
      var leftrow = 1;
      var rightrow = 1;
      if (Frontseatsoff)
      {
        leftrow = 3;
        rightrow = 3;
      }
      var group = 0;
      var groupnotdone = true;
      var groupseats = 0;
      while (group < Groups.Count)
      {
        if (!groupnotdone)
        {
          groupnotdone = true;
          groupseats = 0;
        }
        if (leftrow < rightrow)//LEFT SIDE FILLING
        {
          for (; leftrow < (Balconyoff ? Places.Length - 6 : Places.Length); leftrow++)
          {
            for (int i = 0; i < Places[leftrow].Count / 2; i++)
            {
              if (!(i <= 1 && leftrow < 3))
              {
                if (Places[leftrow][i] == -1 && groupnotdone)
                {
                  Places[leftrow][i] = group;
                  groupseats++;
                }
                if (groupseats == Groups[group].Personen)
                  groupnotdone = false;
              }
            }
            if (!groupnotdone)
              break;
          }
        }
        else
        {
          for (; rightrow < (Balconyoff ? Places.Length - 6 : Places.Length); rightrow++)
          {
            for (int i = Places[rightrow].Count - 1; i >= Places[rightrow].Count / 2; i--)
            {
              if (!((i > 20 && rightrow < 2) || (rightrow == 2 && i > 21)))
              {
                if (Places[rightrow][i] == -1 && groupnotdone)
                {
                  Places[rightrow][i] = group;
                  groupseats++;
                }
                if (groupseats == Groups[group].Personen)
                  groupnotdone = false;
              }
            }
            if (!groupnotdone)
              break;
          }
        }
        if (!groupnotdone)
          group++;
      }


      //rijen en stoel nummers per groep
      bool aan = false;
      var temp = new Tuple<int, int, int>(0, 0, 0);
      for (int grnr = 0; grnr < Groups.Count; grnr++)
      {
        Groups[grnr].Seats = new List<Tuple<int, int, int>>();
        for (int row = 1; row < Places.Length; row++)
        {

          for (int seatnr = 0; seatnr < Places[row].Count; seatnr++)
          {
            if (aan)
            {
              if (Places[row][seatnr] != grnr)
              {
                Groups[grnr].Seats.Add(new Tuple<int, int, int>(temp.Item1, temp.Item2 + 1, seatnr));
                aan = false;
              }
              else if (seatnr == Places[row].Count - 1)
              {
                Groups[grnr].Seats.Add(new Tuple<int, int, int>(temp.Item1, temp.Item2 + 1, seatnr + 1));
                aan = false;
              }
            }
            else if (Places[row][seatnr] == grnr)
            {
              temp = new Tuple<int, int, int>(row, seatnr, 0);
              aan = true;
            }
          }
        }
      }
    }
  }
  public class Lampegiet : Zaal
  {
    public Lampegiet(bool fso, bool balcony)
    {
      Balconyoff = balcony;
      Frontseatsoff = fso;
      _seats = (!fso ? 558 : 511) - (Balconyoff ? 192 : 0);
    }
    protected override void CreatePlaces()
    {
      Places = new List<int>[20];
      for (int i = 1; i < Places.Length; i++)
      {
        Places[i] = new List<int>();
        if (i <= 11)
          for (int j = 0; j < 22 + i; j++)
            Places[i].Add(-1);
        else if (i % 2 == 1)
          Places[i].Add(-1);
        if (i > 11)
          for (int j = 0; j < 31; j++)
            Places[i].Add(-1);

      }
      Places[13].Add(-1);
      Places[19].Add(-1);
      Places[19].Add(-1);
      Places[19].Add(-1);
    }

    public override void Frontseatchange(bool fso)
    {
      Frontseatsoff = fso;
      _seats = (!fso ? 558 : 511) - (Balconyoff ? 192 : 0);
    }

    public override void Draw(XGraphics gr)
    {
      const int size = 16;
      const int topmargin = 55;
      gr.DrawString("Podium", new XFont("Verdana", 15), XBrushes.Black, new XRect(250, 60, 100, 0));
      for (int i = 1; i < Places.Length; i++)
      {
        gr.DrawString(i.ToString(), new XFont("Verdana", 12), XBrushes.Black, new XRect((0 - Places[i].Count / 2) * size + 300 - (Places[i].Count % 2 * (size / 2)) - size, i <= 13 ? topmargin + i * size : topmargin + i * size + 10, size, size), new XStringFormat());
        for (int j = 0; j < Places[i].Count; j++)
        {
          int x = (j - Places[i].Count / 2) * size + 300 - (Places[i].Count % 2 * (size / 2));
          int y = topmargin + i * size;
          if (i > 13)
            y += 10;
          var rect = new XRect(x, y, size, size);
          if (Places[i][j] != -1)
            gr.DrawRectangle(_color[Places[i][j] % _color.Count], rect);
          gr.DrawRectangle(new XPen(XColor.FromArgb(0, 0, 0)), rect);
          var format = new XStringFormat { Alignment = XStringAlignment.Center };
          gr.DrawString((j + 1).ToString(), new XFont("Verdana", 10), XBrushes.Black, rect, format);
        }
      }
      var group = 0;
      var counter = 0;
      var newgroup = false;
      gr.DrawString(Groups[group].Name, new XFont("Verdana", 10), _color[group % _color.Count], new XPoint(40, 420 + group * size));
      for (var i = 0; ; i++)
      {
        if (!(counter < Groups[group].Seats.Count))
        {
          group++;
          counter = 0;
          if (group == Groups.Count)
            break;
          newgroup = true;
          gr.DrawString(Groups[group].Name, new XFont("Verdana", 10), _color[group % _color.Count], new XPoint(40 + i / 25 * 220 + (i / 25) * 100, 420 + i % 25 * size));
        }

        var tuple = Groups[group].Seats[counter];
        gr.DrawString("Rij " + tuple.Item1 + " Stoel " + tuple.Item2 + " t/m " + tuple.Item3, new XFont("Verdana", 10), XBrushes.Black, new XPoint(40 + i / 25 * 220 + (i / 25 + 1) * 100, 420 + i % 25 * size));
        counter++;
        if (newgroup)
        {
          gr.DrawLine(XPens.Black, 40 + i / 25 * 220 + (i / 25) * 100, 409 + i % 25 * size,
                                   270 + i / 25 * 220 + (i / 25) * 100, 409 + i % 25 * size);
          newgroup = false;
        }
      }
    }
    List<XBrush> _color = new List<XBrush> {
          new XSolidBrush(XColor.FromArgb(119,170,221)),
          new XSolidBrush(XColor.FromArgb(170,170,0)),
          new XSolidBrush(XColor.FromArgb(238,136,102)),
          new XSolidBrush(XColor.FromArgb(238,221,136)),
          new XSolidBrush(XColor.FromArgb(255,170,187)),
          new XSolidBrush(XColor.FromArgb(153,221,255)),
          new XSolidBrush(XColor.FromArgb(68,187,153)),
          new XSolidBrush(XColor.FromArgb(187,204,51)),
          new XSolidBrush(XColor.FromArgb(153,153,153))};
    // XBrushes.Red, XBrushes.Fuchsia, XBrushes.Orange, XBrushes.Green, XBrushes.Indigo, XBrushes.SteelBlue, XBrushes.Firebrick, XBrushes.DarkViolet, XBrushes.Turquoise, XBrushes.IndianRed, XBrushes.SeaGreen, XBrushes.CornflowerBlue, XBrushes.BurlyWood };
  }
  //public class Lampegietnobalkon : Lampegiet
  //{
  //    public Lampegietnobalkon(bool fso)
  //    {
  //        Frontseatsoff = fso;
  //        _seats = !fso ? 360 : 313;
  //    }
  //}
}
