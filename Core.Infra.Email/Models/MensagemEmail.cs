using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Infra.Email.Models
{
    public class MensagemEmail
    {
        public MensagemEmail()
        {
            EnderecoDestino = new List<EnderecoEmail>();
            EnderecoRemetente = new List<EnderecoEmail>();
        }

        public List<EnderecoEmail> EnderecoDestino { get; set; }
        public List<EnderecoEmail> EnderecoRemetente { get; set; }
        public string Assunto { get; set; }
        public string Conteudo { get; set; }
    }
}
