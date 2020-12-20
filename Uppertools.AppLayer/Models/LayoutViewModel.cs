using System;
using System.Collections.Generic;
using System.Text;

namespace Uppertools.AppLayer.Models
{
    public class LayoutViewModel
    {
        public int Id { get; set; }
        public int FornecedorId { get; set; }        
        public string ExtensaoArquivo { get; set; }
        public int LinhaValorTotal { get; set; }
        public int LinhaCNPJ { get; set; }
    }
}
