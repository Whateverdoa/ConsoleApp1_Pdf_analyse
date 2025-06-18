using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

public class SpotColorDatabase
{
    private readonly string _connectionString;
    private const string DatabaseFileName = "spot_colors.db";

    public SpotColorDatabase()
    {
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DatabaseFileName);
        _connectionString = $"Data Source={dbPath}";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var createTableCommand = connection.CreateCommand();
        createTableCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS SpotColors (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ColorName TEXT NOT NULL UNIQUE,
                ColorType TEXT NOT NULL,
                Description TEXT,
                PantoneNumber TEXT,
                CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
            );

            CREATE INDEX IF NOT EXISTS idx_colorname ON SpotColors(ColorName);
            CREATE INDEX IF NOT EXISTS idx_colortype ON SpotColors(ColorType);
        ";
        createTableCommand.ExecuteNonQuery();

        // Check if database is empty and populate with initial data
        var countCommand = connection.CreateCommand();
        countCommand.CommandText = "SELECT COUNT(*) FROM SpotColors";
        var count = Convert.ToInt32(countCommand.ExecuteScalar());

        if (count == 0)
        {
            PopulateInitialData(connection);
        }
    }

    private void PopulateInitialData(SqliteConnection connection)
    {
        var insertCommand = connection.CreateCommand();
        insertCommand.CommandText = @"
            INSERT OR IGNORE INTO SpotColors (ColorName, ColorType, Description) VALUES 
            -- Die-cut variations
            ('CutContour', 'DieCut', 'Standard die-cut contour line'),
            ('cutcontour', 'DieCut', 'Lowercase die-cut contour'),
            ('CUTCONTOUR', 'DieCut', 'Uppercase die-cut contour'),
            ('Cut Contour', 'DieCut', 'Spaced die-cut contour'),
            ('cut contour', 'DieCut', 'Lowercase spaced die-cut contour'),
            ('CUT CONTOUR', 'DieCut', 'Uppercase spaced die-cut contour'),
            ('Stans', 'DieCut', 'Standard stans (Dutch/German die-cut)'),
            ('stans', 'DieCut', 'Lowercase stans'),
            ('STANS', 'DieCut', 'Uppercase stans'),
            ('Stans!', 'DieCut', 'Stans with exclamation'),
            ('KissCutt', 'DieCut', 'Kiss cut (partial cut)'),
            ('kisscut', 'DieCut', 'Lowercase kiss cut'),
            ('KISSCUT', 'DieCut', 'Uppercase kiss cut'),
            ('Kiss Cut', 'DieCut', 'Spaced kiss cut'),
            ('kiss cut', 'DieCut', 'Lowercase spaced kiss cut'),
            ('KISS CUT', 'DieCut', 'Uppercase spaced kiss cut'),
            ('KissCut', 'DieCut', 'Alternative kiss cut'),
            ('Die-Cut', 'DieCut', 'Hyphenated die-cut'),
            ('die-cut', 'DieCut', 'Lowercase hyphenated die-cut'),
            ('DIE-CUT', 'DieCut', 'Uppercase hyphenated die-cut'),
            ('DieCut', 'DieCut', 'Combined die-cut'),
            ('diecut', 'DieCut', 'Lowercase combined die-cut'),
            ('DIECUT', 'DieCut', 'Uppercase combined die-cut'),
            ('Die Cut', 'DieCut', 'Spaced die-cut'),
            ('die cut', 'DieCut', 'Lowercase spaced die-cut'),
            ('DIE CUT', 'DieCut', 'Uppercase spaced die-cut'),
            ('Thru-cut', 'DieCut', 'Through cut'),
            ('thru-cut', 'DieCut', 'Lowercase through cut'),
            ('THRU-CUT', 'DieCut', 'Uppercase through cut'),
            ('ThruCut', 'DieCut', 'Combined through cut'),
            ('thrucut', 'DieCut', 'Lowercase combined through cut'),
            ('THRUCUT', 'DieCut', 'Uppercase combined through cut'),
            ('Thru Cut', 'DieCut', 'Spaced through cut'),
            ('thru cut', 'DieCut', 'Lowercase spaced through cut'),
            ('THRU CUT', 'DieCut', 'Uppercase spaced through cut'),
            ('Through Cut', 'DieCut', 'Full through cut'),
            ('through cut', 'DieCut', 'Lowercase full through cut'),
            ('THROUGH CUT', 'DieCut', 'Uppercase full through cut'),
            ('Cut', 'DieCut', 'Simple cut'),
            ('cut', 'DieCut', 'Lowercase simple cut'),
            ('CUT', 'DieCut', 'Uppercase simple cut'),
            ('Cutting', 'DieCut', 'Cutting line'),
            ('cutting', 'DieCut', 'Lowercase cutting line'),
            ('CUTTING', 'DieCut', 'Uppercase cutting line'),

            -- Foil/Hot Stamping
            ('Foil', 'Foil', 'Generic foil stamping'),
            ('foil', 'Foil', 'Lowercase foil'),
            ('FOIL', 'Foil', 'Uppercase foil'),
            ('Hot Foil', 'Foil', 'Hot foil stamping'),
            ('hot foil', 'Foil', 'Lowercase hot foil'),
            ('HOT FOIL', 'Foil', 'Uppercase hot foil'),
            ('HotFoil', 'Foil', 'Combined hot foil'),
            ('hotfoil', 'Foil', 'Lowercase combined hot foil'),
            ('HOTFOIL', 'Foil', 'Uppercase combined hot foil'),
            ('Gold Foil', 'Foil', 'Gold foil stamping'),
            ('gold foil', 'Foil', 'Lowercase gold foil'),
            ('GOLD FOIL', 'Foil', 'Uppercase gold foil'),
            ('Silver Foil', 'Foil', 'Silver foil stamping'),
            ('silver foil', 'Foil', 'Lowercase silver foil'),
            ('SILVER FOIL', 'Foil', 'Uppercase silver foil'),
            ('Metallic', 'Foil', 'Metallic finish'),
            ('metallic', 'Foil', 'Lowercase metallic'),
            ('METALLIC', 'Foil', 'Uppercase metallic'),
            ('Hot Stamp', 'Foil', 'Hot stamping'),
            ('hot stamp', 'Foil', 'Lowercase hot stamp'),
            ('HOT STAMP', 'Foil', 'Uppercase hot stamp'),
            ('HotStamp', 'Foil', 'Combined hot stamp'),
            ('hotstamp', 'Foil', 'Lowercase combined hot stamp'),
            ('HOTSTAMP', 'Foil', 'Uppercase combined hot stamp'),

            -- Embossing/Debossing
            ('Emboss', 'Emboss', 'Embossing'),
            ('emboss', 'Emboss', 'Lowercase emboss'),
            ('EMBOSS', 'Emboss', 'Uppercase emboss'),
            ('Embossing', 'Emboss', 'Embossing process'),
            ('embossing', 'Emboss', 'Lowercase embossing'),
            ('EMBOSSING', 'Emboss', 'Uppercase embossing'),
            ('Deboss', 'Emboss', 'Debossing'),
            ('deboss', 'Emboss', 'Lowercase deboss'),
            ('DEBOSS', 'Emboss', 'Uppercase deboss'),
            ('Debossing', 'Emboss', 'Debossing process'),
            ('debossing', 'Emboss', 'Lowercase debossing'),
            ('DEBOSSING', 'Emboss', 'Uppercase debossing'),
            ('Blind Emboss', 'Emboss', 'Blind embossing'),
            ('blind emboss', 'Emboss', 'Lowercase blind emboss'),
            ('BLIND EMBOSS', 'Emboss', 'Uppercase blind emboss'),
            ('BlindEmboss', 'Emboss', 'Combined blind emboss'),
            ('blindemboss', 'Emboss', 'Lowercase combined blind emboss'),
            ('BLINDEMBOSS', 'Emboss', 'Uppercase combined blind emboss'),

            -- Varnish/UV
            ('Varnish', 'Varnish', 'Varnish coating'),
            ('varnish', 'Varnish', 'Lowercase varnish'),
            ('VARNISH', 'Varnish', 'Uppercase varnish'),
            ('UV', 'Varnish', 'UV coating'),
            ('uv', 'Varnish', 'Lowercase UV'),
            ('Spot UV', 'Varnish', 'Spot UV coating'),
            ('spot uv', 'Varnish', 'Lowercase spot UV'),
            ('SPOT UV', 'Varnish', 'Uppercase spot UV'),
            ('SpotUV', 'Varnish', 'Combined spot UV'),
            ('spotuv', 'Varnish', 'Lowercase combined spot UV'),
            ('SPOTUV', 'Varnish', 'Uppercase combined spot UV'),
            ('UV Varnish', 'Varnish', 'UV varnish coating'),
            ('uv varnish', 'Varnish', 'Lowercase UV varnish'),
            ('UV VARNISH', 'Varnish', 'Uppercase UV varnish'),
            ('Gloss', 'Varnish', 'Gloss finish'),
            ('gloss', 'Varnish', 'Lowercase gloss'),
            ('GLOSS', 'Varnish', 'Uppercase gloss'),
            ('Matt', 'Varnish', 'Matte finish'),
            ('matt', 'Varnish', 'Lowercase matte'),
            ('MATT', 'Varnish', 'Uppercase matte'),
            ('Matte', 'Varnish', 'Matte finish variant'),
            ('matte', 'Varnish', 'Lowercase matte variant'),
            ('MATTE', 'Varnish', 'Uppercase matte variant'),
            ('Satin', 'Varnish', 'Satin finish'),
            ('satin', 'Varnish', 'Lowercase satin'),
            ('SATIN', 'Varnish', 'Uppercase satin'),

            -- Registration and marks
            ('Registration', 'Registration', 'Registration marks'),
            ('registration', 'Registration', 'Lowercase registration'),
            ('REGISTRATION', 'Registration', 'Uppercase registration'),
            ('Crop Marks', 'Registration', 'Crop marks'),
            ('crop marks', 'Registration', 'Lowercase crop marks'),
            ('CROP MARKS', 'Registration', 'Uppercase crop marks'),
            ('CropMarks', 'Registration', 'Combined crop marks'),
            ('cropmarks', 'Registration', 'Lowercase combined crop marks'),
            ('CROPMARKS', 'Registration', 'Uppercase combined crop marks'),
            ('Trim Marks', 'Registration', 'Trim marks'),
            ('trim marks', 'Registration', 'Lowercase trim marks'),
            ('TRIM MARKS', 'Registration', 'Uppercase trim marks'),
            ('TrimMarks', 'Registration', 'Combined trim marks'),
            ('trimmarks', 'Registration', 'Lowercase combined trim marks'),
            ('TRIMMARKS', 'Registration', 'Uppercase combined trim marks'),
            ('Bleed', 'Registration', 'Bleed area'),
            ('bleed', 'Registration', 'Lowercase bleed'),
            ('BLEED', 'Registration', 'Uppercase bleed'),

            -- Perforation
            ('Perf', 'Perforation', 'Perforation'),
            ('perf', 'Perforation', 'Lowercase perforation'),
            ('PERF', 'Perforation', 'Uppercase perforation'),
            ('Perforation', 'Perforation', 'Full perforation'),
            ('perforation', 'Perforation', 'Lowercase full perforation'),
            ('PERFORATION', 'Perforation', 'Uppercase full perforation'),
            ('Perforated', 'Perforation', 'Perforated line'),
            ('perforated', 'Perforation', 'Lowercase perforated'),
            ('PERFORATED', 'Perforation', 'Uppercase perforated'),

            -- Scoring/Creasing
            ('Score', 'Scoring', 'Score line'),
            ('score', 'Scoring', 'Lowercase score'),
            ('SCORE', 'Scoring', 'Uppercase score'),
            ('Scoring', 'Scoring', 'Scoring process'),
            ('scoring', 'Scoring', 'Lowercase scoring'),
            ('SCORING', 'Scoring', 'Uppercase scoring'),
            ('Crease', 'Scoring', 'Crease line'),
            ('crease', 'Scoring', 'Lowercase crease'),
            ('CREASE', 'Scoring', 'Uppercase crease'),
            ('Creasing', 'Scoring', 'Creasing process'),
            ('creasing', 'Scoring', 'Lowercase creasing'),
            ('CREASING', 'Scoring', 'Uppercase creasing'),

            -- Window/Cutout
            ('Window', 'Cutout', 'Window cutout'),
            ('window', 'Cutout', 'Lowercase window'),
            ('WINDOW', 'Cutout', 'Uppercase window'),
            ('Cutout', 'Cutout', 'Generic cutout'),
            ('cutout', 'Cutout', 'Lowercase cutout'),
            ('CUTOUT', 'Cutout', 'Uppercase cutout'),
            ('Cut Out', 'Cutout', 'Spaced cutout'),
            ('cut out', 'Cutout', 'Lowercase spaced cutout'),
            ('CUT OUT', 'Cutout', 'Uppercase spaced cutout');
        ";
        insertCommand.ExecuteNonQuery();

        // Add sample Pantone colors
        var pantoneCommand = connection.CreateCommand();
        pantoneCommand.CommandText = @"
            INSERT OR IGNORE INTO SpotColors (ColorName, ColorType, Description, PantoneNumber) VALUES 
            ('PANTONE Red 032 C', 'Pantone', 'Pantone Red', '032 C'),
            ('PANTONE Blue 072 C', 'Pantone', 'Pantone Blue', '072 C'),
            ('PANTONE Yellow C', 'Pantone', 'Pantone Yellow', 'Yellow C'),
            ('PANTONE Black C', 'Pantone', 'Pantone Black', 'Black C'),
            ('PANTONE White C', 'Pantone', 'Pantone White', 'White C'),
            ('PANTONE Reflex Blue C', 'Pantone', 'Pantone Reflex Blue', 'Reflex Blue C'),
            ('PANTONE Cool Gray 11 C', 'Pantone', 'Pantone Cool Gray', 'Cool Gray 11 C'),
            ('PANTONE Warm Gray 11 C', 'Pantone', 'Pantone Warm Gray', 'Warm Gray 11 C'),
            ('PANTONE 485 C', 'Pantone', 'Pantone Red 485', '485 C'),
            ('PANTONE 286 C', 'Pantone', 'Pantone Blue 286', '286 C'),
            ('PANTONE 348 C', 'Pantone', 'Pantone Green 348', '348 C'),
            ('PANTONE 802 C', 'Pantone', 'Pantone Brown 802', '802 C'),
            ('PANTONE Purple C', 'Pantone', 'Pantone Purple', 'Purple C'),
            ('PANTONE Orange 021 C', 'Pantone', 'Pantone Orange', 'Orange 021 C'),
            ('PANTONE Process Blue C', 'Pantone', 'Pantone Process Blue', 'Process Blue C'),
            -- Common simplified Pantone names
            ('Red 032', 'Pantone', 'Simplified Pantone Red', '032'),
            ('Blue 072', 'Pantone', 'Simplified Pantone Blue', '072'),
            ('485', 'Pantone', 'Pantone number only', '485'),
            ('286', 'Pantone', 'Pantone number only', '286'),
            ('348', 'Pantone', 'Pantone number only', '348');
        ";
        pantoneCommand.ExecuteNonQuery();
    }

    public List<string> GetAllSpotColorNames()
    {
        var colorNames = new List<string>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT ColorName FROM SpotColors ORDER BY ColorName";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            colorNames.Add(reader.GetString(0));
        }

        return colorNames;
    }

    public List<string> GetSpotColorsByType(string colorType)
    {
        var colorNames = new List<string>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT ColorName FROM SpotColors WHERE ColorType = @colorType ORDER BY ColorName";
        command.Parameters.AddWithValue("@colorType", colorType);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            colorNames.Add(reader.GetString(0));
        }

        return colorNames;
    }

    public void AddSpotColor(string colorName, string colorType, string description = "", string pantoneNumber = "")
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT OR IGNORE INTO SpotColors (ColorName, ColorType, Description, PantoneNumber) 
            VALUES (@colorName, @colorType, @description, @pantoneNumber)";
        
        command.Parameters.AddWithValue("@colorName", colorName);
        command.Parameters.AddWithValue("@colorType", colorType);
        command.Parameters.AddWithValue("@description", description);
        command.Parameters.AddWithValue("@pantoneNumber", pantoneNumber);

        command.ExecuteNonQuery();
    }

    public bool ContainsSpotColor(string colorName)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM SpotColors WHERE ColorName = @colorName";
        command.Parameters.AddWithValue("@colorName", colorName);

        var count = Convert.ToInt32(command.ExecuteScalar());
        return count > 0;
    }

    public void AddPantoneColor(string pantoneNumber, string description = "")
    {
        var colorName = $"PANTONE {pantoneNumber}";
        AddSpotColor(colorName, "Pantone", description, pantoneNumber);
    }

    public List<(string ColorName, string ColorType, string Description, string PantoneNumber)> GetAllSpotColorDetails()
    {
        var colors = new List<(string, string, string, string)>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT ColorName, ColorType, Description, PantoneNumber FROM SpotColors ORDER BY ColorType, ColorName";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            colors.Add((
                reader.GetString(0),
                reader.GetString(1),
                reader.IsDBNull(2) ? "" : reader.GetString(2),
                reader.IsDBNull(3) ? "" : reader.GetString(3)
            ));
        }

        return colors;
    }
}