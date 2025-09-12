using System.Drawing;
using System.Windows.Forms;
using Core.Enums;
using Core.Interfaces.Services;

namespace Presentation.WinFormsApp.Forms
{
    public partial class MainForm : Form
    {
        private readonly IThemeService _themeService;
        private readonly IRouterService _routerService;
        
        private Panel _contentPanel = null!;
        private MenuStrip _menuStrip = null!;
        private ToolStripMenuItem _themeMenu = null!;
        private ToolStripMenuItem _navigationMenu = null!;

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

            // Menu Strip
            _menuStrip = new MenuStrip
            {
                Dock = DockStyle.Top
            };

            // Theme Menu
            _themeMenu = new ToolStripMenuItem("Themes");
            _themeMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("Light", null, (s, e) => _themeService.SetTheme(ThemeType.Light)),
                new ToolStripMenuItem("Dark", null, (s, e) => _themeService.SetTheme(ThemeType.Dark)),
                new ToolStripMenuItem("Blue", null, (s, e) => _themeService.SetTheme(ThemeType.Blue)),
                new ToolStripMenuItem("Green", null, (s, e) => _themeService.SetTheme(ThemeType.Green)),
                new ToolStripMenuItem("Purple", null, (s, e) => _themeService.SetTheme(ThemeType.Purple))
            });

            // Navigation Menu
            _navigationMenu = new ToolStripMenuItem("Navigation");
            _navigationMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("Home", null, (s, e) => _routerService.NavigateTo("Home")),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Back", null, (s, e) => _routerService.GoBack()) { Enabled = false },
                new ToolStripMenuItem("Forward", null, (s, e) => _routerService.GoForward()) { Enabled = false }
            });

            _menuStrip.Items.AddRange(new ToolStripItem[] { _themeMenu, _navigationMenu });

            // Content Panel
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            Controls.Add(_contentPanel);
            Controls.Add(_menuStrip);
            MainMenuStrip = _menuStrip;

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
            _contentPanel.BackColor = colors.Background;
            _menuStrip.BackColor = colors.Surface;
            _menuStrip.ForeColor = colors.OnSurface;
            
            // Apply theme to all menu items
            ApplyThemeToMenuItems(_menuStrip.Items);
        }

        private void ApplyThemeToMenuItems(ToolStripItemCollection items)
        {
            var colors = _themeService.CurrentColors;
            
            foreach (ToolStripItem item in items)
            {
                item.BackColor = colors.Surface;
                item.ForeColor = colors.OnSurface;
                
                if (item is ToolStripMenuItem menuItem)
                {
                    ApplyThemeToMenuItems(menuItem.DropDownItems);
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
            
            UpdateNavigationMenu();
        }

        private void UpdateContent(UserControl newView)
        {
            _contentPanel.Controls.Clear();
            newView.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(newView);
        }

        private void UpdateNavigationMenu()
        {
            var backItem = _navigationMenu.DropDownItems[2];
            var forwardItem = _navigationMenu.DropDownItems[3];
            
            backItem.Enabled = _routerService.CanGoBack;
            forwardItem.Enabled = _routerService.CanGoForward;
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