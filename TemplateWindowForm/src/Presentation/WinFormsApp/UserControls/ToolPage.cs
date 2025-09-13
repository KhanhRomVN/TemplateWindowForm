using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;
using Presentation.WinFormsApp.UserControls.Tool;

namespace Presentation.WinFormsApp.UserControls
{
    public partial class ToolPage : UserControl
    {
        private readonly IThemeService _themeService;
        private ToolTable _toolTable = null!;

        public ToolPage(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent();
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Tool Table
            _toolTable = new ToolTable(_themeService)
            {
                Dock = DockStyle.Fill
            };

            Controls.Add(_toolTable);

            Name = "ToolPage";
            Size = new Size(800, 600);
            
            ResumeLayout(false);
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