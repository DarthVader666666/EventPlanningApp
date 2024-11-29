using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;

namespace EventPlanning.Bll.Services
{
    public class EmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<EmailSendOperation?> SendEmailAsync(string email, string subject, string message)
        {
            var sender = _configuration["AzureEmailSender"];
            var connectionString = _configuration["ConnectionStrings:AzureCommunicationService"];

            var client = new EmailClient(connectionString);

            EmailSendOperation? result = null;

            try
            {
                var task = client.SendAsync(Azure.WaitUntil.Completed, sender, email, subject, message);
                result = await task;
            }
            catch (Exception ex) 
            {
                Console.Write(ex.Message);
            }

            return result;
        }
    }
}
