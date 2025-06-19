# Pre-Press Automaton v2.1 (PPA-2.1)

![.NET](https://img.shields.io/badge/.NET-9.0-blue) ![iText7](https://img.shields.io/badge/iText7-9.2.0-green) ![License](https://img.shields.io/badge/license-MIT-blue)

**"Measure twice, cut once, validate always."**

An advanced PDF analysis tool specifically designed for pre-press validation and quality control in print production workflows. PPA-2.1 automatically detects die-cuts, analyzes colors, extracts dimensions, and provides structured export formats for seamless integration with existing production systems.

## 🎯 Key Features

### Core Analysis
- **PDF Validation**: Comprehensive integrity checking with detailed error reporting
- **Die-Cut Detection**: Identifies 180+ spot color variations (CutContour, Stans, KissCutt, etc.)
- **Color Analysis**: CMYK, RGB, and spot color extraction with percentage values
- **Dimension Extraction**: MediaBox, TrimBox, BleedBox, and CropBox in multiple units
- **Line Width Detection**: Precise measurement of die-cut line thickness in points and mm

### Export & Integration
- **JSON Export**: Structured data for API integration and workflow automation
- **XML Export**: Industry-standard format for legacy system compatibility
- **Color Interpretation**: Automatic translation (e.g., C100M100Y100K100 = Black)
- **Database Management**: SQLite-based spot color database with 180+ pre-loaded colors

### Performance
- **Optimized Processing**: ~0.75-1.0 seconds per file analysis
- **Memory Efficient**: Minimal resource usage for batch processing
- **Error Resilient**: Graceful handling of corrupted or invalid files

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 or later
- Windows, macOS, or Linux

### Installation
```bash
git clone https://github.com/Whateverdoa/ConsoleApp1_Pdf_analyse.git
cd ConsoleApp1_Pdf_analyse/ConsoleApp1_Pdf_analyse
dotnet restore
dotnet build
```

### Basic Usage
```bash
# Analyze a PDF file
dotnet run -- path/to/your/file.pdf

# Export to JSON
dotnet run -- --export-json path/to/your/file.pdf

# Export to XML
dotnet run -- --export-xml path/to/your/file.pdf

# View help
dotnet run -- --help
```

## 📊 Example Output

### Console Analysis
```
--- Analysis for: business_card.pdf ---
PDF Valid: True
TrimBox (WxH): 89.00 mm x 51.00 mm
BleedBox (WxH): 95.00 mm x 57.00 mm
CMYK Colors Found: 3
  • C0M0Y0K100 (Black)
  • C100M0Y100K0 (Process Green)
  • C0M100Y100K0 (Process Red)
Spot Colors Found: 1
  • CutContour (DieCut) - Line Width: 1.0 pt (0.353 mm)
Die-Cut Found: True
Die-Cut Color: CutContour
```

### JSON Export Sample
```json
{
  "fileName": "business_card.pdf",
  "isValid": true,
  "pdfInfo": {
    "version": "PDF-1.4",
    "pageCount": 1
  },
  "dimensions": {
    "trimBox": {
      "widthMm": 89.0,
      "heightMm": 51.0,
      "widthInches": 3.504,
      "heightInches": 2.008
    }
  },
  "colors": {
    "cmykColors": [
      {
        "c": 0, "m": 0, "y": 0, "k": 100,
        "colorName": "Black",
        "cmykString": "C0M0Y0K100"
      }
    ],
    "spotColors": [
      {
        "name": "CutContour",
        "type": "DieCut",
        "isDieCut": true,
        "lineWidths": [
          {
            "points": 1.0,
            "millimeters": 0.353
          }
        ]
      }
    ]
  },
  "dieCut": {
    "found": true,
    "colorName": "CutContour",
    "lineWidthPoints": 1.0
  }
}
```

## 🛠 Technical Architecture

### Core Components
- **PdfAnalyzer**: Main analysis engine using iText7
- **ColorListener**: Custom event listener for color space detection
- **SpotColorDatabase**: SQLite database for color management
- **ColorInterpreter**: CMYK/RGB to common name translation
- **StructuredOutput**: JSON/XML serialization classes

### Supported Formats
- **Input**: PDF files (all versions)
- **Output**: Console, JSON, XML
- **Database**: SQLite for spot color storage

## 📚 Documentation

- [User Manual](docs/USER_MANUAL.md) - Complete usage guide
- [Technical Documentation](docs/TECHNICAL_DOCS.md) - Architecture and API reference
- [Installation Guide](docs/INSTALLATION.md) - Detailed setup instructions
- [API Roadmap](docs/API_ROADMAP.md) - Future API development plans

## 🔮 Future Development: API Service

PPA-2.1 is designed with API transformation in mind. The current console application architecture provides a solid foundation for converting to a REST API service. See [API Roadmap](docs/API_ROADMAP.md) for detailed plans.

### Planned API Features
- RESTful endpoints for PDF analysis
- Batch processing capabilities
- Webhook integrations
- Real-time analysis status
- Cloud storage integration
- Multi-tenant support

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with [iText7](https://itextpdf.com/) for PDF processing
- Powered by .NET 9.0
- Designed for the pre-press and print production industry

## 📞 Support

For support, issues, or feature requests, please open an issue on GitHub or contact the development team.

---

**Pre-Press Automaton v2.1** - *Precision in every pixel, accuracy in every analysis.*