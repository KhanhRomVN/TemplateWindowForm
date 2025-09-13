using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Core.Interfaces.Services;
using Presentation.WinFormsApp.UserControls.Common;

namespace Presentation.WinFormsApp.UserControls.Layouts
{
    public partial class MainLayout : UserControl
    {
        private readonly IThemeService _themeService;
        private readonly IRouterService _routerService;
        
        private RoundedPanel _sidebarPanel = null!;
        private Panel _mainContentPanel = null!;
        private Panel _sidebarContainer = null!;
        private CustomButton _homeButton = null!;
        private CustomButton _toolButton = null!;
        private CustomButton _settingsButton = null!; // Added missing declaration
        private Label _appTitleLabel = null!;

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

            // Main container setup
            Size = new Size(1200, 700);
            
            // Sidebar Panel with rounded corners
            _sidebarPanel = new RoundedPanel(_themeService)
            {
                Dock = DockStyle.Left,
                Width = 260,
                Padding = new Padding(15),
                BorderRadius = 4
            };

            // Sidebar Container for content
            _sidebarContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // App Title Label
            _appTitleLabel = new Label
            {
                Text = "Template App",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 70,
                AutoSize = false
            };

            // Navigation Buttons using CustomButton
            _homeButton = new CustomButton(_themeService)
            {
                Text = "🏠 Home",
                Font = new Font("Segoe UI", 12F),
                Height = 45,
                Dock = DockStyle.Top,
                ButtonStyle = ButtonStyle.Secondary,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 8, 0, 4)
            };
            _homeButton.Click += (s, e) => _routerService.NavigateTo("Home");

            _toolButton = new CustomButton(_themeService)
            {
                Text = "🔧 Tools",
                Font = new Font("Segoe UI", 12F),
                Height = 45,
                Dock = DockStyle.Top,
                ButtonStyle = ButtonStyle.Secondary,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 4, 0, 4)
            };
            _toolButton.Click += (s, e) => _routerService.NavigateTo("Tool");

            // Settings Button (positioned at bottom)
            _settingsButton = new CustomButton(_themeService)
            {
                Text = "⚙️ Settings",
                Font = new Font("Segoe UI", 12F),
                Height = 45,
                Dock = DockStyle.Bottom,
                ButtonStyle = ButtonStyle.Secondary,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 4, 0, 15)
            };
            _settingsButton.Click += (s, e) => _routerService.NavigateTo("Settings");

            // Add controls to sidebar container
            _sidebarContainer.Controls.Add(_settingsButton); // Add settings at bottom
            _sidebarContainer.Controls.Add(_toolButton);
            _sidebarContainer.Controls.Add(_homeButton);
            _sidebarContainer.Controls.Add(_appTitleLabel);

            // Add container to sidebar panel
            _sidebarPanel.Controls.Add(_sidebarContainer);

            // Main Content Panel with rounded corners and added margin
            _mainContentPanel = new RoundedPanel(_themeService)
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BorderRadius = 4,
                Margin = new Padding(4, 0, 0, 0) // Added 4px left margin for spacing
            };

            // Add panels to main control
            Controls.Add(_mainContentPanel);
            Controls.Add(_sidebarPanel);

            Name = "MainLayout";
            
            ResumeLayout(false);
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            
            BackColor = colors.Background;
            
            // Title styling
            _appTitleLabel.BackColor = Color.Transparent;
            _appTitleLabel.ForeColor = colors.Primary;
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

    // Extended RoundedPanel for MainLayout specific styling
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
            g.SmoothingMode = SmoothingMode.AntiAlias;

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

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            
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