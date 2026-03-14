using System.Threading.Tasks;

namespace ShowPlanner.Persistence.Reporting
{
    public interface IReportService
    {
        Task<byte[]> GenerateSeatingPdfAsync(string showInstanceId);
    }
}
