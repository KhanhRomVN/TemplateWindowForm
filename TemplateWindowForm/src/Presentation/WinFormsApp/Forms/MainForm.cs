using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;
using Core.Enums;
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
            SetupTheme();
            SetupEventHandlers();
            
            // Navigate to Home page by default
            _routerService.NavigateTo("Home");
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Enhanced Main Layout
            _mainLayout = new MainLayout(_themeService, _routerService)
            {
                Dock = DockStyle.Fill
            };

            Controls.Add(_mainLayout);

            // Modern form settings
            Text = "Template Application";
            Size = new Size(1400, 800);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1200, 700);
            
            // Enhanced form appearance
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = true;
            WindowState = FormWindowState.Normal;
            
            ResumeLayout(false);
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            var isDark = _themeService.CurrentTheme == ThemeType.Dark;
            
            BackColor = colors.Background;
            
            // Apply modern form styling
            if (isDark)
            {
                BackColor = Color.FromArgb(24, 24, 27);
            }
            else
            {
                BackColor = Color.FromArgb(250, 250, 250);
            }
        }

        private void SetupEventHandlers()
        {
            _themeService.ThemeChanged += OnThemeChanged;
            _routerService.Navigated += OnNavigated;
            
            // Window control events
            Load += OnFormLoad;
        }

        private void OnFormLoad(object? sender, EventArgs e)
        {
            // Apply smooth fade-in animation
            Opacity = 0;
            var timer = new System.Windows.Forms.Timer { Interval = 20 }; // Fixed: Specify full namespace
            timer.Tick += (s, args) =>
            {
                Opacity += 0.05;
                if (Opacity >= 1.0)
                {
                    timer.Stop();
                    timer.Dispose();
                }
            };
            timer.Start();
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
            var mainContentPanel = _mainLayout.MainContentPanel;
            mainContentPanel.Controls.Clear();
            
            // Add smooth transition
            newView.Dock = DockStyle.Fill;
            newView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            
            // Fade in animation for content
            newView.Visible = false;
            mainContentPanel.Controls.Add(newView);
            
            var timer = new System.Windows.Forms.Timer { Interval = 10 }; // Fixed: Specify full namespace
            var opacity = 0.0;
            timer.Tick += (s, args) =>
            {
                opacity += 0.1;
                if (opacity >= 1.0)
                {
                    newView.Visible = true;
                    timer.Stop();
                    timer.Dispose();
                }
            };
            timer.Start();
            newView.Visible = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _themeService.ThemeChanged -= OnThemeChanged;
                _routerService.Navigated -= OnNavigated;
                Load -= OnFormLoad;
            }
            base.Dispose(disposing);
        }
    }
}