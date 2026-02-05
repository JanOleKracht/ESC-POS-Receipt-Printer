using ESCPOS_NET.Emitters;
using SixLabors.ImageSharp;

namespace Core.Bonprinter
{
    public enum Align
    {
        None,
        Left,
        Center,
        Right,
    }

    [Flags]
    public enum PrintStyleObject
    {
        None = 0,
        FontB = 1,
        Proportional = 2,
        Condensed = 4,
        Bold = 8,
        DoubleHeight = 0x10,
        DoubleWidth = 0x20,
        Italic = 0x40,
        Underline = 0x80
    }

    public class PrintObject
    {
        // Style
        public PrintStyleObject Style { get; set; }

        public Align AlignStyle { get; set; }

        // Text
        public bool isText { get; set; }

        public string Text { get; set; }

        // Picture
        public bool IsImage { get; set; }

        public byte[]? ImageData { get; set; }
        public int MaxWidth { get; set; }

        //QR Code
        public bool isQRCode { get; set; }

        public string QRData { get; set; } = string.Empty;

        // Bacrode
        public bool isBarcode { get; set; }

        public string BarcodeData { get; set; } = string.Empty;

        //Seperatoren
        public bool IsLine { get; set; }

        public bool IsLineComplete { get; set; }

        public bool NewLine { get; set; }

        public static PrintObject AddText(string text, PrintStyleObject style, Align align)
        {
            return new PrintObject
            {
                isText = true,
                Text = text,
                Style = style,
                AlignStyle = align
            };
        }

        public static PrintObject AddImage(byte[] imageData, Align align, int maxWidth = 150)
        {
            return new PrintObject
            {
                IsImage = true,
                ImageData = imageData,
                MaxWidth = maxWidth,
                AlignStyle = align
            };
        }

        public static PrintObject AddQRCode(string qrData, Align align)
        {
            return new PrintObject
            {
                isQRCode = true,
                QRData = qrData,
                AlignStyle = align
            };
        }

        public static PrintObject AddBarcode(string barcodeData, Align align)
        {
            return new PrintObject
            {
                isBarcode = true,
                BarcodeData = barcodeData,
                AlignStyle = align
            };
        }

        public static PrintObject AddLineSeparator()
        {
            return new PrintObject { IsLine = true };
        }

        public static PrintObject AddLineSeparatorComplete()
        {
            return new PrintObject { IsLineComplete = true };
        }

        public static PrintObject NewLineCreator()
        {
            return new PrintObject { NewLine = true };
        }
    }
}