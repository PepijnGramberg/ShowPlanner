using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using PdfSharpCore.Drawing;
using System.Drawing;

namespace Theaterverdeling
{
  [JsonConverter(typeof(RowConverter))]
  public abstract class Row
  {
    public virtual int TotalSeatCount { get; } = 0;
    public virtual int AvailableSeatCount { get; } = 0;
  }

  public sealed class WalkWay : Row
  {
  }

  public sealed class ChairRow : Row
  {
    public ChairRow(int seatCount, bool inUse, int rowNr = 0, IEnumerable<int> unusable = null, string section = null)
    {
      InUse = inUse;
      RowNr = rowNr;
      Section = section;
      var unusableNumbers = new HashSet<int>(unusable ?? Enumerable.Empty<int>());
      Seats = new Seat[seatCount];
      for (var i = 0; i < Seats.Length; i++)
      {
        Seats[i] = new Seat(!unusableNumbers.Contains(i + 1));
      }
    }
    public bool InUse { get; }
    public int RowNr { get; }

    // Name of the part of the hall this row belongs to, such as the balcony. Null when the row cannot be switched off.
    public string Section { get; }

    // True when the section of this row is switched off for the showing.
    public bool SwitchedOff { get; set; }

    public bool Available => InUse && !SwitchedOff;
    public override int TotalSeatCount => Seats.Length;

    public override int AvailableSeatCount => Available ? Seats.Count(seat => seat.Usable) : 0;

    public Seat[] Seats { get; }
  }

  public sealed class Seat
  {
    public Seat(bool usable)
    {
      Usable = usable;
    }

    // False for a chair that exists in the hall but is never given to a group.
    public bool Usable { get; }
    public Group Group { get; set; }
  }

  // Custom converter for Row polymorphism
  public class RowConverter : JsonConverter<Row>
  {
    public override Row Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      // Save reader state
      var readerCopy = reader;
      
      // Check if this is a JSON object
      if (readerCopy.TokenType != JsonTokenType.StartObject)
      {
        throw new JsonException("Expected JSON object");
      }
      
      // Read to get the type discriminator
      readerCopy.Read();
      if (readerCopy.TokenType != JsonTokenType.PropertyName || readerCopy.GetString() != "Type")
      {
        throw new JsonException("Expected Type property");
      }
      
      // Read the type name
      readerCopy.Read();
      string typeName = readerCopy.GetString();
      
      // Return the appropriate type based on the discriminator
      if (typeName == "WalkWay")
      {
        return JsonSerializer.Deserialize<WalkWay>(ref reader, options);
      }
      else if (typeName == "ChairRow")
      {
        // Read the object into a JsonDocument first
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var json = jsonDoc.RootElement.GetRawText();
        
        // Extract properties needed for constructor
        using var jsonDoc2 = JsonDocument.Parse(json);
        var root = jsonDoc2.RootElement;
        
        int seatCount = 0;
        // A row is in use unless the file says otherwise.
        bool inUse = true;
        int rowNr = 0;
        
        if (root.TryGetProperty("SeatCount", out var seatCountProp))
        {
          seatCount = seatCountProp.GetInt32();
        }
        
        if (root.TryGetProperty("InUse", out var inUseProp))
        {
          inUse = inUseProp.GetBoolean();
        }
        
        if (root.TryGetProperty("RowNr", out var rowNrProp))
        {
          rowNr = rowNrProp.GetInt32();
        }
        
        var unusable = new List<int>();
        if (root.TryGetProperty("Unusable", out var unusableProp))
        {
          unusable.AddRange(unusableProp.EnumerateArray().Select(number => number.GetInt32()));
        }

        // Create new ChairRow with constructor
        string section = null;
        if (root.TryGetProperty("Section", out var sectionProp))
        {
          section = sectionProp.GetString();
        }

        return new ChairRow(seatCount, inUse, rowNr, unusable, section);
      }
      
      throw new JsonException($"Unknown Row type: {typeName}");
    }

    public override void Write(Utf8JsonWriter writer, Row value, JsonSerializerOptions options)
    {
      writer.WriteStartObject();
      
      // Write a type discriminator
      writer.WriteString("Type", value.GetType().Name);
      
      if (value is ChairRow chairRow)
      {
        if (!chairRow.InUse)
        {
          writer.WriteBoolean("InUse", false);
        }
        writer.WriteNumber("SeatCount", chairRow.TotalSeatCount);
        writer.WriteNumber("RowNr", chairRow.RowNr);
        
        if (chairRow.Section != null)
        {
          writer.WriteString("Section", chairRow.Section);
        }

        // Seat numbers, starting at 1, that are never given to a group
        if (chairRow.Seats.Any(seat => !seat.Usable))
        {
          writer.WriteStartArray("Unusable");
          for (var i = 0; i < chairRow.Seats.Length; i++)
          {
            if (!chairRow.Seats[i].Usable)
            {
              writer.WriteNumberValue(i + 1);
            }
          }
          writer.WriteEndArray();
        }

      }
      
      writer.WriteEndObject();
    }
  }

  public class Zaal
  {
    // Name of the hall file this hall was loaded from, without extension.
    public string Name { get; private set; }

    // The folder next to the app that holds one JSON file per hall.
    public static string HallsFolder => Path.Combine(AppContext.BaseDirectory, "Zalen");

    // The names of the halls that can be loaded, in alphabetical order.
    public static List<string> HallNames()
    {
      if (!Directory.Exists(HallsFolder))
      {
        return new List<string>();
      }
      return Directory.GetFiles(HallsFolder, "*.json")
        .Select(Path.GetFileNameWithoutExtension)
        .OrderBy(name => name, StringComparer.CurrentCultureIgnoreCase)
        .ToList();
    }

    public static bool Exists(string name)
    {
      return File.Exists(HallFile(name));
    }

    // Loads a hall from the halls folder next to the app.
    public static Zaal Load(string name)
    {
      if (!Exists(name))
      {
        throw new FileNotFoundException($"De zaal \"{Path.GetFileName(name)}\" staat niet in de map Zalen naast het programma. Zet het zaalbestand in die map, of maak de zaal met \"Zalen beheren\".");
      }
      try
      {
        var hall = LoadFromFile<Zaal>(HallFile(name));
        hall.Name = Path.GetFileName(name);
        return hall;
      }
      catch (JsonException)
      {
        throw new InvalidDataException($"Het bestand van de zaal \"{Path.GetFileName(name)}\" kan niet worden gelezen.");
      }
    }

    // Writes the hall to the halls folder under the given name, over a hall that already has that name.
    public void Save(string name)
    {
      Directory.CreateDirectory(HallsFolder);
      SaveToFile(HallFile(name));
      Name = Path.GetFileName(name);
    }

    // A hall with the given rows, from the stage to the back.
    public static Zaal FromRows(IEnumerable<Row> rows)
    {
      var hall = new Zaal();
      hall._rows.AddRange(rows);
      return hall;
    }

    private static string HallFile(string name)
    {
      // Only the file name counts, so a name from a plan file cannot point outside the halls folder.
      return Path.Combine(HallsFolder, Path.GetFileName(name) + ".json");
    }

    public int Seats => _rows.Sum(r => r.AvailableSeatCount);
    public List<Group> Groups { get; set; }

    // Change to protected with JsonInclude attribute to allow serialization
    [JsonInclude]
    protected List<Row> _rows = new List<Row>();
    public IReadOnlyList<Row> Rows => _rows;

    // Named parts of the hall that can be switched off for a showing, such as the balcony.
    public IReadOnlyList<string> Sections => _rows.OfType<ChairRow>().Select(row => row.Section).Where(section => section != null).Distinct().ToList();

    public IReadOnlyList<string> SwitchedOffSections => _rows.OfType<ChairRow>().Where(row => row.SwitchedOff).Select(row => row.Section).Distinct().ToList();

    public void SwitchOff(string section)
    {
      foreach (var row in _rows.OfType<ChairRow>().Where(row => row.Section == section))
      {
        row.SwitchedOff = true;
      }
    }

    // Gives every group its seats, in the order of the list of groups. A group sits row by row on
    // one side of the hall, on the side that is the fewest rows along. On top of that:
    // - nobody sits alone: when the last row of a group would hold one person, the group leaves a
    //   seat free in the row before, so two sit together;
    // - a group that would sit on both sides of a walkway starts behind the walkway instead;
    // - seats that these two rules leave open go to a later group that fits in them completely.
    // When a group finds no place that way, the hall is too full for these rules and all groups
    // are seated the plain way: row by row, using every seat.
    // A group whose seats were changed by hand keeps exactly those seats; the other groups are
    // seated around it.
    public virtual void AssignSeats()
    {
      var byHand = SeatsByHand();
      ClearSeats(byHand);
      if (!AssignSeatsWithRules())
      {
        ClearSeats(byHand);
        AssignSeatsRowByRow();
      }
    }

    // The seats in this hall of the groups that were seated by hand. A group that has no seat here,
    // because it came from another showing or the hall changed, is seated like the others again.
    private Dictionary<Group, List<Seat>> SeatsByHand()
    {
      var byHand = new Dictionary<Group, List<Seat>>();
      foreach (var group in Groups.Where(group => group.HandSeated))
      {
        var seats = _rows.OfType<ChairRow>()
          .Where(row => row.Available)
          .SelectMany(row => row.Seats)
          .Where(seat => seat.Usable && seat.Group == group)
          .ToList();
        if (seats.Count == 0)
        {
          group.HandSeated = false;
        }
        else
        {
          byHand[group] = seats;
        }
      }
      return byHand;
    }

    // Makes every seat free, except the seats that were given by hand.
    private void ClearSeats(Dictionary<Group, List<Seat>> byHand)
    {
      foreach (var seat in _rows.OfType<ChairRow>().SelectMany(row => row.Seats))
      {
        seat.Group = null;
      }
      foreach (var entry in byHand)
      {
        foreach (var seat in entry.Value)
        {
          seat.Group = entry.Key;
        }
      }
    }

    // Gives a group the seats in the given rows by hand, as far as those seats exist and are free.
    // Used to put back the seats that were saved with a plan.
    public void SeatByHand(Group group, IEnumerable<SeatRange> ranges)
    {
      foreach (var range in ranges)
      {
        var row = _rows.OfType<ChairRow>().FirstOrDefault(candidate => candidate.RowNr == range.Row && candidate.Available);
        if (row == null)
        {
          continue;
        }
        for (var number = Math.Max(range.StartSeat, 1); number <= Math.Min(range.EndSeat, row.Seats.Length); number++)
        {
          var seat = row.Seats[number - 1];
          if (seat.Usable && seat.Group == null)
          {
            seat.Group = group;
          }
        }
      }
      group.Seats = SeatRangesOf(group);
      group.HandSeated = group.Seats.Count > 0;
    }

    // Gives the seats of one group to another: a changed group takes the place of the old one.
    public void HandOver(Group from, Group to)
    {
      foreach (var seat in _rows.OfType<ChairRow>().SelectMany(row => row.Seats).Where(seat => seat.Group == from))
      {
        seat.Group = to;
      }
      to.Seats = SeatRangesOf(to);
    }

    // One side of the hall while it is being filled.
    private sealed class Side
    {
      public Func<ChairRow, IEnumerable<Seat>> Half;

      // The row this side has reached, as position in the rows that can be used.
      public int Row;
    }

    // Seats on one side that were left open: the half rows from Start up to End.
    private sealed class Gap
    {
      public Side Side;
      public int Start;
      public int End;
    }

    // The seats a group gets in one row.
    private sealed class RowPart
    {
      public int Row;
      public List<Seat> Seats;
    }

    // Seats the groups with the rules described at AssignSeats. Returns false when a group finds no place.
    private bool AssignSeatsWithRules()
    {
      // The rows that can be used, each with the part of the hall it is in: walkways separate the parts.
      var rows = new List<ChairRow>();
      var parts = new List<int>();
      var part = 0;
      foreach (var row in _rows)
      {
        if (row is WalkWay)
        {
          part++;
        }
        else if (row is ChairRow chairRow && chairRow.Available)
        {
          rows.Add(chairRow);
          parts.Add(part);
        }
      }

      var left = new Side { Half = LeftHalf };
      var right = new Side { Half = RightHalf };
      var gaps = new List<Gap>();
      foreach (var group in Groups)
      {
        List<RowPart> seats = null;
        if (group.Personen > 0 && !group.HandSeated)
        {
          // Seats that were left open come first, when the whole group fits in them.
          // Nobody sits alone there either: a gap that would put one person of the group in a row is passed over.
          var gap = gaps.OrderBy(open => open.Start).FirstOrDefault(open => TakeTogether(rows, open, group.Personen) != null);
          if (gap != null)
          {
            seats = TakeTogether(rows, gap, group.Personen);
          }
          else
          {
            // The side that is the fewest rows along gets the group. The right side wins a tie.
            var first = left.Row < right.Row ? left : right;
            var second = first == left ? right : left;
            seats = Place(rows, parts, first, gaps, group.Personen) ?? Place(rows, parts, second, gaps, group.Personen);
          }
          if (seats == null)
          {
            return false;
          }
          foreach (var seat in seats.SelectMany(rowPart => rowPart.Seats))
          {
            seat.Group = group;
          }
        }
        group.Seats = SeatRangesOf(group);
      }
      return true;
    }

    // Finds seats for a group on one side, from the row that side has reached, and moves the side along.
    // Returns null when the side has too few seats left.
    private static List<RowPart> Place(List<ChairRow> rows, List<int> parts, Side side, List<Gap> gaps, int people)
    {
      var seats = Take(rows, side, side.Row, rows.Count, people);
      if (seats == null)
      {
        return null;
      }

      // Nobody sits alone at the start either: when the row the side has reached has one seat left,
      // the group starts in the next row and that seat stays open.
      if (seats.Count >= 2 && seats[0].Seats.Count == 1)
      {
        var next = Take(rows, side, seats[0].Row + 1, rows.Count, people);
        if (next != null)
        {
          gaps.Add(new Gap { Side = side, Start = seats[0].Row, End = seats[0].Row + 1 });
          seats = next;
        }
      }

      // A group that would sit on both sides of a walkway starts behind it, when it fits there
      // without crossing the next walkway. The seats in front of the walkway stay open.
      var firstPart = parts[seats[0].Row];
      if (seats.Any(rowPart => parts[rowPart.Row] != firstPart))
      {
        var behind = seats[0].Row;
        while (parts[behind] == firstPart)
        {
          behind++;
        }
        var moved = Take(rows, side, behind, rows.Count, people);
        if (moved != null && moved.All(rowPart => parts[rowPart.Row] == parts[behind]))
        {
          gaps.Add(new Gap { Side = side, Start = side.Row, End = behind });
          seats = moved;
        }
      }

      // Nobody sits alone: one person in the last row gets a neighbour from the row before,
      // as long as that does not leave somebody alone there.
      if (seats.Count >= 2)
      {
        var last = seats[seats.Count - 1];
        var before = seats[seats.Count - 2];
        var neighbour = FreeSeats(rows[last.Row], side).Skip(1).FirstOrDefault();
        if (last.Seats.Count == 1 && before.Seats.Count >= 3 && neighbour != null)
        {
          before.Seats.RemoveAt(before.Seats.Count - 1);
          last.Seats.Add(neighbour);
          gaps.Add(new Gap { Side = side, Start = before.Row, End = before.Row + 1 });
        }
      }

      // The side stays on the row where the group ends, so the next group can use what is left of it.
      side.Row = seats[seats.Count - 1].Row;
      return seats;
    }

    // The first free seats for a number of people on one side, in the rows from start up to end.
    // Returns null when there are too few.
    private static List<RowPart> Take(List<ChairRow> rows, Side side, int start, int end, int people)
    {
      var seats = new List<RowPart>();
      var remaining = people;
      for (var row = start; row < end && remaining > 0; row++)
      {
        var free = FreeSeats(rows[row], side).Take(remaining).ToList();
        if (free.Count > 0)
        {
          seats.Add(new RowPart { Row = row, Seats = free });
          remaining -= free.Count;
        }
      }
      return remaining == 0 ? seats : null;
    }

    // The seats for a number of people in a gap, or null when they do not fit there or one of them
    // would sit alone in a row.
    private static List<RowPart> TakeTogether(List<ChairRow> rows, Gap gap, int people)
    {
      var seats = Take(rows, gap.Side, gap.Start, gap.End, people);
      return seats == null || (people > 1 && seats.Any(rowPart => rowPart.Seats.Count == 1)) ? null : seats;
    }

    // The seats of a row on one side that nobody has yet, from the edge towards the middle.
    private static IEnumerable<Seat> FreeSeats(ChairRow row, Side side)
    {
      return side.Half(row).Where(seat => seat.Usable && seat.Group == null);
    }

    // Seats the groups row by row and uses every seat, without the extra rules.
    private void AssignSeatsRowByRow()
    {
      var rows = _rows.OfType<ChairRow>().Where(row => row.Available).ToList();
      var left = 0;
      var right = 0;
      foreach (var group in Groups)
      {
        // A group that was seated by hand already has its seats.
        var remaining = group.HandSeated ? 0 : group.Personen;
        while (remaining > 0 && (left < rows.Count || right < rows.Count))
        {
          // The side that is the fewest rows along gets the group. The right side wins a tie.
          remaining = left < right
            ? FillSide(rows, ref left, LeftHalf, group, remaining)
            : FillSide(rows, ref right, RightHalf, group, remaining);
        }
        if (remaining > 0)
        {
          throw new InvalidOperationException($"Niet genoeg stoelen voor groep \"{group.Name}\": {remaining} personen hebben geen plaats.");
        }
        group.Seats = SeatRangesOf(group);
      }
    }

    // Number of seats the group has in this hall.
    public int SeatsOf(Group group)
    {
      return _rows.OfType<ChairRow>().SelectMany(row => row.Seats).Count(seat => seat.Group == group);
    }

    // Changes one seat by hand: a seat of the group becomes free, and a free seat goes to the group
    // as long as the group has people without a seat. Says why the seat cannot change, or null when it did.
    // Seats that are changed by hand last until the hall is seated again.
    public string ChangeSeat(ChairRow row, int seatIndex, Group group)
    {
      var seat = row.Seats[seatIndex];
      if (seat.Group == group)
      {
        seat.Group = null;
      }
      else if (seat.Group != null)
      {
        return $"Deze stoel is van {seat.Group.Name}.";
      }
      else if (!row.Available || !seat.Usable)
      {
        return "Deze stoel wordt niet gebruikt.";
      }
      else if (SeatsOf(group) >= group.Personen)
      {
        return $"{group.Name} heeft al {group.Personen} stoelen. Klik eerst op een stoel van de groep om die vrij te maken.";
      }
      else
      {
        seat.Group = group;
      }
      group.Seats = SeatRangesOf(group);
      group.HandSeated = true;
      return null;
    }

    // How a group looks on the seat map and in the legend. Twelve light colours can be told apart
    // well; more cannot. So the first twelve groups of a showing get a colour, and later groups get
    // the same colours again with one dark stripe (groups 13-24) or two (groups 25-36) on each seat.
    // The hue jumps 150 degrees per group, so groups that are seated after each other, and so sit
    // next to each other, are far apart in colour.
    private const int ColourCount = 12;

    // Seats nobody can sit on in a showing: rows that are switched off and seats that are never used.
    public static readonly Color ClosedSeatColor = Color.FromArgb(205, 205, 205);

    public static Color GroupColor(int index)
    {
      var colour = index % ColourCount;
      return FromHsl(GroupHue(index), 0.9, colour % 2 == 0 ? 0.70 : 0.84);
    }

    // Number of dark stripes on the seats of the n-th group: 0, 1 or 2.
    public static int GroupStripes(int index)
    {
      return index / ColourCount % 3;
    }

    // The stripes are a dark shade of the colour of the group.
    public static Color GroupStripeColor(int index)
    {
      return FromHsl(GroupHue(index), 0.9, 0.35);
    }

    private static double GroupHue(int index)
    {
      return (210 + index % ColourCount * 150) % 360;
    }

    // Draws the marker that stands for the n-th group of a showing in a legend or a list:
    // a box that looks like the seats of the group.
    public static void DrawGroupMarker(IDrawingContext ctx, RectangleF rect, int index)
    {
      FillForGroup(ctx, rect, index);
      ctx.DrawRectangles(new[] { rect });
    }

    // Fills a seat or legend marker the way the n-th group looks: its colour, with its stripes
    // along the bottom edge and, for two stripes, also along the top edge.
    private static void FillForGroup(IDrawingContext ctx, RectangleF rect, int index)
    {
      ctx.FillRectangle(rect, GroupColor(index));
      var stripes = GroupStripes(index);
      var height = rect.Height * 0.22f;
      if (stripes >= 1)
      {
        ctx.FillRectangle(new RectangleF(rect.X, rect.Bottom - height, rect.Width, height), GroupStripeColor(index));
      }
      if (stripes >= 2)
      {
        ctx.FillRectangle(new RectangleF(rect.X, rect.Y, rect.Width, height), GroupStripeColor(index));
      }
    }

    private static Color FromHsl(double hue, double saturation, double lightness)
    {
      var chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
      var sector = hue / 60;
      var x = chroma * (1 - Math.Abs(sector % 2 - 1));
      var (r, g, b) = (int)sector switch
      {
        0 => (chroma, x, 0.0),
        1 => (x, chroma, 0.0),
        2 => (0.0, chroma, x),
        3 => (0.0, x, chroma),
        4 => (x, 0.0, chroma),
        _ => (chroma, 0.0, x)
      };
      var m = lightness - chroma / 2;
      return Color.FromArgb(ToByte(r + m), ToByte(g + m), ToByte(b + m));
    }

    private static int ToByte(double value)
    {
      return (int)Math.Round(Math.Min(Math.Max(value, 0), 1) * 255);
    }

    // Draws the stage label, the seat map coloured per group and the legend with the seats of each group.
    // The space above the stage label is left free for a title.
    public virtual void Draw(IDrawingContext ctx)
    {
      const float padding = 20;
      const float lineHeight = 16;
      const float stageTop = 56;
      const float mapTop = stageTop + lineHeight + 4;
      const float mapHeight = 500;

      var mapWidth = ctx.PageWidth - (padding * 2);

      ctx.DrawString("Podium", new RectangleF(0, stageTop, ctx.PageWidth, lineHeight));

      var layout = DrawMap(ctx, new RectangleF(padding, mapTop, mapWidth, mapHeight));

      DrawLegend(ctx, Groups ?? new List<Group>(), mapTop + layout.Height + lineHeight, lineHeight);
    }

    // Draws the seat map alone, coloured per group, in the given rectangle. Returns where the rows
    // and seats ended up, measured from the top left corner of that rectangle.
    public SeatMapLayout DrawMap(IDrawingContext ctx, RectangleF bounds)
    {
      var groups = Groups ?? new List<Group>();
      var layout = new SeatMapLayout(Rows, bounds.Width, bounds.Height);

      using (ctx.BeginContainer(bounds, new RectangleF(0, 0, bounds.Width, bounds.Height)))
      {
        for (var i = 0; i < Rows.Count; i++)
        {
          if (Rows[i] is ChairRow chairRow)
          {
            var chairs = new List<RectangleF>();
            for (var j = 0; j < chairRow.TotalSeatCount; j++)
            {
              chairs.Add(layout.SeatBounds(i, j));
            }

            // Row number on the left and on the right
            ctx.DrawString(chairRow.RowNr.ToString(), layout.RowNumberBounds(i, false));
            ctx.DrawString(chairRow.RowNr.ToString(), layout.RowNumberBounds(i, true));

            for (var j = 0; j < chairs.Count; j++)
            {
              // Colour the chair of the group that sits on it. Chairs nobody can sit on are grey; free chairs stay white.
              var seat = chairRow.Seats[j];
              var groupIndex = groups.IndexOf(seat.Group);
              if (groupIndex >= 0)
              {
                FillForGroup(ctx, chairs[j], groupIndex);
              }
              else if (!chairRow.Available || !seat.Usable)
              {
                ctx.FillRectangle(chairs[j], ClosedSeatColor);
              }
              // Seats in the real world are not 0 indexed so we increment the number for display purposes.
              ctx.DrawString((j + 1).ToString(), chairs[j]);
            }
            // Black outline.
            // A row that is still being typed in the hall editor can be without chairs.
            if (chairs.Count > 0)
            {
              ctx.DrawRectangles(chairs.ToArray());
            }
          }
        }
      }

      return layout;
    }

    // Lists the groups below the seat map: marker, name, then the rows and seats of the group,
    // four per line. The lines are squeezed together when a full hall would not fit on the page.
    private static void DrawLegend(IDrawingContext ctx, List<Group> groups, float top, float lineHeight)
    {
      const int rangesPerLine = 4;
      const float left = 20;
      const float bottomMargin = 20;
      const float markerSize = 10;
      const float nameWidth = 170;
      const float rangeWidth = 85;

      var lineCount = groups.Sum(group => LegendLines(group, rangesPerLine));
      if (lineCount == 0)
      {
        return;
      }
      lineHeight = Math.Min(lineHeight, (ctx.PageHeight - bottomMargin - top) / lineCount);
      var markerHeight = Math.Min(markerSize, lineHeight - 2);
      var nameLeft = left + markerSize + 4;
      var rangesLeft = nameLeft + nameWidth;

      var y = top;
      for (var g = 0; g < groups.Count; g++)
      {
        var group = groups[g];
        if (g > 0)
        {
          ctx.DrawLine(new PointF(left, y), new PointF(rangesLeft + rangesPerLine * rangeWidth, y));
        }
        var marker = new RectangleF(left, y + (lineHeight - markerHeight) / 2, markerSize, markerHeight);
        DrawGroupMarker(ctx, marker, g);
        ctx.DrawText(group.Name, new RectangleF(nameLeft, y, nameWidth, lineHeight));

        for (var i = 0; i < group.Seats.Count; i++)
        {
          var range = group.Seats[i];
          var seats = range.StartSeat == range.EndSeat ? $"{range.StartSeat}" : $"{range.StartSeat}-{range.EndSeat}";
          var position = new RectangleF(rangesLeft + i % rangesPerLine * rangeWidth, y + i / rangesPerLine * lineHeight, rangeWidth, lineHeight);
          ctx.DrawText($"rij {range.Row}: {seats}", position);
        }
        y += LegendLines(group, rangesPerLine) * lineHeight;
      }
    }

    private static int LegendLines(Group group, int rangesPerLine)
    {
      return Math.Max(1, (group.Seats.Count + rangesPerLine - 1) / rangesPerLine);
    }

    // Serialize the Zaal to JSON
    public string ToJson()
    {
      var options = new JsonSerializerOptions
      {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
      };
      
      // Create a serializable representation
      var zaalData = new ZaalSerializationData
      {
        ZaalType = GetType().Name,
        Rows = _rows
      };
      
      return JsonSerializer.Serialize(zaalData, options);
    }

    // Deserialize from JSON
    public static T FromJson<T>(string json) where T : Zaal, new()
    {
      var options = new JsonSerializerOptions
      {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
      };
      
      var zaalData = JsonSerializer.Deserialize<ZaalSerializationData>(json, options);
      
      // Create a new instance of the specific Zaal type
      T result = new T();
      
      // Populate the rows
      if (zaalData.Rows != null)
      {
        result._rows = zaalData.Rows;
      }
      
      return result;
    }

    // Save to a JSON file
    public void SaveToFile(string filePath)
    {
      File.WriteAllText(filePath, ToJson());
    }

    // Load from a JSON file
    public static T LoadFromFile<T>(string filePath) where T : Zaal, new()
    {
      if (!File.Exists(filePath))
      {
        throw new FileNotFoundException($"File not found: {filePath}");
      }
      
      string json = File.ReadAllText(filePath);
      return FromJson<T>(json);
    }

    // Seats a group row by row on one side of the hall, starting at the row that side has reached.
    // The side stays on the row where the group ends, so the next group can use what is left of it.
    private static int FillSide(List<ChairRow> rows, ref int row, Func<ChairRow, IEnumerable<Seat>> half, Group group, int remaining)
    {
      for (; row < rows.Count; row++)
      {
        foreach (var seat in half(rows[row]))
        {
          if (remaining == 0)
            break;
          if (seat.Usable && seat.Group == null)
          {
            seat.Group = group;
            remaining--;
          }
        }
        if (remaining == 0)
          break;
      }
      return remaining;
    }

    // Left half of a row, from the left edge towards the middle.
    private static IEnumerable<Seat> LeftHalf(ChairRow row)
    {
      return row.Seats.Take(row.Seats.Length / 2);
    }

    // Right half of a row, from the right edge towards the middle. The middle seat of an odd row belongs to the right.
    private static IEnumerable<Seat> RightHalf(ChairRow row)
    {
      return row.Seats.Skip(row.Seats.Length / 2).Reverse();
    }

    // The seats of a group as runs of adjacent seats per row, with seat numbers starting at 1.
    private List<SeatRange> SeatRangesOf(Group group)
    {
      var ranges = new List<SeatRange>();
      foreach (var row in _rows.OfType<ChairRow>())
      {
        var start = -1;
        for (var i = 0; i <= row.Seats.Length; i++)
        {
          var ofGroup = i < row.Seats.Length && row.Seats[i].Group == group;
          if (ofGroup && start < 0)
          {
            start = i;
          }
          else if (!ofGroup && start >= 0)
          {
            ranges.Add(new SeatRange(row.RowNr, start + 1, i));
            start = -1;
          }
        }
      }
      return ranges;
    }
  }

  // Helper class for serialization
  public class ZaalSerializationData
  {
    public string ZaalType { get; set; }
    public List<Row> Rows { get; set; }
  }

  public struct SeatRange
  {
    public int Row { get; }
    public int StartSeat { get; }
    public int EndSeat { get; }
    public SeatRange(int row, int startSeat, int endSeat)
    {
      Row = row;
      StartSeat = startSeat;
      EndSeat = endSeat;
    }
  }
}
