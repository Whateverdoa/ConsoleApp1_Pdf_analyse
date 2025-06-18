using System;
using System.Collections.Generic;

public static class ColorInterpreter
{
    public static string InterpretCmykColor(float c, float m, float y, float k)
    {
        // Tolerance for floating point comparison
        const float tolerance = 0.5f;

        // Pure colors
        if (IsNear(c, 0, tolerance) && IsNear(m, 0, tolerance) && IsNear(y, 0, tolerance) && IsNear(k, 0, tolerance))
            return "White";
        
        if (IsNear(c, 0, tolerance) && IsNear(m, 0, tolerance) && IsNear(y, 0, tolerance) && IsNear(k, 100, tolerance))
            return "Black";

        // Primary colors (high single channel)
        if (IsNear(c, 100, tolerance) && IsNear(m, 0, tolerance) && IsNear(y, 0, tolerance) && IsNear(k, 0, tolerance))
            return "Cyan";
        
        if (IsNear(c, 0, tolerance) && IsNear(m, 100, tolerance) && IsNear(y, 0, tolerance) && IsNear(k, 0, tolerance))
            return "Magenta";
        
        if (IsNear(c, 0, tolerance) && IsNear(m, 0, tolerance) && IsNear(y, 100, tolerance) && IsNear(k, 0, tolerance))
            return "Yellow";

        // Secondary colors (two high channels)
        if (IsNear(c, 100, tolerance) && IsNear(m, 100, tolerance) && IsNear(y, 0, tolerance) && IsNear(k, 0, tolerance))
            return "Blue";
        
        if (IsNear(c, 0, tolerance) && IsNear(m, 100, tolerance) && IsNear(y, 100, tolerance) && IsNear(k, 0, tolerance))
            return "Red";
        
        if (IsNear(c, 100, tolerance) && IsNear(m, 0, tolerance) && IsNear(y, 100, tolerance) && IsNear(k, 0, tolerance))
            return "Green";

        // Rich black (process black with color)
        if (k >= 80 && (c > 10 || m > 10 || y > 10))
            return "Rich Black";

        // Gray scale (only K value, low CMY)
        if (c <= 5 && m <= 5 && y <= 5 && k > 5)
        {
            if (k <= 25) return "Light Gray";
            if (k <= 50) return "Medium Gray";
            if (k <= 75) return "Dark Gray";
            return "Very Dark Gray";
        }

        // Brown tones (high Y and K with some M)
        if (y >= 60 && k >= 30 && m >= 20 && c <= 30)
            return "Brown";

        // Orange tones (high Y and M, low C)
        if (y >= 80 && m >= 60 && c <= 20 && k <= 20)
            return "Orange";

        // Pink tones (high M, low C and Y)
        if (m >= 50 && c <= 30 && y <= 30 && k <= 20)
            return "Pink";

        // Purple tones (high M and C, low Y)
        if (m >= 60 && c >= 40 && y <= 30)
            return "Purple";

        // If no specific color match, return a descriptive name
        return $"CMYK({c:F0},{m:F0},{y:F0},{k:F0})";
    }

    public static string InterpretRgbColor(float r, float g, float b)
    {
        const float tolerance = 5f;

        // Pure colors
        if (IsNear(r, 255, tolerance) && IsNear(g, 255, tolerance) && IsNear(b, 255, tolerance))
            return "White";
        
        if (IsNear(r, 0, tolerance) && IsNear(g, 0, tolerance) && IsNear(b, 0, tolerance))
            return "Black";

        // Primary colors
        if (IsNear(r, 255, tolerance) && IsNear(g, 0, tolerance) && IsNear(b, 0, tolerance))
            return "Red";
        
        if (IsNear(r, 0, tolerance) && IsNear(g, 255, tolerance) && IsNear(b, 0, tolerance))
            return "Green";
        
        if (IsNear(r, 0, tolerance) && IsNear(g, 0, tolerance) && IsNear(b, 255, tolerance))
            return "Blue";

        // Secondary colors
        if (IsNear(r, 255, tolerance) && IsNear(g, 255, tolerance) && IsNear(b, 0, tolerance))
            return "Yellow";
        
        if (IsNear(r, 255, tolerance) && IsNear(g, 0, tolerance) && IsNear(b, 255, tolerance))
            return "Magenta";
        
        if (IsNear(r, 0, tolerance) && IsNear(g, 255, tolerance) && IsNear(b, 255, tolerance))
            return "Cyan";

        // Gray scale (equal or near-equal RGB values)
        if (Math.Abs(r - g) <= 10 && Math.Abs(g - b) <= 10 && Math.Abs(r - b) <= 10)
        {
            float avg = (r + g + b) / 3;
            if (avg <= 64) return "Dark Gray";
            if (avg <= 128) return "Medium Gray";
            if (avg <= 192) return "Light Gray";
            return "Very Light Gray";
        }

        return $"RGB({r:F0},{g:F0},{b:F0})";
    }

    private static bool IsNear(float value1, float value2, float tolerance)
    {
        return Math.Abs(value1 - value2) <= tolerance;
    }
}