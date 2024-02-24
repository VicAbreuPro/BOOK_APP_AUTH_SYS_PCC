using System.Net.Mail;

namespace Users.Services
{
    public class EmailService
    {
        public int SendEmail(string frasebody, string tituloemail, string userEmail)
        {
            string emailTittle = tituloemail;
            string emaildestiny = userEmail;
            string emailsend = "ipcamei@outlook.pt";
            string subject = tituloemail;
            string emailpass = "testprojectsmei23";

            try
            {
                MailMessage message = new MailMessage(new MailAddress(emailsend, emailTittle), new MailAddress(emaildestiny));
                message.Subject = subject;


                message.Body = frasebody;
                message.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.office365.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;


                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                credentials.UserName = emailsend;
                credentials.Password = emailpass;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = credentials;

                smtp.Send(message);

                return 1;
            }catch(Exception)
            {
                return 0;
            }
        }
    }
}
