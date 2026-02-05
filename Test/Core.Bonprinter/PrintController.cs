using Core.Bonprinter.db;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using System.Text;

namespace Core.Bonprinter
{
    public class PrintController
    {
        private readonly string _printerHost;
        private readonly string _printerPort;

        public PrintController(string printerHost, string port)
        {
            _printerHost = printerHost;
            _printerPort = port;
        }

        public void PrintBon(List<PrintObject> poList)
        {
            using var context = new VIContext();
            //ALL
            var belege = context.Belege
                .ToList();

            //ByID
            var beleg = context.Belege
                .FirstOrDefault(x => x.Id == new Guid("7651FBC2-6030-424E-A719-7981DCBC335A"));
            //var settings = new NetworkPrinterSettings
            //{
            //    ConnectionString = $"{_printerHost}:{_printerPort}",
            //    PrinterName = "EPSON"
            //};

            //using var printer = new NetworkPrinter(settings);

            var e = new EPSON();

            List<byte[]> cmds = new List<byte[]>();
            cmds.Add(e.Initialize());

            foreach (var po in poList)
            {
                // Align setzen
                if (po.AlignStyle == Align.Left || po.AlignStyle == Align.None)
                {
                    cmds.Add(e.LeftAlign());
                }
                else if (po.AlignStyle == Align.Center)
                {
                    cmds.Add(e.CenterAlign());
                }
                else
                {
                    cmds.Add(e.RightAlign());
                }

                // Styles setzen. Bold, FontB usw
                cmds.Add(e.SetStyles(PrintStyle.None)); // Reset
                if (po.Style != PrintStyleObject.None)
                {
                    cmds.Add(e.SetStyles((PrintStyle)po.Style));
                }

                // Image
                if (po.IsImage && po.ImageData != null)
                {
                    int maxW;
                    if (po.MaxWidth > 0)
                    {
                        maxW = po.MaxWidth;
                    }
                    else
                    {
                        maxW = 384;
                    }
                    // Wichtig isLegacy=true bei alten ESC * BitImage Modus
                    cmds.Add(e.PrintImage(po.ImageData, false, true, maxW, 1));
                    continue;
                }

                // QR Code
                if (po.isQRCode && !string.IsNullOrEmpty(po.QRData))
                {
                    cmds.Add(e.PrintQRCode(po.QRData));
                    continue;
                }

                //Barcode
                if (po.isBarcode && !string.IsNullOrEmpty(po.BarcodeData))
                {
                    cmds.Add(e.SetBarcodeHeightInDots(100));
                    cmds.Add(e.SetBarWidth(BarWidth.Thin));
                    cmds.Add(e.PrintBarcode(BarcodeType.CODE128, po.BarcodeData));
                    continue;
                }

                //  Text
                if (!string.IsNullOrWhiteSpace(po.Text))
                {
                    //cmds.Add(e.Print(po.Text));
                    var codePage = Encoding.GetEncoding(850);
                    var bytes = codePage.GetBytes(po.Text);
                    cmds.Add(bytes);
                }

                // Trennzeile
                if (po.IsLine)
                {
                    string separator = "------------------------------------------";
                    cmds.Add(e.Print(separator));
                    continue;
                }

                if (po.IsLineComplete)
                {
                    string separator = "__________________________________________";
                    cmds.Add(e.Print(separator));
                    continue;
                }

                // Neue Zeile
                if (po.NewLine)
                {
                    int newLine = 1;
                    cmds.Add(e.FeedLines(newLine));
                }
            }

            cmds.Add(e.FeedLines(5));
            cmds.Add(e.PartialCut());

            var all = ByteSplicer.Combine(cmds.ToArray());

            var transport = new TcpTransport(_printerHost, int.Parse(_printerPort));
            transport.Send(all);

            //printer.Write(all);
        }
    }
}