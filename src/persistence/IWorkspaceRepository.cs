using System.Threading.Tasks;
using ShowPlanner.Persistence.Dto;

namespace ShowPlanner.Persistence
{
    // Persistence contract for workspace graph
    public interface IWorkspaceRepository
    {
        Task<string> SaveWorkspaceAsync(WorkspaceDto workspace, string path);
        Task<WorkspaceDto> LoadWorkspaceAsync(string path);
        Task<string> GetWorkspaceVersionAsync(string path);
    }
}
