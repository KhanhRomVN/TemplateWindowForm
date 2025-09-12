using Core.Enums;
using Core.Interfaces.Services;
using Core.ValueObjects;

namespace Infrastructure.Services
{
    public class ThemeService : IThemeService
    {
        private ThemeType _currentTheme = ThemeType.Light;
        private ColorPalette _currentColors;
        private ColorPalette? _customColors;

        public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

        public ThemeType CurrentTheme => _currentTheme;
        public ColorPalette CurrentColors => _currentColors;

        public ThemeService()
        {
            _currentColors = GetThemeColors(_currentTheme);
        }

        public void SetTheme(ThemeType themeType)
        {
            if (_currentTheme == themeType) return;

            _currentTheme = themeType;
            _currentColors = GetThemeColors(themeType);
            
            OnThemeChanged();
        }

        public void SetCustomTheme(ColorPalette customColors)
        {
            _customColors = customColors;
            _currentTheme = ThemeType.Custom;
            _currentColors = customColors;
            
            OnThemeChanged();
        }

        public ColorPalette GetThemeColors(ThemeType themeType)
        {
            return themeType switch
            {
                ThemeType.Light => ColorPalette.Light,
                ThemeType.Dark => ColorPalette.Dark,
                ThemeType.Blue => ColorPalette.Blue,
                ThemeType.Green => ColorPalette.Green,
                ThemeType.Purple => ColorPalette.Purple,
                ThemeType.Custom => _customColors ?? ColorPalette.Light,
                _ => ColorPalette.Light
            };
        }

        private void OnThemeChanged()
        {
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(_currentTheme, _currentColors));
        }
    }
}