using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TesteUppertools.Workers.Interface;

namespace TesteUppertools.Workers.Core
{
    public class InicializadorDeWorkers : BackgroundService
    {
        private readonly ILogger _logger;
        private IWorkerCore _workerCore;
        private readonly int _intervaloEmMilisegundosMinimoParaExecutarWorker = 1000;

        public InicializadorDeWorkers(string[] args, 
            ILogger logger, 
            IHostApplicationLifetime hostApplicationLifetime, 
            IWorkerCore workerCore)
        {
            _workerCore = workerCore;
            _logger = logger;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(PararWorker);
            while (!stoppingToken.IsCancellationRequested)
            {
                var beginWorkTime = DateTime.Now;
                _logger.LogInformation("{1}: executando.", DateTimeOffset.Now);
                try
                {
                    var mensagensRetorno = _workerCore.IniciarTrabalhoPrincipal();
                    foreach (var msg in mensagensRetorno)
                    {
                        _logger.LogInformation("{1}: {2}.", DateTimeOffset.Now, msg);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{1}: {2}.", DateTimeOffset.Now, ex.Message);
                }
                var segundosTotalExecutado = Convert.ToInt32((DateTime.Now - beginWorkTime).TotalSeconds);
                var delay = Task.Delay(calcularMilisegundosParaExecutarNovamente(segundosTotalExecutado), stoppingToken);
                await delay;
            }
            _logger.LogInformation("{1}: O serviço está parando.", DateTimeOffset.Now);
        }

        private int calcularMilisegundosParaExecutarNovamente(int executado)
        {
            var intervalo = _workerCore.InformarSegundosParaExecutar();
            var minimo = _intervaloEmMilisegundosMinimoParaExecutarWorker;
            return intervalo - executado < 0 ? minimo : (intervalo - executado) * 1000;
        }

        private void PararWorker()
        {
            _logger.LogInformation("{1}: Tarefa de segundo plano está parando.", DateTimeOffset.Now);
        }

    }

}

