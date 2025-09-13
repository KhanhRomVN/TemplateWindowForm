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
        Info,
        Outline,
        Ghost
    }

    public partial class CustomButton : Button
    {
        private readonly IThemeService _themeService;
        private ButtonStyle _buttonStyle = ButtonStyle.Primary;
        private string _iconText = "";
        private int _borderRadius = 8;

        public ButtonStyle ButtonStyle
        {
            get => _buttonStyle;
            set
            {
                _buttonStyle = value;
                UpdateButtonStyle();
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
            UpdateButtonStyle();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            // Modern button configuration
            Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Size = new Size(120, 40);
            Cursor = Cursors.Hand;
            FlatStyle = FlatStyle.Flat;
            UseVisualStyleBackColor = false;
            
            // Remove default border
            FlatAppearance.BorderSize = 0;
            
            // Enable custom painting
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);
        }

        private void UpdateButtonText()
        {
            if (!string.IsNullOrEmpty(_iconText) && !string.IsNullOrEmpty(Text))
            {
                base.Text = $"{_iconText} {Text}";
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Create rounded rectangle path
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            GraphicsPath path = GetRoundedRectanglePath(rect, _borderRadius);

            // Fill background
            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                g.FillPath(brush, path);
            }

            // Draw border if needed
            if (_buttonStyle == ButtonStyle.Outline)
            {
                using (Pen pen = new Pen(ForeColor, 2))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Draw text
            if (!string.IsNullOrEmpty(Text))
            {
                using (SolidBrush textBrush = new SolidBrush(ForeColor))
                {
                    StringFormat stringFormat = new StringFormat();
                    
                    // Handle TextAlign conversion
                    switch (TextAlign)
                    {
                        case ContentAlignment.TopLeft:
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.LineAlignment = StringAlignment.Near;
                            break;
                        case ContentAlignment.TopCenter:
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Near;
                            break;
                        case ContentAlignment.TopRight:
                            stringFormat.Alignment = StringAlignment.Far;
                            stringFormat.LineAlignment = StringAlignment.Near;
                            break;
                        case ContentAlignment.MiddleLeft:
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.LineAlignment = StringAlignment.Center;
                            break;
                        case ContentAlignment.MiddleCenter:
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Center;
                            break;
                        case ContentAlignment.MiddleRight:
                            stringFormat.Alignment = StringAlignment.Far;
                            stringFormat.LineAlignment = StringAlignment.Center;
                            break;
                        case ContentAlignment.BottomLeft:
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.LineAlignment = StringAlignment.Far;
                            break;
                        case ContentAlignment.BottomCenter:
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Far;
                            break;
                        case ContentAlignment.BottomRight:
                            stringFormat.Alignment = StringAlignment.Far;
                            stringFormat.LineAlignment = StringAlignment.Far;
                            break;
                        default:
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Center;
                            break;
                    }

                    g.DrawString(Text, Font, textBrush, rect, stringFormat);
                }
            }

            path.Dispose();
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

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

        private void UpdateButtonStyle()
        {
            var colors = _themeService.CurrentColors;

            switch (_buttonStyle)
            {
                case ButtonStyle.Primary:
                    BackColor = colors.Primary;
                    ForeColor = colors.OnPrimary;
                    FlatAppearance.BorderSize = 0;
                    break;

                case ButtonStyle.Secondary:
                    BackColor = colors.Secondary;
                    ForeColor = colors.OnSecondary;
                    FlatAppearance.BorderSize = 0;
                    break;

                case ButtonStyle.Success:
                    BackColor = colors.Success;
                    ForeColor = Color.White;
                    FlatAppearance.BorderSize = 0;
                    break;

                case ButtonStyle.Warning:
                    BackColor = colors.Warning;
                    ForeColor = Color.White;
                    FlatAppearance.BorderSize = 0;
                    break;

                case ButtonStyle.Error:
                    BackColor = colors.Error;
                    ForeColor = Color.White;
                    FlatAppearance.BorderSize = 0;
                    break;

                case ButtonStyle.Info:
                    BackColor = colors.Info;
                    ForeColor = Color.White;
                    FlatAppearance.BorderSize = 0;
                    break;

                case ButtonStyle.Outline:
                    BackColor = Color.Transparent;
                    ForeColor = colors.Primary;
                    FlatAppearance.BorderSize = 2;
                    FlatAppearance.BorderColor = colors.Primary;
                    break;

                case ButtonStyle.Ghost:
                    BackColor = Color.Transparent;
                    ForeColor = colors.TextPrimary;
                    FlatAppearance.BorderSize = 0;
                    break;
            }

            Invalidate(); // Trigger repaint
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            var colors = _themeService.CurrentColors;
            
            // Create hover effect
            switch (_buttonStyle)
            {
                case ButtonStyle.Primary:
                    BackColor = ControlPaint.Light(colors.Primary, 0.1f);
                    break;
                case ButtonStyle.Secondary:
                    BackColor = ControlPaint.Light(colors.Secondary, 0.1f);
                    break;
                case ButtonStyle.Outline:
                    BackColor = Color.FromArgb(10, colors.Primary.R, colors.Primary.G, colors.Primary.B);
                    break;
                case ButtonStyle.Ghost:
                    BackColor = Color.FromArgb(10, colors.TextPrimary.R, colors.TextPrimary.G, colors.TextPrimary.B);
                    break;
                default:
                    BackColor = ControlPaint.Light(BackColor, 0.1f);
                    break;
            }
            
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            UpdateButtonStyle(); // Reset to original colors
            base.OnMouseLeave(e);
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
            }
            base.Dispose(disposing);
        }
    }
}