# Pre-Press Automaton v2.1 - Installation Guide

## Table of Contents
1. [System Requirements](#system-requirements)
2. [Installation Methods](#installation-methods)
3. [Platform-Specific Instructions](#platform-specific-instructions)
4. [Verification](#verification)
5. [Configuration](#configuration)
6. [Troubleshooting](#troubleshooting)
7. [Uninstallation](#uninstallation)

## System Requirements

### Minimum Requirements
- **Operating System**: 
  - Windows 10 (1809) or later
  - macOS 10.15 (Catalina) or later  
  - Linux (Ubuntu 18.04, CentOS 7, or equivalent)
- **.NET Runtime**: Version 9.0 or later
- **Memory**: 512MB RAM available
- **Storage**: 50MB free disk space
- **Network**: Internet connection for initial setup (package downloads)

### Recommended Requirements
- **Memory**: 1GB RAM or more
- **Storage**: 100MB free disk space
- **CPU**: Multi-core processor for better performance
- **SSD**: Solid-state drive for faster file I/O

### Dependencies
The following packages are automatically installed via NuGet:
- `itext7` (v9.2.0) - PDF processing library
- `Microsoft.Data.Sqlite` (v9.0.6) - Database functionality
- `Newtonsoft.Json` (v13.0.3) - JSON serialization

## Installation Methods

### Method 1: Git Clone (Recommended)
Best for developers and users who want the latest version.

```bash
# Clone the repository
git clone https://github.com/Whateverdoa/ConsoleApp1_Pdf_analyse.git

# Navigate to the project directory
cd ConsoleApp1_Pdf_analyse/ConsoleApp1_Pdf_analyse

# Restore NuGet packages
dotnet restore

# Build the application
dotnet build

# Verify installation
dotnet run -- --help
```

### Method 2: Download ZIP Archive
Best for users who prefer not to use Git.

1. **Download**: Visit https://github.com/Whateverdoa/ConsoleApp1_Pdf_analyse
2. **Extract**: Click "Code" → "Download ZIP" and extract to your desired location
3. **Build**: Follow the same `dotnet restore` and `dotnet build` steps as above

### Method 3: Pre-compiled Binaries
*(Available in future releases)*

Download platform-specific executables that don't require .NET SDK installation.

## Platform-Specific Instructions

### Windows

#### Prerequisites
1. **Install .NET 9.0 Runtime or SDK**:
   - Download from: https://dotnet.microsoft.com/download/dotnet/9.0
   - Run the installer and follow the wizard
   - Choose "ASP.NET Core Runtime" for runtime-only installation
   - Choose ".NET SDK" for development installation

2. **Verify .NET Installation**:
   ```cmd
   dotnet --version
   ```
   Should display version 9.0.x or later.

#### Installation Steps
```cmd
# Open Command Prompt or PowerShell
git clone https://github.com/Whateverdoa/ConsoleApp1_Pdf_analyse.git
cd ConsoleApp1_Pdf_analyse\ConsoleApp1_Pdf_analyse
dotnet restore
dotnet build
```

#### Windows-Specific Notes
- Use Command Prompt, PowerShell, or Windows Terminal
- File paths should use backslashes (`\`) or be quoted
- Windows Defender may scan large PDF files, affecting performance

### macOS

#### Prerequisites
1. **Install .NET 9.0**:
   ```bash
   # Using Homebrew (recommended)
   brew install dotnet

   # Or download from Microsoft
   # Visit: https://dotnet.microsoft.com/download/dotnet/9.0
   ```

2. **Install Git** (if not already installed):
   ```bash
   # Using Homebrew
   brew install git
   
   # Or use Xcode Command Line Tools
   xcode-select --install
   ```

#### Installation Steps
```bash
# Open Terminal
git clone https://github.com/Whateverdoa/ConsoleApp1_Pdf_analyse.git
cd ConsoleApp1_Pdf_analyse/ConsoleApp1_Pdf_analyse
dotnet restore
dotnet build
```

#### macOS-Specific Notes
- May need to allow app execution in Security & Privacy settings
- Use Terminal or iTerm2 for command-line access
- File paths are case-sensitive

### Linux (Ubuntu/Debian)

#### Prerequisites
1. **Update package manager**:
   ```bash
   sudo apt update
   ```

2. **Install .NET 9.0**:
   ```bash
   # Add Microsoft package repository
   wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   sudo dpkg -i packages-microsoft-prod.deb
   
   # Install .NET SDK
   sudo apt update
   sudo apt install -y dotnet-sdk-9.0
   ```

3. **Install Git**:
   ```bash
   sudo apt install git
   ```

#### Installation Steps
```bash
git clone https://github.com/Whateverdoa/ConsoleApp1_Pdf_analyse.git
cd ConsoleApp1_Pdf_analyse/ConsoleApp1_Pdf_analyse
dotnet restore
dotnet build
```

### Linux (CentOS/RHEL/Fedora)

#### Prerequisites
```bash
# CentOS/RHEL
sudo yum install git
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
sudo yum install dotnet-sdk-9.0

# Fedora
sudo dnf install git
sudo rpm --import https://packages.microsoft.com/keys/microsoft.asc
sudo dnf install dotnet-sdk-9.0
```

## Verification

### Basic Functionality Test
```bash
# Navigate to project directory
cd ConsoleApp1_Pdf_analyse/ConsoleApp1_Pdf_analyse

# Test help command
dotnet run -- --help

# Test color database
dotnet run -- --list-colors

# Test with a sample PDF (if available)
dotnet run -- path/to/sample.pdf
```

### Expected Output
```
PPA-2.1: Pre-Press Automaton Initialized.

--- PPA-2.1 COMMAND REFERENCE ---
Usage: dotnet run [command|filepath]

Analysis Commands:
  <filepath>                              Analyze PDF and show results
  --export-xml <filepath>                 Analyze PDF and export to XML
  --export-json <filepath>                Analyze PDF and export to JSON
...
```

### Performance Verification
Test with a sample PDF to ensure reasonable performance:
```bash
# Should complete in under 2 seconds for typical files
time dotnet run -- sample.pdf
```

## Configuration

### Default Settings
PPA-2.1 works out-of-the-box with sensible defaults:
- **Spot Color Database**: Auto-created in application directory
- **Output Format**: Console display by default
- **File Validation**: Strict PDF validation enabled
- **Memory Limits**: Optimized for typical PDF sizes

### Customizable Options

#### Spot Color Database Location
The SQLite database is created automatically in the application directory as `spot_colors.db`. To use a different location:

```bash
# Set environment variable (optional)
export PPA_DATABASE_PATH="/custom/path/spot_colors.db"
dotnet run -- sample.pdf
```

#### Performance Tuning
For large files or batch processing:

```bash
# Increase .NET garbage collection performance
export DOTNET_gcServer=1
export DOTNET_GCRetainVM=1
dotnet run -- large_file.pdf
```

### Environment Variables
| Variable | Description | Default |
|----------|-------------|---------|
| `PPA_DATABASE_PATH` | Custom database location | `./spot_colors.db` |
| `DOTNET_gcServer` | Enable server GC | `0` (disabled) |
| `PPA_TEMP_DIR` | Temporary file directory | System temp |

## Troubleshooting

### Common Issues

#### "dotnet command not found"
**Problem**: .NET is not installed or not in PATH  
**Solution**: 
1. Install .NET 9.0 SDK/Runtime
2. Restart terminal/command prompt
3. Verify with `dotnet --version`

#### "Could not load file or assembly 'itext7'"
**Problem**: NuGet packages not restored  
**Solution**:
```bash
dotnet clean
dotnet restore
dotnet build
```

#### "Access denied" when creating database
**Problem**: Insufficient permissions in application directory  
**Solution**:
```bash
# Run with elevated permissions or use custom database path
sudo dotnet run -- sample.pdf
# OR
export PPA_DATABASE_PATH="~/Documents/spot_colors.db"
dotnet run -- sample.pdf
```

#### "PDF file could not be opened"
**Problem**: Invalid or corrupted PDF file  
**Solution**:
1. Try opening file in PDF viewer
2. Re-export from original application
3. Use a different PDF file for testing

#### Slow performance
**Problem**: Large files or limited resources  
**Solution**:
```bash
# Enable server garbage collection
export DOTNET_gcServer=1
dotnet run -- large_file.pdf

# Close other applications to free memory
# Use SSD storage if available
```

### Platform-Specific Issues

#### Windows
- **Windows Defender**: May scan PDF files, causing delays
- **Path Issues**: Use quotes around paths with spaces
- **Permission Errors**: Run as Administrator if needed

#### macOS
- **Gatekeeper**: May block execution of downloaded files
  ```bash
  xattr -d com.apple.quarantine /path/to/downloaded/files
  ```
- **Case Sensitivity**: File paths are case-sensitive

#### Linux
- **Package Manager**: Different distributions use different package managers
- **Permissions**: May need `chmod +x` for shell scripts
- **Dependencies**: Some distributions may need additional packages

### Getting Help

#### Log Information
When reporting issues, include:
```bash
# System information
dotnet --info

# Version information
dotnet run -- --help

# Error output
dotnet run -- problematic_file.pdf 2>&1 | tee error.log
```

#### Support Channels
1. **GitHub Issues**: https://github.com/Whateverdoa/ConsoleApp1_Pdf_analyse/issues
2. **Documentation**: Check docs/ directory
3. **Community**: GitHub Discussions

## Uninstallation

### Complete Removal
```bash
# Remove project directory
rm -rf /path/to/ConsoleApp1_Pdf_analyse

# Remove custom database (if created outside project)
rm -f ~/Documents/spot_colors.db

# Optional: Remove .NET (if not needed for other projects)
# Windows: Use Programs and Features
# macOS: brew uninstall dotnet
# Linux: sudo apt remove dotnet-sdk-9.0
```

### Partial Removal (Keep Source)
```bash
# Clean build artifacts
cd ConsoleApp1_Pdf_analyse/ConsoleApp1_Pdf_analyse
dotnet clean

# Remove packages cache
dotnet nuget locals all --clear

# Remove database
rm -f spot_colors.db
```

---

For usage instructions, see the [User Manual](USER_MANUAL.md).  
For technical details, see the [Technical Documentation](TECHNICAL_DOCS.md).