using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Core.Interfaces.Services;

namespace Presentation.WinFormsApp.UserControls.Common
{
    public enum ButtonStyle
    {
        Primary,
        Secondary,
        Outline,
        Ghost,
        Success,
        Warning,
        Error,
        Info
    }

    public enum ButtonSize
    {
        Small,
        Medium,
        Large,
        ExtraLarge
    }

    public partial class CustomButton : Button
    {
        private readonly IThemeService _themeService;
        private ButtonStyle _buttonStyle = ButtonStyle.Primary;
        private ButtonSize _buttonSize = ButtonSize.Medium;
        private bool _loading = false;
        private string _iconText = "";
        private System.Windows.Forms.Timer? _loadingTimer;
        private int _loadingAngle = 0;
        private bool _rounded = true;
        private int _borderRadius = 8;

        // Properties
        public ButtonStyle ButtonStyle
        {
            get => _buttonStyle;
            set
            {
                _buttonStyle = value;
                UpdateButtonStyle();
            }
        }

        public ButtonSize ButtonSize
        {
            get => _buttonSize;
            set
            {
                _buttonSize = value;
                UpdateButtonSize();
            }
        }

        public bool Loading
        {
            get => _loading;
            set
            {
                _loading = value;
                if (_loading)
                {
                    StartLoadingAnimation();
                    Enabled = false;
                }
                else
                {
                    StopLoadingAnimation();
                    Enabled = true;
                }
                Invalidate();
            }
        }

        public string IconText
        {
            get => _iconText;
            set
            {
                _iconText = value;
                UpdateButtonText();
            }
        }

        public bool Rounded
        {
            get => _rounded;
            set
            {
                _rounded = value;
                Invalidate();
            }
        }

        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = Math.Max(0, value);
                Invalidate();
            }
        }

        public CustomButton(IThemeService themeService)
        {
            _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
            
            InitializeCustomButton();
            SetupButton();
            UpdateButtonStyle();
            UpdateButtonSize();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeCustomButton()
        {
            SuspendLayout();
            
            // Basic button setup
            Name = "CustomButton";
            UseVisualStyleBackColor = false;
            FlatStyle = FlatStyle.Flat;
            Cursor = Cursors.Hand;
            
            // Remove default button appearance
            FlatAppearance.BorderSize = 0;
            FlatAppearance.BorderColor = Color.Transparent;
            
            // Set default properties
            Size = new Size(120, 40);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            
            ResumeLayout(false);
        }

        private void SetupButton()
        {
            // Enable double buffering for smooth rendering
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            
            // Event handlers
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            
            UpdateButtonText();
        }

        private void UpdateButtonSize()
        {
            switch (_buttonSize)
            {
                case ButtonSize.Small:
                    Size = new Size(80, 28);
                    Font = new Font("Segoe UI", 8F, FontStyle.Regular);
                    _borderRadius = 4;
                    break;
                case ButtonSize.Medium:
                    Size = new Size(120, 40);
                    Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                    _borderRadius = 8;
                    break;
                case ButtonSize.Large:
                    Size = new Size(150, 48);
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular);
                    _borderRadius = 10;
                    break;
                case ButtonSize.ExtraLarge:
                    Size = new Size(180, 56);
                    Font = new Font("Segoe UI", 14F, FontStyle.Regular);
                    _borderRadius = 12;
                    break;
            }
            Invalidate();
        }

        private void UpdateButtonText()
        {
            if (!string.IsNullOrEmpty(_iconText) && !string.IsNullOrEmpty(Text))
            {
                // Don't modify the actual Text property, handle in paint
            }
        }

        private void UpdateButtonStyle()
        {
            var colors = _themeService.CurrentColors;

            switch (_buttonStyle)
            {
                case ButtonStyle.Primary:
                    BackColor = colors.Primary;
                    ForeColor = colors.OnPrimary;
                    break;

                case ButtonStyle.Secondary:
                    BackColor = colors.Secondary;
                    ForeColor = colors.OnSecondary;
                    break;

                case ButtonStyle.Success:
                    BackColor = colors.Success;
                    ForeColor = Color.White;
                    break;

                case ButtonStyle.Warning:
                    BackColor = colors.Warning;
                    ForeColor = Color.White;
                    break;

                case ButtonStyle.Error:
                    BackColor = colors.Error;
                    ForeColor = Color.White;
                    break;

                case ButtonStyle.Info:
                    BackColor = colors.Info;
                    ForeColor = Color.White;
                    break;

                case ButtonStyle.Outline:
                    BackColor = Color.Transparent;
                    ForeColor = colors.Primary;
                    FlatAppearance.BorderSize = 1;
                    FlatAppearance.BorderColor = colors.Primary;
                    break;

                case ButtonStyle.Ghost:
                    BackColor = Color.Transparent;
                    ForeColor = colors.TextPrimary;
                    FlatAppearance.BorderSize = 0;
                    break;
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Get current colors
            var colors = _themeService.CurrentColors;
            var buttonRect = new Rectangle(0, 0, Width, Height);
            
            // Create rounded rectangle path
            using (var path = CreateRoundedRectangle(buttonRect, _rounded ? _borderRadius : 0))
            {
                // Fill background
                using (var brush = new SolidBrush(GetCurrentBackColor()))
                {
                    g.FillPath(brush, path);
                }

                // Draw border for outline style
                if (_buttonStyle == ButtonStyle.Outline)
                {
                    using (var pen = new Pen(GetCurrentForeColor(), 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            // Draw loading spinner if loading
            if (_loading)
            {
                DrawLoadingSpinner(g);
            }
            else
            {
                // Draw text and icon
                DrawButtonText(g);
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            var size = new Size(diameter, diameter);
            var arc = new Rectangle(rect.Location, size);

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        private Color GetCurrentBackColor()
        {
            if (!Enabled) return Color.Gray;
            
            // Handle different states
            if (ClientRectangle.Contains(PointToClient(MousePosition)))
            {
                return ControlPaint.Light(BackColor, 0.1f);
            }
            
            return BackColor;
        }

        private Color GetCurrentForeColor()
        {
            return Enabled ? ForeColor : Color.Gray;
        }

        private void DrawLoadingSpinner(Graphics g)
        {
            var centerX = Width / 2;
            var centerY = Height / 2;
            var radius = Math.Min(Width, Height) / 8;

            using (var pen = new Pen(GetCurrentForeColor(), 2))
            {
                var rect = new Rectangle(centerX - radius, centerY - radius, radius * 2, radius * 2);
                g.DrawArc(pen, rect, _loadingAngle, 90);
            }
        }

        private void DrawButtonText(Graphics g)
        {
            var displayText = !string.IsNullOrEmpty(_iconText) ? $"{_iconText} {Text}" : Text;
            
            if (string.IsNullOrEmpty(displayText)) return;

            using (var brush = new SolidBrush(GetCurrentForeColor()))
            {
                var stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                var textRect = new RectangleF(0, 0, Width, Height);
                g.DrawString(displayText, Font, brush, textRect, stringFormat);
            }
        }

        private void StartLoadingAnimation()
        {
            if (_loadingTimer == null)
            {
                _loadingTimer = new System.Windows.Forms.Timer();
                _loadingTimer.Interval = 50; // 50ms for smooth animation
                _loadingTimer.Tick += LoadingTimer_Tick;
            }
            _loadingTimer.Start();
        }

        private void StopLoadingAnimation()
        {
            _loadingTimer?.Stop();
            _loadingAngle = 0;
        }

        private void LoadingTimer_Tick(object? sender, EventArgs e)
        {
            _loadingAngle += 10;
            if (_loadingAngle >= 360) _loadingAngle = 0;
            Invalidate();
        }

        private void OnMouseEnter(object? sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnMouseLeave(object? sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            Invalidate();
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            Invalidate();
        }

        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateButtonStyle()));
            }
            else
            {
                UpdateButtonStyle();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _themeService.ThemeChanged -= OnThemeChanged;
                _loadingTimer?.Stop();
                _loadingTimer?.Dispose();
                
                // Remove event handlers
                MouseEnter -= OnMouseEnter;
                MouseLeave -= OnMouseLeave;
                MouseDown -= OnMouseDown;
                MouseUp -= OnMouseUp;
            }
            base.Dispose(disposing);
        }
    }
}