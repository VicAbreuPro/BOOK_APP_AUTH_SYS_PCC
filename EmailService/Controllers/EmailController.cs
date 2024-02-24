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
        private readonly IConfiguration _configuration;

        public EmailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("send")]
        [HttpPost]
        public async Task<ActionResult> RegisterAsync([FromBody] MailObject mail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var apiKey = "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";

            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("a18841@alunos.ipca.pt", "BookApps APP");
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
