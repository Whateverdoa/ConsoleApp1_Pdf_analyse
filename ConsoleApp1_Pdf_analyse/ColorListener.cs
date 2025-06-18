using System;
using System.Collections.Generic;
using System.Linq;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Colorspace;

public class ColorListener : IEventListener
{
    private readonly ICollection<string> _spotColorNames;
    public bool DieCutFound { get; private set; } = false;
    public string FoundName { get; private set; } = string.Empty;
    
    // Store unique CMYK colors found
    private readonly HashSet<(float C, float M, float Y, float K)> _cmykColors = new();
    private readonly HashSet<(float R, float G, float B)> _rgbColors = new();
    private readonly HashSet<string> _spotColors = new();
    private readonly Dictionary<string, HashSet<float>> _spotColorLineWidths = new();
    
    public List<(float C, float M, float Y, float K)> CmykColors => _cmykColors.ToList();
    public List<(float R, float G, float B)> RgbColors => _rgbColors.ToList();
    public List<string> SpotColors => _spotColors.ToList();
    public Dictionary<string, List<float>> SpotColorLineWidths => 
        _spotColorLineWidths.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.OrderBy(w => w).ToList());

    public ColorListener(IEnumerable<string> spotColorNames)
    {
        _spotColorNames = new HashSet<string>(spotColorNames, StringComparer.OrdinalIgnoreCase);
    }

    public void EventOccurred(IEventData data, EventType type)
    {
        // We care about paths being drawn (stroked) and filled
        if (type != EventType.RENDER_PATH)
        {
            return;
        }

        var renderInfo = (PathRenderInfo)data;
        var gs = renderInfo.GetGraphicsState();
        
        // Check stroke color and width
        var strokeColor = gs.GetStrokeColor();
        if (strokeColor != null)
        {
            var lineWidth = gs.GetLineWidth();
            ProcessColor(strokeColor, "stroke", lineWidth);
        }
        
        // Check fill color
        var fillColor = gs.GetFillColor();
        if (fillColor != null)
        {
            ProcessColor(fillColor, "fill", 0);
        }
    }

    private void ProcessColor(Color color, string colorType, float lineWidth)
    {
        if (color == null) return;

        var colorSpace = color.GetColorSpace();
        
        // Handle Separation (Spot) colors
        if (colorSpace is PdfSpecialCs.Separation separationCs)
        {
            var pdfDict = separationCs.GetPdfObject();
            if (pdfDict is iText.Kernel.Pdf.PdfArray pdfArray && pdfArray.Size() > 1)
            {
                var nameObj = pdfArray.Get(1);
                if (nameObj is iText.Kernel.Pdf.PdfName pdfName)
                {
                    string spotColorName = pdfName.GetValue();
                    _spotColors.Add(spotColorName);
                    
                    // Track line width for stroked spot colors
                    if (colorType == "stroke" && lineWidth > 0)
                    {
                        if (!_spotColorLineWidths.ContainsKey(spotColorName))
                        {
                            _spotColorLineWidths[spotColorName] = new HashSet<float>();
                        }
                        _spotColorLineWidths[spotColorName].Add(lineWidth);
                    }
                    
                    // Check if this spot color is in our die-cut list
                    if (_spotColorNames.Contains(spotColorName))
                    {
                        DieCutFound = true;
                        FoundName = spotColorName;
                    }
                }
            }
        }
        // Handle CMYK colors
        else if (colorSpace is PdfDeviceCs.Cmyk)
        {
            var values = color.GetColorValue();
            if (values.Length >= 4)
            {
                var cmyk = (
                    C: (float)Math.Round(values[0] * 100, 1),  // Convert to percentage
                    M: (float)Math.Round(values[1] * 100, 1),
                    Y: (float)Math.Round(values[2] * 100, 1),
                    K: (float)Math.Round(values[3] * 100, 1)
                );
                _cmykColors.Add(cmyk);
            }
        }
        // Handle RGB colors
        else if (colorSpace is PdfDeviceCs.Rgb)
        {
            var values = color.GetColorValue();
            if (values.Length >= 3)
            {
                var rgb = (
                    R: (float)Math.Round(values[0] * 255, 0),  // Convert to 0-255 range
                    G: (float)Math.Round(values[1] * 255, 0),
                    B: (float)Math.Round(values[2] * 255, 0)
                );
                _rgbColors.Add(rgb);
            }
        }
        // Handle Gray colors (convert to CMYK)
        else if (colorSpace is PdfDeviceCs.Gray)
        {
            var values = color.GetColorValue();
            if (values.Length >= 1)
            {
                float gray = values[0];
                // Convert grayscale to CMYK (0,0,0,K)
                var cmyk = (
                    C: 0f,
                    M: 0f,
                    Y: 0f,
                    K: (float)Math.Round((1 - gray) * 100, 1)
                );
                _cmykColors.Add(cmyk);
            }
        }
    }

    public ICollection<EventType> GetSupportedEvents()
    {
        return new HashSet<EventType> { EventType.RENDER_PATH };
    }
}