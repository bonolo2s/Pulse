using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Pulse.Shared.Interfaces;

namespace Pulse.Infrastructure.Messaging;

public class SesEmailSender : IEmailSender
{
    private readonly IAmazonSimpleEmailService _ses;
    private readonly string _fromAddress;

    public SesEmailSender(IAmazonSimpleEmailService ses, string fromAddress)
    {
        _ses = ses;
        _fromAddress = fromAddress;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var request = new SendEmailRequest
        {
            Source = _fromAddress,
            Destination = new Destination { ToAddresses = [to] },
            Message = new Message
            {
                Subject = new Content(subject), // i need me a costume Template
                Body = new Body
                {
                    Html = new Content(body)
                }
            }
        };

        await _ses.SendEmailAsync(request);
    }
}