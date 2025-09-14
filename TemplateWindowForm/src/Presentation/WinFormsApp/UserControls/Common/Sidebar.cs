using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Core.Interfaces.Services;

namespace Presentation.WinFormsApp.UserControls.Common
{
    public partial class Sidebar : UserControl
    {
        private readonly IThemeService _themeService;
        private readonly IRouterService _routerService;
        
        private Panel _headerSection = null!;
        private Panel _navigationSection = null!;
        private Panel _footerSection = null!;
        private TextBox _searchBox = null!;
        private Label _appTitleLabel = null!;
        private string _activeRoute = "Home";
        private List<NavButton> _navButtons = new();

        public Sidebar(IThemeService themeService, IRouterService routerService)
        {
            _themeService = themeService;
            _routerService = routerService;
            
            InitializeComponent();
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
            _routerService.Navigated += OnRouteChanged;
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Sidebar setup
            Size = new Size(280, 700);
            BackColor = Color.White;
            Padding = new Padding(0);
            Margin = new Padding(0);

            // Header Section (App Title + Search)
            _headerSection = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                Padding = new Padding(20, 20, 20, 15),
                BackColor = Color.Transparent
            };

            // App Title
            _appTitleLabel = new Label
            {
                Text = "Taskk",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(0, 0),
                Size = new Size(240, 30),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Modern Search Box
            var searchContainer = new Panel
            {
                Location = new Point(0, 45),
                Size = new Size(240, 40),
                BackColor = Color.Transparent
            };

            _searchBox = new TextBox
            {
                Location = new Point(5, 5),
                Size = new Size(230, 30),
                Font = new Font("Segoe UI", 9F),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = Color.FromArgb(107, 114, 128),
                Text = "Search"
            };

            // Add search icon
            var searchIcon = new Label
            {
                Text = "🔍",
                Font = new Font("Segoe UI", 10F),
                Size = new Size(20, 20),
                Location = new Point(15, 10),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(107, 114, 128)
            };

            // Search box events
            _searchBox.Enter += (s, e) =>
            {
                if (_searchBox.Text == "Search")
                {
                    _searchBox.Text = "";
                    _searchBox.ForeColor = Color.FromArgb(31, 41, 55);
                }
            };

            _searchBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(_searchBox.Text))
                {
                    _searchBox.Text = "Search";
                    _searchBox.ForeColor = Color.FromArgb(107, 114, 128);
                }
            };

            searchContainer.Controls.Add(_searchBox);
            searchContainer.Controls.Add(searchIcon);
            searchIcon.BringToFront();

            _headerSection.Controls.Add(_appTitleLabel);
            _headerSection.Controls.Add(searchContainer);

            // Navigation Section
            _navigationSection = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 10, 16, 10),
                BackColor = Color.Transparent,
                AutoScroll = true
            };

            CreateNavigationButtons();

            // Footer Section (Settings)
            _footerSection = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                Padding = new Padding(16, 10, 16, 15),
                BackColor = Color.Transparent
            };

            var settingsButton = new NavButton(_themeService)
            {
                Text = "Settings",
                IconText = "⚙️",
                Location = new Point(0, 0),
                Size = new Size(248, 40),
                Tag = "Settings"
            };
            settingsButton.Click += (s, e) => NavigateToRoute("Settings");
            _navButtons.Add(settingsButton);

            _footerSection.Controls.Add(settingsButton);

            // Assemble sidebar
            Controls.Add(_navigationSection);
            Controls.Add(_footerSection);
            Controls.Add(_headerSection);

            Name = "Sidebar";
            
            ResumeLayout(false);
        }

        private void CreateNavigationButtons()
        {
            var navigationItems = new[]
            {
                new { Text = "Home", Icon = "🏠", Route = "Home", Y = 0 },
                new { Text = "Tasks", Icon = "📋", Route = "Tool", Y = 46 },
                new { Text = "Docs", Icon = "📄", Route = "Docs", Y = 92 },
                new { Text = "Schedule", Icon = "📅", Route = "Schedule", Y = 138 },
                new { Text = "Chat", Icon = "💬", Route = "Chat", Y = 184 },
                new { Text = "Payments", Icon = "💳", Route = "Payments", Y = 230 },
                new { Text = "Customers", Icon = "👥", Route = "Customers", Y = 276 },
                new { Text = "Automations", Icon = "⚡", Route = "Automations", Y = 322 },
                new { Text = "User Management", Icon = "👤", Route = "UserManagement", Y = 368 },
                new { Text = "Workflows", Icon = "🔄", Route = "Workflows", Y = 414 }
            };

            foreach (var item in navigationItems)
            {
                var button = new NavButton(_themeService)
                {
                    Text = item.Text,
                    IconText = item.Icon,
                    Location = new Point(0, item.Y),
                    Size = new Size(248, 40),
                    Tag = item.Route,
                    IsActive = item.Route == "Home"
                };

                button.Click += (s, e) => NavigateToRoute(item.Route);
                _navButtons.Add(button);
                _navigationSection.Controls.Add(button);
            }
        }

        private void NavigateToRoute(string routeName)
        {
            // Only navigate if route is implemented
            if (routeName == "Home" || routeName == "Tool" || routeName == "Settings")
            {
                _routerService.NavigateTo(routeName);
            }
        }

        private void OnRouteChanged(object? sender, NavigatedEventArgs e)
        {
            _activeRoute = e.RouteName;
            UpdateActiveButton();
        }

        private void UpdateActiveButton()
        {
            foreach (var button in _navButtons)
            {
                button.IsActive = button.Tag?.ToString() == _activeRoute;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Draw right border
            var g = e.Graphics;
            using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
            {
                g.DrawLine(pen, Width - 1, 0, Width - 1, Height);
            }
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            var isDark = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark;
            
            if (isDark)
            {
                BackColor = Color.FromArgb(39, 39, 42);
                _searchBox.BackColor = Color.FromArgb(63, 63, 70);
                _searchBox.ForeColor = Color.FromArgb(161, 161, 170);
                _appTitleLabel.ForeColor = Color.White;
            }
            else
            {
                BackColor = Color.White;
                _searchBox.BackColor = Color.FromArgb(248, 249, 250);
                _searchBox.ForeColor = Color.FromArgb(107, 114, 128);
                _appTitleLabel.ForeColor = Color.FromArgb(30, 30, 30);
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
                _routerService.Navigated -= OnRouteChanged;
            }
            base.Dispose(disposing);
        }
    }

    // Navigation Button for Sidebar
    public class NavButton : Button
    {
        private readonly IThemeService _themeService;
        private bool _isActive = false;
        private string _iconText = "";
        private bool _isHovered = false;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                UpdateAppearance();
                Invalidate();
            }
        }

        public string IconText
        {
            get => _iconText;
            set => _iconText = value;
        }

        public NavButton(IThemeService themeService)
        {
            _themeService = themeService;
            
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);
            
            Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            TextAlign = ContentAlignment.MiddleLeft;
            Cursor = Cursors.Hand;
            Margin = new Padding(0, 3, 0, 3);
            
            _themeService.ThemeChanged += OnThemeChanged;
            UpdateAppearance();
        }

        private void UpdateAppearance()
        {
            var isDark = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark;
            
            if (_isActive)
            {
                BackColor = Color.FromArgb(37, 99, 235);
                ForeColor = Color.White;
                Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }
            else if (_isHovered)
            {
                BackColor = isDark ? Color.FromArgb(63, 63, 70) : Color.FromArgb(243, 244, 246);
                ForeColor = isDark ? Color.FromArgb(229, 231, 235) : Color.FromArgb(75, 85, 99);
                Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }
            else
            {
                BackColor = Color.Transparent;
                ForeColor = isDark ? Color.FromArgb(156, 163, 175) : Color.FromArgb(107, 114, 128);
                Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Background
            if (_isActive || _isHovered)
            {
                var rect = new Rectangle(4, 2, Width - 8, Height - 4);
                using (var brush = new SolidBrush(BackColor))
                {
                    var path = GetRoundedRectPath(rect, 6);
                    g.FillPath(brush, path);
                }
            }

            // Draw icon and text
            if (!string.IsNullOrEmpty(_iconText) && !string.IsNullOrEmpty(Text))
            {
                using (var textBrush = new SolidBrush(ForeColor))
                {
                    // Draw icon
                    var iconRect = new Rectangle(16, 0, 20, Height);
                    var iconFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(_iconText, Font, textBrush, iconRect, iconFormat);

                    // Draw text
                    var textRect = new Rectangle(44, 0, Width - 60, Height);
                    var textFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(Text, Font, textBrush, textRect, textFormat);
                }
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            var arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

            // Top left
            path.AddArc(arcRect, 180, 90);

            // Top right
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);

            // Bottom right
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);

            // Bottom left
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();
            return path;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            UpdateAppearance();
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            UpdateAppearance();
            Invalidate();
            base.OnMouseLeave(e);
        }

        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateAppearance()));
            }
            else
            {
                UpdateAppearance();
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