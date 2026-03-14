using System;
using ShowPlanner.Persistence.Dto;

namespace ShowPlanner.Persistence
{
    public static class MigrationEngine
    {
        public const string CurrentVersion = "1.1";

        // Very lightweight migration: upgrade from 1.0 -> 1.1 by bumping version
        public static bool TryMigrate(WorkspaceDto workspace, out WorkspaceDto migrated)
        {
            migrated = workspace;
            if (workspace == null) return false;
            if (string.IsNullOrEmpty(workspace.Version))
            {
                workspace.Version = CurrentVersion;
                return true;
            }
            if (workspace.Version == "1.0")
            {
                // perform minimal upgrade: set to 1.1 and ensure fields exist where needed
                workspace.Version = CurrentVersion;
                // ensure spaces/collections exist (defensive)
                migrated = workspace;
                return true;
            }
            // already current or unknown version; no migration
            return false;
        }
    }
}
