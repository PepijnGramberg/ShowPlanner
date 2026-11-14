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
    public ChairRow(int seatCount, bool inUse, int rowNr = 0, IEnumerable<int> unusable = null)
    {
      InUse = inUse;
      RowNr = rowNr;
      var unusableNumbers = new HashSet<int>(unusable ?? Enumerable.Empty<int>());
      Seats = new Seat[seatCount];
      for (var i = 0; i < Seats.Length; i++)
      {
        Seats[i] = new Seat(!unusableNumbers.Contains(i + 1));
      }
    }
    public bool InUse { get; }
    public int RowNr { get; }
    public override int TotalSeatCount => Seats.Length;

    public override int AvailableSeatCount => InUse ? Seats.Count(seat => seat.Usable) : 0;

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
        bool inUse = false;
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
        return new ChairRow(seatCount, inUse, rowNr, unusable);
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
        writer.WriteBoolean("InUse", chairRow.InUse);
        writer.WriteNumber("SeatCount", chairRow.TotalSeatCount);
        writer.WriteNumber("RowNr", chairRow.RowNr);
        
        // Seat numbers, starting at 1, that are never given to a group
        writer.WriteStartArray("Unusable");
        for (var i = 0; i < chairRow.Seats.Length; i++)
        {
          if (!chairRow.Seats[i].Usable)
          {
            writer.WriteNumberValue(i + 1);
          }
        }
        writer.WriteEndArray();

        // Handle seats array
        writer.WriteStartArray("Seats");
        foreach (var seat in chairRow.Seats)
        {
          writer.WriteStartObject();
          writer.WriteEndObject(); // Empty seat object (can add properties later)
        }
        writer.WriteEndArray();
      }
      
      writer.WriteEndObject();
    }
  }

  public abstract class Zaal
  {
    public int Seats => _rows.Sum(r => r.AvailableSeatCount);
    public List<Group> Groups { get; set; }

    // Change to protected with JsonInclude attribute to allow serialization
    [JsonInclude]
    protected List<Row> _rows = new List<Row>();
    public IReadOnlyList<Row> Rows => _rows;

    public virtual void AssignSeats()
    {
      foreach (var seat in _rows.OfType<ChairRow>().SelectMany(row => row.Seats))
      {
        seat.Group = null;
      }

      var rows = _rows.OfType<ChairRow>().Where(row => row.InUse).ToList();
      var left = 0;
      var right = 0;
      foreach (var group in Groups)
      {
        var remaining = group.Personen;
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

    public void Draw(XGraphics gr) 
    {
      Draw(new XGraphicsDrawingContext(gr));
    }

    public void Draw(Graphics gr)
    {
      Draw(new GdiDrawingContext(gr));
    }

    private static readonly Color[] GroupColors =
    {
      Color.FromArgb(119, 170, 221),
      Color.FromArgb(170, 170, 0),
      Color.FromArgb(238, 136, 102),
      Color.FromArgb(238, 221, 136),
      Color.FromArgb(255, 170, 187),
      Color.FromArgb(153, 221, 255),
      Color.FromArgb(68, 187, 153),
      Color.FromArgb(187, 204, 51),
      Color.FromArgb(153, 153, 153)
    };

    // Draws the stage label, the seat map coloured per group and the legend with the seats of each group.
    // The space above the stage label is left free for a title.
    public virtual void Draw(IDrawingContext ctx)
    {
      const float padding = 20;
      const float lineHeight = 16;
      const float stageTop = 56;
      const float mapTop = stageTop + lineHeight + 4;
      const float mapHeight = 500;

      var groups = Groups ?? new List<Group>();
      var maxWidthChairs = Rows.Max(r => r.TotalSeatCount) + 2; // two extra for the row number on both sides
      var mapWidth = ctx.PageWidth - (padding * 2);
      var seatSize = Math.Min(mapWidth / maxWidthChairs, mapHeight / Rows.Count);

      ctx.DrawString("Podium", new RectangleF(0, stageTop, ctx.PageWidth, lineHeight));

      using (ctx.BeginContainer(
             new RectangleF(padding, mapTop, mapWidth, mapHeight),
             new RectangleF(0, 0, mapWidth, mapHeight)))
      {
        for (var i = 0; i < Rows.Count; i++)
        {
          var rowHeight = i * seatSize;
          if (Rows[i] is ChairRow chairRow)
          {
            // Start of the row needs to be offset from the Graphics box so the entire row is centered.
            var startRow = (mapWidth - ((chairRow.TotalSeatCount + 2) * seatSize)) / 2;

            var chairs = new List<RectangleF>();
            for (var j = 0; j < chairRow.TotalSeatCount; j++)
            {
              // Add 1 to J for so there is space for the row number in front
              chairs.Add(new RectangleF(startRow + (j + 1) * seatSize, rowHeight, seatSize, seatSize));
            }

            // Row number on the left and on the right
            ctx.DrawString(chairRow.RowNr.ToString(), new RectangleF(startRow, rowHeight, seatSize, seatSize));
            ctx.DrawString(chairRow.RowNr.ToString(), new RectangleF(startRow + (chairRow.TotalSeatCount + 1) * seatSize, rowHeight, seatSize, seatSize));

            for (var j = 0; j < chairs.Count; j++)
            {
              // Colour the chair of the group that sits on it; free chairs stay white.
              var groupIndex = groups.IndexOf(chairRow.Seats[j].Group);
              if (groupIndex >= 0)
              {
                ctx.FillRectangle(chairs[j], GroupColors[groupIndex % GroupColors.Length]);
              }
              // Seats in the real world are not 0 indexed so we increment the number for display purposes.
              ctx.DrawString((j + 1).ToString(), chairs[j]);
            }
            // Black outline.
            ctx.DrawRectangles(chairs.ToArray());
          }
        }
      }

      DrawLegend(ctx, groups, mapTop + Rows.Count * seatSize + lineHeight, lineHeight);
    }

    // Lists per group the rows and seats it got, in columns of 25 lines below the seat map.
    private static void DrawLegend(IDrawingContext ctx, List<Group> groups, float top, float lineHeight)
    {
      const int linesPerColumn = 25;
      const float left = 20;
      const float columnWidth = 285;
      const float nameWidth = 150;
      const float markerSize = 10;

      var line = 0;
      for (var g = 0; g < groups.Count; g++)
      {
        var group = groups[g];
        var x = left + (line / linesPerColumn) * columnWidth;
        var y = top + (line % linesPerColumn) * lineHeight;

        if (g > 0)
        {
          ctx.DrawLine(new PointF(x, y), new PointF(x + columnWidth - 10, y));
        }
        var marker = new RectangleF(x, y + (lineHeight - markerSize) / 2, markerSize, markerSize);
        ctx.FillRectangle(marker, GroupColors[g % GroupColors.Length]);
        ctx.DrawRectangles(new[] { marker });
        ctx.DrawText(group.Name, new RectangleF(x + markerSize + 4, y, nameWidth - markerSize - 4, lineHeight));

        foreach (var range in group.Seats)
        {
          x = left + (line / linesPerColumn) * columnWidth;
          y = top + (line % linesPerColumn) * lineHeight;
          ctx.DrawText($"Rij {range.Row} Stoel {range.StartSeat} t/m {range.EndSeat}", new RectangleF(x + nameWidth, y, columnWidth - nameWidth, lineHeight));
          line++;
        }
        if (group.Seats.Count == 0)
        {
          line++;
        }
      }
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

  public class Lampegiet : Zaal
  {
    public Lampegiet()
    {
      // Default constructor for serialization
    }

    public static Lampegiet Load()
    {
      return LoadFromFile<Lampegiet>(Path.Combine(AppContext.BaseDirectory, "Zalen", "Lampegiet.json"));
    }
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
