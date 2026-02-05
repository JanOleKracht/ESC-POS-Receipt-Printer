using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Bonprinter
{
    public class PrinterUnavailableException : Exception
    {
        public PrinterUnavailableException(string message) : base(message)
        {
        }
    }
}