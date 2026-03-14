using ShowPlanner.Persistence.Dto;
namespace ShowPlanner.Persistence
{
    public interface IWorkspaceMigration
    {
        // Attempts to migrate the workspace to the current schema.
        // Returns true if migration occurred and outputs the migrated workspace.
        bool TryMigrate(WorkspaceDto workspace, out WorkspaceDto migrated);
    }
}
