using Core.Bonprinter;
using Core.Bonprinter.db;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Emitters.BaseCommandValues;
using Microsoft.Identity.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using PrinterAlign = Core.Bonprinter.Align;

internal class Program
{
    private static void Main()
    {
        // Drucker-Konfiguration
        string host = "PRINTER_IP_HERE";
        int port = 9100;

        // TestObjekt
        var meinBeleg = new Beleg
        {
            BelegTypIdName = "Rechnung ",
            BelegNummer = "Nr.RE17-753",
            CreationTime = DateTime.Now,
            Zahlart = "(BAR)",
            NettoGesamt = 10.31m,
            BruttoGesamt = 11.44m,
            MwstSatz = 1,
            MwstZeichen = "A",

            BelegPositionen = new List<BelegPosition>
            {
                new BelegPosition { Menge = 2, PositionsText = "MOHNKUCHEN", Brutto = 2.98m, MwstZeichen = "A" },
                new BelegPosition { Menge = 1, PositionsText = "SAHNE",      Brutto = 0.99m, MwstZeichen = "A" },
                new BelegPosition { Menge = 2, PositionsText = "KAFFEE",     Brutto = 5.98m, MwstZeichen = "B" },
                new BelegPosition { Menge = 3, PositionsText = "Espresso",   Brutto = 1.47m, MwstZeichen = "B" }
            },

            _TechnischeBelegdaten = new List<TechnischeBelegdaten>
            {
               new TechnischeBelegdaten
               {
                   Transaktionsnummer= 12345678,
                   SeriennummerKasse = "Kasse-01-Ser12345",
                   TransaktionAnfang = DateTime.Now.AddSeconds(-120),
                   TransaktionEnde = DateTime.Now,
                   Signaturzähler = "584",
                   Prüfwert = "AQHT45/GFd=="
               }
            },
            _AnschriftBelegdaten = new List<AnschriftBelegdaten>
            {
                new AnschriftBelegdaten
                {
                    Name = "Fine Food & Drinks",
                    Straße = "Max Mustermann Allee 1",
                    Ort = "D-1234 Winden",
                    TelefonNr = "01234/567 8910"
                }
            }
        };

        bool bewirtung = true;
        CustumorBonPrint(meinBeleg, bewirtung, host, port);
    }

    private static void CustumorBonPrint(Beleg meinBeleg, bool bewirtung, string host, int port)
    {
        // Printer Connection
        var pc = new PrintController(host, port.ToString());

        string imagePath = Path.Combine("Img", "Test-Logo.png");
        var pictureByte = File.ReadAllBytes(imagePath);

        var poList = new List<PrintObject>();

        #region Kopfzeile

        //Logo
        poList.Add(PrintObject.AddImage(pictureByte, PrinterAlign.Center, 200));
        poList.Add(PrintObject.NewLineCreator());

        // Adresse
        foreach (var pos in meinBeleg._AnschriftBelegdaten)
        {
            poList.Add(PrintObject.AddText(
                pos.Name,
                PrintStyleObject.FontB,
                PrinterAlign.Center
                ));
            poList.Add(PrintObject.NewLineCreator());

            poList.Add(PrintObject.AddText(pos.Straße,
                PrintStyleObject.FontB,
                PrinterAlign.Center
                ));
            poList.Add(PrintObject.NewLineCreator());

            poList.Add(PrintObject.AddText(pos.Ort,
                PrintStyleObject.FontB,
                PrinterAlign.Center
                ));
            poList.Add(PrintObject.NewLineCreator());

            poList.Add(PrintObject.AddText($"Tel:{pos.TelefonNr}",
                PrintStyleObject.FontB,
                PrinterAlign.Center
                ));
            poList.Add(PrintObject.NewLineCreator());
        }
        poList.Add(PrintObject.NewLineCreator());

        #endregion Kopfzeile

        #region Bezichnung und Datum

        //Kopfzeile
        poList.Add(PrintObject.AddText(
            meinBeleg.BelegTypIdName,
            PrintStyleObject.Bold | PrintStyleObject.DoubleHeight,
            PrinterAlign.Center
        ));

        poList.Add(PrintObject.AddText(
            meinBeleg.BelegNummer,
            PrintStyleObject.Bold | PrintStyleObject.DoubleHeight,
            PrinterAlign.Center
        ));

        poList.Add(PrintObject.AddText(
            meinBeleg.Zahlart,
            PrintStyleObject.Bold | PrintStyleObject.DoubleHeight,
            PrinterAlign.Center
        ));

        poList.Add(PrintObject.NewLineCreator());
        poList.Add(PrintObject.NewLineCreator());

        //Datum
        poList.Add(PrintObject.AddText(
            meinBeleg.CreationTime.ToString(),
            PrintStyleObject.None,
            PrinterAlign.Left
        ));

        // Währung
        poList.Add(PrintObject.AddText(
            "EUR".PadLeft(21),
            PrintStyleObject.None,
            PrinterAlign.Left
        ));

        poList.Add(PrintObject.NewLineCreator());
        poList.Add(PrintObject.AddLineSeparator());

        #endregion Bezichnung und Datum

        #region Rechnung

        //Artikel Auflistung
        int maxBreite = 42;
        foreach (var pos in meinBeleg.BelegPositionen)
        {
            string preisString = $"{pos.Brutto:F2} {pos.MwstZeichen}";
            string text = pos.PositionsText;
            int maxTextLänge = maxBreite - preisString.Length - 1;

            // Umbruch bei langem Text
            while (text.Length > maxTextLänge)
            {
                int längeZeile = Math.Min(text.Length, maxBreite);
                string teilText = text.Substring(0, längeZeile);

                poList.Add(PrintObject.AddText(
                    teilText,
                    PrintStyleObject.None,
                    PrinterAlign.Left
                ));

                text = text.Substring(längeZeile);
                if (text.Length == 0)
                {
                    break;
                }
            }

            string ganzeZeile = text.PadRight(maxBreite - preisString.Length) + preisString;

            poList.Add(PrintObject.AddText(
                ganzeZeile,
                PrintStyleObject.None,
                PrinterAlign.Left
            ));

            // Menge
            if (pos.Menge > 1)
            {
                decimal einzelPreis = pos.Brutto / pos.Menge;

                poList.Add(PrintObject.AddText(
                    $"  {pos.Menge} x {einzelPreis:F2}",
                    PrintStyleObject.None,
                    PrinterAlign.Left
                ));

                poList.Add(PrintObject.NewLineCreator());
            }
        }

        poList.Add(PrintObject.AddLineSeparator());

        #endregion Rechnung

        #region Bezahlung

        //Mwst berechnen
        poList.Add(PrintObject.AddText(
            "Netto:",
            PrintStyleObject.FontB,
            PrinterAlign.Left
        ));

        poList.Add(PrintObject.AddText(
           $"{meinBeleg.NettoGesamt}".PadLeft(48),
           PrintStyleObject.FontB,
           PrinterAlign.Left
        ));
        poList.Add(PrintObject.NewLineCreator());

        // Mwst 19%
        poList.Add(PrintObject.AddText(
            "A:",
            PrintStyleObject.FontB,
            PrinterAlign.Left
        ));
        poList.Add(PrintObject.AddText(
            $"{meinBeleg.GetMwstProzent(1)}% Mwst:",
            PrintStyleObject.FontB,
            PrinterAlign.Left
        ));

        poList.Add(PrintObject.AddText(
          $"{0.63m:F2} A".PadLeft(44),
          PrintStyleObject.FontB,
          PrinterAlign.Left
        ));
        poList.Add(PrintObject.NewLineCreator());

        //mwst 7%
        poList.Add(PrintObject.AddText(
             "B:",
             PrintStyleObject.FontB,
             PrinterAlign.Left
        ));
        poList.Add(PrintObject.AddText(
            $" {meinBeleg.GetMwstProzent(2)}% Mwst:",
            PrintStyleObject.FontB,
            PrinterAlign.Left
        ));

        poList.Add(PrintObject.AddText(
          $"{0.49m:F2} B".PadLeft(44),
          PrintStyleObject.FontB,
          PrinterAlign.Left
        ));
        poList.Add(PrintObject.NewLineCreator());
        poList.Add(PrintObject.AddLineSeparator());
        poList.Add(PrintObject.AddLineSeparator());
        poList.Add(PrintObject.NewLineCreator());

        // GesamtPreis
        int maxBreiteSumme = 21;
        string summeLabel = "Summe:";
        string summeWert = meinBeleg.BruttoGesamt.ToString("F2");

        string ganzeSummenZeile = summeLabel + summeWert.PadLeft(maxBreiteSumme - summeLabel.Length);

        poList.Add(PrintObject.AddText(
            ganzeSummenZeile,
            PrintStyleObject.Bold | PrintStyleObject.DoubleWidth,
            PrinterAlign.Left
        ));

        poList.Add(PrintObject.NewLineCreator());
        poList.Add(PrintObject.NewLineCreator());

        #endregion Bezahlung

        #region Footer

        // Abschiedstext?
        poList.Add(PrintObject.AddText(
          "Es bediente Sie:",
          PrintStyleObject.Bold,
          PrinterAlign.Center
        ));

        poList.Add(PrintObject.NewLineCreator());

        poList.Add(PrintObject.AddText(
         "Team Krumbad",
         PrintStyleObject.Bold,
         PrinterAlign.Center
        ));

        poList.Add(PrintObject.NewLineCreator());

        poList.Add(PrintObject.AddText(
         "im Restaurant",
         PrintStyleObject.Bold,
         PrinterAlign.Center
        ));

        poList.Add(PrintObject.NewLineCreator());

        // QR Code Technische Belegdaten

        if (meinBeleg._TechnischeBelegdaten != null)
        {
            string qrDatenString = "";
            foreach (var pos in meinBeleg._TechnischeBelegdaten)
            {
                qrDatenString =
                $"SN: {pos.SeriennummerKasse};\r\n" +
                $"Trans-Nr.: {pos.Transaktionsnummer};\r\n" +
                $"Start: {pos.TransaktionAnfang:s};\r\n" +
                $"Ende: {pos.TransaktionEnde:s};\r\n" +
                $"Sig-Z: {pos.Signaturzähler};\r\n" +
                $"Code: {pos.Prüfwert}";
            }

            poList.Add(PrintObject.AddQRCode(
        qrDatenString,
         PrinterAlign.Center
       ));
        }

        poList.Add(PrintObject.NewLineCreator());
        poList.Add(PrintObject.AddLineSeparator());
        poList.Add(PrintObject.AddLineSeparator());
        poList.Add(PrintObject.NewLineCreator());

        #endregion Footer

        #region Bewirtungszusatz

        // Bewirtung Zusatz
        if (bewirtung)
        {
            poList.Add(PrintObject.AddText(
               "Bewirtungsaufwand-Angaben:",
               PrintStyleObject.Bold,
               PrinterAlign.Left
            ));

            poList.Add(PrintObject.NewLineCreator());
            poList.Add(PrintObject.NewLineCreator());

            poList.Add(PrintObject.AddText(
               "Bewirtete Personen:",
               PrintStyleObject.Bold | PrintStyleObject.FontB,
               PrinterAlign.Left
            ));

            poList.Add(PrintObject.NewLineCreator());
            poList.Add(PrintObject.AddLineSeparatorComplete());
            poList.Add(PrintObject.AddLineSeparatorComplete());
            poList.Add(PrintObject.AddLineSeparatorComplete());
            poList.Add(PrintObject.AddLineSeparatorComplete());
            poList.Add(PrintObject.NewLineCreator());

            poList.Add(PrintObject.AddText(
               "Anlass der Bewirtung:",
               PrintStyleObject.Bold | PrintStyleObject.FontB,
               PrinterAlign.Left
            ));

            poList.Add(PrintObject.NewLineCreator());
            poList.Add(PrintObject.AddLineSeparatorComplete());
            poList.Add(PrintObject.AddLineSeparatorComplete());
            poList.Add(PrintObject.AddLineSeparatorComplete());
            poList.Add(PrintObject.NewLineCreator());

            poList.Add(PrintObject.AddText(
               "Betrag:",
               PrintStyleObject.Bold | PrintStyleObject.FontB,
               PrinterAlign.Left
            ));

            poList.Add(PrintObject.NewLineCreator());
            poList.Add(PrintObject.AddLineSeparatorComplete());
            poList.Add(PrintObject.NewLineCreator());
            poList.Add(PrintObject.NewLineCreator());

            poList.Add(PrintObject.AddText(
               "Unterschrift:",
               PrintStyleObject.Bold | PrintStyleObject.FontB,
               PrinterAlign.Left
            ));
        }

        #endregion Bewirtungszusatz

        pc.PrintBon(poList);

        // Timeout wichtig, sonst kein Druck!
        Thread.Sleep(200);
        var printObj = poList.FirstOrDefault();

        Console.WriteLine("Gesendet.");
    }
}