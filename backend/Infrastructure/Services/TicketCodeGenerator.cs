using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Domain.Models;

public class TicketCodeGenerator : ITicketCodeGenerator
{
    private readonly ApplicationDbContext _db;

    public TicketCodeGenerator(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<string> GenerateTicketCodeAsync(Guid teamId, TicketType ticketType)
    {
        var prefix = ticketType switch
        {
            TicketType.Infra => "INFRA",
            TicketType.Bug => "BUG",
            TicketType.Docs => "DOCS",
            TicketType.QA => "QA",
            TicketType.Feature => "FEATURE",
            TicketType.Task => "TASK",
            _ => "TASK"
        };

        var codes = await _db.Tasks.AsNoTracking()
            .Where(x => x.TeamId == teamId && x.TicketCode.StartsWith(prefix + "-"))
            .Select(x => x.TicketCode)
            .ToListAsync();

        var max = 0;
        var regex = new Regex(@"-(\d+)$");
        foreach (var code in codes)
        {
            var match = regex.Match(code ?? string.Empty);
            if (match.Success && int.TryParse(match.Groups[1].Value, out var num))
            {
                if (num > max) max = num;
            }
        }

        return $"{prefix}-{max + 1}";
    }
}
