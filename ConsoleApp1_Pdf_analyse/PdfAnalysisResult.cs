using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

public class PdfAnalysisResult
{
    public string FilePath { get; set; } = "";
    public bool IsValidPdf { get; set; }
    public string ErrorMessage { get; set; } = "";
    public string PdfVersion { get; set; } = "";
    public int PageCount { get; set; }
    
    // PDF Box dimensions (all in mm)
    public double TrimBoxWidthMm { get; set; }
    public double TrimBoxHeightMm { get; set; }
    public double BleedBoxWidthMm { get; set; }
    public double BleedBoxHeightMm { get; set; }
    public double MediaBoxWidthMm { get; set; }
    public double MediaBoxHeightMm { get; set; }
    public double CropBoxWidthMm { get; set; }
    public double CropBoxHeightMm { get; set; }
    
    // Box existence flags
    public bool HasTrimBox { get; set; }
    public bool HasBleedBox { get; set; }
    public bool HasCropBox { get; set; }
    
    public bool DieCutFound { get; set; }
    public string FoundDieCutName { get; set; } = "";
    
    // Color information
    public List<(float C, float M, float Y, float K)> CmykColors { get; set; } = new();
    public List<(float R, float G, float B)> RgbColors { get; set; } = new();
    public List<string> SpotColors { get; set; } = new();
    public Dictionary<string, List<float>> SpotColorLineWidths { get; set; } = new();

    public override string ToString()
    {
        if (!IsValidPdf)
        {
            return $"File: {FilePath}\n" +
                   $"Status: INVALID PDF\n" +
                   $"Error: {ErrorMessage}";
        }

        var result = $"--- Analysis for: {System.IO.Path.GetFileName(FilePath)} ---\n" +
                     $"PDF Valid: {IsValidPdf}\n" +
                     $"PDF Version: {PdfVersion}\n" +
                     $"Page Count: {PageCount}\n\n" +
                     $"PDF Boxes:\n";

        // Show MediaBox (always present)
        result += $"  • MediaBox: {MediaBoxWidthMm:F2} mm x {MediaBoxHeightMm:F2} mm\n";
        
        // Show TrimBox if present, otherwise indicate it's using MediaBox
        if (HasTrimBox)
        {
            result += $"  • TrimBox: {TrimBoxWidthMm:F2} mm x {TrimBoxHeightMm:F2} mm\n";
        }
        else
        {
            result += $"  • TrimBox: Not defined (using MediaBox)\n";
        }
        
        // Show BleedBox if present
        if (HasBleedBox)
        {
            result += $"  • BleedBox: {BleedBoxWidthMm:F2} mm x {BleedBoxHeightMm:F2} mm\n";
        }
        else
        {
            result += $"  • BleedBox: Not defined\n";
        }
        
        // Show CropBox if different from MediaBox
        if (HasCropBox)
        {
            result += $"  • CropBox: {CropBoxWidthMm:F2} mm x {CropBoxHeightMm:F2} mm\n";
        }

        // CMYK Colors
        if (CmykColors.Any())
        {
            result += $"\nCMYK Colors Found ({CmykColors.Count}):\n";
            foreach (var cmyk in CmykColors.OrderBy(c => c.K).ThenBy(c => c.C).ThenBy(c => c.M).ThenBy(c => c.Y))
            {
                result += $"  • C:{cmyk.C}% M:{cmyk.M}% Y:{cmyk.Y}% K:{cmyk.K}%\n";
            }
        }

        // RGB Colors
        if (RgbColors.Any())
        {
            result += $"\nRGB Colors Found ({RgbColors.Count}):\n";
            foreach (var rgb in RgbColors.OrderBy(c => c.R).ThenBy(c => c.G).ThenBy(c => c.B))
            {
                result += $"  • R:{rgb.R} G:{rgb.G} B:{rgb.B}\n";
            }
        }

        // Spot Colors
        if (SpotColors.Any())
        {
            result += $"\nSpot Colors Found ({SpotColors.Count}):\n";
            foreach (var spot in SpotColors.OrderBy(s => s))
            {
                result += $"  • {spot}";
                
                // Add line width info if available
                if (SpotColorLineWidths.ContainsKey(spot) && SpotColorLineWidths[spot].Any())
                {
                    var widths = SpotColorLineWidths[spot];
                    if (widths.Count == 1)
                    {
                        result += $" (Line width: {widths[0]:F2} pt)";
                    }
                    else
                    {
                        result += $" (Line widths: {string.Join(", ", widths.Select(w => $"{w:F2} pt"))})";
                    }
                }
                
                if (DieCutFound && spot == FoundDieCutName)
                {
                    result += " [DIE-CUT DETECTED]";
                }
                result += "\n";
            }
        }

        // Legacy output for backward compatibility
        if (!SpotColors.Any())
        {
            result += $"\nSpecial Spot Color Found: {DieCutFound}\n";
            if (DieCutFound)
            {
                result += $"Spot Color Name: {FoundDieCutName}";
            }
        }

        return result;
    }

    public StructuredPdfAnalysis ToStructuredOutput()
    {
        var structured = new StructuredPdfAnalysis
        {
            FileName = Path.GetFileName(FilePath),
            FilePath = FilePath,
            AnalysisDateTime = DateTime.Now,
            IsValid = IsValidPdf,
            ErrorMessage = ErrorMessage
        };

        if (IsValidPdf)
        {
            // PDF Info
            structured.PdfInfo = new PdfInfo
            {
                Version = PdfVersion,
                PageCount = PageCount
            };

            // Dimensions - MediaBox (always present)
            structured.Dimensions.MediaBox = new BoxDimensions
            {
                WidthMm = MediaBoxWidthMm,
                HeightMm = MediaBoxHeightMm,
                WidthInches = Math.Round(MediaBoxWidthMm / 25.4, 3),
                HeightInches = Math.Round(MediaBoxHeightMm / 25.4, 3),
                WidthPoints = Math.Round(MediaBoxWidthMm * 72.0 / 25.4, 2),
                HeightPoints = Math.Round(MediaBoxHeightMm * 72.0 / 25.4, 2)
            };

            // TrimBox (if present)
            if (HasTrimBox)
            {
                structured.Dimensions.TrimBox = new BoxDimensions
                {
                    WidthMm = TrimBoxWidthMm,
                    HeightMm = TrimBoxHeightMm,
                    WidthInches = Math.Round(TrimBoxWidthMm / 25.4, 3),
                    HeightInches = Math.Round(TrimBoxHeightMm / 25.4, 3),
                    WidthPoints = Math.Round(TrimBoxWidthMm * 72.0 / 25.4, 2),
                    HeightPoints = Math.Round(TrimBoxHeightMm * 72.0 / 25.4, 2)
                };
            }

            // BleedBox (if present)
            if (HasBleedBox)
            {
                structured.Dimensions.BleedBox = new BoxDimensions
                {
                    WidthMm = BleedBoxWidthMm,
                    HeightMm = BleedBoxHeightMm,
                    WidthInches = Math.Round(BleedBoxWidthMm / 25.4, 3),
                    HeightInches = Math.Round(BleedBoxHeightMm / 25.4, 3),
                    WidthPoints = Math.Round(BleedBoxWidthMm * 72.0 / 25.4, 2),
                    HeightPoints = Math.Round(BleedBoxHeightMm * 72.0 / 25.4, 2)
                };
            }

            // CropBox (if present and different from MediaBox)
            if (HasCropBox)
            {
                structured.Dimensions.CropBox = new BoxDimensions
                {
                    WidthMm = CropBoxWidthMm,
                    HeightMm = CropBoxHeightMm,
                    WidthInches = Math.Round(CropBoxWidthMm / 25.4, 3),
                    HeightInches = Math.Round(CropBoxHeightMm / 25.4, 3),
                    WidthPoints = Math.Round(CropBoxWidthMm * 72.0 / 25.4, 2),
                    HeightPoints = Math.Round(CropBoxHeightMm * 72.0 / 25.4, 2)
                };
            }

            // CMYK Colors
            structured.Colors.CmykColors = CmykColors.Select(cmyk => new CmykColorInfo
            {
                C = cmyk.C,
                M = cmyk.M,
                Y = cmyk.Y,
                K = cmyk.K,
                ColorName = ColorInterpreter.InterpretCmykColor(cmyk.C, cmyk.M, cmyk.Y, cmyk.K),
                CmykString = $"C{cmyk.C:F0}M{cmyk.M:F0}Y{cmyk.Y:F0}K{cmyk.K:F0}"
            }).ToList();

            // RGB Colors
            structured.Colors.RgbColors = RgbColors.Select(rgb => new RgbColorInfo
            {
                R = rgb.R,
                G = rgb.G,
                B = rgb.B,
                ColorName = ColorInterpreter.InterpretRgbColor(rgb.R, rgb.G, rgb.B),
                HexValue = $"#{(int)rgb.R:X2}{(int)rgb.G:X2}{(int)rgb.B:X2}"
            }).ToList();

            // Spot Colors
            structured.Colors.SpotColors = SpotColors.Select(spot => new SpotColorInfo
            {
                Name = spot,
                Type = GetSpotColorType(spot),
                IsDieCut = DieCutFound && spot == FoundDieCutName,
                LineWidths = SpotColorLineWidths.ContainsKey(spot) 
                    ? SpotColorLineWidths[spot].Select(width => new LineWidthInfo
                    {
                        Points = width,
                        Millimeters = (float)Math.Round(width * 0.352778, 3) // 1 pt = 0.352778 mm
                    }).ToList()
                    : new List<LineWidthInfo>()
            }).ToList();

            // Die-Cut Analysis
            structured.DieCut = new DieCutAnalysis
            {
                Found = DieCutFound,
                ColorName = FoundDieCutName,
                LineWidthPoints = DieCutFound && SpotColorLineWidths.ContainsKey(FoundDieCutName) 
                    ? SpotColorLineWidths[FoundDieCutName].FirstOrDefault() 
                    : 0,
                LineWidthMm = DieCutFound && SpotColorLineWidths.ContainsKey(FoundDieCutName) 
                    ? (float)Math.Round(SpotColorLineWidths[FoundDieCutName].FirstOrDefault() * 0.352778, 3)
                    : 0,
                AllDieCutColors = SpotColors.Where(spot => 
                    spot.Contains("Cut", StringComparison.OrdinalIgnoreCase) || 
                    spot.Contains("Stans", StringComparison.OrdinalIgnoreCase) ||
                    spot.Contains("Die", StringComparison.OrdinalIgnoreCase)).ToList()
            };
        }

        return structured;
    }

    public string ExportToJson(string? outputPath = null)
    {
        var structured = ToStructuredOutput();
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        var json = JsonSerializer.Serialize(structured, options);
        
        if (!string.IsNullOrEmpty(outputPath))
        {
            File.WriteAllText(outputPath, json);
        }
        
        return json;
    }

    public string ExportToXml(string? outputPath = null)
    {
        var structured = ToStructuredOutput();
        var serializer = new XmlSerializer(typeof(StructuredPdfAnalysis));
        
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings 
        { 
            Indent = true,
            IndentChars = "  "
        });
        
        serializer.Serialize(xmlWriter, structured);
        var xml = stringWriter.ToString();
        
        if (!string.IsNullOrEmpty(outputPath))
        {
            File.WriteAllText(outputPath, xml);
        }
        
        return xml;
    }

    public void ExportToFile(string format = "xml")
    {
        var fileName = Path.GetFileNameWithoutExtension(FilePath);
        var outputPath = Path.Combine(Path.GetDirectoryName(FilePath) ?? ".", $"{fileName}.{format.ToLower()}");
        
        switch (format.ToLower())
        {
            case "json":
                ExportToJson(outputPath);
                break;
            case "xml":
            default:
                ExportToXml(outputPath);
                break;
        }
    }

    private string GetSpotColorType(string spotColorName)
    {
        var name = spotColorName.ToLower();
        
        if (name.Contains("cut") || name.Contains("stans") || name.Contains("die"))
            return "DieCut";
        if (name.Contains("pantone"))
            return "Pantone";
        if (name.Contains("foil") || name.Contains("metallic"))
            return "Foil";
        if (name.Contains("emboss") || name.Contains("deboss"))
            return "Emboss";
        if (name.Contains("varnish") || name.Contains("uv") || name.Contains("gloss"))
            return "Varnish";
        if (name.Contains("registration") || name.Contains("crop") || name.Contains("trim"))
            return "Registration";
        if (name.Contains("perf"))
            return "Perforation";
        if (name.Contains("score") || name.Contains("crease"))
            return "Scoring";
        
        return "Unknown";
    }
}