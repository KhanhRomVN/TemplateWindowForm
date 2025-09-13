using System.Drawing;
using System.Windows.Forms;
using Core.Enums;
using Core.Interfaces.Services;
using Presentation.WinFormsApp.UserControls.Common;

namespace Presentation.WinFormsApp.UserControls
{
    public partial class SettingsPage : UserControl
    {
        private readonly IThemeService _themeService;
        private RoundedPanel _settingsPanel = null!;
        private Label _titleLabel = null!;
        private Label _themeLabel = null!;
        private ComboBox _themeComboBox = null!;

        public SettingsPage(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent();
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Main Settings Panel
            _settingsPanel = new RoundedPanel(_themeService)
            {
                Location = new Point(20, 20),
                Size = new Size(400, 200),
                BorderRadius = 8,
                Padding = new Padding(20)
            };

            // Title Label
            _titleLabel = new Label
            {
                Text = "Settings",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Location = new Point(0, 0),
                Size = new Size(360, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Theme Label
            _themeLabel = new Label
            {
                Text = "Theme:",
                Font = new Font("Segoe UI", 12F),
                Location = new Point(0, 60),
                Size = new Size(80, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Theme ComboBox
            _themeComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(90, 58),
                Size = new Size(150, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };

            // Populate theme options
            _themeComboBox.Items.AddRange(new object[]
            {
                "Light",
                "Dark", 
                "Blue",
                "Green",
                "Purple"
            });

            // Set current theme as selected
            _themeComboBox.SelectedItem = _themeService.CurrentTheme.ToString();
            _themeComboBox.SelectedIndexChanged += OnThemeSelectionChanged;

            // Add controls to settings panel
            _settingsPanel.Controls.Add(_titleLabel);
            _settingsPanel.Controls.Add(_themeLabel);
            _settingsPanel.Controls.Add(_themeComboBox);

            // Add panel to main control
            Controls.Add(_settingsPanel);

            Name = "SettingsPage";
            Size = new Size(800, 600);
            
            ResumeLayout(false);
        }

        private void OnThemeSelectionChanged(object? sender, EventArgs e)
        {
            if (_themeComboBox.SelectedItem != null)
            {
                var selectedTheme = _themeComboBox.SelectedItem.ToString();
                if (Enum.TryParse<ThemeType>(selectedTheme, out var themeType))
                {
                    _themeService.SetTheme(themeType);
                }
            }
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            
            BackColor = colors.Background;
            _titleLabel.ForeColor = colors.TextPrimary;
            _themeLabel.ForeColor = colors.TextPrimary;
            
            // Style the ComboBox
            _themeComboBox.BackColor = colors.Surface;
            _themeComboBox.ForeColor = colors.OnSurface;
        }

        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => {
                    SetupTheme();
                    // Update selected item without triggering the event
                    _themeComboBox.SelectedIndexChanged -= OnThemeSelectionChanged;
                    _themeComboBox.SelectedItem = e.ThemeType.ToString();
                    _themeComboBox.SelectedIndexChanged += OnThemeSelectionChanged;
                }));
            }
            else
            {
                SetupTheme();
                // Update selected item without triggering the event
                _themeComboBox.SelectedIndexChanged -= OnThemeSelectionChanged;
                _themeComboBox.SelectedItem = e.ThemeType.ToString();
                _themeComboBox.SelectedIndexChanged += OnThemeSelectionChanged;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _themeService.ThemeChanged -= OnThemeChanged;
                if (_themeComboBox != null)
                {
                    _themeComboBox.SelectedIndexChanged -= OnThemeSelectionChanged;
                }
            }
            base.Dispose(disposing);
        }
    }

    // RoundedPanel for Settings (reusing the class from MainLayout)
    public class RoundedPanel : Panel
    {
        private readonly IThemeService _themeService;
        private int _borderRadius = 4;

        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value;
                Invalidate();
            }
        }

        public RoundedPanel(IThemeService themeService)
        {
            _themeService = themeService;
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var colors = _themeService.CurrentColors;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            if (_borderRadius > 0)
            {
                var path = GetRoundedRectanglePath(rect, _borderRadius);

                // Fill background
                using (var brush = new SolidBrush(colors.Surface))
                {
                    g.FillPath(brush, path);
                }

                // Draw subtle border
                using (var pen = new Pen(colors.Border, 1))
                {
                    g.DrawPath(pen, path);
                }

                path.Dispose();
            }
            else
            {
                // Fallback to regular rectangle
                using (var brush = new SolidBrush(colors.Surface))
                {
                    g.FillRectangle(brush, rect);
                }

                using (var pen = new Pen(colors.Border, 1))
                {
                    g.DrawRectangle(pen, rect);
                }
            }
        }

        private System.Drawing.Drawing2D.GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            var arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

            // Top left arc
            path.AddArc(arcRect, 180, 90);

            // Top right arc
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);

            // Bottom right arc
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);

            // Bottom left arc
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();
            return path;
        }

        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Invalidate()));
            }
            else
            {
                Invalidate();
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