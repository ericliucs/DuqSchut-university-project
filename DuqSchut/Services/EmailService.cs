using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Communication.Email;


namespace DuqSchut.Services
{
    //from the interface
    public class EmailService: IEmailDependency
    {
        private readonly EmailClient _emailClient;
        private readonly ILogger<EmailService> _logger;

        //connection string
        private readonly string connectionString = "endpoint=https://duqschutcommunicationservice.unitedstates.communication.azure.com/;accesskey=8Vjeqsl3zGClgRRjdin5gRqa3l3sx5oIM4u3UtLkdKAKstglggw6JQQJ99BCACULyCpUSk9FAAAAAZCSDgky"; 

        public EmailService(ILogger<EmailService>logger)
        {
            _emailClient = new EmailClient(connectionString);
            _logger = logger;
        }

        //Function that sends the email. 
        public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                var emailMessage = new EmailMessage(
                    senderAddress: "DoNotReply@9fff819e-dded-4dc4-a15a-74bf27e9d58e.azurecomm.net",
                    content: new EmailContent(subject)
                    //simple HTML with body 
                    {
                        PlainText = body,
                        Html = $@"
                        <html>
                        <body>
                        <p>{body}</p>
                        </body>
                        </html>"
                    },
                    recipients: new EmailRecipients(new List<EmailAddress>
                    {
                        new EmailAddress(recipientEmail)
                    }));
                //sends email
                EmailSendOperation emailSendOperation = await _emailClient.SendAsync(Azure.WaitUntil.Completed, emailMessage);
                if(emailSendOperation.Value.Status == EmailSendStatus.Succeeded){
                    _logger.LogInformation($"Email sent successfully with MessageId: {emailSendOperation.Id}");
                    return true;
                }
                else{
                    _logger.LogError($"Email with MessageId: {emailSendOperation.Id} failed to send");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}