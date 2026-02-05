namespace Test
{
    public class BelegPosition
    {
        public int PositionsNummer { get; set; }

        public string PositionsText { get; set; }

        public int Menge { get; set; }

        public decimal Einzelpreis { get; set; }

        public decimal Gesamtpreis { get; set; }
        public decimal Netto { get; set; }
        public decimal Brutto { get; set; }

        public string? MwstZeichen { get; set; }
    }
}