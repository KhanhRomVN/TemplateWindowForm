using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;
using Presentation.WinFormsApp.UserControls.Layouts;

namespace Presentation.WinFormsApp.Forms
{
    public partial class MainForm : Form
    {
        private readonly IThemeService _themeService;
        private readonly IRouterService _routerService;
        
        private MainLayout _mainLayout = null!;

        public MainForm(IThemeService themeService, IRouterService routerService)
        {
            _themeService = themeService;
            _routerService = routerService;
            
            InitializeComponent();
            SetupEventHandlers();
            SetupTheme();
            
            // Navigate to Home page by default
            _routerService.NavigateTo("Home");
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Main Layout (removed MenuStrip)
            _mainLayout = new MainLayout(_themeService, _routerService)
            {
                Dock = DockStyle.Fill
            };

            Controls.Add(_mainLayout);

            Text = "Template WinForms App";
            Size = new Size(1274, 620);
            StartPosition = FormStartPosition.CenterScreen;
            
            ResumeLayout(false);
            PerformLayout();
        }

        private void SetupEventHandlers()
        {
            _themeService.ThemeChanged += OnThemeChanged;
            _routerService.Navigated += OnNavigated;
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            BackColor = colors.Background;
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

        private void OnNavigated(object? sender, NavigatedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateContent(e.View)));
            }
            else
            {
                UpdateContent(e.View);
            }
        }

        private void UpdateContent(UserControl newView)
        {
            _mainLayout.MainContentPanel.Controls.Clear();
            newView.Dock = DockStyle.Fill;
            _mainLayout.MainContentPanel.Controls.Add(newView);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _themeService.ThemeChanged -= OnThemeChanged;
                _routerService.Navigated -= OnNavigated;
            }
            base.Dispose(disposing);
        }
    }
}