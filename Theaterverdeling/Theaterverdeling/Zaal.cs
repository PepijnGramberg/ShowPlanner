using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using PdfSharpCore.Drawing;

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
    public ChairRow(int seatCount, bool inUse, int rowNr = 0)
    {
      InUse = inUse;
      RowNr = rowNr;
      Seats = new Seat[seatCount];
      for (var i = 0; i < Seats.Length; i++)
      {
        Seats[i] = new Seat();
      }
    }
    public bool InUse { get; }
    public int RowNr { get; }
    public override int TotalSeatCount => Seats.Length;

    public override int AvailableSeatCount => InUse ? TotalSeatCount : 0;

    public Seat[] Seats { get; }
  }

  public sealed class Seat { }

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
        
        // Create new ChairRow with constructor
        return new ChairRow(seatCount, inUse, rowNr);
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
    protected List<int>[] Places;

    // Change to protected with JsonInclude attribute to allow serialization
    [JsonInclude]
    protected List<Row> _rows = new List<Row>();
    public IReadOnlyList<Row> Rows => _rows;

    public virtual void AssignSeats()
    {
      CreatePlaces();
      FillPlaces();
    }

    public virtual void Draw(XGraphics gr) 
    {
      var font = new XFont("Verdana", 12);
      var padding = 20;
      var maxWidthChairs = Rows.Max(r => r.TotalSeatCount) + 2; // add two for row number to be displayed. and keeep it centered
      var maxHeightChairs = Rows.Count;

      var mapSize = new XSize(gr.PageSize.Width - (padding * 2), 500);
      var mapContainer = gr.BeginContainer(new XRect(new XPoint(padding, padding), mapSize), new XRect(mapSize), gr.PageUnit);
      var seatSize = Math.Min(mapSize.Width / maxWidthChairs, mapSize.Height / maxHeightChairs);

      for (var i = 0; i < Rows.Count; i++)
      {
        var rowHeight = i * seatSize;
        if (Rows[i] is ChairRow chairRow)
        {
          // Start of the row needs to be offset from the Graphics box so the entire row is centered.
          var startRow = (mapSize.Width - (chairRow.TotalSeatCount * seatSize))/2;

          var chairs = new List<XRect>();
          for (var j = 0; j < chairRow.TotalSeatCount; j++) {
            // Add 1 to J for so there is space for the row number in front
            chairs.Add(new XRect(startRow + (j+1)*seatSize,rowHeight, seatSize, seatSize));
          }

          // Draw row number on the left
          DrawInt(chairRow.RowNr, new XRect(startRow, rowHeight, seatSize, seatSize));
          // Draw row number on the right
          DrawInt(chairRow.RowNr, new XRect(startRow + (chairRow.TotalSeatCount + 1) * seatSize, rowHeight, seatSize, seatSize));

          // Draw the chairs.
          for(var j = 0; j < chairs.Count; j++)
          {
            // Color the chairs according to which group is assigned.
            // TODO
            // Draw the numbers. Seats in the real world are not 0 indexed so we increment the number for display purposes.
            DrawInt(j + 1, chairs[j]);
          }
          // Black outline.
          gr.DrawRectangles(XPens.Black, chairs.ToArray());
        }
      }

      gr.EndContainer(mapContainer);

      void DrawInt(int number, XRect position)
      {
        DrawString(number.ToString(), position);
      }
      void DrawString(string text, XRect position)
      {
        gr.DrawString(text, font, XBrushes.Black, position, XStringFormats.Center);
      }
    }
    protected virtual void CreatePlaces() { }

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

    protected void FillPlaces()
    {
      var leftrow =0;
      var rightrow = 0;
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
          for (; leftrow < _rows.Count; leftrow++)
          {
            if(!(_rows[leftrow] is ChairRow chairs))
            {
              continue;
            }
            for (int i = 0; i < chairs.TotalSeatCount / 2; i++)
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
          for (; rightrow < _rows.Count; rightrow++)
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

    public override void Draw(XGraphics gr)
    {
      base.Draw(gr);
      return;
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
