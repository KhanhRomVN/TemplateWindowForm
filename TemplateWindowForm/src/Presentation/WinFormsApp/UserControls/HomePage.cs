using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;

namespace Presentation.WinFormsApp.UserControls
{
    public partial class HomePage : UserControl
    {
        private readonly IThemeService _themeService;

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

            // Simple, working layout
            var welcomeLabel = new Label
            {
                Text = "Welcome to Template Application",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(50, 50),
                BackColor = Color.Transparent
            };

            var descriptionLabel = new Label
            {
                Text = "This is a modern Windows Forms application template with theming support.",
                Font = new Font("Segoe UI", 12F),
                AutoSize = true,
                Location = new Point(50, 100),
                BackColor = Color.Transparent
            };

            var getStartedButton = new Button
            {
                Text = "Get Started",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Size = new Size(120, 40),
                Location = new Point(50, 150),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };
            getStartedButton.FlatAppearance.BorderSize = 0;

            Controls.Add(welcomeLabel);
            Controls.Add(descriptionLabel);
            Controls.Add(getStartedButton);

            Name = "HomePage";
            Size = new Size(800, 600);
            BackColor = Color.Transparent;
            
            ResumeLayout(false);
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            BackColor = colors.Background;

            // Update control colors
            foreach (Control control in Controls)
            {
                if (control is Label label)
                {
                    label.ForeColor = colors.TextPrimary;
                }
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