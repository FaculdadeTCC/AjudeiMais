using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;

namespace AjudeiMais.API.Services
{
    public class EmailService
    {

        public async Task EnviarEmail(string Para, string assunto, string corpoHtml)
        {
            var mensagem = new MimeMessage();
            mensagem.From.Add(MailboxAddress.Parse("Eidaniloderio@gmial.com"));
            mensagem.To.Add(MailboxAddress.Parse(Para));
            mensagem.Subject = assunto;

            var corpo = new BodyBuilder { HtmlBody = corpoHtml };
            mensagem.Body = corpo.ToMessageBody();


            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("seuemail@gmail.com", "sua-senha-ou-app-password");
            await smtp.SendAsync(mensagem);
            await smtp.DisconnectAsync(true);   
            

                


        }
    }
}
