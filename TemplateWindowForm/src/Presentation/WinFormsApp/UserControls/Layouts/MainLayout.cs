using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;
using Presentation.WinFormsApp.UserControls.Common;

namespace Presentation.WinFormsApp.UserControls.Layouts
{
    public partial class MainLayout : UserControl
    {
        private readonly IThemeService _themeService;
        private readonly IRouterService _routerService;
        
        private Sidebar _sidebar = null!;
        private Panel _mainContentPanel = null!;

        public Panel MainContentPanel => _mainContentPanel;

        public MainLayout(IThemeService themeService, IRouterService routerService)
        {
            _themeService = themeService;
            _routerService = routerService;
            
            InitializeComponent();
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            Size = new Size(1200, 700);
            
            // Sidebar Component
            _sidebar = new Sidebar(_themeService, _routerService)
            {
                Dock = DockStyle.Left
            };

            // Main Content Panel
            _mainContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(248, 250, 252),
                Margin = new Padding(0)
            };

            // Add panels to main control
            Controls.Add(_mainContentPanel);
            Controls.Add(_sidebar);

            Name = "MainLayout";
            
            ResumeLayout(false);
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            var isDark = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark;
            
            if (isDark)
            {
                BackColor = Color.FromArgb(24, 24, 27);
                _mainContentPanel.BackColor = Color.FromArgb(24, 24, 27);
            }
            else
            {
                BackColor = Color.FromArgb(248, 250, 252);
                _mainContentPanel.BackColor = Color.FromArgb(248, 250, 252);
            }
        }

        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetupTheme()));
            }
            else
            {
                SetupTheme();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _themeService.ThemeChanged -= OnThemeChanged;
            }
            base.Dispose(disposing);
        }
    }
}