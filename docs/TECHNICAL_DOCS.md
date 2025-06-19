# Pre-Press Automaton v2.1 - Technical Documentation

## Table of Contents
1. [Architecture Overview](#architecture-overview)
2. [Core Components](#core-components)
3. [Data Models](#data-models)
4. [PDF Processing Pipeline](#pdf-processing-pipeline)
5. [Color Detection System](#color-detection-system)
6. [Database Schema](#database-schema)
7. [Export System](#export-system)
8. [Performance Characteristics](#performance-characteristics)
9. [Extension Points](#extension-points)
10. [API Reference](#api-reference)

## Architecture Overview

PPA-2.1 follows a modular architecture designed for maintainability, extensibility, and performance. The system is built on .NET 9.0 using iText7 for PDF processing.

```
┌─────────────────────────────────────────────┐
│                 Program.cs                  │
│            (CLI Interface)                  │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│              PdfAnalyzer                    │
│           (Main Analysis Engine)            │
└─────────────────┬───────────────────────────┘
                  │
    ┌─────────────┼─────────────┐
    │             │             │
┌───▼────┐ ┌─────▼─────┐ ┌─────▼─────┐
│ColorLi-│ │SpotColor  │ │ColorInter-│
│stener  │ │Database   │ │preter     │
└────────┘ └───────────┘ └───────────┘
    │             │             │
    └─────────────┼─────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│          PdfAnalysisResult                  │
│         (Output Data Model)                 │
└─────────────────┬───────────────────────────┘
                  │
    ┌─────────────┼─────────────┐
    │             │             │
┌───▼────┐ ┌─────▼─────┐ ┌─────▼─────┐
│Console │ │JSON       │ │XML        │
│Output  │ │Export     │ │Export     │
└────────┘ └───────────┘ └───────────┘
```

### Design Principles
- **Single Responsibility**: Each class has a focused purpose
- **Dependency Injection**: Loose coupling between components
- **Event-Driven Processing**: PDF content processed via event listeners
- **Immutable Data**: Analysis results are immutable once created
- **Fail-Fast**: Early validation and clear error messages

## Core Components

### PdfAnalyzer
**Location**: `PdfAnalyzer.cs`  
**Purpose**: Main orchestration class that coordinates PDF analysis

```csharp
public class PdfAnalyzer
{
    public PdfAnalysisResult Analyze(string filePath)
    public void AddSpotColor(string name, string type, string description = "")
    public void AddPantoneColor(string pantoneNumber, string description = "")
    public List<SpotColorInfo> GetAllSpotColors()
}
```

**Key Features**:
- PDF validation using iText7's `PdfReader`
- Page dimension extraction (MediaBox, TrimBox, BleedBox, CropBox)
- Color space analysis coordination
- Database interaction management
- Error handling and reporting

### ColorListener
**Location**: `ColorListener.cs`  
**Purpose**: iText7 event listener for detecting colors and drawing operations

```csharp
public class ColorListener : IEventListener
{
    public void EventOccurred(IEventData data, EventType type)
    public ICollection<EventType> GetSupportedEvents()
    
    // Properties
    public List<CmykColorInfo> CmykColors { get; }
    public List<RgbColorInfo> RgbColors { get; }
    public List<SpotColorInfo> SpotColors { get; }
}
```

**Supported Events**:
- `RENDER_PATH`: Vector drawing operations
- `RENDER_TEXT`: Text rendering (for embedded colors)
- `RENDER_IMAGE`: Image color analysis

**Color Space Detection**:
- **CMYK**: `PdfDeviceCs.CMYK`
- **RGB**: `PdfDeviceCs.RGB`
- **Gray**: `PdfDeviceCs.GRAY`
- **Separation**: `PdfSpecialCs.Separation` (spot colors)

### SpotColorDatabase
**Location**: `SpotColorDatabase.cs`  
**Purpose**: SQLite-based storage for spot color definitions

```csharp
public class SpotColorDatabase
{
    public void InitializeDatabase()
    public void AddSpotColor(string name, string type, string description, string pantoneNumber)
    public List<SpotColorInfo> GetAllSpotColors()
    public bool IsSpotColor(string colorName)
    public string GetSpotColorType(string colorName)
}
```

**Database Features**:
- Case-insensitive color matching
- 180+ pre-loaded color definitions
- Custom color addition
- Pantone color support
- Automatic database creation

### ColorInterpreter
**Location**: `ColorInterpreter.cs`  
**Purpose**: Translates CMYK/RGB values to human-readable color names

```csharp
public class ColorInterpreter
{
    public static string InterpretCmyk(float c, float m, float y, float k)
    public static string InterpretRgb(float r, float g, float b)
}
```

**Color Interpretation Rules**:
- **Black**: K=100%, CMY near 0%
- **White**: All values near 0%
- **Primary Colors**: Single channel at 100%
- **Process Colors**: Standard CMYK combinations
- **Rich Black**: K=100% with CMY support

## Data Models

### PdfAnalysisResult
Primary data container for analysis results:

```csharp
public class PdfAnalysisResult
{
    // File Information
    public string FilePath { get; set; }
    public bool IsValidPdf { get; set; }
    public string ErrorMessage { get; set; }
    
    // PDF Metadata
    public string PdfVersion { get; set; }
    public int PageCount { get; set; }
    
    // Dimensions (in multiple units)
    public double MediaBoxWidthMm { get; set; }
    public double MediaBoxHeightMm { get; set; }
    public double TrimBoxWidthMm { get; set; }
    public double TrimBoxHeightMm { get; set; }
    public double BleedBoxWidthMm { get; set; }
    public double BleedBoxHeightMm { get; set; }
    
    // Color Analysis
    public List<CmykColorInfo> CmykColors { get; set; }
    public List<RgbColorInfo> RgbColors { get; set; }
    public List<SpotColorInfo> SpotColors { get; set; }
    
    // Die-Cut Analysis
    public bool DieCutFound { get; set; }
    public string FoundDieCutName { get; set; }
    public float DieCutLineWidthPoints { get; set; }
    public float DieCutLineWidthMm { get; set; }
}
```

### Color Information Classes

```csharp
public class CmykColorInfo
{
    public float C { get; set; }  // Cyan percentage (0-100)
    public float M { get; set; }  // Magenta percentage (0-100)
    public float Y { get; set; }  // Yellow percentage (0-100) 
    public float K { get; set; }  // Black percentage (0-100)
    public string ColorName { get; set; }
    public string CmykString { get; set; }  // e.g., "C100M0Y100K0"
}

public class SpotColorInfo
{
    public string ColorName { get; set; }
    public string ColorType { get; set; }  // DieCut, Foil, etc.
    public bool IsDieCut { get; set; }
    public List<LineWidthInfo> LineWidths { get; set; }
}

public class LineWidthInfo
{
    public float Points { get; set; }        // Width in points
    public float Millimeters { get; set; }   // Width in millimeters
}
```

## PDF Processing Pipeline

### 1. File Validation
```csharp
try
{
    using var pdfReader = new PdfReader(filePath);
    using var pdfDoc = new PdfDocument(pdfReader);
    // File is valid PDF
}
catch (PdfException ex)
{
    // Invalid PDF file
}
```

### 2. Metadata Extraction
```csharp
var pdfVersion = pdfDoc.GetPdfVersion().ToString();
var pageCount = pdfDoc.GetNumberOfPages();
var firstPage = pdfDoc.GetPage(1);
```

### 3. Dimension Analysis
```csharp
var mediaBox = firstPage.GetMediaBox();
var trimBox = firstPage.GetTrimBox() ?? mediaBox;  // Fallback
var bleedBox = firstPage.GetBleedBox() ?? mediaBox;
var cropBox = firstPage.GetCropBox();
```

**Unit Conversion**:
- Points to Millimeters: `mm = points / 72.0 * 25.4`
- Points to Inches: `inches = points / 72.0`

### 4. Color Space Processing
```csharp
var colorListener = new ColorListener(spotColorDatabase);
var processor = new PdfCanvasProcessor(colorListener);
processor.ProcessPageContent(firstPage);
```

### 5. Result Compilation
```csharp
var result = new PdfAnalysisResult
{
    // Populate from analysis components
    CmykColors = colorListener.CmykColors,
    SpotColors = colorListener.SpotColors,
    DieCutFound = colorListener.SpotColors.Any(c => c.IsDieCut)
};
```

## Color Detection System

### Event Processing Flow
1. **Event Trigger**: iText7 processes PDF content and triggers events
2. **Event Filtering**: ColorListener filters for relevant event types
3. **Graphics State Analysis**: Extract current drawing state
4. **Color Space Identification**: Determine color space type
5. **Color Value Extraction**: Get specific color values
6. **Database Lookup**: Check if spot color is in database
7. **Result Storage**: Store color information with metadata

### Color Space Handling

#### CMYK Colors
```csharp
if (color.GetColorSpace() is PdfDeviceCs.Cmyk)
{
    float[] values = color.GetColorValue();
    var cmykInfo = new CmykColorInfo
    {
        C = (float)Math.Round(values[0] * 100, 1),
        M = (float)Math.Round(values[1] * 100, 1),
        Y = (float)Math.Round(values[2] * 100, 1),
        K = (float)Math.Round(values[3] * 100, 1)
    };
}
```

#### Spot Colors (Separation Color Space)
```csharp
if (color.GetColorSpace() is PdfSpecialCs.Separation separationCs)
{
    var pdfArray = separationCs.getPdfObject() as PdfArray;
    var nameObj = pdfArray.Get(1);
    string spotColorName = nameObj.ToString();
    
    // Database lookup for color type
    string colorType = spotColorDatabase.GetSpotColorType(spotColorName);
    bool isDieCut = colorType == "DieCut";
}
```

### Line Width Detection
For spot colors, the system tracks the line width (stroke width) used:

```csharp
var graphicsState = renderInfo.GetGraphicsState();
float lineWidth = graphicsState.GetLineWidth();
float lineWidthMm = lineWidth / 72.0f * 25.4f;  // Convert to mm
```

## Database Schema

### SQLite Table Structure

```sql
CREATE TABLE IF NOT EXISTS SpotColors (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ColorName TEXT NOT NULL UNIQUE COLLATE NOCASE,
    ColorType TEXT NOT NULL,
    Description TEXT,
    PantoneNumber TEXT,
    DateAdded DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Index for fast lookups
CREATE INDEX IF NOT EXISTS idx_color_name ON SpotColors(ColorName COLLATE NOCASE);
CREATE INDEX IF NOT EXISTS idx_color_type ON SpotColors(ColorType);
```

### Pre-loaded Data Categories
- **Die-Cut Colors**: 50+ variations (CutContour, Stans, KissCutt, etc.)
- **Foil Colors**: Metallic and specialty foils
- **Varnish Colors**: UV and conventional varnishes
- **Registration Marks**: Registration color variations
- **Custom Colors**: User-added colors

## Export System

### JSON Serialization
Uses `Newtonsoft.Json` with custom formatting:

```csharp
var settings = new JsonSerializerSettings
{
    Formatting = Formatting.Indented,
    NullValueHandling = NullValueHandling.Ignore,
    DateFormatHandling = DateFormatHandling.IsoDateFormat
};
```

### XML Serialization
Uses `System.Xml.Serialization`:

```csharp
var serializer = new XmlSerializer(typeof(StructuredOutput.PdfAnalysis));
using var writer = new StreamWriter(outputPath, false, Encoding.UTF8);
serializer.Serialize(writer, analysisData);
```

### Structured Output Classes
Located in `StructuredOutput.cs`:
- `PdfAnalysis`: Root container
- `PdfInfo`: PDF metadata
- `BoxDimensions`: Dimension information
- `ColorAnalysis`: Color data container

## Performance Characteristics

### Benchmarks (Typical Files)
- **Small PDF (1-2MB)**: 0.3-0.5 seconds
- **Medium PDF (5-10MB)**: 0.7-1.0 seconds  
- **Large PDF (20MB+)**: 1.5-3.0 seconds
- **Complex Graphics**: +50-100% processing time

### Memory Usage
- **Base Application**: ~50MB
- **Per PDF Analysis**: +10-30MB (depending on complexity)
- **Database**: ~5MB (with full color database)

### Optimization Techniques
- **Single Page Analysis**: Only analyzes first page
- **Event Filtering**: Processes only relevant PDF events
- **Color Deduplication**: Removes duplicate color entries
- **Lazy Loading**: Database connections opened on demand
- **Memory Management**: Proper disposal of PDF resources

## Extension Points

### Adding New Color Types
1. **Update Database**: Add new color type to SpotColors table
2. **Update Enum**: Add to ColorType enumeration
3. **Update Logic**: Modify detection logic if needed

### Custom Event Listeners
Implement `IEventListener` for additional PDF content analysis:

```csharp
public class CustomListener : IEventListener
{
    public void EventOccurred(IEventData data, EventType type)
    {
        // Custom processing logic
    }
    
    public ICollection<EventType> GetSupportedEvents()
    {
        return new[] { EventType.RENDER_TEXT, EventType.RENDER_IMAGE };
    }
}
```

### Export Format Extensions
Add new export formats by:
1. Creating serialization classes
2. Implementing export methods in `PdfAnalysisResult`
3. Adding command-line options

## API Reference

### PdfAnalyzer Methods

#### `Analyze(string filePath)`
**Returns**: `PdfAnalysisResult`  
**Throws**: `FileNotFoundException`, `PdfException`

Performs complete PDF analysis including validation, dimension extraction, and color detection.

#### `AddSpotColor(string name, string type, string description)`
**Returns**: `void`  
**Throws**: `SQLiteException`

Adds a custom spot color to the database.

#### `GetAllSpotColors()`
**Returns**: `List<SpotColorInfo>`

Retrieves all spot colors from the database.

### PdfAnalysisResult Methods

#### `ToString()`
**Returns**: `string`

Formats analysis results for console display.

#### `ExportToJson(string outputPath)`
**Returns**: `string`  
**Throws**: `IOException`

Exports analysis results to JSON format.

#### `ExportToXml(string outputPath)`
**Returns**: `string`  
**Throws**: `IOException`

Exports analysis results to XML format.

### Extension Methods

#### `PointsToMm(float points)`
**Returns**: `double`

Converts points to millimeters using the formula: `mm = points / 72.0 * 25.4`

#### `PointsToInches(float points)`
**Returns**: `double`

Converts points to inches using the formula: `inches = points / 72.0`

---

For user-friendly documentation, see the [User Manual](USER_MANUAL.md).  
For API development plans, see the [API Roadmap](API_ROADMAP.md).