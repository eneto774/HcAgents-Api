using System.Net;
using System.Net.Mail;
using HcAgents.Domain.Abstractions;
using Microsoft.Extensions.Configuration;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async void SendEmail(string to, string subject, string body)
    {
        try
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(
                    _configuration["SmtpSettings:FromEmail"],
                    _configuration["SmtpSettings:FromName"]
                );
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.Normal;

                using (
                    SmtpClient smtpClient = new SmtpClient(
                        _configuration["SmtpSettings:SmtpHost"],
                        Int32.Parse(_configuration["SmtpSettings:SmtpPort"])
                    )
                )
                {
                    smtpClient.Credentials = new NetworkCredential(
                        _configuration["SmtpSettings:SmtpUsername"],
                        _configuration["SmtpSettings:SmtpPassword"]
                    );
                    smtpClient.EnableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"]);
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;

                    await smtpClient.SendMailAsync(mail);

                    Console.WriteLine($"Email enviado com sucesso para: {to}");
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
}
