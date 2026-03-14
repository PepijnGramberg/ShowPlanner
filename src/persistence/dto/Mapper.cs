using System;
using System.Collections.Generic;
using ShowPlanner.Persistence.Dto;
using ShowPlanner.Domain;

namespace ShowPlanner.Persistence.Dto
{
    public static class Mapper
    {
        // Domain -> DTO mappings (scaffolded using minimal domain models)
        public static SpaceDto ToDto(Space s)
        {
            if (s == null) return null;
            var dto = new SpaceDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description
            };
            foreach (var sec in s.Sections ?? new List<Section>())
            {
                dto.Sections.Add(ToDto(sec));
            }
            return dto;
        }

        public static SectionDto ToDto(Section sec)
        {
            if (sec == null) return null;
            var dto = new SectionDto
            {
                Id = sec.Id,
                Name = sec.Name,
                Priority = sec.Priority,
                IsBalcony = sec.IsBalcony
            };
            foreach (var r in sec.Rows ?? new List<Row>())
            {
                dto.Rows.Add(ToDto(r));
            }
            return dto;
        }

        public static RowDto ToDto(Row row)
        {
            if (row == null) return null;
            var dto = new RowDto
            {
                Id = row.Id,
                Name = row.Name,
                RowNumber = row.RowNumber,
                StartSeatNumber = row.StartSeatNumber,
                EndSeatNumber = row.EndSeatNumber,
                HasAccessFromLeft = row.HasAccessFromLeft,
                HasAccessFromRight = row.HasAccessFromRight
            };
            foreach (var s in row.Seats ?? new List<Seat>())
            {
                dto.Seats.Add(ToDto(s));
            }
            return dto;
        }

        public static SeatDto ToDto(Seat seat)
        {
            if (seat == null) return null;
            return new SeatDto
            {
                Id = seat.Id,
                SeatNumber = seat.SeatNumber,
                IsOccupied = seat.IsOccupied,
                OccupantGroupId = seat.OccupantGroupId
            };
        }

        public static ShowDto ToDto(Show show)
        {
            if (show == null) return null;
            var dto = new ShowDto
            {
                Id = show.Id,
                Name = show.Name
            };
            foreach (var inst in show.Instances ?? new List<ShowInstance>())
            {
                dto.Instances.Add(ToDto(inst));
            }
            return dto;
        }

        public static ShowInstanceDto ToDto(ShowInstance inst)
        {
            if (inst == null) return null;
            return new ShowInstanceDto
            {
                Id = inst.Id,
                Date = inst.Date,
                TimeIso = inst.Time.ToString(),
                AssignedSpaceId = inst.AssignedSpaceId,
                Allocations = new List<AllocationDto>(ToDto(inst.Allocations))
            };
        }

        public static List<AllocationDto> ToDto(List<Allocation> allocations)
        {
            var list = new List<AllocationDto>();
            if (allocations == null) return list;
            foreach (var a in allocations)
            {
                list.Add(new AllocationDto { GroupId = a.GroupId, SeatIds = new List<string>(a.SeatIds) });
            }
            return list;
        }

        public static GroupDto ToDto(Group g)
        {
            if (g == null) return null;
            return new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Size = g.Size,
                Preferences = g.Preferences == null ? null : new PreferencesDto
                {
                    PrefersFrontRow = g.Preferences.PrefersFrontRow,
                    PreferredShowDaysTimes = new List<ShowPreferenceDto>()
                    {
                        // naive mapping for placeholder
                        new ShowPreferenceDto { Date = DateTime.Today, Time = TimeSpan.Zero }
                    }
                }
            };
        }
    }
}
