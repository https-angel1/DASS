using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;


namespace DASS.Services
{
	public class EmailService
	{
		private readonly IConfiguration _configuration;

		public EmailService (IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailAsync (string toEmail, string subject, string message)
		{
			var apiKey = _configuration["SendGrid:ApiKey"];
			var client = new SendGridClient(apiKey);
			var from = new EmailAddress("angelolivarez810@gmail.com", "Dental Appointment Scheduling System");
			var to = new EmailAddress(toEmail);
			var emailMessage = MailHelper.CreateSingleEmail(from, to, subject, message, message);
			var response = await client.SendEmailAsync(emailMessage);
			var responseBody = await response.Body.ReadAsStringAsync();

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception($"Failed to send email: {response.StatusCode}. Response: {responseBody}");
			}
		}
	}
}
