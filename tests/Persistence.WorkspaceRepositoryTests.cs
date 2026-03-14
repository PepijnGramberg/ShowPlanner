using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShowPlanner.Persistence;
using ShowPlanner.Persistence.Dto;
using Xunit;

namespace ShowPlanner.Tests.Persistence
{
    public class WorkspaceRepositoryTests
    {
        [Fact]
        public async Task SaveLoad_RoundTrip_Works()
        {
            var repo = new WorkspaceRepository();
            var path = Path.Combine(Path.GetTempPath(), "workspace-test.json");

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

            // Act: Save
            await repo.SaveWorkspaceAsync(workspace, path);
            // Act: Load
            var loaded = await repo.LoadWorkspaceAsync(path);

            // Assert
            Assert.NotNull(loaded);
            Assert.Equal(workspace.Version, loaded.Version);
            Assert.Single(loaded.Spaces);
            Assert.Equal("space-1", loaded.Spaces[0].Id);

            // Cleanup
            if (File.Exists(path)) File.Delete(path);
        }

        [Fact]
        public async Task Migration_OnLoad_FromOldVersion_Upgrades()
        {
            var repo = new WorkspaceRepository();
            var path = Path.Combine(Path.GetTempPath(), "workspace-migrate.json");

            // Create an old-version workspace JSON
            var old = new ShowPlanner.Persistence.Dto.WorkspaceDto
            {
                Version = "1.0",
                Spaces = new List<ShowPlanner.Persistence.Dto.SpaceDto>(),
                Shows = new List<ShowPlanner.Persistence.Dto.ShowDto>(),
                Groups = new List<ShowPlanner.Persistence.Dto.GroupDto>()
            };
            await repo.SaveWorkspaceAsync(old, path);

            // Act: load triggers migration
            var loaded = await repo.LoadWorkspaceAsync(path);

            // Assert: migrated
            Assert.NotNull(loaded);
            Assert.Equal(MigrationEngine.CurrentVersion, loaded.Version);

            // Cleanup
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
