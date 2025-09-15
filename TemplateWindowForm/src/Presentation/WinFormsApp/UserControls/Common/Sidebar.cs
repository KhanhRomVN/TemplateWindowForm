using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;
using Guna.UI2.WinForms;

namespace Presentation.WinFormsApp.UserControls.Common
{
    public partial class Sidebar : UserControl
    {
        private readonly IThemeService _themeService;
        private readonly IRouterService _routerService;
        
        private Guna2Panel _headerSection = null!;
        private Guna2Panel _navigationSection = null!;
        private Guna2Panel _footerSection = null!;
        private Guna2TextBox _searchBox = null!;
        private Label _appTitleLabel = null!;
        private string _activeRoute = "Home";
        private List<Guna2Button> _navButtons = new();
        
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer? components = null;

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

            // Sidebar setup with modern styling
            Size = new Size(280, 700);
            BackColor = Color.White;

            // Padding đồng nhất 20px (trái, trên, phải, dưới)
            Padding = new Padding(20);
            Margin = new Padding(0);

            // Header Section (App Title + Search)
            _headerSection = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.Transparent,
                BorderRadius = 0
            };

            // App Title with simple label
            _appTitleLabel = new Label
            {
                Text = "Taskk",
                Font = new Font(FontFamily.GenericSansSerif, 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(0, 0),
                Size = new Size(230, 35),
                BackColor = Color.Transparent
            };

            // Modern Search Box with Guna2TextBox
            _searchBox = new Guna2TextBox
            {
                Location = new Point(0, 45),
                Size = new Size(230, 40),
                Font = new Font(FontFamily.GenericSansSerif, 9.5F),
                PlaceholderText = "Search...",
                PlaceholderForeColor = Color.FromArgb(125, 137, 149),
                ForeColor = Color.FromArgb(68, 88, 112),
                FillColor = Color.FromArgb(248, 249, 250),
                BorderRadius = 8,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(213, 218, 225),
                Cursor = Cursors.IBeam,
                DefaultText = ""
            };

            // Set focus and hover states
            _searchBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            _searchBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);

            _headerSection.Controls.Add(_appTitleLabel);
            _headerSection.Controls.Add(_searchBox);

            // Navigation Section with Guna2Panel
            _navigationSection = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = true,
                BorderRadius = 0
            };

            CreateNavigationButtons();

            // Footer Section (Settings) with reduced padding
            _footerSection = new Guna2Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55, // Reduced height
                BackColor = Color.Transparent,
                BorderRadius = 0
            };

            var settingsButton = CreateModernNavButton("Settingsssss", "⚙️", "Settings", 0);
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
                var button = CreateModernNavButton(item.Text, item.Icon, item.Route, item.Y);
                if (item.Route == "Home")
                {
                    SetActiveButton(button, true);
                }
                
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
                SetActiveButton(button, button.Tag?.ToString() == _activeRoute);
            }
        }
        
        private void SetActiveButton(Guna2Button button, bool isActive)
        {
            var isDark = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark;
            
            if (isActive)
            {
                button.FillColor = Color.FromArgb(37, 99, 235);
                button.ForeColor = Color.White;
                button.Font = new Font(FontFamily.GenericSansSerif, 9.5F, FontStyle.Bold);
                
                // Remove hover effects for active button
                button.HoverState.FillColor = Color.FromArgb(37, 99, 235);
                button.HoverState.ForeColor = Color.White;
            }
            else
            {
                button.FillColor = Color.Transparent;
                button.ForeColor = isDark ? Color.FromArgb(156, 163, 175) : Color.FromArgb(107, 114, 128);
                button.Font = new Font(FontFamily.GenericSansSerif, 9.5F, FontStyle.Regular);
                
                // Set hover effects for inactive buttons
                button.HoverState.FillColor = isDark ? Color.FromArgb(63, 63, 70) : Color.FromArgb(243, 244, 246);
                button.HoverState.ForeColor = isDark ? Color.FromArgb(229, 231, 235) : Color.FromArgb(75, 85, 99);
            }
        }
        
        private Guna2Button CreateModernNavButton(string text, string icon, string route, int yPosition)
        {
            var button = new Guna2Button
            {
                Text = $"  {icon}   {text}",
                Font = new Font(FontFamily.GenericSansSerif, 9.5F, FontStyle.Regular),
                Size = new Size(240, 42),
                Location = new Point(0, yPosition),
                Tag = route,
                FillColor = Color.Transparent,
                BorderRadius = 8,
                BorderThickness = 0,
                TextAlign = HorizontalAlignment.Left,
                Cursor = Cursors.Hand,
                UseTransparentBackground = true
            };
            
            // Set hover state colors
            button.HoverState.FillColor = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark 
                ? Color.FromArgb(63, 63, 70) 
                : Color.FromArgb(243, 244, 246);
            button.HoverState.ForeColor = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark 
                ? Color.FromArgb(229, 231, 235) 
                : Color.FromArgb(75, 85, 99);
            
            return button;
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
            var isDark = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark;
            
            if (isDark)
            {
                BackColor = Color.FromArgb(39, 39, 42);
                _headerSection.BackColor = Color.FromArgb(39, 39, 42);
                _navigationSection.BackColor = Color.FromArgb(39, 39, 42);
                _footerSection.BackColor = Color.FromArgb(39, 39, 42);
                
                _searchBox.FillColor = Color.FromArgb(63, 63, 70);
                _searchBox.ForeColor = Color.FromArgb(161, 161, 170);
                _searchBox.PlaceholderForeColor = Color.FromArgb(115, 115, 115);
                _searchBox.BorderColor = Color.FromArgb(82, 82, 89);
                _searchBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
                
                _appTitleLabel.ForeColor = Color.White;
            }
            else
            {
                BackColor = Color.White;
                _headerSection.BackColor = Color.White;
                _navigationSection.BackColor = Color.White;
                _footerSection.BackColor = Color.White;
                
                _searchBox.FillColor = Color.FromArgb(248, 249, 250);
                _searchBox.ForeColor = Color.FromArgb(68, 88, 112);
                _searchBox.PlaceholderForeColor = Color.FromArgb(125, 137, 149);
                _searchBox.BorderColor = Color.FromArgb(213, 218, 225);
                _searchBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
                
                _appTitleLabel.ForeColor = Color.FromArgb(30, 30, 30);
            }
            
            foreach (var button in _navButtons)
            {
                UpdateButtonTheme(button, isDark);
            }
        }
        
        private void UpdateButtonTheme(Guna2Button button, bool isDark)
        {
            var isActive = button.Tag?.ToString() == _activeRoute;
            
            if (!isActive)
            {
                button.ForeColor = isDark ? Color.FromArgb(156, 163, 175) : Color.FromArgb(107, 114, 128);
                button.HoverState.FillColor = isDark ? Color.FromArgb(63, 63, 70) : Color.FromArgb(243, 244, 246);
                button.HoverState.ForeColor = isDark ? Color.FromArgb(229, 231, 235) : Color.FromArgb(75, 85, 99);
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
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}