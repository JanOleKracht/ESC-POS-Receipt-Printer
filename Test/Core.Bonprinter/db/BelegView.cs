namespace Core.Bonprinter.db
{
    public class BelegView
    {
        public Guid Id { get; set; }

        public Guid? PatientId { get; set; }

        public DateTime Belegdatum { get; set; }

        public byte BelegTypId { get; set; }

        public string? Belegtext { get; set; }

        public string? Belegnummer { get; set; }

        public decimal Betrag { get; set; }

        public decimal BetragSteuer { get; set; }

        public bool Abgeschlossen { get; set; }

        public string BelegTypIdName { get; set; } = null!;

        public bool IstZuzahlung { get; set; }

        public string? Verordnungsnummer { get; set; }

        public bool IstOffen { get; set; }

        public decimal OffenerBetrag { get; set; }
    }
}