using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;
using Guna.UI2.WinForms;

namespace Presentation.WinFormsApp.UserControls.Common
{
    public enum ModalSize
    {
        Small,    // max-w-md (400px)
        Medium,   // max-w-lg (500px)
        Large,    // max-w-2xl (700px)
        XLarge,   // max-w-4xl (900px)
        XXLarge   // max-w-6xl (1200px)
    }

    public partial class CustomModal : Form
    {
        private readonly IThemeService _themeService;
        private ModalSize _modalSize = ModalSize.Large;
        private bool _hideFooter = false;
        private string _cancelText = "Cancel";
        private string _actionText = "";
        private bool _actionDisabled = false;
        private bool _actionLoading = false;

        // Controls
        private Panel _overlayPanel = null!;
        private Guna2Panel _modalPanel = null!;
        private Panel _headerPanel = null!;
        private Panel _contentPanel = null!;
        private Panel _footerPanel = null!;
        private Label _titleLabel = null!;
        private Guna2Button _closeButton = null!;
        private Panel _headerActionsPanel = null!;
        private FlowLayoutPanel _footerLeftPanel = null!;
        private FlowLayoutPanel _footerRightPanel = null!;
        private CustomButton _cancelButton = null!;
        private CustomButton _actionButton = null!;
        private System.Windows.Forms.Timer _animationTimer = null!;

        // Animation
        private int _animationStep = 0;
        private const int ANIMATION_STEPS = 10;
        private bool _isOpening = true;

        // Events
        public event EventHandler? ActionClicked;
        public event EventHandler? CancelClicked;

        // Properties
        public new ModalSize Size
        {
            get => _modalSize;
            set
            {
                _modalSize = value;
                UpdateModalSize();
            }
        }

        public string ModalTitle
        {
            get => _titleLabel?.Text ?? string.Empty;
            set
            {
                if (_titleLabel != null)
                    _titleLabel.Text = value;
            }
        }

        public Panel ContentPanel => _contentPanel;
        public Panel HeaderActionsPanel => _headerActionsPanel;
        public Panel FooterLeftPanel => _footerLeftPanel;

        public bool HideFooter
        {
            get => _hideFooter;
            set
            {
                _hideFooter = value;
                if (_footerPanel != null)
                    _footerPanel.Visible = !value;
            }
        }

        public string CancelText
        {
            get => _cancelText;
            set
            {
                _cancelText = value;
                if (_cancelButton != null)
                    _cancelButton.Text = value;
            }
        }

        public string ActionText
        {
            get => _actionText;
            set
            {
                _actionText = value;
                if (_actionButton != null)
                {
                    _actionButton.Text = value;
                    _actionButton.Visible = !string.IsNullOrEmpty(value);
                }
            }
        }

        public bool ActionDisabled
        {
            get => _actionDisabled;
            set
            {
                _actionDisabled = value;
                if (_actionButton != null)
                    _actionButton.Enabled = !value;
            }
        }

        public bool ActionLoading
        {
            get => _actionLoading;
            set
            {
                _actionLoading = value;
                if (_actionButton != null)
                    _actionButton.Loading = value;
            }
        }

        public CustomModal(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent(); // Use the standard designer method
            InitializeCustomModal(); // Then our custom initialization
            SetupTheme();

            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeCustomModal()
        {
            SuspendLayout();

            // Form properties
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            TopMost = true;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.Black;
            Opacity = 0;

            // Overlay Panel
            _overlayPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(150, 0, 0, 0), // Semi-transparent overlay
                Cursor = Cursors.Default
            };
            _overlayPanel.Click += OnOverlayClick;

            // Modal Panel (using Guna2Panel for better styling)
            _modalPanel = new Guna2Panel
            {
                BackColor = Color.White,
                BorderRadius = 12,
                ShadowDecoration =
                {
                    Enabled = true,
                    Color = Color.Black,
                    Depth = 25,
                    Shadow = new Padding(10, 10, 20, 20)
                },
                Size = new System.Drawing.Size(700, 500) // Default large size
            };

            // Header Panel
            _headerPanel = new Panel
            {
                Height = 70,
                Dock = DockStyle.Top,
                Padding = new Padding(20, 15, 20, 15),
                BackColor = Color.Transparent
            };

            // Title
            _titleLabel = new Label
            {
                Text = "Modal Title",
                Font = new Font("Segoe UI", 16F, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(0, 15),
                BackColor = Color.Transparent
            };

            // Header Actions Panel
            _headerActionsPanel = new Panel
            {
                Size = new System.Drawing.Size(150, 40),
                Location = new Point(0, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Transparent
            };

            // Close Button
            _closeButton = new Guna2Button
            {
                Text = "×",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Size = new System.Drawing.Size(40, 40),
                BorderRadius = 8,
                UseTransparentBackground = true,
                ShadowDecoration = { Enabled = false },
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _closeButton.Click += OnCloseButtonClick;

            // Position close button in header actions
            _headerActionsPanel.Controls.Add(_closeButton);
            _closeButton.Location = new Point(_headerActionsPanel.Width - _closeButton.Width, 0);

            // Add border line to header
            var headerBorder = new Panel
            {
                Height = 1,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(229, 231, 235)
            };
            _headerPanel.Controls.Add(headerBorder);

            _headerPanel.Controls.AddRange(new Control[] { _titleLabel, _headerActionsPanel });

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
                BackColor = Color.Transparent
            };

            // Add border line to footer
            var footerBorder = new Panel
            {
                Height = 1,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(229, 231, 235)
            };
            _footerPanel.Controls.Add(footerBorder);

            // Footer Left Panel (for additional info or actions)
            _footerLeftPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Location = new Point(0, 15),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };

            // Footer Right Panel (for main action buttons)
            _footerRightPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Location = new Point(0, 15),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            // Cancel Button (using existing CustomButton)
            _cancelButton = new CustomButton(_themeService)
            {
                Text = _cancelText,
                ButtonStyle = ButtonStyle.Secondary,
                ButtonSize = ButtonSize.Medium
            };
            _cancelButton.Click += OnCancelButtonClick;

            // Action Button (using existing CustomButton)
            _actionButton = new CustomButton(_themeService)
            {
                Text = _actionText,
                ButtonStyle = ButtonStyle.Primary,
                ButtonSize = ButtonSize.Medium,
                Visible = !string.IsNullOrEmpty(_actionText)
            };
            _actionButton.Click += OnActionButtonClick;

            _footerRightPanel.Controls.AddRange(new Control[] { _actionButton, _cancelButton });
            _footerPanel.Controls.AddRange(new Control[] { _footerLeftPanel, _footerRightPanel });

            // Position footer panels
            UpdateFooterLayout();

            // Assemble modal
            _modalPanel.Controls.Add(_contentPanel);
            _modalPanel.Controls.Add(_footerPanel);
            _modalPanel.Controls.Add(_headerPanel);

            Controls.Add(_modalPanel);
            Controls.Add(_overlayPanel);

            // Center modal panel
            CenterModalPanel();

            // Animation Timer
            _animationTimer = new System.Windows.Forms.Timer
            {
                Interval = 20 // 50 FPS
            };
            _animationTimer.Tick += OnAnimationTick;

            UpdateModalSize();

            Name = "CustomModal";
            Text = "Custom Modal";

            ResumeLayout(false);
        }

        private void UpdateModalSize()
        {
            if (_modalPanel == null) return;

            var screenBounds = Screen.PrimaryScreen.WorkingArea;
            System.Drawing.Size newSize;

            switch (_modalSize)
            {
                case ModalSize.Small:
                    newSize = new System.Drawing.Size(Math.Min(400, screenBounds.Width - 40), Math.Min(300, screenBounds.Height - 40));
                    break;
                case ModalSize.Medium:
                    newSize = new System.Drawing.Size(Math.Min(500, screenBounds.Width - 40), Math.Min(400, screenBounds.Height - 40));
                    break;
                case ModalSize.Large:
                    newSize = new System.Drawing.Size(Math.Min(700, screenBounds.Width - 40), Math.Min(500, screenBounds.Height - 40));
                    break;
                case ModalSize.XLarge:
                    newSize = new System.Drawing.Size(Math.Min(900, screenBounds.Width - 40), Math.Min(600, screenBounds.Height - 40));
                    break;
                case ModalSize.XXLarge:
                    newSize = new System.Drawing.Size(Math.Min(1200, screenBounds.Width - 40), Math.Min(700, screenBounds.Height - 40));
                    break;
                default:
                    newSize = new System.Drawing.Size(700, 500);
                    break;
            }

            _modalPanel.Size = newSize;
            CenterModalPanel();
            UpdateFooterLayout();
        }

        private void CenterModalPanel()
        {
            if (_modalPanel == null) return;

            var screenBounds = Screen.PrimaryScreen.WorkingArea;
            _modalPanel.Location = new Point(
                (screenBounds.Width - _modalPanel.Width) / 2,
                (screenBounds.Height - _modalPanel.Height) / 2
            );
        }

        private void UpdateFooterLayout()
        {
            if (_footerRightPanel == null) return;

            _footerRightPanel.Location = new Point(
                _footerPanel.Width - _footerRightPanel.Width - 20,
                15
            );
        }

        public new DialogResult ShowDialog()
        {
            _isOpening = true;
            _animationStep = 0;

            // Set initial state
            _modalPanel.Size = new System.Drawing.Size(0, 0);
            CenterModalPanel();

            base.Show();
            BringToFront();
            _animationTimer.Start();

            return base.ShowDialog();
        }

        public void HideModal()
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
                // Fade in overlay and scale in modal
                Opacity = progress * 0.8; // Max 80% opacity

                var targetSize = GetTargetSize();
                var currentWidth = (int)(targetSize.Width * EaseOutBack(progress));
                var currentHeight = (int)(targetSize.Height * EaseOutBack(progress));

                _modalPanel.Size = new System.Drawing.Size(currentWidth, currentHeight);
                CenterModalPanel();

                if (_animationStep >= ANIMATION_STEPS)
                {
                    _animationTimer.Stop();
                    Opacity = 0.8;
                    _modalPanel.Size = targetSize;
                    CenterModalPanel();
                }
            }
            else
            {
                // Fade out overlay and scale out modal
                var reverseProgress = 1.0f - progress;
                Opacity = reverseProgress * 0.8;

                var targetSize = GetTargetSize();
                var currentWidth = (int)(targetSize.Width * EaseInBack(reverseProgress));
                var currentHeight = (int)(targetSize.Height * EaseInBack(reverseProgress));

                _modalPanel.Size = new System.Drawing.Size(Math.Max(0, currentWidth), Math.Max(0, currentHeight));
                CenterModalPanel();

                if (_animationStep >= ANIMATION_STEPS)
                {
                    _animationTimer.Stop();
                    Hide();
                    DialogResult = DialogResult.Cancel;
                }
            }
        }

        private System.Drawing.Size GetTargetSize()
        {
            var screenBounds = Screen.PrimaryScreen.WorkingArea;

            return _modalSize switch
            {
                ModalSize.Small => new System.Drawing.Size(Math.Min(400, screenBounds.Width - 40), Math.Min(300, screenBounds.Height - 40)),
                ModalSize.Medium => new System.Drawing.Size(Math.Min(500, screenBounds.Width - 40), Math.Min(400, screenBounds.Height - 40)),
                ModalSize.Large => new System.Drawing.Size(Math.Min(700, screenBounds.Width - 40), Math.Min(500, screenBounds.Height - 40)),
                ModalSize.XLarge => new System.Drawing.Size(Math.Min(900, screenBounds.Width - 40), Math.Min(600, screenBounds.Height - 40)),
                ModalSize.XXLarge => new System.Drawing.Size(Math.Min(1200, screenBounds.Width - 40), Math.Min(700, screenBounds.Height - 40)),
                _ => new System.Drawing.Size(700, 500)
            };
        }

        // Easing functions for smooth animation
        private float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return 1 + c3 * (float)Math.Pow(t - 1, 3) + c1 * (float)Math.Pow(t - 1, 2);
        }

        private float EaseInBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return c3 * t * t * t - c1 * t * t;
        }

        private void OnOverlayClick(object? sender, EventArgs e)
        {
            HideModal();
        }

        private void OnCloseButtonClick(object? sender, EventArgs e)
        {
            HideModal();
        }

        private void OnCancelButtonClick(object? sender, EventArgs e)
        {
            CancelClicked?.Invoke(this, EventArgs.Empty);
            DialogResult = DialogResult.Cancel;
            HideModal();
        }

        private void OnActionButtonClick(object? sender, EventArgs e)
        {
            ActionClicked?.Invoke(this, EventArgs.Empty);
            DialogResult = DialogResult.OK;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                HideModal();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;

            // Modal panel
            _modalPanel.FillColor = colors.Surface;

            // Header
            _titleLabel.ForeColor = colors.TextPrimary;

            // Close button
            _closeButton.FillColor = Color.Transparent;
            _closeButton.ForeColor = colors.TextSecondary;
            _closeButton.HoverState.FillColor = Color.FromArgb(30, colors.Error.R, colors.Error.G, colors.Error.B);
            _closeButton.HoverState.ForeColor = colors.Error;

            // Content panel
            _contentPanel.BackColor = colors.Surface;

            // Footer panel
            _footerPanel.BackColor = colors.Surface;

            // Update border colors
            foreach (Control control in _headerPanel.Controls)
            {
                if (control is Panel panel && panel.Height == 1)
                {
                    panel.BackColor = colors.Border;
                }
            }

            foreach (Control control in _footerPanel.Controls)
            {
                if (control is Panel panel && panel.Height == 1)
                {
                    panel.BackColor = colors.Border;
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

        private void CleanupResources()
        {
            _themeService.ThemeChanged -= OnThemeChanged;
            _animationTimer?.Dispose();
        }
    }
}