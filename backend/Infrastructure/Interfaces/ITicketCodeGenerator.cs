public interface ITicketCodeGenerator
{
    Task<string> GenerateTicketCodeAsync(Guid teamId, TicketType ticketType);
}
