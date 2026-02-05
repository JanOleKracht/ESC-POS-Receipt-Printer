using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Bonprinter
{
    public class TechnischeBelegdaten
    {
        public int? Transaktionsnummer { get; set; }

        public string? SeriennummerKasse { get; set; }

        public DateTime? TransaktionAnfang { get; set; }
        public DateTime? TransaktionEnde { get; set; }

        public string? Signaturzähler { get; set; }

        public string? Prüfwert { get; set; }

        public TechnischeBelegdaten()
        { }

        public TechnischeBelegdaten(int? transaktionsnummer, string? seriennummerKasse, DateTime? transaktionAnfang, DateTime? transaktionEnde, string? signaturzähler, string? prüfwert)
        {
            Transaktionsnummer = transaktionsnummer;
            SeriennummerKasse = seriennummerKasse;
            TransaktionAnfang = transaktionAnfang;
            TransaktionEnde = transaktionEnde;
            Signaturzähler = signaturzähler;
            Prüfwert = prüfwert;
        }
    }
}