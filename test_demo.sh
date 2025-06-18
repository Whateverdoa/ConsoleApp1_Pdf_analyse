#!/bin/bash
# PPA-2.1 Demo Script

echo "=== PPA-2.1 Pre-Press Automaton Demo ==="
echo

# Show help
echo "1. Showing available commands:"
dotnet run -- --help
echo

# Add a custom Pantone color
echo "2. Adding custom Pantone 7548 C:"
dotnet run -- --add-pantone "7548 C" "Light Blue"
echo

# Add a custom die-cut name
echo "3. Adding custom die-cut name:"
dotnet run -- --add-color "LaserCut" DieCut "Laser cutting line"
echo

# List colors by type
echo "4. Showing some Pantone colors in database:"
dotnet run -- --list-colors | grep -A 10 "PANTONE Colors:" | head -15
echo

echo "5. To analyze a PDF, run:"
echo "   dotnet run -- /path/to/your/file.pdf"
echo
echo "The analyzer will detect any spot colors matching the 170+ entries in the database!"