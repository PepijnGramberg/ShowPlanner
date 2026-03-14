using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ShowPlanner.Persistence.Dto;

namespace ShowPlanner.Persistence
{
    public class WorkspaceRepository : IWorkspaceRepository
    {
        private readonly JsonSerializerOptions _options;

        public WorkspaceRepository()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                AllowTrailingCommas = true,
            };
        }

        public async Task<string> SaveWorkspaceAsync(WorkspaceDto workspace, string path)
        {
            if (workspace == null) throw new ArgumentNullException(nameof(workspace));
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("path required", nameof(path));

            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            string json = JsonSerializer.Serialize(workspace, _options);
            await File.WriteAllTextAsync(path, json);
            return path;
        }

        public async Task<WorkspaceDto> LoadWorkspaceAsync(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException(path);
            string json = await File.ReadAllTextAsync(path);
            var ws = JsonSerializer.Deserialize<WorkspaceDto>(json, _options);
            // Apply migrations if needed
            if (MigrationEngine.TryMigrate(ws, out var migrated))
            {
                // Persist migrated workspace back to the same path
                await SaveWorkspaceAsync(migrated, path);
                return migrated;
            }
            return ws;
        }

        public async Task<string> GetWorkspaceVersionAsync(string path)
        {
            var ws = await LoadWorkspaceAsync(path);
            return ws?.Version;
        }
    }
}
