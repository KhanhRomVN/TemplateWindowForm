using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;
using Guna.UI2.WinForms;

namespace Presentation.WinFormsApp.UserControls.Common
{
    public enum DrawerDirection
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public enum DrawerSize
    {
        Small,   // 25%
        Medium,  // 35%
        Large,   // 45%
        XLarge,  // 60%
        Full     // 100%
    }

    public partial class CustomDrawer : Form
    {
        private readonly IThemeService _themeService;
        private DrawerDirection _direction = DrawerDirection.Right;
        private DrawerSize _size = DrawerSize.Large;
        private bool _enableBlur = true;
        private bool _closeOnOverlayClick = true;
        private bool _showCloseButton = true;
        private bool _hideHeader = false;

        // Controls
        private Panel _overlayPanel = null!;
        private Guna2Panel _drawerPanel = null!;
        private Panel _headerPanel = null!;
        private Panel _contentPanel = null!;
        private Panel _footerPanel = null!;
        private Label _titleLabel = null!;
        private Label _subtitleLabel = null!;
        private Guna2Button _closeButton = null!;
        private System.Windows.Forms.Timer _animationTimer = null!; // Fixed: Use fully qualified name
        
        // Animation
        private int _animationStep = 0;
        private const int ANIMATION_STEPS = 10;
        private bool _isOpening = true;

        // Properties
        public DrawerDirection Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                UpdateDrawerLayout();
            }
        }

        public DrawerSize Size
        {
            get => _size;
            set
            {
                _size = value;
                UpdateDrawerLayout();
            }
        }

        public string DrawerTitle
        {
            get => _titleLabel?.Text ?? string.Empty;
            set
            {
                if (_titleLabel != null)
                    _titleLabel.Text = value;
            }
        }

        public string DrawerSubtitle
        {
            get => _subtitleLabel?.Text ?? string.Empty;
            set
            {
                if (_subtitleLabel != null)
                {
                    _subtitleLabel.Text = value;
                    _subtitleLabel.Visible = !string.IsNullOrEmpty(value);
                }
            }
        }

        public Panel ContentPanel => _contentPanel;
        public Panel HeaderActionsPanel { get; private set; } = null!;
        public Panel FooterActionsPanel => _footerPanel;

        public bool EnableBlur
        {
            get => _enableBlur;
            set => _enableBlur = value;
        }

        public bool CloseOnOverlayClick
        {
            get => _closeOnOverlayClick;
            set => _closeOnOverlayClick = value;
        }

        public bool ShowCloseButton
        {
            get => _showCloseButton;
            set
            {
                _showCloseButton = value;
                if (_closeButton != null)
                    _closeButton.Visible = value;
            }
        }

        public bool HideHeader
        {
            get => _hideHeader;
            set
            {
                _hideHeader = value;
                if (_headerPanel != null)
                    _headerPanel.Visible = !value;
            }
        }

        public CustomDrawer(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeCustomDrawer();
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeCustomDrawer()
        {
            SuspendLayout();

            // Form properties
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            TopMost = true;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.Black;
            Opacity = 0;

            // Overlay Panel
            _overlayPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(100, 0, 0, 0), // Semi-transparent overlay
                Cursor = Cursors.Default
            };
            _overlayPanel.Click += OnOverlayClick;

            // Drawer Panel (using Guna2Panel for better styling)
            _drawerPanel = new Guna2Panel
            {
                BackColor = Color.White,
                BorderRadius = 0,
                ShadowDecoration = 
                {
                    Enabled = true,
                    Color = Color.Black,
                    Depth = 20,
                    Shadow = new Padding(5, 0, 0, 0)
                }
            };

            // Header Panel
            _headerPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                Padding = new Padding(20, 15, 20, 15),
                BackColor = Color.Transparent
            };

            // Title
            _titleLabel = new Label
            {
                Text = "Drawer Title",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0),
                BackColor = Color.Transparent
            };

            // Subtitle
            _subtitleLabel = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(0, 30),
                BackColor = Color.Transparent,
                Visible = false
            };

            // Header Actions Panel
            HeaderActionsPanel = new Panel
            {
                Size = new Size(100, 50),
                Location = new Point(0, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Transparent
            };

            // Close Button
            _closeButton = new Guna2Button
            {
                Text = "×",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Size = new Size(40, 40),
                BorderRadius = 8,
                UseTransparentBackground = true,
                ShadowDecoration = { Enabled = false },
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _closeButton.Click += OnCloseButtonClick;

            // Position close button in header actions
            HeaderActionsPanel.Controls.Add(_closeButton);
            _closeButton.Location = new Point(HeaderActionsPanel.Width - _closeButton.Width, 5);

            _headerPanel.Controls.AddRange(new Control[] { _titleLabel, _subtitleLabel, HeaderActionsPanel });

            // Content Panel
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.Transparent,
                AutoScroll = true
            };

            // Footer Panel
            _footerPanel = new Panel
            {
                Height = 70,
                Dock = DockStyle.Bottom,
                Padding = new Padding(20, 15, 20, 15),
                BackColor = Color.Transparent,
                Visible = false
            };

            // Assemble drawer
            _drawerPanel.Controls.Add(_contentPanel);
            _drawerPanel.Controls.Add(_footerPanel);
            _drawerPanel.Controls.Add(_headerPanel);

            Controls.Add(_drawerPanel);
            Controls.Add(_overlayPanel);

            // Animation Timer
            _animationTimer = new System.Windows.Forms.Timer
            {
                Interval = 20 // 50 FPS
            };
            _animationTimer.Tick += OnAnimationTick;

            UpdateDrawerLayout();

            Name = "CustomDrawer";
            Text = "Custom Drawer";
            
            ResumeLayout(false);
        }

        private void UpdateDrawerLayout()
        {
            if (_drawerPanel == null) return;

            var screenBounds = Screen.PrimaryScreen.Bounds;
            var sizePercentages = new Dictionary<DrawerSize, float>
            {
                { DrawerSize.Small, 0.25f },
                { DrawerSize.Medium, 0.35f },
                { DrawerSize.Large, 0.45f },
                { DrawerSize.XLarge, 0.60f },
                { DrawerSize.Full, 1.0f }
            };

            var sizePercent = sizePercentages[_size];

            switch (_direction)
            {
                case DrawerDirection.Right:
                    var rightWidth = (int)(screenBounds.Width * sizePercent);
                    _drawerPanel.Size = new Size(rightWidth, screenBounds.Height);
                    _drawerPanel.Location = new Point(screenBounds.Width - rightWidth, 0);
                    _drawerPanel.ShadowDecoration.Shadow = new Padding(10, 0, 0, 0);
                    break;

                case DrawerDirection.Left:
                    var leftWidth = (int)(screenBounds.Width * sizePercent);
                    _drawerPanel.Size = new Size(leftWidth, screenBounds.Height);
                    _drawerPanel.Location = new Point(0, 0);
                    _drawerPanel.ShadowDecoration.Shadow = new Padding(0, 0, 10, 0);
                    break;

                case DrawerDirection.Top:
                    var topHeight = (int)(screenBounds.Height * sizePercent);
                    _drawerPanel.Size = new Size(screenBounds.Width, topHeight);
                    _drawerPanel.Location = new Point(0, 0);
                    _drawerPanel.ShadowDecoration.Shadow = new Padding(0, 0, 0, 10);
                    break;

                case DrawerDirection.Bottom:
                    var bottomHeight = (int)(screenBounds.Height * sizePercent);
                    _drawerPanel.Size = new Size(screenBounds.Width, bottomHeight);
                    _drawerPanel.Location = new Point(0, screenBounds.Height - bottomHeight);
                    _drawerPanel.ShadowDecoration.Shadow = new Padding(0, 10, 0, 0);
                    break;
            }

            // Update header actions panel position
            if (HeaderActionsPanel != null)
            {
                HeaderActionsPanel.Location = new Point(_headerPanel.Width - HeaderActionsPanel.Width - 20, 0);
            }
        }

        public void ShowDrawer()
        {
            _isOpening = true;
            _animationStep = 0;
            
            // Set initial position (off-screen)
            SetDrawerAnimationPosition(0);
            
            Show();
            BringToFront();
            _animationTimer.Start();
        }

        public void HideDrawer()
        {
            _isOpening = false;
            _animationStep = 0;
            _animationTimer.Start();
        }

        private void OnAnimationTick(object? sender, EventArgs e)
        {
            _animationStep++;
            var progress = (float)_animationStep / ANIMATION_STEPS;

            if (_isOpening)
            {
                // Fade in overlay and slide in drawer
                Opacity = progress * 0.7; // Max 70% opacity
                SetDrawerAnimationPosition(progress);

                if (_animationStep >= ANIMATION_STEPS)
                {
                    _animationTimer.Stop();
                    Opacity = 0.7;
                }
            }
            else
            {
                // Fade out overlay and slide out drawer
                var reverseProgress = 1.0f - progress;
                Opacity = reverseProgress * 0.7;
                SetDrawerAnimationPosition(reverseProgress);

                if (_animationStep >= ANIMATION_STEPS)
                {
                    _animationTimer.Stop();
                    Hide();
                }
            }
        }

        private void SetDrawerAnimationPosition(float progress)
        {
            if (_drawerPanel == null) return;

            var screenBounds = Screen.PrimaryScreen.Bounds;
            var targetLocation = _drawerPanel.Location; // Final position
            Point startLocation;

            // Calculate start position (off-screen)
            switch (_direction)
            {
                case DrawerDirection.Right:
                    startLocation = new Point(screenBounds.Width, 0);
                    break;
                case DrawerDirection.Left:
                    startLocation = new Point(-_drawerPanel.Width, 0);
                    break;
                case DrawerDirection.Top:
                    startLocation = new Point(0, -_drawerPanel.Height);
                    break;
                case DrawerDirection.Bottom:
                    startLocation = new Point(0, screenBounds.Height);
                    break;
                default:
                    startLocation = targetLocation;
                    break;
            }

            // Interpolate position
            var currentX = (int)(startLocation.X + (targetLocation.X - startLocation.X) * progress);
            var currentY = (int)(startLocation.Y + (targetLocation.Y - startLocation.Y) * progress);

            _drawerPanel.Location = new Point(currentX, currentY);
        }

        private void OnOverlayClick(object? sender, EventArgs e)
        {
            if (_closeOnOverlayClick)
            {
                HideDrawer();
            }
        }

        private void OnCloseButtonClick(object? sender, EventArgs e)
        {
            HideDrawer();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                HideDrawer();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            var isDark = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark;

            // Drawer panel
            _drawerPanel.FillColor = colors.Surface;

            // Header
            _titleLabel.ForeColor = colors.TextPrimary;
            _subtitleLabel.ForeColor = colors.TextSecondary;

            // Close button
            _closeButton.FillColor = Color.Transparent;
            _closeButton.ForeColor = colors.TextSecondary;
            _closeButton.HoverState.FillColor = Color.FromArgb(30, colors.Error.R, colors.Error.G, colors.Error.B);
            _closeButton.HoverState.ForeColor = colors.Error;

            // Content panel
            _contentPanel.BackColor = colors.Surface;

            // Footer panel
            _footerPanel.BackColor = colors.Surface;

            // Overlay
            _overlayPanel.BackColor = Color.FromArgb(100, 0, 0, 0);
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
                _animationTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}