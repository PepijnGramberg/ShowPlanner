using System;
using System.Threading.Tasks;
namespace ShowPlanner.Persistence.Reporting
{
    public class PdfServiceStub : IReportService
    {
        public Task<byte[]> GenerateSeatingPdfAsync(string showInstanceId)
        {
            // Return empty PDF-like byte array as placeholder
            return Task.FromResult(new byte[0]);
        }
    }
}
