using System;
using System.IO;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("PPA-2.1: Pre-Press Automaton Initialized.");
        
        var analyzer = new PdfAnalyzer();

        // Check for special commands
        if (args.Length > 0)
        {
            switch (args[0].ToLower())
            {
                case "--list-colors":
                    ListSpotColors(analyzer);
                    return;
                case "--add-color":
                    if (args.Length >= 3)
                    {
                        string description = args.Length > 3 ? args[3] : "";
                        analyzer.AddSpotColor(args[1], args[2], description);
                        Console.WriteLine($"Added spot color: {args[1]} (Type: {args[2]})");
                    }
                    else
                    {
                        Console.WriteLine("Usage: --add-color <ColorName> <ColorType> [Description]");
                    }
                    return;
                case "--add-pantone":
                    if (args.Length >= 2)
                    {
                        string description = args.Length > 2 ? args[2] : "";
                        analyzer.AddPantoneColor(args[1], description);
                        Console.WriteLine($"Added Pantone color: PANTONE {args[1]}");
                    }
                    else
                    {
                        Console.WriteLine("Usage: --add-pantone <PantoneNumber> [Description]");
                    }
                    return;
                case "--help":
                    ShowHelp();
                    return;
                case "--export-xml":
                case "--export-json":
                    if (args.Length >= 2)
                    {
                        string format = args[0] == "--export-xml" ? "xml" : "json";
                        string pdfPath = args[1];
                        ExportPdfAnalysis(pdfPath, format);
                    }
                    else
                    {
                        Console.WriteLine($"Usage: {args[0]} <pdf_file_path>");
                    }
                    return;
            }
        }

        Console.WriteLine("Awaiting file path for analysis...");

        string pdfFilePath;
        
        if (args.Length > 0 && !args[0].StartsWith("--"))
        {
            pdfFilePath = args[0];
        }
        else
        {
            Console.Write("\nEnter the path to your PDF file (or --help for commands): ");
            pdfFilePath = Console.ReadLine()?.Trim().Trim('"') ?? "";
        }

        if (string.IsNullOrWhiteSpace(pdfFilePath))
        {
            Console.WriteLine("\nERROR: No file path provided.");
            return;
        }

        if (!File.Exists(pdfFilePath))
        {
            Console.WriteLine($"\nERROR: File not found at '{pdfFilePath}'");
            return;
        }

        PdfAnalysisResult result = analyzer.Analyze(pdfFilePath);

        Console.WriteLine("\n--- MISSION COMPLETE ---");
        Console.WriteLine(result.ToString());
    }

    static void ExportPdfAnalysis(string pdfPath, string format)
    {
        if (!File.Exists(pdfPath))
        {
            Console.WriteLine($"ERROR: File not found at '{pdfPath}'");
            return;
        }

        var analyzer = new PdfAnalyzer();
        PdfAnalysisResult result = analyzer.Analyze(pdfPath);

        if (!result.IsValidPdf)
        {
            Console.WriteLine($"ERROR: Invalid PDF - {result.ErrorMessage}");
            return;
        }

        try
        {
            result.ExportToFile(format);
            var fileName = Path.GetFileNameWithoutExtension(pdfPath);
            var outputPath = Path.Combine(Path.GetDirectoryName(pdfPath) ?? ".", $"{fileName}.{format.ToLower()}");
            
            Console.WriteLine($"Analysis exported to: {outputPath}");
            Console.WriteLine($"Format: {format.ToUpper()}");
            
            // Show summary
            Console.WriteLine($"\nSummary:");
            Console.WriteLine($"  • PDF: {Path.GetFileName(pdfPath)}");
            Console.WriteLine($"  • Dimensions: {result.TrimBoxWidthMm:F1} x {result.TrimBoxHeightMm:F1} mm");
            Console.WriteLine($"  • CMYK Colors: {result.CmykColors.Count}");
            Console.WriteLine($"  • Spot Colors: {result.SpotColors.Count}");
            Console.WriteLine($"  • Die-Cut Found: {result.DieCutFound}");
            if (result.DieCutFound)
            {
                Console.WriteLine($"  • Die-Cut Color: {result.FoundDieCutName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: Failed to export - {ex.Message}");
        }
    }

    static void ListSpotColors(PdfAnalyzer analyzer)
    {
        Console.WriteLine("\n--- SPOT COLOR DATABASE ---");
        var colors = analyzer.GetAllSpotColors();
        
        var groupedColors = colors.GroupBy(c => c.ColorType).OrderBy(g => g.Key);
        
        foreach (var group in groupedColors)
        {
            Console.WriteLine($"\n{group.Key.ToUpper()} Colors:");
            foreach (var color in group.OrderBy(c => c.ColorName))
            {
                string display = color.ColorName;
                if (!string.IsNullOrEmpty(color.PantoneNumber))
                {
                    display += $" (Pantone: {color.PantoneNumber})";
                }
                if (!string.IsNullOrEmpty(color.Description))
                {
                    display += $" - {color.Description}";
                }
                Console.WriteLine($"  • {display}");
            }
        }
        
        Console.WriteLine($"\nTotal: {colors.Count} spot colors in database");
    }

    static void ShowHelp()
    {
        Console.WriteLine("\n--- PPA-2.1 COMMAND REFERENCE ---");
        Console.WriteLine("Usage: dotnet run [command|filepath]");
        Console.WriteLine("\nAnalysis Commands:");
        Console.WriteLine("  <filepath>                              Analyze PDF and show results");
        Console.WriteLine("  --export-xml <filepath>                 Analyze PDF and export to XML");
        Console.WriteLine("  --export-json <filepath>                Analyze PDF and export to JSON");
        Console.WriteLine("\nDatabase Commands:");
        Console.WriteLine("  --list-colors                          List all spot colors in database");
        Console.WriteLine("  --add-color <Name> <Type> [Description] Add a new spot color");
        Console.WriteLine("  --add-pantone <Number> [Description]    Add a new Pantone color");
        Console.WriteLine("  --help                                  Show this help");
        Console.WriteLine("\nColor Types:");
        Console.WriteLine("  DieCut, Foil, Emboss, Varnish, Registration, Perforation, Scoring, Cutout, Pantone");
        Console.WriteLine("\nExamples:");
        Console.WriteLine("  dotnet run myfile.pdf                   # Interactive analysis");
        Console.WriteLine("  dotnet run --export-xml myfile.pdf      # Export to myfile.xml");
        Console.WriteLine("  dotnet run --export-json myfile.pdf     # Export to myfile.json");
        Console.WriteLine("  dotnet run --add-color \"Custom Cut\" DieCut \"Custom die-cut line\"");
        Console.WriteLine("  dotnet run --add-pantone \"185 C\" \"Bright red Pantone color\"");
        Console.WriteLine("  dotnet run --list-colors");
        Console.WriteLine("\nExported files contain:");
        Console.WriteLine("  • Complete color analysis with CMYK/RGB values and interpretations");
        Console.WriteLine("  • Spot color details with line widths in points and millimeters"); 
        Console.WriteLine("  • Die-cut detection and analysis");
        Console.WriteLine("  • PDF dimensions in mm and inches");
        Console.WriteLine("  • Structured data for API/workflow integration");
    }
}