using MailKit.Net.Smtp;
using MimeKit;
using System.Security.Policy;
using WatersAD.Data.Entities;

namespace WatersAD.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IUserHelper _userHelper;

        public MailHelper(IConfiguration configuration, IUserHelper userHelper)
        {
            _configuration = configuration;
           _userHelper = userHelper;
        }

        public async Task<Response> SendMail(string toName, string toEmail, string subject, string body, MemoryStream pdfStream = null, string fileName = "fatura.pdf")
        {
            try
            {
                string from = _configuration["Mail:From"];
                if (from?.Contains("${EMAIL_FROM}") == true)
                {
                    string keys = Environment.GetEnvironmentVariable("EMAIL_FROM");
                    from = from.Replace("${EMAIL_FROM}", keys);
                }

                string name = _configuration["Mail:Name"];
                string smtp = _configuration["Mail:Smtp"];

                string port = _configuration["Mail:Port"];
                if (port?.Contains("${EMAIL_PORT}") == true)
                {
                    string keys = Environment.GetEnvironmentVariable("EMAIL_PORT");
                    port = port.Replace("${EMAIL_PORT}", keys);
                }

                string password = _configuration["Mail:Password"];
                if (password?.Contains("${EMAIL_PASSWORD}") == true)
                {
                    string keys = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
                    password = password.Replace("${EMAIL_PASSWORD}", keys);
                }

                MimeMessage message = new();
                message.From.Add(new MailboxAddress(name, from));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;
                BodyBuilder bodyBuilder = new()
                {
                    HtmlBody = body
                };
                if (pdfStream != null && pdfStream.Length > 0)
                {
                    pdfStream.Position = 0; // Certifique-se de que a posição do stream está no início
                    bodyBuilder.Attachments.Add(fileName, pdfStream.ToArray(), ContentType.Parse("application/pdf"));
                }
                else
                {
                    // Se não houver anexo, você pode adicionar uma mensagem ao corpo do e-mail
                    bodyBuilder.HtmlBody += "<br/><strong>Obrigado.</strong> ";
                }
                message.Body = bodyBuilder.ToMessageBody();

                using (SmtpClient client = new())
                {
                   await client.ConnectAsync(smtp, int.Parse(port!), false);
                   await client.AuthenticateAsync(from, password);
                   await client.SendAsync(message);
                   await client.DisconnectAsync(true);
                }

                return new Response { IsSuccess = true };

            }
            catch (Exception ex)
            {

                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Result = ex
                };

            }
        }

       


        //public async Task<Response> SendMail(string toName, string toEmail, string subject, string body)
        //{
        //    try
        //    {
        //        string from = _configuration["Mail:From"];
        //        if (from?.Contains("${EMAIL_FROM}") == true)
        //        {
        //            string keys = Environment.GetEnvironmentVariable("EMAIL_FROM");
        //            from = from.Replace("${EMAIL_FROM}", keys);
        //        }

        //        string name = _configuration["Mail:Name"];
        //        string smtp = _configuration["Mail:Smtp"];

        //        string port = _configuration["Mail:Port"];
        //        if (port?.Contains("${EMAIL_PORT}") == true)
        //        {
        //            string keys = Environment.GetEnvironmentVariable("EMAIL_PORT");
        //            port = port.Replace("${EMAIL_PORT}", keys);
        //        }

        //        string password = _configuration["Mail:Password"];
        //        if (password?.Contains("${EMAIL_PASSWORD}") == true)
        //        {
        //            string keys = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
        //            password = password.Replace("${EMAIL_PASSWORD}", keys);
        //        }

        //        MimeMessage message = new();
        //        message.From.Add(new MailboxAddress(name, from));
        //        message.To.Add(new MailboxAddress(toName, toEmail));
        //        message.Subject = subject;
        //        BodyBuilder bodyBuilder = new()
        //        {
        //            HtmlBody = body
        //        };
        //        message.Body = bodyBuilder.ToMessageBody();

        //        using (SmtpClient client = new())
        //        {
        //            await client.ConnectAsync(smtp, int.Parse(port!), false);
        //            await client.AuthenticateAsync(from, password);
        //            await client.SendAsync(message);
        //            await client.DisconnectAsync(true);
        //        }

        //        return new Response { IsSuccess = true };

        //    }
        //    catch (Exception ex)
        //    {

        //        return new Response
        //        {
        //            IsSuccess = false,
        //            Message = ex.Message,
        //            Result = ex
        //        };

        //    }
        //}
    }
}
