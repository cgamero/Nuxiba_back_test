using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TestBackNuxiba.Data;
using TestBackNuxiba.DTOs;

namespace TestBackNuxiba.Services;

public class ReportService : IReportService
{
    private readonly CCenterDbContext _context;

    public ReportService(CCenterDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> GenerateLoginReportAsync()
    {
        var users = await _context.Users
            .AsNoTracking()
            .Include(u => u.Area)
            .ToListAsync();

        var movements = await _context.Logins
            .AsNoTracking()
            .OrderBy(l => l.User_id)
            .ThenBy(l => l.fecha)
            .ThenBy(l => l.LogLoginId)
            .ToListAsync();

        var report = new List<LoginReportDto>();

        foreach (var user in users)
        {
            var userMovements = movements
                .Where(m => m.User_id == user.User_id)
                .OrderBy(m => m.fecha)
                .ThenBy(m => m.LogLoginId)
                .ToList();

            long totalSeconds = 0;

            for (int i = 0; i < userMovements.Count - 1; i++)
            {
                var current = userMovements[i];
                var next = userMovements[i + 1];

                if (current.TipoMov == 1 && next.TipoMov == 0)
                {
                    totalSeconds +=
                        (long)(next.fecha - current.fecha).TotalSeconds;
                }
            }

            var totalHours = Math.Round(
                totalSeconds / 3600m,
                2);

            var fullName = string.Join(
                " ",
                new[]
                {
                    user.Nombres,
                    user.ApellidoPaterno,
                    user.ApellidoMaterno
                }
                .Where(value => !string.IsNullOrWhiteSpace(value))
            );

            report.Add(new LoginReportDto
            {
                Login = user.Login,
                FullName = fullName,
                Area = user.Area?.AreaName ?? string.Empty,
                TotalHours = totalHours
            });
        }

        return GenerateCsv(report);
    }

    private static byte[] GenerateCsv(
        IEnumerable<LoginReportDto> records)
    {
        var builder = new StringBuilder();

        builder.AppendLine(
            "Login,Full Name,Area,Total Hours");

        foreach (var record in records)
        {
            builder.AppendLine(
                $"{EscapeCsv(record.Login)}," +
                $"{EscapeCsv(record.FullName)}," +
                $"{EscapeCsv(record.Area)}," +
                $"{record.TotalHours.ToString(CultureInfo.InvariantCulture)}");
        }

        return Encoding.UTF8.GetBytes(builder.ToString());
    }

    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        if (value.Contains(',') ||
            value.Contains('"') ||
            value.Contains('\n') ||
            value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}