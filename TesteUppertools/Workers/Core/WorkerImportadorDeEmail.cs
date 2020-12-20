using Core.Infra.Email;
using System;
using System.Collections.Generic;
using System.Text;
using TesteUppertools.Workers.Interface;

namespace TesteUppertools.Workers.Core
{
    public class WorkerImportadorDeEmail : IWorkerCore
    {
        private ICollection<string> _mensagensRetorno = new List<string>();
        private IEmailService _emailService;

        public WorkerImportadorDeEmail(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public int InformarSegundosParaExecutar()
        {
            return 30;
        }

        public ICollection<string> IniciarTrabalhoPrincipal()
        {
            _mensagensRetorno.Clear();
            _mensagensRetorno.Add("Nenhum e-mail recebido");
            return _mensagensRetorno;
        }
    }
}
