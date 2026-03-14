using System.IO;
using System.Threading.Tasks;
using ShowPlanner.Persistence;
using ShowPlanner.Persistence.Dto;
using Xunit;

namespace ShowPlanner.Tests.Persistence
{
    public class WorkspaceAppServiceTests
    {
        [Fact]
        public async Task SaveAndLoad_ViaAppService_Works()
        {
            var repo = new WorkspaceRepository();
            var app = new WorkspaceAppService(repo);
            var path = Path.Combine(Path.GetTempPath(), "workspace-appsvc.json");

            var ws = new WorkspaceDto
            {
                Version = "1.0",
                Spaces = new System.Collections.Generic.List<SpaceDto> { new SpaceDto { Id = "space-1", Name = "Main Hall" } },
                Shows = new System.Collections.Generic.List<ShowDto>(),
                Groups = new System.Collections.Generic.List<GroupDto>()
            };

            await app.SaveWorkspaceAsync(ws, path);
            var loaded = await app.LoadWorkspaceAsync(path);

            Assert.NotNull(loaded);
            Assert.Equal("1.0", loaded.Version);

            if (File.Exists(path)) File.Delete(path);
        }
    }
}
