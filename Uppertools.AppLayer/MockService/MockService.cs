using Uppertools.AppLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace Uppertools.AppLayer.MockService
{
    public static class MockService
    {
        public static ICollection<FornecedorViewModel> ObterFornecedores()
        {
            var lista = new List<FornecedorViewModel>();
            lista.Add(new FornecedorViewModel() { Id = 1, Nome = "DanfeTeste", Email = "danfe@teste.com" });
            lista.Add(new FornecedorViewModel() { Id = 2, Nome = "NfeTeste", Email = "nfe@teste.com" });
            return lista;
        }

        public static ICollection<LayoutViewModel> ObterLayouts()
        {
            var lista = new List<LayoutViewModel>();
            lista.Add(new LayoutViewModel() { Id = 1, FornecedorId = 1, ExtensaoArquivo = "pdf", LinhaValorTotal = 79, LinhaCNPJ = 40 });
            return lista;
        }

        public static ICollection<LayoutViewModel> ObterLayoutFornecedor(int fornecedorId)
        {
            var _layouts = ObterLayouts();
            return _layouts.Where(_layout => _layout.FornecedorId == fornecedorId).ToList();
        }
    }
}
