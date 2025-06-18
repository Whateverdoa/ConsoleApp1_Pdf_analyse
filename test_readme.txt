PPA-2.1 Test Instructions
========================

To test the PDF analyzer with spot colors:

1. Basic PDF Analysis:
   dotnet run -- /path/to/your/file.pdf

2. The analyzer will report:
   - PDF validity
   - TrimBox dimensions in mm
   - Any special spot colors found (die-cuts, Pantone colors, etc.)

3. Expected output for a PDF with spot colors:
   --- Analysis for: sample.pdf ---
   PDF Valid: True
   TrimBox (WxH): 210.00 mm x 297.00 mm
   Special Spot Color Found: True
   Spot Color Name: CutContour

4. View all spot colors in database:
   dotnet run -- --list-colors

5. Add custom colors before analysis:
   dotnet run -- --add-color "MyCustomDieCut" DieCut
   dotnet run -- --add-pantone "185 C"

The analyzer will detect:
- Die-cut lines (CutContour, Stans, etc.)
- Pantone colors
- Special finishes (foil, emboss, varnish, UV)
- Registration marks
- Perforation and scoring lines

Note: The PDF must use actual spot colors (Separation color space) 
for detection to work. Process colors (CMYK) won't be detected.