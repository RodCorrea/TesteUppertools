using Core.Tools.PDF;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TesteUppertools.Workers.Interface;

namespace TesteUppertools.Workers.Core
{
    public class WorkerFormatadorDeArquivos : IWorkerCore
    {
        private readonly string _diretorioDeTrabalhoOrigem = "Importacoes\\Arquivos";
        private readonly string _diretorioDeExportaçãoDestino = "Importacoes\\Processar";
        private ICollection<string> _mensagensRetorno = new List<string>();
        public int InformarSegundosParaExecutar()
        {
            return 15;
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
                var _extensaoArquivo = Path.GetExtension(_arq);
                if (_extensaoArquivo.ToLower() == ".pdf")
                {
                    numeroArquivoProcessados++;
                    ConverterPdfEmTexto(_arq, _diretorioDeTrabalhoOrigem, _diretorioDeExportaçãoDestino);
                }
            }
            _mensagensRetorno.Add(numeroArquivoProcessados == 0 ? "Nenhum arquivo para processar!" : $"Processados {numeroArquivoProcessados} arquivos!");
            return _mensagensRetorno;
        }

        private string ConverterPdfEmTexto(string nomeArquivoPdf, string diretorioOrigem, string diretorioDestino)
        {
            try
            {
                PdfSharp.Pdf.PdfDocument PDFDoc = PdfReader.Open(nomeArquivoPdf, PdfDocumentOpenMode.Import);
                for (int cont = 0; cont < PDFDoc.Pages.Count; cont++)
                {
                    try
                    {
                        var _paginaPdf = PDFDoc.Pages[cont];
                        var _textoDaPagina = PdfSharpExtensions.ExtractText(_paginaPdf);
                        var _dados = _textoDaPagina.ToList();
                        var _nomeArquivoTemporario = $"{Guid.NewGuid()}.DAT";
                        var _nomeArquivoExportado = $"{Path.GetFileNameWithoutExtension(nomeArquivoPdf)}_ConvertidoPdf.TXT";
                        using (TextWriter tw = new StreamWriter(Path.Combine(diretorioOrigem, _nomeArquivoTemporario)))
                        {
                            foreach (string s in _textoDaPagina) tw.WriteLine(s);
                        }
                        try
                        {
                            File.Move(Path.Combine(diretorioOrigem, _nomeArquivoTemporario),
                                Path.Combine(diretorioDestino, _nomeArquivoExportado), true);
                        }
                        finally
                        {
                            File.Delete(_nomeArquivoTemporario);
                        }
                    }
                    finally
                    {
                        File.Delete(nomeArquivoPdf);
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}