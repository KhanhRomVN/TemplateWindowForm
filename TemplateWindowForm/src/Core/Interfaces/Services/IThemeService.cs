using Core.Enums;
using Core.ValueObjects;

namespace Core.Interfaces.Services
{
    public interface IThemeService
    {
        event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
        
        ThemeType CurrentTheme { get; }
        ColorPalette CurrentColors { get; }
        
        void SetTheme(ThemeType themeType);
        void SetCustomTheme(ColorPalette customColors);
        ColorPalette GetThemeColors(ThemeType themeType);
    }

    public class ThemeChangedEventArgs : EventArgs
    {
        public ThemeType ThemeType { get; }
        public ColorPalette Colors { get; }

        public ThemeChangedEventArgs(ThemeType themeType, ColorPalette colors)
        {
            ThemeType = themeType;
            Colors = colors;
        }
    }
}