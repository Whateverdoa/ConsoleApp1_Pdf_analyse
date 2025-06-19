# Pre-Press Automaton v2.1 - User Manual

## Table of Contents
1. [Introduction](#introduction)
2. [Installation](#installation)
3. [Basic Usage](#basic-usage)
4. [Command Reference](#command-reference)
5. [Understanding Output](#understanding-output)
6. [Export Formats](#export-formats)
7. [Color Management](#color-management)
8. [Troubleshooting](#troubleshooting)
9. [Best Practices](#best-practices)

## Introduction

Pre-Press Automaton v2.1 (PPA-2.1) is a specialized tool for analyzing PDF files in pre-press production workflows. It automatically detects die-cuts, analyzes colors, extracts dimensions, and provides detailed reports in multiple formats.

### What PPA-2.1 Does
- **Validates PDF integrity** - Ensures files are properly formatted and readable
- **Detects die-cuts** - Identifies cutting lines using 180+ spot color variations
- **Analyzes colors** - Extracts CMYK, RGB, and spot colors with precise values
- **Measures dimensions** - Reports TrimBox, BleedBox, and MediaBox sizes
- **Exports structured data** - Provides JSON/XML output for workflow integration

### Who Should Use PPA-2.1
- Pre-press operators
- Print production managers
- Quality control teams
- Workflow automation developers
- Digital asset managers

## Installation

### System Requirements
- **Operating System**: Windows 10+, macOS 10.15+, or Linux (Ubuntu 18.04+)
- **.NET Runtime**: Version 9.0 or later
- **Memory**: 512MB RAM minimum, 1GB recommended
- **Storage**: 50MB free space

### Quick Install
```bash
# Clone the repository
git clone https://github.com/Whateverdoa/ConsoleApp1_Pdf_analyse.git

# Navigate to project directory
cd ConsoleApp1_Pdf_analyse/ConsoleApp1_Pdf_analyse

# Restore dependencies
dotnet restore

# Build the application
dotnet build
```

### Verify Installation
```bash
dotnet run -- --help
```
You should see the PPA-2.1 help menu.

## Basic Usage

### Analyzing a Single PDF
```bash
# Basic analysis with console output
dotnet run -- /path/to/your/file.pdf

# Interactive mode (prompts for file path)
dotnet run
```

### Export Analysis Results
```bash
# Export to JSON format
dotnet run -- --export-json /path/to/file.pdf

# Export to XML format  
dotnet run -- --export-xml /path/to/file.pdf
```

### Example Workflow
1. **Prepare your PDF file** - Ensure it's saved and accessible
2. **Run analysis** - Use the appropriate command for your needs
3. **Review results** - Check console output or exported files
4. **Take action** - Address any issues or continue with production

## Command Reference

### Analysis Commands
| Command | Description | Example |
|---------|-------------|---------|
| `<filepath>` | Analyze PDF and display results | `dotnet run -- document.pdf` |
| `--export-json <filepath>` | Analyze and export to JSON | `dotnet run -- --export-json document.pdf` |
| `--export-xml <filepath>` | Analyze and export to XML | `dotnet run -- --export-xml document.pdf` |

### Database Commands
| Command | Description | Example |
|---------|-------------|---------|
| `--list-colors` | List all spot colors in database | `dotnet run -- --list-colors` |
| `--add-color <Name> <Type> [Description]` | Add custom spot color | `dotnet run -- --add-color "Custom Cut" DieCut "Custom cutting line"` |
| `--add-pantone <Number> [Description]` | Add Pantone color | `dotnet run -- --add-pantone "185 C" "Bright red"` |

### Utility Commands
| Command | Description |
|---------|-------------|
| `--help` | Show help and command reference |

### Color Types
When adding custom colors, use these types:
- `DieCut` - Cutting/die-cutting lines
- `Foil` - Foil stamping areas
- `Emboss` - Embossing/debossing areas
- `Varnish` - Varnish/coating areas
- `Registration` - Registration marks
- `Perforation` - Perforation lines
- `Scoring` - Scoring/creasing lines
- `Cutout` - Cutout areas
- `Pantone` - Pantone spot colors

## Understanding Output

### Console Output Format
```
--- Analysis for: business_card.pdf ---
PDF Valid: True
PDF Version: PDF-1.4
Page Count: 1
TrimBox (WxH): 89.00 mm x 51.00 mm (3.504" x 2.008")
BleedBox (WxH): 95.00 mm x 57.00 mm (3.740" x 2.244")
MediaBox (WxH): 95.00 mm x 57.00 mm (3.740" x 2.244")

CMYK Colors Found: 3
  • C0M0Y0K100 (Black)
  • C100M0Y100K0 (Process Green)  
  • C0M100Y100K0 (Process Red)

Spot Colors Found: 1
  • CutContour (DieCut) - Line Width: 1.0 pt (0.353 mm)

Die-Cut Found: True
Die-Cut Color: CutContour
Line Width: 1.0 pt (0.353 mm)
```

### Understanding Dimensions
- **MediaBox**: The full page size including any printer marks
- **TrimBox**: The final size after trimming (most important for production)
- **BleedBox**: The area including bleed (typically 3mm larger than TrimBox)
- **CropBox**: Visible area (not always present)

### Color Information
- **CMYK Values**: Shown as percentages (0-100%)
- **Color Names**: Interpreted names (Black, White, etc.) or CMYK notation
- **Spot Colors**: Named colors with type classification
- **Line Widths**: Measured in points (pt) and millimeters (mm)

## Export Formats

### JSON Export
Best for modern applications and APIs:
- Structured data with nested objects
- Easy to parse programmatically
- Compact file size
- UTF-8 encoding

**File naming**: `filename.json` (replaces PDF extension)

### XML Export
Best for legacy systems and workflows:
- Hierarchical structure
- Self-documenting format
- Wide compatibility
- XML schema validation possible

**File naming**: `filename.xml` (replaces PDF extension)

### Export Location
Exported files are saved in the same directory as the source PDF file.

## Color Management

### Built-in Color Database
PPA-2.1 includes a SQLite database with 180+ pre-loaded spot colors:
- **Die-cut variations**: CutContour, Stans, KissCutt, Die-Cut, DieCut, etc.
- **Case-insensitive matching**: Handles CUTCONTOUR, cutcontour, CutContour
- **Multiple languages**: English, German, Dutch variations
- **Industry standards**: Common pre-press color names

### Adding Custom Colors
```bash
# Add a new die-cut color
dotnet run -- --add-color "Custom Cut" DieCut "Company-specific cutting line"

# Add a Pantone color
dotnet run -- --add-pantone "Process Blue C" "Standard process blue"

# Add a foil color
dotnet run -- --add-color "Gold Foil" Foil "Metallic gold foil stamp"
```

### Viewing Database Contents
```bash
# List all colors by type
dotnet run -- --list-colors
```

## Troubleshooting

### Common Issues

#### "File not found" Error
**Problem**: Cannot locate the specified PDF file
**Solution**: 
- Check file path spelling and case sensitivity
- Use quotes around paths with spaces: `"My Documents/file.pdf"`
- Verify file exists and you have read permissions

#### "Not a valid PDF file" Error
**Problem**: File is corrupted or not a PDF
**Solution**:
- Try opening the file in a PDF viewer
- Re-export from original application
- Check file extension is .pdf

#### "No die-cuts found" But You Expected Them
**Problem**: Die-cuts exist but aren't detected
**Solution**:
- Check if die-cut uses a different spot color name
- Add the custom color: `--add-color "YourColorName" DieCut`
- Verify die-cuts are actual spot colors, not process colors

#### Export Files Not Created
**Problem**: JSON/XML files aren't generated
**Solution**:
- Check write permissions in target directory
- Ensure sufficient disk space
- Verify PDF analysis completed successfully

### Performance Issues

#### Slow Analysis
- **Large files**: Files over 50MB may take longer
- **Complex graphics**: Many vector elements increase processing time
- **Multiple pages**: Only first page is analyzed, but loading time depends on total pages

#### Memory Usage
- Close other applications if memory is limited
- Process files individually rather than in batch

### Getting Help
1. Run `dotnet run -- --help` for command reference
2. Check error messages carefully - they often indicate the exact issue
3. Verify your PDF file opens correctly in other applications
4. Test with a simple, known-good PDF file first

## Best Practices

### File Preparation
- **Save as PDF/X-1a or PDF/X-4** for best compatibility
- **Embed all fonts** to avoid font-related issues
- **Convert text to outlines** if experiencing font problems
- **Use standard spot color names** for automatic detection

### Workflow Integration
- **Batch processing**: Process multiple files using shell scripts
- **Automation**: Use JSON/XML export for automated workflows
- **Quality control**: Set up automated checks using the exit codes
- **Documentation**: Keep records of analysis results for production tracking

### Color Management
- **Standardize spot color names** across your organization
- **Document custom colors** added to the database
- **Regular backups** of the spot color database
- **Test new colors** before production use

### Performance Optimization
- **Process files locally** when possible (faster than network drives)
- **Use SSD storage** for better I/O performance
- **Close unnecessary applications** during batch processing
- **Monitor memory usage** for very large files

---

For technical details and advanced usage, see the [Technical Documentation](TECHNICAL_DOCS.md).