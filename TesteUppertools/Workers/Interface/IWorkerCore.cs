using System;
using System.Collections.Generic;
using System.Text;

namespace TesteUppertools.Workers.Interface
{
    public interface IWorkerCore
    {
        public ICollection<string> IniciarTrabalhoPrincipal();
        public int InformarSegundosParaExecutar();
    }
}
