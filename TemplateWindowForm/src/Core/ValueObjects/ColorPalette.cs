using System.Drawing;

namespace Core.ValueObjects
{
    public class ColorPalette
    {
        public Color Primary { get; set; }
        public Color Secondary { get; set; }
        public Color Background { get; set; }
        public Color Surface { get; set; }
        public Color OnPrimary { get; set; }
        public Color OnSecondary { get; set; }
        public Color OnSurface { get; set; }
        public Color TextPrimary { get; set; }
        public Color TextSecondary { get; set; }
        public Color Border { get; set; }
        public Color Shadow { get; set; }
        public Color Accent { get; set; }
        public Color Success { get; set; }
        public Color Warning { get; set; }
        public Color Error { get; set; }
        public Color Info { get; set; }

        public ColorPalette(
            Color primary,
            Color secondary,
            Color background,
            Color surface,
            Color onPrimary,
            Color onSecondary,
            Color onSurface,
            Color textPrimary,
            Color textSecondary,
            Color border,
            Color shadow,
            Color accent,
            Color success,
            Color warning,
            Color error,
            Color info)
        {
            Primary = primary;
            Secondary = secondary;
            Background = background;
            Surface = surface;
            OnPrimary = onPrimary;
            OnSecondary = onSecondary;
            OnSurface = onSurface;
            TextPrimary = textPrimary;
            TextSecondary = textSecondary;
            Border = border;
            Shadow = shadow;
            Accent = accent;
            Success = success;
            Warning = warning;
            Error = error;
            Info = info;
        }

        // Predefined color palettes
        public static ColorPalette Light => new ColorPalette(
            primary: Color.FromArgb(99, 102, 241),      // Indigo
            secondary: Color.FromArgb(107, 114, 128),    // Gray
            background: Color.FromArgb(249, 250, 251),   // Light gray
            surface: Color.White,
            onPrimary: Color.White,
            onSecondary: Color.White,
            onSurface: Color.FromArgb(17, 24, 39),       // Dark gray
            textPrimary: Color.FromArgb(17, 24, 39),     // Dark gray
            textSecondary: Color.FromArgb(75, 85, 99),   // Medium gray
            border: Color.FromArgb(229, 231, 235),       // Light border
            shadow: Color.FromArgb(0, 0, 0),             // Black shadow
            accent: Color.FromArgb(59, 130, 246),        // Blue
            success: Color.FromArgb(34, 197, 94),        // Green
            warning: Color.FromArgb(251, 191, 36),       // Yellow
            error: Color.FromArgb(239, 68, 68),          // Red
            info: Color.FromArgb(59, 130, 246)           // Blue
        );

        public static ColorPalette Dark => new ColorPalette(
            primary: Color.FromArgb(129, 140, 248),      // Light indigo
            secondary: Color.FromArgb(156, 163, 175),    // Light gray
            background: Color.FromArgb(17, 24, 39),      // Dark background
            surface: Color.FromArgb(31, 41, 55),         // Dark surface
            onPrimary: Color.FromArgb(17, 24, 39),       // Dark text on primary
            onSecondary: Color.FromArgb(17, 24, 39),     // Dark text on secondary
            onSurface: Color.FromArgb(243, 244, 246),    // Light text on surface
            textPrimary: Color.FromArgb(243, 244, 246),  // Light text
            textSecondary: Color.FromArgb(156, 163, 175), // Medium gray text
            border: Color.FromArgb(75, 85, 99),          // Dark border
            shadow: Color.FromArgb(0, 0, 0),             // Black shadow
            accent: Color.FromArgb(96, 165, 250),        // Light blue
            success: Color.FromArgb(52, 211, 153),       // Light green
            warning: Color.FromArgb(251, 191, 36),       // Yellow
            error: Color.FromArgb(248, 113, 113),        // Light red
            info: Color.FromArgb(96, 165, 250)           // Light blue
        );

        public static ColorPalette Blue => new ColorPalette(
            primary: Color.FromArgb(59, 130, 246),       // Blue
            secondary: Color.FromArgb(107, 114, 128),    // Gray
            background: Color.FromArgb(239, 246, 255),   // Light blue
            surface: Color.White,
            onPrimary: Color.White,
            onSecondary: Color.White,
            onSurface: Color.FromArgb(17, 24, 39),
            textPrimary: Color.FromArgb(17, 24, 39),
            textSecondary: Color.FromArgb(75, 85, 99),
            border: Color.FromArgb(191, 219, 254),       // Light blue border
            shadow: Color.FromArgb(0, 0, 0),
            accent: Color.FromArgb(99, 102, 241),        // Indigo
            success: Color.FromArgb(34, 197, 94),
            warning: Color.FromArgb(251, 191, 36),
            error: Color.FromArgb(239, 68, 68),
            info: Color.FromArgb(59, 130, 246)
        );

        public static ColorPalette Green => new ColorPalette(
            primary: Color.FromArgb(34, 197, 94),        // Green
            secondary: Color.FromArgb(107, 114, 128),    // Gray
            background: Color.FromArgb(240, 253, 244),   // Light green
            surface: Color.White,
            onPrimary: Color.White,
            onSecondary: Color.White,
            onSurface: Color.FromArgb(17, 24, 39),
            textPrimary: Color.FromArgb(17, 24, 39),
            textSecondary: Color.FromArgb(75, 85, 99),
            border: Color.FromArgb(187, 247, 208),       // Light green border
            shadow: Color.FromArgb(0, 0, 0),
            accent: Color.FromArgb(16, 185, 129),        // Emerald
            success: Color.FromArgb(34, 197, 94),
            warning: Color.FromArgb(251, 191, 36),
            error: Color.FromArgb(239, 68, 68),
            info: Color.FromArgb(59, 130, 246)
        );

        public static ColorPalette Purple => new ColorPalette(
            primary: Color.FromArgb(147, 51, 234),       // Purple
            secondary: Color.FromArgb(107, 114, 128),    // Gray
            background: Color.FromArgb(250, 245, 255),   // Light purple
            surface: Color.White,
            onPrimary: Color.White,
            onSecondary: Color.White,
            onSurface: Color.FromArgb(17, 24, 39),
            textPrimary: Color.FromArgb(17, 24, 39),
            textSecondary: Color.FromArgb(75, 85, 99),
            border: Color.FromArgb(221, 214, 254),       // Light purple border
            shadow: Color.FromArgb(0, 0, 0),
            accent: Color.FromArgb(168, 85, 247),        // Light purple
            success: Color.FromArgb(34, 197, 94),
            warning: Color.FromArgb(251, 191, 36),
            error: Color.FromArgb(239, 68, 68),
            info: Color.FromArgb(59, 130, 246)
        );
    }
}