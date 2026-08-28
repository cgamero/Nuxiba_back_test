using TestBackNuxiba.DTOs;

namespace TestBackNuxiba.Services;

public interface IReportService
{
    Task<byte[]> GenerateLoginReportAsync();
}