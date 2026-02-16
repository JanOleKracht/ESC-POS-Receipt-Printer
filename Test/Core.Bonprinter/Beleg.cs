using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Bonprinter
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

        public decimal MwstBetragA { get; set; }
        public decimal MwstBetragB { get; set; }

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

        public void BerechneSumme()
        {
            decimal bruttoSumme = 0m;
            decimal nettoSumme = 0m;
            decimal mwstA = 0m;
            decimal mwstB = 0m;

            foreach (var position in BelegPositionen)
            {
                decimal brutto = position.Brutto;

                int mwstProzent = 0;

                if (position.MwstZeichen == "A")
                {
                    mwstProzent = 19;
                }
                else if (position.MwstZeichen == "B")
                {
                    mwstProzent = 7;
                }

                decimal faktor = 1m + (mwstProzent / 100m);

                decimal netto = brutto / faktor;
                netto = Math.Round(netto, 2);
                position.Netto = netto;

                bruttoSumme += brutto;
                nettoSumme += netto;

                if (position.MwstZeichen == "A")
                {
                    mwstA += brutto - netto;
                }
                else if (position.MwstZeichen == "B")
                {
                    mwstB += brutto - netto;
                }
            }
            BruttoGesamt = bruttoSumme;
            NettoGesamt = nettoSumme;
            MwstBetragA = mwstA;
            MwstBetragB = mwstB;
        }
    }
}