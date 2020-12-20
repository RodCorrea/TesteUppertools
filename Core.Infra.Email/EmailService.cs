using Core.Infra.Email.Models;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Core.Infra.Email
{
    public interface IEmailService
    {
        void Send(MensagemEmail mensagem);
        List<MensagemEmail> ReceberEmail(int maxCount = 10);
    }
    public class EmailService : IEmailService
    {
        private readonly IConfiguracaoEmail _configuracaoEmail;
        public EmailService(IConfiguracaoEmail emailConfiguration)
        {
            _configuracaoEmail = emailConfiguration;
        }
        public List<MensagemEmail> ReceberEmail(int maxCount = 10)
        {
            using (var emailClient = new Pop3Client())
            {
                emailClient.Connect(_configuracaoEmail.PopServer, _configuracaoEmail.PopPort, true);
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                emailClient.Authenticate(_configuracaoEmail.PopUsername, _configuracaoEmail.PopPassword);
                List<MensagemEmail> emails = new List<MensagemEmail>();
                for (int i = 0; i < emailClient.Count && i < maxCount; i++)
                {
                    var message = emailClient.GetMessage(i);
                    var mensagemEmail = new MensagemEmail
                    {
                        Conteudo = !string.IsNullOrEmpty(message.HtmlBody) ? message.HtmlBody : message.TextBody,
                        Assunto = message.Subject
                    };
                    mensagemEmail.EnderecoDestino.AddRange(message.To.Select(x => (MailboxAddress)x).Select(x => new EnderecoEmail { 
                        Endereco = x.Address, Nome = x.Name 
                    }));
                    mensagemEmail.EnderecoRemetente.AddRange(message.From.Select(x => (MailboxAddress)x).Select(x => new EnderecoEmail { 
                        Endereco = x.Address, Nome = x.Name 
                    }));
                    emails.Add(mensagemEmail);
                }
                return emails;
            }
        }
        public void Send(MensagemEmail mensagemEmail)
        {
            var message = new MimeMessage();
            message.To.AddRange(mensagemEmail.EnderecoDestino.Select(x => new MailboxAddress(x.Nome, x.Endereco)));
            message.From.AddRange(mensagemEmail.EnderecoRemetente.Select(x => new MailboxAddress(x.Nome, x.Endereco)));
            message.Subject = mensagemEmail.Assunto;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = mensagemEmail.Conteudo
            };
            using (var emailClient = new SmtpClient())
            {
                emailClient.Connect(_configuracaoEmail.SmtpServer, _configuracaoEmail.SmtpPort, true);
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                emailClient.Authenticate(_configuracaoEmail.SmtpUsername, _configuracaoEmail.SmtpPassword);
                emailClient.Send(message);
                emailClient.Disconnect(true);
            }
        }
    }
}
