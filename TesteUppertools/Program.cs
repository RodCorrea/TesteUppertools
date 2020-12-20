using Core.Infra.Email;
using Core.Infra.Email.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using TesteUppertools.Workers.Core;

namespace TesteUppertools
{
    public class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();

        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseWindowsService().ConfigureServices((hostContext, services) =>
            {
                IConfiguration configuration = hostContext.Configuration;
                services.AddSingleton<IConfiguracaoEmail>(configuration.GetSection("ConfiguracaoEmail").Get<ConfiguracaoEmail>());
                services.AddTransient<IEmailService, EmailService>();

                services.AddSingleton<IHostedService>(serviceProvider => new InicializadorDeWorkers(
                    args,
                    serviceProvider.GetService<ILogger<WorkerImportadorDeEmail>>(),
                    serviceProvider.GetService<IHostApplicationLifetime>(),
                    new WorkerImportadorDeEmail(serviceProvider.GetService<IEmailService>())
                    ));

                services.AddSingleton<IHostedService>(serviceProvider => new InicializadorDeWorkers(
                    args,
                    serviceProvider.GetService<ILogger<WorkerFormatadorDeArquivos>>(),
                    serviceProvider.GetService<IHostApplicationLifetime>(),
                    new WorkerFormatadorDeArquivos()
                    ));

                services.AddSingleton<IHostedService>(serviceProvider => new InicializadorDeWorkers(
                    args,
                    serviceProvider.GetService<ILogger<WorkerInterpretadorDeDados>>(),
                    serviceProvider.GetService<IHostApplicationLifetime>(),
                    new WorkerInterpretadorDeDados()
                    ));

            });
    }
}