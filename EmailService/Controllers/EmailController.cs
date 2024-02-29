using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using Users.Dtos;

namespace EmailService.Controllers
{
    [Route("Email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly string? sendGridKey;
        private readonly string? senderEmail;
        private readonly string? senderName;

        public EmailController()
        {
            sendGridKey = Environment.GetEnvironmentVariable("SendGridKey");
            senderEmail = Environment.GetEnvironmentVariable("Sender");
            senderName = Environment.GetEnvironmentVariable("SenderName");
        }

        [Route("send")]
        [HttpPost]
        public async Task<ActionResult> RegisterAsync([FromBody] MailObject mail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(sendGridKey == null || sendGridKey == string.Empty || senderEmail == null || senderEmail == string.Empty || senderName == null || senderName == string.Empty)
            {
                return StatusCode(503);
            }

            var client = new SendGridClient(sendGridKey);

            var from = new EmailAddress(senderEmail, senderName);
            var to = new EmailAddress(mail.To, "User");

            var subject = mail.Subject;
            var plainTextContent = mail.Message;
            var htmlContent = "<strong>"+ plainTextContent + "</strong>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);

            response.IsSuccessStatusCode.ToString();

            if(!response.IsSuccessStatusCode)
            {
                return StatusCode(503);
            }

            return Ok(200);
        }
    }
}
