using System;
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

public class PdfAnalyzer
{
    private readonly SpotColorDatabase _spotColorDatabase;

    public PdfAnalyzer()
    {
        _spotColorDatabase = new SpotColorDatabase();
    }

    public PdfAnalysisResult Analyze(string filePath)
    {
        var result = new PdfAnalysisResult { FilePath = filePath };

        try
        {
            using (var pdfReader = new PdfReader(filePath))
            using (var pdfDoc = new PdfDocument(pdfReader))
            {
                result.IsValidPdf = true;
                result.PdfVersion = pdfDoc.GetPdfVersion().ToString();
                result.PageCount = pdfDoc.GetNumberOfPages();

                if (pdfDoc.GetNumberOfPages() == 0)
                {
                    result.ErrorMessage = "PDF has no pages.";
                    return result;
                }

                var firstPage = pdfDoc.GetPage(1);

                // --- Task: Extract All PDF Box Dimensions ---
                
                // MediaBox (always present)
                var mediaBox = firstPage.GetMediaBox();
                if (mediaBox != null)
                {
                    result.MediaBoxWidthMm = PointsToMm(mediaBox.GetWidth());
                    result.MediaBoxHeightMm = PointsToMm(mediaBox.GetHeight());
                }

                // TrimBox (may not be present)
                var trimBox = firstPage.GetTrimBox();
                if (trimBox != null)
                {
                    result.HasTrimBox = true;
                    result.TrimBoxWidthMm = PointsToMm(trimBox.GetWidth());
                    result.TrimBoxHeightMm = PointsToMm(trimBox.GetHeight());
                }
                else
                {
                    // Use MediaBox as fallback for legacy compatibility
                    result.HasTrimBox = false;
                    result.TrimBoxWidthMm = result.MediaBoxWidthMm;
                    result.TrimBoxHeightMm = result.MediaBoxHeightMm;
                }

                // BleedBox (may not be present)
                var bleedBox = firstPage.GetBleedBox();
                if (bleedBox != null)
                {
                    result.HasBleedBox = true;
                    result.BleedBoxWidthMm = PointsToMm(bleedBox.GetWidth());
                    result.BleedBoxHeightMm = PointsToMm(bleedBox.GetHeight());
                }

                // CropBox (may not be present or same as MediaBox)
                var cropBox = firstPage.GetCropBox();
                if (cropBox != null && !cropBox.EqualsWithEpsilon(mediaBox))
                {
                    result.HasCropBox = true;
                    result.CropBoxWidthMm = PointsToMm(cropBox.GetWidth());
                    result.CropBoxHeightMm = PointsToMm(cropBox.GetHeight());
                }

                // --- Task: Find Colors and Special Spot Color Vector Lines ---
                var spotColorNames = _spotColorDatabase.GetAllSpotColorNames();
                var listener = new ColorListener(spotColorNames);
                var processor = new PdfCanvasProcessor(listener);
                processor.ProcessPageContent(firstPage);

                result.DieCutFound = listener.DieCutFound;
                result.FoundDieCutName = listener.FoundName;
                result.CmykColors = listener.CmykColors;
                result.RgbColors = listener.RgbColors;
                result.SpotColors = listener.SpotColors;
                result.SpotColorLineWidths = listener.SpotColorLineWidths;
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

    public void AddSpotColor(string colorName, string colorType, string description = "")
    {
        _spotColorDatabase.AddSpotColor(colorName, colorType, description);
    }

    public void AddPantoneColor(string pantoneNumber, string description = "")
    {
        _spotColorDatabase.AddPantoneColor(pantoneNumber, description);
    }

    public List<(string ColorName, string ColorType, string Description, string PantoneNumber)> GetAllSpotColors()
    {
        return _spotColorDatabase.GetAllSpotColorDetails();
    }

    public List<string> GetSpotColorsByType(string colorType)
    {
        return _spotColorDatabase.GetSpotColorsByType(colorType);
    }
}