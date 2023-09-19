using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.Json;

namespace Core.Services.MailService;

public class EmailSendService
{
    private readonly EmailSettings _emailSettingsOptions;
    private readonly ILogger<EmailSendService> _logger;
    public EmailSendService(IOptions<EmailSettings> emailSettingsOptions, ILogger<EmailSendService> logger)
    {
        _emailSettingsOptions = emailSettingsOptions.Value;
        _logger = logger;
    }

    public async Task SendAsync(AlternateView htmlView, EmailModel mailModel)
    {
        Stopwatch spw = Stopwatch.StartNew();
        try
        {
            await Send(htmlView, mailModel);
        }
        catch (Exception ex)
        {
            spw.Stop();

            var enrichers = new List<ILogEventEnricher>
            {
                new PropertyEnricher("StatusCode", 503),
                new PropertyEnricher("EmailIsSender", false),
                new PropertyEnricher("DisplayName", mailModel.DisplayName),
                new PropertyEnricher("Subject", mailModel.Subject),
                new PropertyEnricher("ReceiveInfo", JsonSerializer.Serialize(mailModel.MailInfo)),
                new PropertyEnricher("RequestInterval", spw.ElapsedMilliseconds.ToString())
            };
            using (LogContext.Push(enrichers.ToArray()))
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        finally
        {
            spw.Stop();
        }
    }
    private async Task Send(AlternateView htmlView, EmailModel mailModel)
    {
        if (!string.IsNullOrEmpty(mailModel?.Subject))
            _emailSettingsOptions.Subject = mailModel.Subject;
        if (!string.IsNullOrEmpty(mailModel?.DisplayName))
            _emailSettingsOptions.DisplayName = mailModel.DisplayName;

        // ====================== Email Objesini Oluştur ======================
        MailMessage mailMessage = new();
        mailMessage = await AddAttachments(mailMessage, mailModel);
        mailMessage.To.Add(new MailAddress(mailModel.ReceiveUser, _emailSettingsOptions.DisplayName));
        mailMessage.From = new MailAddress(_emailSettingsOptions.Email, _emailSettingsOptions.DisplayName);
        mailMessage.Subject = _emailSettingsOptions.Subject;
        mailMessage.IsBodyHtml = true;
        mailMessage.AlternateViews.Add(htmlView);

        //mailMessage.Body = body_txt;


        // ====================== Email Sender yani email gönderici Objesini Oluştur ======================
        using SmtpClient client = new(_emailSettingsOptions.Host, _emailSettingsOptions.Port);
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(_emailSettingsOptions.Email, _emailSettingsOptions.Password);
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.EnableSsl = true;

        await client.SendMailAsync(mailMessage);
        mailMessage.Dispose();
    }
    private static async ValueTask<MailMessage> AddAttachments(MailMessage mailMessage, EmailModel mailModel)
    {
        // ====================== Mail ile Dosya göndermek - Localdeki dosyalar için ======================
        if (mailModel.AttachmentsAsPaths != null)
        {
            foreach (var path in mailModel.AttachmentsAsPaths)
            {
                if (!File.Exists(path))
                    continue;
                mailMessage.Attachments.Add(new Attachment(path));
            }

        }
        // ====================== Mail ile Dosya göndermek - Arayüzden yüklenen dosyalar için ======================
        if (mailModel.Attachments != null)
        {
            foreach (var attachment in mailModel.Attachments)
            {
                MemoryStream ms = new();
                await attachment.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);

                mailMessage.Attachments.Add(new Attachment(ms, attachment.FileName, MediaTypeNames.Application.Octet));
            }
        }

        return mailMessage;
    }
}
