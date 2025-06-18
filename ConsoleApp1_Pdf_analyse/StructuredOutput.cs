using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

[XmlRoot("PdfAnalysis")]
public class StructuredPdfAnalysis
{
    [XmlElement("FileName")]
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = "";

    [XmlElement("FilePath")]
    [JsonPropertyName("filePath")]
    public string FilePath { get; set; } = "";

    [XmlElement("AnalysisDateTime")]
    [JsonPropertyName("analysisDateTime")]
    public DateTime AnalysisDateTime { get; set; }

    [XmlElement("IsValid")]
    [JsonPropertyName("isValid")]
    public bool IsValid { get; set; }

    [XmlElement("ErrorMessage")]
    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; set; } = "";

    [XmlElement("PdfInfo")]
    [JsonPropertyName("pdfInfo")]
    public PdfInfo PdfInfo { get; set; } = new();

    [XmlElement("Dimensions")]
    [JsonPropertyName("dimensions")]
    public PdfDimensions Dimensions { get; set; } = new();

    [XmlElement("Colors")]
    [JsonPropertyName("colors")]
    public ColorAnalysis Colors { get; set; } = new();

    [XmlElement("DieCut")]
    [JsonPropertyName("dieCut")]
    public DieCutAnalysis DieCut { get; set; } = new();
}

public class PdfInfo
{
    [XmlElement("Version")]
    [JsonPropertyName("version")]
    public string Version { get; set; } = "";

    [XmlElement("PageCount")]
    [JsonPropertyName("pageCount")]
    public int PageCount { get; set; }
}

public class PdfDimensions
{
    [XmlElement("MediaBox")]
    [JsonPropertyName("mediaBox")]
    public BoxDimensions MediaBox { get; set; } = new();

    [XmlElement("TrimBox")]
    [JsonPropertyName("trimBox")]
    public BoxDimensions? TrimBox { get; set; }

    [XmlElement("BleedBox")]
    [JsonPropertyName("bleedBox")]
    public BoxDimensions? BleedBox { get; set; }

    [XmlElement("CropBox")]
    [JsonPropertyName("cropBox")]
    public BoxDimensions? CropBox { get; set; }
}

public class BoxDimensions
{
    [XmlElement("WidthMm")]
    [JsonPropertyName("widthMm")]
    public double WidthMm { get; set; }

    [XmlElement("HeightMm")]
    [JsonPropertyName("heightMm")]
    public double HeightMm { get; set; }

    [XmlElement("WidthInches")]
    [JsonPropertyName("widthInches")]
    public double WidthInches { get; set; }

    [XmlElement("HeightInches")]
    [JsonPropertyName("heightInches")]
    public double HeightInches { get; set; }

    [XmlElement("WidthPoints")]
    [JsonPropertyName("widthPoints")]
    public double WidthPoints { get; set; }

    [XmlElement("HeightPoints")]
    [JsonPropertyName("heightPoints")]
    public double HeightPoints { get; set; }
}

public class ColorAnalysis
{
    [XmlArray("CmykColors")]
    [XmlArrayItem("CmykColor")]
    [JsonPropertyName("cmykColors")]
    public List<CmykColorInfo> CmykColors { get; set; } = new();

    [XmlArray("RgbColors")]
    [XmlArrayItem("RgbColor")]
    [JsonPropertyName("rgbColors")]
    public List<RgbColorInfo> RgbColors { get; set; } = new();

    [XmlArray("SpotColors")]
    [XmlArrayItem("SpotColor")]
    [JsonPropertyName("spotColors")]
    public List<SpotColorInfo> SpotColors { get; set; } = new();
}

public class CmykColorInfo
{
    [XmlElement("C")]
    [JsonPropertyName("c")]
    public float C { get; set; }

    [XmlElement("M")]
    [JsonPropertyName("m")]
    public float M { get; set; }

    [XmlElement("Y")]
    [JsonPropertyName("y")]
    public float Y { get; set; }

    [XmlElement("K")]
    [JsonPropertyName("k")]
    public float K { get; set; }

    [XmlElement("ColorName")]
    [JsonPropertyName("colorName")]
    public string ColorName { get; set; } = "";

    [XmlElement("CmykString")]
    [JsonPropertyName("cmykString")]
    public string CmykString { get; set; } = "";
}

public class RgbColorInfo
{
    [XmlElement("R")]
    [JsonPropertyName("r")]
    public float R { get; set; }

    [XmlElement("G")]
    [JsonPropertyName("g")]
    public float G { get; set; }

    [XmlElement("B")]
    [JsonPropertyName("b")]
    public float B { get; set; }

    [XmlElement("ColorName")]
    [JsonPropertyName("colorName")]
    public string ColorName { get; set; } = "";

    [XmlElement("HexValue")]
    [JsonPropertyName("hexValue")]
    public string HexValue { get; set; } = "";
}

public class SpotColorInfo
{
    [XmlElement("Name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [XmlElement("Type")]
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [XmlElement("IsDieCut")]
    [JsonPropertyName("isDieCut")]
    public bool IsDieCut { get; set; }

    [XmlArray("LineWidths")]
    [XmlArrayItem("Width")]
    [JsonPropertyName("lineWidths")]
    public List<LineWidthInfo> LineWidths { get; set; } = new();
}

public class LineWidthInfo
{
    [XmlElement("Points")]
    [JsonPropertyName("points")]
    public float Points { get; set; }

    [XmlElement("Millimeters")]
    [JsonPropertyName("millimeters")]
    public float Millimeters { get; set; }
}

public class DieCutAnalysis
{
    [XmlElement("Found")]
    [JsonPropertyName("found")]
    public bool Found { get; set; }

    [XmlElement("ColorName")]
    [JsonPropertyName("colorName")]
    public string ColorName { get; set; } = "";

    [XmlElement("LineWidthPoints")]
    [JsonPropertyName("lineWidthPoints")]
    public float LineWidthPoints { get; set; }

    [XmlElement("LineWidthMm")]
    [JsonPropertyName("lineWidthMm")]
    public float LineWidthMm { get; set; }

    [XmlArray("AllDieCutColors")]
    [XmlArrayItem("DieCutColor")]
    [JsonPropertyName("allDieCutColors")]
    public List<string> AllDieCutColors { get; set; } = new();
}