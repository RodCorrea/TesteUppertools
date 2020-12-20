using Uppertools.AppLayer.MockService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TesteUppertools.Workers.Interface;
using Uppertools.AppLayer.Models;

namespace TesteUppertools.Workers.Core
{
    public class WorkerInterpretadorDeDados : IWorkerCore
    {
        private readonly string _diretorioDeTrabalhoOrigem = "Importacoes\\Processar";
        private readonly string _diretorioDeExportaçãoDestino = "Importacoes\\Dados";
        private ICollection<string> _mensagensRetorno = new List<string>();
        public int InformarSegundosParaExecutar()
        {
            return 10;
        }
        public ICollection<string> IniciarTrabalhoPrincipal()
        {
            _mensagensRetorno.Clear();
            string[] arquivos = Directory.GetFiles(_diretorioDeTrabalhoOrigem);
            Directory.CreateDirectory(_diretorioDeTrabalhoOrigem);
            Directory.CreateDirectory(_diretorioDeExportaçãoDestino);
            var numeroArquivoProcessados = 0;
            foreach (string _arq in arquivos)
            {
                numeroArquivoProcessados++;
                var dadosNomeArquivo = Path.GetFileName(_arq).Split("_");
                var _fornecedorId = Convert.ToInt32(dadosNomeArquivo[0]);
                var mensagemRetorno = ObterLayoutFornecedorExtraindoDados(_fornecedorId, _arq);
                _mensagensRetorno.Add(mensagemRetorno);
            }
            _mensagensRetorno.Add(numeroArquivoProcessados == 0 ? "Nenhum arquivo para intepretar!" : $"Interpretados {numeroArquivoProcessados} arquivos!");
            return _mensagensRetorno;
        }

        private string ObterLayoutFornecedorExtraindoDados(int fornecedorId, string arquivoOrigem)
        {
            try
            {
                var _extensaoArquivoOrigem = Path.GetExtension(arquivoOrigem).ToLower();
                var _layouts = MockService.ObterLayoutFornecedor(fornecedorId);
                var _linhaComDadosExtraidos = $"Fornecedor;{fornecedorId};";
                if (_layouts.Count() == 0)
                {
                    return ($"Nenhum layout configurado para o fornecedor {fornecedorId}");
                }
                else
                {
                    string _linhaDoTexto;
                    int _numeroDaLinha = 0;
                    StreamReader file = new StreamReader(arquivoOrigem);
                    while ((_linhaDoTexto = file.ReadLine()) != null)
                    {
                        _numeroDaLinha++;
                        if (_layouts.Any(lerLinha => lerLinha.LinhaValorTotal == _numeroDaLinha))
                            _linhaComDadosExtraidos += $"valorTotal;{_linhaDoTexto.Trim()};";
                        if (_layouts.Any(lerLinha => lerLinha.LinhaCNPJ == _numeroDaLinha))
                            _linhaComDadosExtraidos += $"CNPJ;{_linhaDoTexto.Replace(".", "").Replace("/", "").Replace("-", "").Trim()};";
                    }
                }
                return _linhaComDadosExtraidos;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


    }


}
