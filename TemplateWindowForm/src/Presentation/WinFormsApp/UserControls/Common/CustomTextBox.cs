#nullable enable
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Core.Interfaces.Services;

namespace Presentation.WinFormsApp.UserControls.Common
{
    public partial class CustomTextBox : UserControl
    {
        private readonly IThemeService _themeService;
        private TextBox _textBox = null!;
        private int _borderRadius = 4;
        private string _placeholderText = string.Empty;
        private bool _isPasswordChar = false;

        public override string Text
        {
            get => _textBox?.Text ?? string.Empty;
            set
            {
                if (_textBox != null)
                    _textBox.Text = value ?? string.Empty;
            }
        }

        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                _placeholderText = value ?? string.Empty;
                if (_textBox != null && string.IsNullOrEmpty(_textBox.Text))
                {
                    ShowPlaceholder();
                }
            }
        }

        public bool IsPasswordChar
        {
            get => _isPasswordChar;
            set
            {
                _isPasswordChar = value;
                if (_textBox != null)
                    _textBox.UseSystemPasswordChar = value;
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

        public bool ReadOnly
        {
            get => _textBox?.ReadOnly ?? false;
            set
            {
                if (_textBox != null)
                    _textBox.ReadOnly = value;
            }
        }

        public override Font? Font
        {
            get => _textBox?.Font;
            set
            {
                if (_textBox != null && value != null)
                    _textBox.Font = value;
            }
        }

        public new event EventHandler? TextChanged;

        public CustomTextBox(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent();
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            Size = new Size(200, 35);
            
            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(8, 8),
                Multiline = false
            };

            _textBox.TextChanged += OnTextBoxTextChanged;
            _textBox.Enter += OnTextBoxEnter;
            _textBox.Leave += OnTextBoxLeave;

            Controls.Add(_textBox);

            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);

            Resize += OnResize;

            ResumeLayout(false);
        }

        private void OnResize(object? sender, EventArgs e)
        {
            if (_textBox != null)
            {
                _textBox.Size = new Size(Width - 16, Height - 16);
                _textBox.Location = new Point(8, (Height - _textBox.Height) / 2);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var colors = _themeService.CurrentColors;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            var path = GetRoundedRectanglePath(rect, _borderRadius);

            // Fill background
            using (var brush = new SolidBrush(colors.Surface))
            {
                g.FillPath(brush, path);
            }

            // Draw border
            var borderColor = (_textBox?.Focused ?? false) ? colors.Primary : colors.Border;
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawPath(pen, path);
            }

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

        private void OnTextBoxTextChanged(object? sender, EventArgs e)
        {
            TextChanged?.Invoke(this, e);
            Invalidate(); // Repaint border if needed
        }

        private void OnTextBoxEnter(object? sender, EventArgs e)
        {
            if (_textBox?.Text == _placeholderText)
            {
                HidePlaceholder();
            }
            Invalidate(); // Repaint with focus border
        }

        private void OnTextBoxLeave(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_textBox?.Text))
            {
                ShowPlaceholder();
            }
            Invalidate(); // Repaint without focus border
        }

        private void ShowPlaceholder()
        {
            if (_textBox != null && !string.IsNullOrEmpty(_placeholderText))
            {
                _textBox.Text = _placeholderText;
                _textBox.ForeColor = _themeService.CurrentColors.TextSecondary;
            }
        }

        private void HidePlaceholder()
        {
            if (_textBox?.Text == _placeholderText)
            {
                _textBox.Text = string.Empty;
                _textBox.ForeColor = _themeService.CurrentColors.TextPrimary;
            }
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            
            BackColor = Color.Transparent;
            if (_textBox != null)
            {
                _textBox.BackColor = colors.Surface;
                
                if (string.IsNullOrEmpty(_textBox.Text) || _textBox.Text == _placeholderText)
                {
                    _textBox.ForeColor = colors.TextSecondary;
                }
                else
                {
                    _textBox.ForeColor = colors.TextPrimary;
                }
            }
            
            Invalidate();
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
                
                if (_textBox != null)
                {
                    _textBox.TextChanged -= OnTextBoxTextChanged;
                    _textBox.Enter -= OnTextBoxEnter;
                    _textBox.Leave -= OnTextBoxLeave;
                }
                
                Resize -= OnResize;
            }
            base.Dispose(disposing);
        }
    }
}