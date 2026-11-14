using System;
using System.Collections.Generic;
using System.Linq;

namespace Theaterverdeling.Tests
{
  // Small halls and groups for tests, built through the same JSON format as the hall files.
  internal static class TestHalls
  {
    public const string Walkway = "{\"Type\":\"WalkWay\"}";

    public static Zaal Hall(params string[] rows)
    {
      return Zaal.FromJson<Lampegiet>("{\"ZaalType\":\"Test\",\"Rows\":[" + string.Join(",", rows) + "]}");
    }

    public static string Row(int number, int seats, bool inUse = true, params int[] unusable)
    {
      return $"{{\"Type\":\"ChairRow\",\"RowNr\":{number},\"InUse\":{(inUse ? "true" : "false")},\"SeatCount\":{seats},\"Unusable\":[{string.Join(",", unusable)}]}}";
    }

    public static Group Group(string name, int people)
    {
      return new Group(name, people, false, false, new List<DateTime>());
    }

    public static void Seat(Zaal hall, params Group[] groups)
    {
      hall.Groups = groups.ToList();
      hall.AssignSeats();
    }

    public static string Describe(Group group)
    {
      return string.Join(", ", group.Seats.Select(range => $"rij {range.Row}: {range.StartSeat}-{range.EndSeat}"));
    }
  }
}
