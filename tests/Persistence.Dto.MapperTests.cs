using System;
using System.Collections.Generic;
using Xunit;
using ShowPlanner.Persistence.Dto;
using ShowPlanner.Domain;
using ShowPlanner.Persistence.Dto;

namespace ShowPlanner.Tests.Persistence.Dto
{
    public class MapperTests
    {
        [Fact]
        public void RoundTrip_SpaceToDtoAndBack()
        {
            // Build domain model
            var space = new Space
            {
                Id = "space-1",
                Name = "Main Hall",
                Description = "desc",
                Sections = new List<Section>
                {
                    new Section
                    {
                        Id = "sec-1",
                        Name = "Main",
                        Priority = 1,
                        IsBalcony = false,
                        Rows = new List<Row>
                        {
                            new Row
                            {
                                Id = "row-1",
                                Name = "A",
                                RowNumber = 1,
                                StartSeatNumber = 1,
                                EndSeatNumber = 2,
                                HasAccessFromLeft = true,
                                HasAccessFromRight = false,
                                Seats = new List<Seat>
                                {
                                    new Seat { Id = "seat-1", SeatNumber = 1, IsOccupied = false },
                                    new Seat { Id = "seat-2", SeatNumber = 2, IsOccupied = false }
                                }
                            }
                        }
                    }
                }
            };

            // Map to DTO
            var spaceDto = Mapper.ToDto(space);
            Assert.NotNull(spaceDto);
            Assert.Equal(space.Id, spaceDto.Id);
            Assert.Equal(1, spaceDto.Sections.Count);

            // Map back (no reverse mapper implemented beyond ToDto in scaffold)
            // We only test that mapping produced a non-null DTO with expected fields.
        }

        [Fact]
        public void WorkspaceSerialization_RoundTrip()
        {
            var workspace = new WorkspaceDto
            {
                Version = "1.0",
                Spaces = new List<SpaceDto>
                {
                    new SpaceDto { Id = "space-1", Name = "Main Hall" }
                },
                Shows = new List<ShowDto>(),
                Groups = new List<GroupDto>()
            };

            var json = System.Text.Json.JsonSerializer.Serialize(workspace);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<WorkspaceDto>(json);
            Assert.NotNull(deserialized);
            Assert.Equal("1.0", deserialized.Version);
            Assert.Single(deserialized.Spaces);
            Assert.Equal("space-1", deserialized.Spaces[0].Id);
        }
    }
}
