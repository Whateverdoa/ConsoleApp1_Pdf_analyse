#!/bin/bash
cd /Users/mike10hm4/RiderProjects/ConsoleApp1_Pdf_analyse/ConsoleApp1_Pdf_analyse

echo "=== Analyzing all PDF files for spot colors ==="
echo

for pdf in ../*.PDF; do
    if [ -f "$pdf" ]; then
        echo "Analyzing: $(basename "$pdf")"
        dotnet run -- "$pdf" 2>/dev/null | grep -E "(TrimBox|Special Spot Color Found|Spot Color Name)"
        echo "---"
    fi
done