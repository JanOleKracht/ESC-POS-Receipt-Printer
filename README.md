# 🧾 ESC/POS Receipt Printer

A C# application for creating and printing receipts on ESC/POS compatible thermal printers via TCP/IP.

> **Note:** This project was developed during my internship as part of my retraining as an IT Specialist for Application Development.

---

## 📸 Example Receipt

<!-- Add a photo of your printed receipt here:
![Example Receipt](docs/example-receipt.jpg)
-->

*Image coming soon*

---

## ✨ Features

- **Receipt Layout** – Structured design with header, line items, VAT calculation, and footer
- **Automatic Calculation** – Net/gross totals and VAT amounts calculated automatically
- **VAT Support** – Separate calculation for different tax rates (19% and 7%)
- **Logo Printing** – Embed images (PNG) in the receipt header
- **QR Code** – Automatic generation for TSE-compliant technical receipt data
- **Barcode Support** – CODE128 barcodes
- **Business Entertainment Receipt** – Optional section for business meal documentation
- **Text Formatting** – Bold, double height/width, various fonts
- **Connection Check** – Timeout-based printer availability check
- **Unit Tests** – Comprehensive test coverage for calculation logic

---

## 🏗️ Project Structure

```
Core.Bonprinter/
├── Program.cs                    # Entry point with sample receipt
├── Beleg.cs                      # Data model with calculation logic
├── BelegPosition.cs              # Single line item on receipt
├── AnschriftBelegdaten.cs        # Business address data
├── TechnischeBelegdaten.cs       # TSE data for QR code
├── PrintObject.cs                # Abstraction for print elements
├── PrintController.cs            # Print process controller
├── TcpTransport.cs               # TCP communication with printer
├── PrinterConnectionCheck.cs     # Availability check
├── PrinterUnavailableException.cs# Custom exception
└── Img/
    └── Test-Logo.png             # Logo for receipt header

Core.Bonprinter.Tests/
└── CalculationTests.cs           # Unit tests for calculation logic
```

---

## 🚀 Installation & Usage

### Prerequisites

- .NET 6.0 or higher
- ESC/POS compatible thermal printer (e.g. Epson TM series)
- Printer accessible on network (default port: 9100)

### Setup

1. Clone repository:
```bash
git clone https://github.com/YOUR-USERNAME/escpos-receipt-printer.git
```

2. Update printer IP in `Program.cs`:
```csharp
string host = "192.168.1.100";  // Your printer IP
int port = 9100;
```

3. Run project:
```bash
dotnet run
```

---

## 💡 Code Example

Creating and printing a receipt:

```csharp
var receipt = new Beleg
{
    BelegTypIdName = "Invoice",
    BelegNummer = "No.RE17-753",
    CreationTime = DateTime.Now,
    Zahlart = "(CASH)",

    BelegPositionen = new List<BelegPosition>
    {
        new BelegPosition { Menge = 2, PositionsText = "Aperol Spritz", Brutto = 14.98m, MwstZeichen = "B" },
        new BelegPosition { Menge = 1, PositionsText = "Rib Eye Steak", Brutto = 54.99m, MwstZeichen = "A" }
    }
};

// Automatic calculation of net, gross and VAT
receipt.BerechneSumme();

PrintReceipt(receipt, bewirtung: false, host, port);
```

---

## 🧮 Calculation Logic

The `BerechneSumme()` method automatically calculates:

- **Net amount** per position (from gross price)
- **Total gross** sum of all positions
- **Total net** sum of all positions
- **VAT amount A** (19% tax rate)
- **VAT amount B** (7% tax rate)

```csharp
// Example output:
// Gross total: 116.95 €
// Net total:   101.91 €
// VAT A (19%):  14.21 €
// VAT B (7%):    1.83 €
```

---

## 🧪 Unit Tests

The project includes unit tests using xUnit to verify the calculation logic:

| Test | Description |
|------|-------------|
| `CalculateBruttoGesamt_ShouldCalculateCorrectly` | Verifies gross total calculation |
| `CalculateNettoGesamt_ShouldCalculateCorrectly` | Verifies net total calculation |
| `CalculateMwstBetragA_ShouldCalculateCorrectly` | Verifies 19% VAT calculation |
| `CalculateMwstBetragB_ShouldCalculateCorrectly` | Verifies 7% VAT calculation |
| `CalculateSumme_EmptyPositions_EverythingShouldBeZero` | Verifies edge case with empty list |

Run tests:
```bash
dotnet test
```

---

## 🔧 Technologies Used

| Technology | Purpose |
|------------|---------|
| C# / .NET | Programming language |
| ESC/POS | Printer command protocol |
| TCP/IP | Network communication |
| ESCPOS-NET | NuGet package for ESC/POS commands |
| SixLabors.ImageSharp | Image processing |
| xUnit | Unit testing framework |

---

## 📋 Planned Improvements

- [ ] Configuration file for printer settings
- [ ] Support for additional tax rates
- [ ] Async printing

---

## 📝 License

This project was created for learning and demonstration purposes.

---

## 👤 Author

Created during an internship as part of retraining as an **IT Specialist for Application Development** (Fachinformatiker für Anwendungsentwicklung).

<!-- Optional: Your contact info
- GitHub: [@your-username](https://github.com/your-username)
- LinkedIn: [Your Name](https://linkedin.com/in/your-profile)
-->
