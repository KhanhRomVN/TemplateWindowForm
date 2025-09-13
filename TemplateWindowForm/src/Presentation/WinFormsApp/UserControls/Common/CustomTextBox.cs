using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;

namespace Presentation.WinFormsApp.UserControls.Common
{
    public partial class CustomTextBox : TextBox
    {
        private readonly IThemeService _themeService;
        private string _placeholderText = "";
        private bool _isPasswordField = false;
        private bool _hasError = false;
        private string _errorMessage = "";

        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                _placeholderText = value;
                SetPlaceholderVisibility();
            }
        }

        public bool IsPasswordField
        {
            get => _isPasswordField;
            set
            {
                _isPasswordField = value;
                UseSystemPasswordChar = value;
            }
        }

        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                UpdateErrorState();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => _errorMessage = value;
        }

        public CustomTextBox(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent();
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            // Modern textbox styling
            Font = new Font("Segoe UI", 11F);
            Size = new Size(200, 35);
            BorderStyle = BorderStyle.FixedSingle;
            
            // Event handlers
            Enter += OnTextBoxEnter;
            Leave += OnTextBoxLeave;
            TextChanged += OnTextBoxTextChanged;
            
            SetPlaceholderVisibility();
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            
            BackColor = colors.Surface;
            ForeColor = colors.TextPrimary;
            
            UpdateErrorState();
        }

        private void UpdateErrorState()
        {
            var colors = _themeService.CurrentColors;
            
            if (_hasError)
            {
                BackColor = Color.FromArgb(254, 242, 242); // Light red background
                ForeColor = colors.Error;
            }
            else
            {
                BackColor = colors.Surface;
                ForeColor = colors.TextPrimary;
            }
        }

        private void SetPlaceholderVisibility()
        {
            if (string.IsNullOrEmpty(Text) && !Focused && !string.IsNullOrEmpty(_placeholderText))
            {
                var colors = _themeService.CurrentColors;
                Text = _placeholderText;
                ForeColor = colors.TextSecondary;
            }
        }

        private void OnTextBoxEnter(object? sender, EventArgs e)
        {
            var colors = _themeService.CurrentColors;
            
            if (Text == _placeholderText)
            {
                Text = "";
                ForeColor = colors.TextPrimary;
            }
        }

        private void OnTextBoxLeave(object? sender, EventArgs e)
        {
            SetPlaceholderVisibility();
        }

        private void OnTextBoxTextChanged(object? sender, EventArgs e)
        {
            if (_hasError && !string.IsNullOrEmpty(Text) && Text != _placeholderText)
            {
                HasError = false; // Clear error when user starts typing
            }
        }

        public string GetActualText()
        {
            return Text == _placeholderText ? "" : Text;
        }

        public void SetError(string errorMessage)
        {
            ErrorMessage = errorMessage;
            HasError = true;
        }

        public void ClearError()
        {
            ErrorMessage = "";
            HasError = false;
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
                Enter -= OnTextBoxEnter;
                Leave -= OnTextBoxLeave;
                TextChanged -= OnTextBoxTextChanged;
            }
            base.Dispose(disposing);
        }
    }
}