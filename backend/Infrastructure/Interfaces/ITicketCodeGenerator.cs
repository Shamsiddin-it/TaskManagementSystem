public interface ITicketCodeGenerator
{
    Task<string> GenerateTicketCodeAsync(int teamId, TicketType ticketType);
}
