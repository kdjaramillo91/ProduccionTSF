using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilitarios.ProdException
{
    public class ProdHandlerException : Exception
    {
        public ProdHandlerException(string message) :base(message)
        {
        
        }
    }
}
