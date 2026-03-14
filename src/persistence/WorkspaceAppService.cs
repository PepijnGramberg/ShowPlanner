using System.Threading.Tasks;
using ShowPlanner.Persistence.Dto;

namespace ShowPlanner.Persistence
{
    public class WorkspaceAppService
    {
        private readonly IWorkspaceRepository _repository;

        public WorkspaceAppService(IWorkspaceRepository repository)
        {
            _repository = repository;
        }

        public Task<string> SaveWorkspaceAsync(WorkspaceDto workspace, string path)
        {
            return _repository.SaveWorkspaceAsync(workspace, path);
        }

        public Task<WorkspaceDto> LoadWorkspaceAsync(string path)
        {
            return _repository.LoadWorkspaceAsync(path);
        }
    }
}
