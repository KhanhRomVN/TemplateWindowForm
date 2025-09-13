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
        Success,
        Warning,
        Error,
        Info
    }

    public partial class CustomButton : Button
    {
        private readonly IThemeService _themeService;
        private ButtonStyle _buttonStyle = ButtonStyle.Primary;
        private int _borderRadius = 4;
        private bool _isHovered = false;

        public ButtonStyle ButtonStyle
        {
            get => _buttonStyle;
            set
            {
                _buttonStyle = value;
                UpdateButtonStyle();
            }
        }

        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value;
                Invalidate();
            }
        }

        public CustomButton(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);
            
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            
            UpdateButtonStyle();
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            Font = new Font("Segoe UI", 10F);
            Size = new Size(100, 35);
            Cursor = Cursors.Hand;
            
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
        }

        private void OnMouseEnter(object? sender, EventArgs e)
        {
            _isHovered = true;
            Invalidate();
        }

        private void OnMouseLeave(object? sender, EventArgs e)
        {
            _isHovered = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            var path = GetRoundedRectanglePath(rect, _borderRadius);

            // Get colors based on style and state
            var (backColor, foreColor) = GetButtonColors();

            // Fill background
            using (var brush = new SolidBrush(backColor))
            {
                g.FillPath(brush, path);
            }

            // Draw border if needed
            if (_borderRadius > 0)
            {
                var borderColor = _themeService.CurrentColors.Border;
                using (var pen = new Pen(borderColor, 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Draw text
            var textRect = new Rectangle(0, 0, Width, Height);
            var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
            
            TextRenderer.DrawText(g, Text, Font, textRect, foreColor, flags);

            path.Dispose();
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

        private (Color backColor, Color foreColor) GetButtonColors()
        {
            var colors = _themeService.CurrentColors;
            Color baseColor, textColor;

            switch (_buttonStyle)
            {
                case ButtonStyle.Primary:
                    baseColor = colors.Primary;
                    textColor = colors.OnPrimary;
                    break;
                case ButtonStyle.Secondary:
                    baseColor = colors.Secondary;
                    textColor = colors.OnSecondary;
                    break;
                case ButtonStyle.Success:
                    baseColor = colors.Success;
                    textColor = Color.White;
                    break;
                case ButtonStyle.Warning:
                    baseColor = colors.Warning;
                    textColor = Color.White;
                    break;
                case ButtonStyle.Error:
                    baseColor = colors.Error;
                    textColor = Color.White;
                    break;
                case ButtonStyle.Info:
                    baseColor = colors.Info;
                    textColor = Color.White;
                    break;
                default:
                    baseColor = colors.Primary;
                    textColor = colors.OnPrimary;
                    break;
            }

            // Apply hover effect
            if (_isHovered)
            {
                baseColor = ControlPaint.Light(baseColor, 0.2f);
            }

            return (baseColor, textColor);
        }

        private void UpdateButtonStyle()
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
                MouseEnter -= OnMouseEnter;
                MouseLeave -= OnMouseLeave;
            }
            base.Dispose(disposing);
        }
    }
}