using System.ComponentModel;
using System.Drawing;
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
        private TextBox _searchBox = null!;
        private Label _appTitleLabel = null!;
        private string _activeRoute = "Home";
        private List<Button> _navButtons = new();
        
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
            Padding = new Padding(20);
            Margin = new Padding(0);

            // Header Section (App Title + Search)
            _headerSection = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.Transparent
            };

            // App Title with simple label
            _appTitleLabel = new Label
            {
                Text = "Template App",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(0, 0),
                Size = new Size(230, 35),
                BackColor = Color.Transparent
            };

            // Modern Search Box
            _searchBox = new TextBox
            {
                Location = new Point(0, 45),
                Size = new Size(230, 30),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(68, 88, 112),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Search..."
            };

            // Add placeholder text behavior
            _searchBox.Enter += (s, e) => {
                if (_searchBox.Text == "Search...")
                {
                    _searchBox.Text = "";
                    _searchBox.ForeColor = Color.FromArgb(68, 88, 112);
                }
            };

            _searchBox.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(_searchBox.Text))
                {
                    _searchBox.Text = "Search...";
                    _searchBox.ForeColor = Color.FromArgb(125, 137, 149);
                }
            };

            _headerSection.Controls.Add(_appTitleLabel);
            _headerSection.Controls.Add(_searchBox);

            // Navigation Section
            _navigationSection = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = false
            };

            CreateNavigationButtons();

            // Assemble sidebar
            Controls.Add(_navigationSection);
            Controls.Add(_headerSection);

            Name = "Sidebar";
            
            ResumeLayout(false);
        }

        private void CreateNavigationButtons()
        {
            // Only 3 navigation items as requested
            var navigationItems = new[]
            {
                new { Text = "Home", Icon = "🏠", Route = "Home", Y = 20 },
                new { Text = "Tools", Icon = "🔧", Route = "Tool", Y = 70 },
                new { Text = "Settings", Icon = "⚙️", Route = "Settings", Y = 120 }
            };

            foreach (var item in navigationItems)
            {
                var button = CreateModernNavButton(item.Text, item.Icon, item.Route, item.Y);
                
                // Set Home as default active
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
            try
            {
                _routerService.NavigateTo(routeName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                // Optionally show a message to user
                MessageBox.Show($"Could not navigate to {routeName}", "Navigation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        
        private void SetActiveButton(Button button, bool isActive)
        {
            var isDark = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark;
            
            if (isActive)
            {
                button.BackColor = Color.FromArgb(37, 99, 235);
                button.ForeColor = Color.White;
                button.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            }
            else
            {
                button.BackColor = Color.Transparent;
                button.ForeColor = isDark ? Color.FromArgb(156, 163, 175) : Color.FromArgb(107, 114, 128);
                button.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            }
        }
        
        private Button CreateModernNavButton(string text, string icon, string route, int yPosition)
        {
            var button = new Button
            {
                Text = $"  {icon}   {text}",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                Size = new Size(240, 42),
                Location = new Point(0, yPosition),
                Tag = route,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            // Remove border
            button.FlatAppearance.BorderSize = 0;
            
            // Add hover effects
            button.MouseEnter += (s, e) => {
                if (button.Tag?.ToString() != _activeRoute)
                {
                    button.BackColor = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark 
                        ? Color.FromArgb(63, 63, 70) 
                        : Color.FromArgb(243, 244, 246);
                }
            };

            button.MouseLeave += (s, e) => {
                if (button.Tag?.ToString() != _activeRoute)
                {
                    button.BackColor = Color.Transparent;
                }
            };
            
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
                
                _searchBox.BackColor = Color.FromArgb(63, 63, 70);
                _searchBox.ForeColor = Color.FromArgb(161, 161, 170);
                
                _appTitleLabel.ForeColor = Color.White;
            }
            else
            {
                BackColor = Color.White;
                _headerSection.BackColor = Color.White;
                _navigationSection.BackColor = Color.White;
                
                _searchBox.BackColor = Color.FromArgb(248, 249, 250);
                _searchBox.ForeColor = Color.FromArgb(68, 88, 112);
                
                _appTitleLabel.ForeColor = Color.FromArgb(30, 30, 30);
            }
            
            foreach (var button in _navButtons)
            {
                UpdateButtonTheme(button, isDark);
            }
        }
        
        private void UpdateButtonTheme(Button button, bool isDark)
        {
            var isActive = button.Tag?.ToString() == _activeRoute;
            
            if (!isActive)
            {
                button.ForeColor = isDark ? Color.FromArgb(156, 163, 175) : Color.FromArgb(107, 114, 128);
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