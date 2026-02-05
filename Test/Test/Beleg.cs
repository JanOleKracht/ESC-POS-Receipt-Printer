using Core.Bonprinter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class Beleg
    {
        public DateTime CreationTime { get; set; }

        public string? BelegTypIdName { get; set; }

        public string? BelegNummer { get; set; }

        public string? Zahlart { get; set; }
        public decimal BruttoGesamt { get; set; }
        public decimal BruttoEinzeln { get; set; }

        public decimal NettoGesamt { get; set; }
        public decimal NettoEinzeln { get; set; }

        public int MwstSatz { get; set; }
        public string? MwstZeichen { get; set; }

        public bool Bewirtung { get; set; } = false;

        public List<BelegPosition> BelegPositionen { get; set; }

        public List<TechnischeBelegdaten> _TechnischeBelegdaten { get; set; }
        public List<AnschriftBelegdaten> _AnschriftBelegdaten { get; set; }

        public decimal GetMwstProzent(int mwstZeichen)
        {
            if (mwstZeichen == 1)
            {
                return 19;
            }

            if (mwstZeichen == 2)
            {
                return 7;
            }

            return 0;
        }
    }
}