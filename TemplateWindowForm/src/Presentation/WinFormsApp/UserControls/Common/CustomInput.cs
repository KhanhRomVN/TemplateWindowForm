using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;
using Guna.UI2.WinForms;

namespace Presentation.WinFormsApp.UserControls.Common
{
    public enum InputVariant
    {
        Default,
        Filled,
        Outlined,
        Underlined,
        Floating,
        Primary
    }

    public enum InputSize
    {
        Small,
        Medium,
        Large
    }

    public partial class EnhancedCustomInput : UserControl
    {
        private readonly IThemeService _themeService;
        private InputVariant _variant = InputVariant.Primary;
        private InputSize _inputSize = InputSize.Medium;
        private bool _isPasswordField = false;
        private bool _hasError = false;
        private bool _hasSuccess = false;
        private string _errorMessage = "";
        private string _successMessage = "";
        private string _hintMessage = "";
        private bool _showCharCount = false;
        private bool _loading = false;
        private string _prefix = "";
        private string _suffix = "";

        // Controls
        private Label _label = null!;
        private Guna2TextBox _inputTextBox = null!;
        private Guna2Button _passwordToggleButton = null!;
        private Panel _leftIconPanel = null!;
        private Panel _rightIconPanel = null!;
        private Label _messageLabel = null!;
        private Label _charCountLabel = null!;
        private PictureBox _statusIcon = null!;
        private Panel _loadingPanel = null!;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer? components = null;

        // Events
        public event EventHandler<string>? ValueChanged;

        // Properties
        public string Label
        {
            get => _label?.Text ?? string.Empty;
            set
            {
                if (_label != null)
                    _label.Text = value;
            }
        }

        public InputVariant Variant
        {
            get => _variant;
            set
            {
                _variant = value;
                UpdateInputStyle();
            }
        }

        public InputSize InputSize
        {
            get => _inputSize;
            set
            {
                _inputSize = value;
                UpdateInputSize();
            }
        }

        public bool IsPasswordField
        {
            get => _isPasswordField;
            set
            {
                _isPasswordField = value;
                UpdatePasswordField();
            }
        }

        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                _hasSuccess = false; // Clear success when error is set
                UpdateStatus();
            }
        }

        public bool HasSuccess
        {
            get => _hasSuccess;
            set
            {
                _hasSuccess = value;
                _hasError = false; // Clear error when success is set
                UpdateStatus();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                HasError = !string.IsNullOrEmpty(value);
            }
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                _successMessage = value;
                HasSuccess = !string.IsNullOrEmpty(value);
            }
        }

        public string HintMessage
        {
            get => _hintMessage;
            set
            {
                _hintMessage = value;
                UpdateMessage();
            }
        }

        public bool ShowCharCount
        {
            get => _showCharCount;
            set
            {
                _showCharCount = value;
                UpdateCharCount();
            }
        }

        public bool Loading
        {
            get => _loading;
            set
            {
                _loading = value;
                UpdateLoadingState();
            }
        }

        public string Prefix
        {
            get => _prefix;
            set
            {
                _prefix = value;
                UpdatePrefixSuffix();
            }
        }

        public string Suffix
        {
            get => _suffix;
            set
            {
                _suffix = value;
                UpdatePrefixSuffix();
            }
        }

        public string PlaceholderText
        {
            get => _inputTextBox?.PlaceholderText ?? string.Empty;
            set
            {
                if (_inputTextBox != null)
                    _inputTextBox.PlaceholderText = value;
            }
        }

        public string Value
        {
            get => _inputTextBox?.Text ?? string.Empty;
            set
            {
                if (_inputTextBox != null)
                    _inputTextBox.Text = value;
            }
        }

        public bool ReadOnly
        {
            get => _inputTextBox?.ReadOnly ?? false;
            set
            {
                if (_inputTextBox != null)
                    _inputTextBox.ReadOnly = value;
            }
        }

        public int MaxLength
        {
            get => _inputTextBox?.MaxLength ?? 0;
            set
            {
                if (_inputTextBox != null)
                    _inputTextBox.MaxLength = value;
            }
        }

        public Image? LeftIcon { get; set; }
        public Image? RightIcon { get; set; }

        public EnhancedCustomInput(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent();
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            Size = new Size(300, 90);
            BackColor = Color.Transparent;

            // Label
            _label = new Label
            {
                Text = "Input Label",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(0, 0),
                BackColor = Color.Transparent
            };

            // Input TextBox (using Guna2TextBox)
            _inputTextBox = new Guna2TextBox
            {
                Location = new Point(0, 25),
                Size = new Size(300, 40),
                Font = new Font("Segoe UI", 10F),
                BorderRadius = 8,
                BorderThickness = 1,
                PlaceholderText = "Enter text...",
                UseSystemPasswordChar = false
            };
            _inputTextBox.TextChanged += OnTextChanged;
            _inputTextBox.Enter += OnInputFocus;
            _inputTextBox.Leave += OnInputBlur;

            // Password Toggle Button
            _passwordToggleButton = new Guna2Button
            {
                Size = new Size(30, 30),
                Text = "👁",
                Font = new Font("Segoe UI", 10F),
                BorderRadius = 15,
                UseTransparentBackground = true,
                ShadowDecoration = { Enabled = false },
                Visible = false
            };
            _passwordToggleButton.Click += OnPasswordToggleClick;

            // Left Icon Panel
            _leftIconPanel = new Panel
            {
                Size = new Size(30, 30),
                BackColor = Color.Transparent,
                Visible = false
            };

            // Right Icon Panel
            _rightIconPanel = new Panel
            {
                Size = new Size(30, 30),
                BackColor = Color.Transparent,
                Visible = false
            };

            // Status Icon
            _statusIcon = new PictureBox
            {
                Size = new Size(16, 16),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent,
                Visible = false
            };

            // Loading Panel
            _loadingPanel = new Panel
            {
                Size = new Size(20, 20),
                BackColor = Color.Transparent,
                Visible = false
            };

            // Message Label (for error, success, hint)
            _messageLabel = new Label
            {
                Location = new Point(0, 70),
                Size = new Size(250, 16),
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                BackColor = Color.Transparent,
                Visible = false,
                AutoSize = true
            };

            // Character Count Label
            _charCountLabel = new Label
            {
                Location = new Point(250, 70),
                Size = new Size(50, 16),
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleRight,
                Visible = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            Controls.AddRange(new Control[] 
            { 
                _label, _inputTextBox, _passwordToggleButton, _leftIconPanel, 
                _rightIconPanel, _statusIcon, _loadingPanel, _messageLabel, _charCountLabel 
            });

            UpdateInputSize();
            UpdateInputStyle();

            Name = "EnhancedCustomInput";
            ResumeLayout(false);
        }

        private void UpdateInputSize()
        {
            switch (_inputSize)
            {
                case InputSize.Small:
                    _inputTextBox.Size = new Size(Width, 32);
                    _inputTextBox.Font = new Font("Segoe UI", 9F);
                    Height = 70;
                    break;
                case InputSize.Medium:
                    _inputTextBox.Size = new Size(Width, 40);
                    _inputTextBox.Font = new Font("Segoe UI", 10F);
                    Height = 90;
                    break;
                case InputSize.Large:
                    _inputTextBox.Size = new Size(Width, 48);
                    _inputTextBox.Font = new Font("Segoe UI", 12F);
                    Height = 100;
                    break;
            }

            // Update positions
            _messageLabel.Location = new Point(0, _inputTextBox.Bottom + 5);
            _charCountLabel.Location = new Point(Width - 50, _inputTextBox.Bottom + 5);
        }

        private void UpdateInputStyle()
        {
            var colors = _themeService.CurrentColors;

            switch (_variant)
            {
                case InputVariant.Default:
                    _inputTextBox.BorderColor = colors.Border;
                    _inputTextBox.FocusedState.BorderColor = colors.Primary;
                    _inputTextBox.FillColor = Color.White;
                    _inputTextBox.BorderRadius = 8;
                    break;

                case InputVariant.Filled:
                    _inputTextBox.BorderColor = Color.Transparent;
                    _inputTextBox.FocusedState.BorderColor = colors.Primary;
                    _inputTextBox.FillColor = Color.FromArgb(248, 249, 250);
                    _inputTextBox.BorderRadius = 8;
                    break;

                case InputVariant.Outlined:
                    _inputTextBox.BorderColor = colors.Border;
                    _inputTextBox.BorderThickness = 2;
                    _inputTextBox.FocusedState.BorderColor = colors.Primary;
                    _inputTextBox.FillColor = Color.Transparent;
                    _inputTextBox.BorderRadius = 8;
                    break;

                case InputVariant.Underlined:
                    _inputTextBox.BorderColor = colors.Border;
                    _inputTextBox.FocusedState.BorderColor = colors.Primary;
                    _inputTextBox.FillColor = Color.Transparent;
                    _inputTextBox.BorderRadius = 0;
                    break;

                case InputVariant.Primary:
                    _inputTextBox.BorderColor = colors.Border;
                    _inputTextBox.FocusedState.BorderColor = colors.Primary;
                    _inputTextBox.FillColor = colors.Surface;
                    _inputTextBox.BorderRadius = 8;
                    break;
            }
        }

        private void UpdatePasswordField()
        {
            _inputTextBox.UseSystemPasswordChar = _isPasswordField && !_passwordToggleButton.Checked;
            _passwordToggleButton.Visible = _isPasswordField;
            
            if (_isPasswordField)
            {
                PositionPasswordToggle();
            }
        }

        private void PositionPasswordToggle()
        {
            _passwordToggleButton.Location = new Point(
                _inputTextBox.Right - _passwordToggleButton.Width - 10,
                _inputTextBox.Top + (_inputTextBox.Height - _passwordToggleButton.Height) / 2
            );
            _passwordToggleButton.BringToFront();
        }

        private void UpdateStatus()
        {
            var colors = _themeService.CurrentColors;

            if (_hasError)
            {
                _inputTextBox.BorderColor = colors.Error;
                _inputTextBox.FocusedState.BorderColor = colors.Error;
                _statusIcon.Image = CreateStatusIcon(colors.Error, "✕");
                _statusIcon.Visible = true;
            }
            else if (_hasSuccess)
            {
                _inputTextBox.BorderColor = colors.Success;
                _inputTextBox.FocusedState.BorderColor = colors.Success;
                _statusIcon.Image = CreateStatusIcon(colors.Success, "✓");
                _statusIcon.Visible = true;
            }
            else
            {
                _statusIcon.Visible = false;
                UpdateInputStyle(); // Reset to normal colors
            }

            PositionStatusIcon();
            UpdateMessage();
        }

        private void PositionStatusIcon()
        {
            var rightOffset = 10;
            if (_isPasswordField) rightOffset += 40;
            
            _statusIcon.Location = new Point(
                _inputTextBox.Right - _statusIcon.Width - rightOffset,
                _inputTextBox.Top + (_inputTextBox.Height - _statusIcon.Height) / 2
            );
            _statusIcon.BringToFront();
        }

        private void UpdateMessage()
        {
            if (_hasError && !string.IsNullOrEmpty(_errorMessage))
            {
                _messageLabel.Text = $"⚠ {_errorMessage}";
                _messageLabel.ForeColor = _themeService.CurrentColors.Error;
                _messageLabel.Visible = true;
            }
            else if (_hasSuccess && !string.IsNullOrEmpty(_successMessage))
            {
                _messageLabel.Text = $"✓ {_successMessage}";
                _messageLabel.ForeColor = _themeService.CurrentColors.Success;
                _messageLabel.Visible = true;
            }
            else if (!string.IsNullOrEmpty(_hintMessage))
            {
                _messageLabel.Text = $"ℹ {_hintMessage}";
                _messageLabel.ForeColor = _themeService.CurrentColors.TextSecondary;
                _messageLabel.Visible = true;
            }
            else
            {
                _messageLabel.Visible = false;
            }
        }

        private void UpdateCharCount()
        {
            if (_showCharCount && MaxLength > 0)
            {
                var currentLength = _inputTextBox.Text.Length;
                _charCountLabel.Text = $"{currentLength}/{MaxLength}";
                _charCountLabel.ForeColor = currentLength > MaxLength * 0.9 
                    ? _themeService.CurrentColors.Warning 
                    : _themeService.CurrentColors.TextSecondary;
                _charCountLabel.Visible = true;
            }
            else
            {
                _charCountLabel.Visible = false;
            }
        }

        private void UpdateLoadingState()
        {
            _loadingPanel.Visible = _loading;
            if (_loading)
            {
                PositionLoadingPanel();
                StartLoadingAnimation();
            }
        }

        private void PositionLoadingPanel()
        {
            var rightOffset = 10;
            if (_isPasswordField) rightOffset += 40;
            if (_statusIcon.Visible) rightOffset += 26;
            
            _loadingPanel.Location = new Point(
                _inputTextBox.Right - _loadingPanel.Width - rightOffset,
                _inputTextBox.Top + (_inputTextBox.Height - _loadingPanel.Height) / 2
            );
        }

        private void UpdatePrefixSuffix()
        {
            // Update text box padding based on prefix/suffix
            // This would need custom implementation with Guna2TextBox
        }

        private Image CreateStatusIcon(Color color, string symbol)
        {
            var bitmap = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                
                using (var brush = new SolidBrush(color))
                using (var font = new Font("Segoe UI", 10F, FontStyle.Bold))
                {
                    var stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(symbol, font, brush, new RectangleF(0, 0, 16, 16), stringFormat);
                }
            }
            return bitmap;
        }

        private void StartLoadingAnimation()
        {
            // Simple loading animation implementation
            var timer = new System.Windows.Forms.Timer { Interval = 100 };
            var dots = 0;
            timer.Tick += (s, e) =>
            {
                if (!_loading)
                {
                    timer.Stop();
                    timer.Dispose();
                    return;
                }
                
                dots = (dots + 1) % 4;
                // Update loading visual here
            };
            timer.Start();
        }

        private void OnTextChanged(object? sender, EventArgs e)
        {
            UpdateCharCount();
            
            // Clear error when user starts typing
            if (_hasError && !string.IsNullOrEmpty(_inputTextBox.Text))
            {
                HasError = false;
            }

            ValueChanged?.Invoke(this, _inputTextBox.Text);
        }

        private void OnInputFocus(object? sender, EventArgs e)
        {
            // Add focus styling if needed
        }

        private void OnInputBlur(object? sender, EventArgs e)
        {
            // Add blur styling if needed
        }

        private void OnPasswordToggleClick(object? sender, EventArgs e)
        {
            _inputTextBox.UseSystemPasswordChar = !_inputTextBox.UseSystemPasswordChar;
            _passwordToggleButton.Text = _inputTextBox.UseSystemPasswordChar ? "👁" : "🙈";
        }

        public void SetError(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public void SetSuccess(string successMessage)
        {
            SuccessMessage = successMessage;
        }

        public void ClearError()
        {
            ErrorMessage = "";
        }

        public void ClearSuccess()
        {
            SuccessMessage = "";
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            BackColor = colors.Background;
            
            _label.ForeColor = colors.TextPrimary;
            _inputTextBox.ForeColor = colors.TextPrimary;
            
            UpdateInputStyle();
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

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _themeService.ThemeChanged -= OnThemeChanged;
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}