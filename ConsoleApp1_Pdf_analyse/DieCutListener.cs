using System;
using System.Collections.Generic;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Colorspace;

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
            // Get the separation color space dictionary and extract the colorant name
            var pdfDict = separationCs.GetPdfObject();
            if (pdfDict is iText.Kernel.Pdf.PdfArray pdfArray && pdfArray.Size() > 1)
            {
                var nameObj = pdfArray.Get(1);
                if (nameObj is iText.Kernel.Pdf.PdfName pdfName)
                {
                    string spotColorName = pdfName.GetValue();
                    
                    // Check if this spot color name is in our list of die-cut names
                    if (_dieCutNames.Contains(spotColorName))
                    {
                        DieCutFound = true;
                        FoundName = spotColorName;
                    }
                }
            }
        }
    }

    public ICollection<EventType> GetSupportedEvents()
    {
        // We only need to listen for RENDER_PATH events
        return new HashSet<EventType> { EventType.RENDER_PATH };
    }
}