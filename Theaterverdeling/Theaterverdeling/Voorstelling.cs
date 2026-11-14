using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
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

    // Draws one page for this showing: show name, date and time, then the hall with its legend.
    public void Draw(IDrawingContext ctx, string showName)
    {
      const float titleTop = 20;
      const float lineHeight = 16;
      ctx.DrawString(showName, new RectangleF(0, titleTop, ctx.PageWidth, lineHeight));
      ctx.DrawString($"{Datum.ToShortDateString()} {Time.ToShortTimeString()}", new RectangleF(0, titleTop + lineHeight, ctx.PageWidth, lineHeight));
      Zaal.Draw(ctx);
    }

    public void Draw(XGraphics gr, string showName)
    {
      Draw(new XGraphicsDrawingContext(gr), showName);
    }
  }
}
