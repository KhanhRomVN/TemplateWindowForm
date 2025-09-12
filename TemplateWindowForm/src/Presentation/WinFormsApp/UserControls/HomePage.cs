using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;

namespace Presentation.WinFormsApp.UserControls
{
    public partial class HomePage : UserControl
    {
        private readonly IThemeService _themeService;
        private Label _helloLabel;
        private Panel _containerPanel;

        public HomePage(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent();
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Container Panel
            _containerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(50)
            };

            // Hello World Label
            _helloLabel = new Label
            {
                Text = "Hello World",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                AutoSize = false
            };

            _containerPanel.Controls.Add(_helloLabel);
            Controls.Add(_containerPanel);

            Name = "HomePage";
            Size = new Size(800, 600);
            
            ResumeLayout(false);
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            
            BackColor = colors.Background;
            _containerPanel.BackColor = colors.Background;
            _helloLabel.ForeColor = colors.TextPrimary;
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