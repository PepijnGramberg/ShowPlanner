using System;
using System.Collections.Generic;

namespace ShowPlanner.Domain
{
    // Minimal domain model stubs for Milestone 1 scaffolding
    public class Space
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Section> Sections { get; set; } = new List<Section>();
    }

    public class Section
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public bool IsBalcony { get; set; }
        public List<Row> Rows { get; set; } = new List<Row>();
    }

    public class Row
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int RowNumber { get; set; }
        public int StartSeatNumber { get; set; }
        public int EndSeatNumber { get; set; }
        public bool HasAccessFromLeft { get; set; }
        public bool HasAccessFromRight { get; set; }
        public List<Seat> Seats { get; set; } = new List<Seat>();
    }

    public class Seat
    {
        public string Id { get; set; }
        public int SeatNumber { get; set; }
        public bool IsOccupied { get; set; }
        public string OccupantGroupId { get; set; }
    }

    public class Show
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ShowInstance> Instances { get; set; } = new List<ShowInstance>();
    }

    public class ShowInstance
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string AssignedSpaceId { get; set; }
        public List<Allocation> Allocations { get; set; } = new List<Allocation>();
    }

    public class Allocation
    {
        public string GroupId { get; set; }
        public List<string> SeatIds { get; set; } = new List<string>();
    }

    public class Group
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public Preferences Preferences { get; set; }
    }

    public class Preferences
    {
        public bool PrefersFrontRow { get; set; }
        public List<ShowPreference> PreferredShowDaysTimes { get; set; } = new List<ShowPreference>();
    }

    public class ShowPreference
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }
}
