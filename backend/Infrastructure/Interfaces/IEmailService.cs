using System;

namespace WebApi.EmailService;

public interface IEmailService
{
    System.Threading.Tasks.Task SendAsync(string to, string subject, string body);
}
