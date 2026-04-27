using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShowPlanner.Persistence;
using ShowPlanner.Persistence.Dto;

namespace ShowPlanner.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var repo = new WorkspaceRepository();
            var path = "workspace-demo.json";

            var workspace = new WorkspaceDto
            {
                Version = "1.0",
                Spaces = new List<SpaceDto>
                {
                    new SpaceDto
                    {
                        Id = "space-1",
                        Name = "Main Hall",
                        Description = "Demo space",
                        Sections = new List<SectionDto>
                        {
                            new SectionDto
                            {
                                Id = "sec-1",
                                Name = "Main",
                                Priority = 1,
                                IsBalcony = false,
                                Rows = new List<RowDto>
                                {
                                    new RowDto
                                    {
                                        Id = "row-1",
                                        Name = "A",
                                        RowNumber = 1,
                                        StartSeatNumber = 1,
                                        EndSeatNumber = 3,
                                        HasAccessFromLeft = true,
                                        HasAccessFromRight = false,
                                        Seats = new List<SeatDto>
                                        {
                                            new SeatDto { Id = "seat-1", SeatNumber = 1, IsOccupied = false },
                                            new SeatDto { Id = "seat-2", SeatNumber = 2, IsOccupied = false },
                                            new SeatDto { Id = "seat-3", SeatNumber = 3, IsOccupied = false }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Shows = new List<ShowDto>
                {
                    new ShowDto
                    {
                        Id = "show-1",
                        Name = "Hamlet",
                        Instances = new List<ShowInstanceDto>
                        {
                            new ShowInstanceDto
                            {
                                Id = "inst-1",
                                Date = DateTime.Today,
                                TimeIso = "18:00",
                                AssignedSpaceId = "space-1",
                                Allocations = new List<AllocationDto>
                                {
                                    new AllocationDto { GroupId = "grp-1", SeatIds = new List<string> { "seat-1", "seat-2" } }
                                }
                            }
                        }
                    }
                },
                Groups = new List<GroupDto>()
            };

            Console.WriteLine("Saving workspace to " + path);
            await repo.SaveWorkspaceAsync(workspace, path);
            Console.WriteLine("Saved. Loading...");
            var loaded = await repo.LoadWorkspaceAsync(path);
            Console.WriteLine($"Loaded workspace with {loaded?.Spaces?.Count ?? 0} spaces.");
        }
    }
}
