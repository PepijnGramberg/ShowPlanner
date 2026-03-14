using System;
using System.Collections.Generic;

namespace ShowPlanner.Persistence.Dto
{
    // Data Transfer Objects mirroring Section 5 persistence plan
    public class SpaceDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<SectionDto> Sections { get; set; } = new List<SectionDto>();
    }

    public class SectionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public bool IsBalcony { get; set; }
        public List<RowDto> Rows { get; set; } = new List<RowDto>();
    }

    public class RowDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int RowNumber { get; set; }
        public int StartSeatNumber { get; set; }
        public int EndSeatNumber { get; set; }
        public bool HasAccessFromLeft { get; set; }
        public bool HasAccessFromRight { get; set; }
        public List<SeatDto> Seats { get; set; } = new List<SeatDto>();
    }

    public class SeatDto
    {
        public string Id { get; set; }
        public int SeatNumber { get; set; }
        public bool IsOccupied { get; set; }
        public string OccupantGroupId { get; set; }
    }

    public class ShowDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ShowInstanceDto> Instances { get; set; } = new List<ShowInstanceDto>();
    }

    public class ShowInstanceDto
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string TimeIso { get; set; } // alternative to TimeSpan for simplicity
        public string AssignedSpaceId { get; set; }
        public List<AllocationDto> Allocations { get; set; } = new List<AllocationDto>();
    }

    public class AllocationDto
    {
        public string GroupId { get; set; }
        public List<string> SeatIds { get; set; } = new List<string>();
    }

    public class GroupDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public PreferencesDto Preferences { get; set; }
    }

    public class PreferencesDto
    {
        public bool PrefersFrontRow { get; set; }
        public List<ShowPreferenceDto> PreferredShowDaysTimes { get; set; } = new List<ShowPreferenceDto>();
    }

    public class ShowPreferenceDto
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }

    public class AllocationPlanDto
    {
        public string ShowInstanceId { get; set; }
        public List<SeatAssignmentDto> Assignments { get; set; } = new List<SeatAssignmentDto>();
    }

    public class SeatAssignmentDto
    {
        public string GroupId { get; set; }
        public List<string> SeatIds { get; set; } = new List<string>();
    }

    public class WorkspaceDto
    {
        public string Version { get; set; }
        public List<SpaceDto> Spaces { get; set; } = new List<SpaceDto>();
        public List<ShowDto> Shows { get; set; } = new List<ShowDto>();
        public List<GroupDto> Groups { get; set; } = new List<GroupDto>();
        public dynamic Metadata { get; set; }
    }
}
