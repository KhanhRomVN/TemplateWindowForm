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
        public Color OnBackground { get; set; }
        public Color OnSurface { get; set; }
        public Color Error { get; set; }
        public Color Warning { get; set; }
        public Color Success { get; set; }
        public Color Info { get; set; }
        public Color TextPrimary { get; set; }
        public Color TextSecondary { get; set; }
        public Color Border { get; set; }
        public Color Disabled { get; set; }

        public static ColorPalette Light => new()
        {
            Primary = Color.FromArgb(63, 81, 181),
            Secondary = Color.FromArgb(255, 193, 7),
            Background = Color.FromArgb(255, 255, 255), // Changed to pure white (#ffffff)
            Surface = Color.FromArgb(247, 247, 247), // Changed to #f7f7f7 for sidebar
            OnPrimary = Color.White,
            OnSecondary = Color.Black,
            OnBackground = Color.FromArgb(33, 37, 41),
            OnSurface = Color.FromArgb(33, 37, 41),
            Error = Color.FromArgb(244, 67, 54),
            Warning = Color.FromArgb(255, 152, 0),
            Success = Color.FromArgb(76, 175, 80),
            Info = Color.FromArgb(33, 150, 243),
            TextPrimary = Color.FromArgb(33, 37, 41),
            TextSecondary = Color.FromArgb(108, 117, 125),
            Border = Color.FromArgb(222, 226, 230),
            Disabled = Color.FromArgb(173, 181, 189)
        };

        public static ColorPalette Dark => new()
        {
            Primary = Color.FromArgb(121, 134, 203),
            Secondary = Color.FromArgb(255, 193, 7),
            Background = Color.FromArgb(18, 18, 18),
            Surface = Color.FromArgb(33, 37, 41),
            OnPrimary = Color.White,
            OnSecondary = Color.Black,
            OnBackground = Color.FromArgb(248, 249, 250),
            OnSurface = Color.FromArgb(248, 249, 250),
            Error = Color.FromArgb(244, 67, 54),
            Warning = Color.FromArgb(255, 152, 0),
            Success = Color.FromArgb(76, 175, 80),
            Info = Color.FromArgb(33, 150, 243),
            TextPrimary = Color.FromArgb(248, 249, 250),
            TextSecondary = Color.FromArgb(173, 181, 189),
            Border = Color.FromArgb(73, 80, 87),
            Disabled = Color.FromArgb(108, 117, 125)
        };

        // Add additional color palettes for other themes
        public static ColorPalette Blue => new()
        {
            Primary = Color.FromArgb(33, 150, 243),
            Secondary = Color.FromArgb(255, 193, 7),
            Background = Color.FromArgb(255, 255, 255),
            Surface = Color.FromArgb(247, 247, 247),
            OnPrimary = Color.White,
            OnSecondary = Color.Black,
            OnBackground = Color.FromArgb(33, 37, 41),
            OnSurface = Color.FromArgb(33, 37, 41),
            Error = Color.FromArgb(244, 67, 54),
            Warning = Color.FromArgb(255, 152, 0),
            Success = Color.FromArgb(76, 175, 80),
            Info = Color.FromArgb(33, 150, 243),
            TextPrimary = Color.FromArgb(33, 37, 41),
            TextSecondary = Color.FromArgb(108, 117, 125),
            Border = Color.FromArgb(222, 226, 230),
            Disabled = Color.FromArgb(173, 181, 189)
        };

        public static ColorPalette Green => new()
        {
            Primary = Color.FromArgb(76, 175, 80),
            Secondary = Color.FromArgb(255, 193, 7),
            Background = Color.FromArgb(255, 255, 255),
            Surface = Color.FromArgb(247, 247, 247),
            OnPrimary = Color.White,
            OnSecondary = Color.Black,
            OnBackground = Color.FromArgb(33, 37, 41),
            OnSurface = Color.FromArgb(33, 37, 41),
            Error = Color.FromArgb(244, 67, 54),
            Warning = Color.FromArgb(255, 152, 0),
            Success = Color.FromArgb(76, 175, 80),
            Info = Color.FromArgb(33, 150, 243),
            TextPrimary = Color.FromArgb(33, 37, 41),
            TextSecondary = Color.FromArgb(108, 117, 125),
            Border = Color.FromArgb(222, 226, 230),
            Disabled = Color.FromArgb(173, 181, 189)
        };

        public static ColorPalette Purple => new()
        {
            Primary = Color.FromArgb(156, 39, 176),
            Secondary = Color.FromArgb(255, 193, 7),
            Background = Color.FromArgb(255, 255, 255),
            Surface = Color.FromArgb(247, 247, 247),
            OnPrimary = Color.White,
            OnSecondary = Color.Black,
            OnBackground = Color.FromArgb(33, 37, 41),
            OnSurface = Color.FromArgb(33, 37, 41),
            Error = Color.FromArgb(244, 67, 54),
            Warning = Color.FromArgb(255, 152, 0),
            Success = Color.FromArgb(76, 175, 80),
            Info = Color.FromArgb(33, 150, 243),
            TextPrimary = Color.FromArgb(33, 37, 41),
            TextSecondary = Color.FromArgb(108, 117, 125),
            Border = Color.FromArgb(222, 226, 230),
            Disabled = Color.FromArgb(173, 181, 189)
        };
    }
}