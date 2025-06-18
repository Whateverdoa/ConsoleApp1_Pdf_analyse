Of course. Let's define the persona for your coding agent and then lay out a detailed to-do list with accompanying C# code using iText7.

---

### Persona: Pre-Press Automaton v2.1 (PPA-2.1)

**Designation:** PPA-2.1 (Codename: "Gutenberg")

**Core Directive:** To meticulously analyze and validate digital print-ready files, ensuring they meet production specifications with machinelike precision.

**Personality & Style:**
*   **Precise & Factual:** Communicates in clear, unambiguous terms. Avoids jargon where possible but uses technical terms correctly when necessary.
*   **Efficient:** Focuses on the most direct path to a solution. Provides clean, commented, and reusable code.
*   **Proactive:** Anticipates potential issues (e.g., file corruption, missing page boxes, different ways die-cuts are defined) and builds in checks to handle them gracefully.
*   **Structured:** Organizes tasks and results in a logical, easy-to-follow format. Prefers lists, structured data, and clear "Pass/Fail" outputs.

**Motto:** "Measure twice, cut once, validate always."

---

### PPA-2.1: Mission To-Do List

**Mission:** Analyze a given PDF file to extract critical pre-press information.

| **#** | **Task**                                                               | **Status** | **Details & iText7 Implementation Strategy**                                                                                                                                                             |
| :-- | :--------------------------------------------------------------------- | :--------- | :----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1   | **Initialize & Validate Environment**                                  | `PENDING`  | Ensure the C# project is set up. Add the `iText7` NuGet package. Define a list of known die-cut names (`CutContour`, `Stans`, `KissCutt`, `Die-Cut`, etc.).                                          |
| 2   | **Verify PDF Integrity**                                               | `PENDING`  | Attempt to open the file using `new PdfReader(filePath)`. If this constructor throws an `iText.Kernel.Exceptions.PdfException` or `IOException`, the file is not a valid/readable PDF. This is the primary validation step. |
| 3   | **Extract TrimBox Dimensions**                                         | `PENDING`  | 1. Get the first page: `pdfDoc.GetPage(1)`. <br>2. Get the TrimBox: `page.GetTrimBox()`. <br>3. Handle cases where TrimBox is null (fall back to MediaBox). <br>4. Convert width/height from points to millimeters. **Formula: `mm = points / 72 * 25.4`**. |
| 4   | **Design Vector Line Detector**                                        | `PENDING`  | Create a custom class `DieCutListener` that implements `iText.Kernel.Pdf.Canvas.Parser.IEventListener`. This listener will inspect all drawing operations on the page.                       |
| 5   | **Implement Detector Logic**                                           | `PENDING`  | Inside the `DieCutListener`'s `EventOccurred` method: <br>1. Check if the event is a path being drawn (`EventType.RENDER_PATH`).<br>2. Get the current graphics state: `data.GetGraphicsState()`.<br>3. Get the stroke color: `gs.GetStrokeColor()`.<br>4. Check if the color is a "Separation" (spot) color: `color.GetColorSpace() is PdfSpecialCs.Separation`.<br>5. Extract the spot color name and check it against our list of die-cut names. |
| 6   | **Process the PDF and Report**                                         | `PENDING`  | 1. Create a main analysis method that takes a file path. <br>2. Instantiate the `DieCutListener`.<br>3. Create a `PdfCanvasProcessor` with the listener.<br>4. Process the page content: `processor.ProcessPageContent(page)`.<br>5. Collate all findings (Integrity, Dimensions, Die-Cut Found) into a simple result object. |

---

### C# Code Implementation

Here is the complete C# code to accomplish the mission.

#### Prerequisites

1.  Create a new C# Console Application.
2.  Install the iText7 NuGet package:
    ```sh
    dotnet add package itext7
    ```

#### Step 1: Create a Result Class

This class will hold our findings in a structured way.

**PdfAnalysisResult.cs**
```csharp
public class PdfAnalysisResult
{
    public string FilePath { get; set; }
    public bool IsValidPdf { get; set; }
    public string ErrorMessage { get; set; }
    public double TrimBoxWidthMm { get; set; }
    public double TrimBoxHeightMm { get; set; }
    public bool DieCutFound { get; set; }
    public string FoundDieCutName { get; set; }

    public override string ToString()
    {
        if (!IsValidPdf)
        {
            return $"File: {FilePath}\n" +
                   $"Status: INVALID PDF\n" +
                   $"Error: {ErrorMessage}";
        }

        return $"--- Analysis for: {System.IO.Path.GetFileName(FilePath)} ---\n" +
               $"PDF Valid: {IsValidPdf}\n" +
               $"TrimBox (WxH): {TrimBoxWidthMm:F2} mm x {TrimBoxHeightMm:F2} mm\n" +
               $"Die-Cut Found: {DieCutFound}\n" +
               (DieCutFound ? $"Die-Cut Name: {FoundDieCutName}" : "");
    }
}
```

#### Step 2: Implement the `DieCutListener`

This is the core of the vector line detection. It listens for drawing events and checks the name of any spot colors used.

**DieCutListener.cs**
```csharp
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Colorspace;
using System.Collections.Generic;

public class DieCutListener : IEventListener
{
    private readonly ICollection<string> _dieCutNames;
    public bool DieCutFound { get; private set; } = false;
    public string FoundName { get; private set; } = string.Empty;

    // We use a HashSet for fast, case-insensitive lookups.
    public DieCutListener(IEnumerable<string> dieCutNames)
    {
        _dieCutNames = new HashSet<string>(dieCutNames, StringComparer.OrdinalIgnoreCase);
    }

    public void EventOccurred(IEventData data, EventType type)
    {
        // We only care about paths being drawn (stroked)
        if (type != EventType.RENDER_PATH)
        {
            return;
        }

        // PathRenderInfo gives us access to the graphics state
        var renderInfo = (PathRenderInfo)data;
        var gs = renderInfo.GetGraphicsState();
        var strokeColor = gs.GetStrokeColor();

        // Check if the color exists and is a special color space (like a spot color)
        if (strokeColor != null && strokeColor.GetColorSpace() is PdfSpecialCs.Separation separationCs)
        {
            // Get the name of the spot color
            string colorantName = separationCs.GetColorantName().GetValue();
            
            // Check if this spot color name is in our list of die-cut names
            if (_dieCutNames.Contains(colorantName))
            {
                DieCutFound = true;
                FoundName = colorantName;
            }
        }
    }

    public ICollection<EventType> GetSupportedEvents()
    {
        // We only need to listen for RENDER_PATH events
        return new HashSet<EventType> { EventType.RENDER_PATH };
    }
}
```

#### Step 3: Create the Main Analyzer Class

This class ties everything together.

**PdfAnalyzer.cs**
```csharp
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Collections.Generic;

public class PdfAnalyzer
{
    // Define the known names for die-cut spot colors
    private static readonly List<string> DieCutNames = new List<string>
    {
        "CutContour",
        "Stans",
        "KissCutt",
        "Die-Cut",
        "DieCut",
        "Thru-cut"
    };

    public PdfAnalysisResult Analyze(string filePath)
    {
        var result = new PdfAnalysisResult { FilePath = filePath };

        try
        {
            using (var pdfReader = new PdfReader(filePath))
            using (var pdfDoc = new PdfDocument(pdfReader))
            {
                result.IsValidPdf = true;

                if (pdfDoc.GetNumberOfPages() == 0)
                {
                    result.ErrorMessage = "PDF has no pages.";
                    return result;
                }

                var firstPage = pdfDoc.GetPage(1);

                // --- Task: Extract TrimBox Dimensions ---
                Rectangle box = firstPage.GetTrimBox() ?? firstPage.GetMediaBox(); // Fallback to MediaBox
                if (box != null)
                {
                    result.TrimBoxWidthMm = PointsToMm(box.GetWidth());
                    result.TrimBoxHeightMm = PointsToMm(box.GetHeight());
                }

                // --- Task: Find Die-Cut Vector Lines ---
                var listener = new DieCutListener(DieCutNames);
                var processor = new PdfCanvasProcessor(listener);
                processor.ProcessPageContent(firstPage);

                result.DieCutFound = listener.DieCutFound;
                result.FoundDieCutName = listener.FoundName;
            }
        }
        catch (iText.Kernel.Exceptions.PdfException ex)
        {
            // This catches invalid or corrupted PDF files
            result.IsValidPdf = false;
            result.ErrorMessage = $"Not a valid PDF file. iText Error: {ex.Message}";
        }
        catch (Exception ex)
        {
            // Catches other errors like file not found
            result.IsValidPdf = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    private double PointsToMm(float points)
    {
        // 1 point = 1/72 inch
        // 1 inch = 25.4 mm
        return points / 72.0 * 25.4;
    }
}
```

#### Step 4: Put it all together in `Program.cs`

**Program.cs**
```csharp
public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("PPA-2.1: Pre-Press Automaton Initialized.");
        Console.WriteLine("Awaiting file path for analysis...");

        // NOTE: Replace this with the actual path to your PDF file.
        // You might have one PDF with a die-cut and one without to test both cases.
        // Also, try a non-PDF file (like a .txt or .jpg) to test the validation.
        string pdfFilePath = @"C:\Path\To\Your\File.pdf"; 

        if (!File.Exists(pdfFilePath))
        {
            Console.WriteLine($"\nERROR: File not found at '{pdfFilePath}'");
            return;
        }

        var analyzer = new PdfAnalyzer();
        PdfAnalysisResult result = analyzer.Analyze(pdfFilePath);

        Console.WriteLine("\n--- MISSION COMPLETE ---");
        Console.WriteLine(result.ToString());
    }
}
```