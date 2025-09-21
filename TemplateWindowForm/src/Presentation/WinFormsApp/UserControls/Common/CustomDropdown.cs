using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Interfaces.Services;
using Guna.UI2.WinForms;

namespace Presentation.WinFormsApp.UserControls.Common
{
    public class DropdownOption
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public Image? Icon { get; set; }
        public bool Danger { get; set; } = false;
        public bool Disabled { get; set; } = false;
        public object? Tag { get; set; }
    }

    public enum DropdownAlign
    {
        Left,
        Right
    }

    public partial class CustomDropdown : Form
    {
        private readonly IThemeService _themeService;
        private List<DropdownOption> _options = new();
        private DropdownAlign _align = DropdownAlign.Right;
        private int _width = 150;
        private Point _showPosition;
        private Control? _parentControl;

        // Controls
        private Guna2Panel _dropdownPanel = null!;
        private System.Windows.Forms.Timer _animationTimer = null!;
        private int _animationStep = 0;
        private const int ANIMATION_STEPS = 8;

        // Events
        public event EventHandler<string>? OptionSelected;

        // Properties
        public List<DropdownOption> Options
        {
            get => _options;
            set
            {
                _options = value ?? new List<DropdownOption>();
                RefreshOptions();
            }
        }

        public DropdownAlign Align
        {
            get => _align;
            set => _align = value;
        }

        public int DropdownWidth
        {
            get => _width;
            set
            {
                _width = value;
                UpdateSize();
            }
        }

        public CustomDropdown(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent(); // Use the standard designer method
            InitializeDropdown(); // Then our custom initialization
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeDropdown()
        {
            SuspendLayout();

            // Form properties
            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.Magenta; // Will be made transparent
            TransparencyKey = Color.Magenta;
            Opacity = 0;

            // Dropdown Panel (using Guna2Panel for better styling)
            _dropdownPanel = new Guna2Panel
            {
                BackColor = Color.White,
                BorderRadius = 8,
                ShadowDecoration = 
                {
                    Enabled = true,
                    Color = Color.Black,
                    Depth = 15,
                    Shadow = new Padding(3, 3, 8, 8)
                },
                Padding = new Padding(5),
                AutoScroll = false
            };

            Controls.Add(_dropdownPanel);

            // Animation Timer
            _animationTimer = new System.Windows.Forms.Timer
            {
                Interval = 20 // 50 FPS
            };
            _animationTimer.Tick += OnAnimationTick;

            // Click outside to close
            Deactivate += (s, e) => HideDropdown();

            UpdateSize();

            Name = "CustomDropdown";
            
            ResumeLayout(false);
        }

        public void ShowDropdown(Control parentControl, Point position)
        {
            _parentControl = parentControl;
            _showPosition = position;
            
            UpdatePosition();
            RefreshOptions();
            
            // Start show animation
            _animationStep = 0;
            Show();
            BringToFront();
            _animationTimer.Start();
        }

        public void HideDropdown()
        {
            _animationTimer.Stop();
            Hide();
        }

        private void UpdatePosition()
        {
            if (_parentControl == null) return;

            var screenPosition = _parentControl.PointToScreen(_showPosition);
            
            // Adjust position based on alignment
            switch (_align)
            {
                case DropdownAlign.Right:
                    Location = new Point(screenPosition.X - Width, screenPosition.Y);
                    break;
                case DropdownAlign.Left:
                    Location = new Point(screenPosition.X, screenPosition.Y);
                    break;
            }

            // Ensure dropdown stays on screen
            var screen = Screen.FromPoint(screenPosition);
            var screenBounds = screen.WorkingArea;

            var adjustedLocation = Location;
            if (adjustedLocation.X < screenBounds.X)
                adjustedLocation.X = screenBounds.X;
            if (adjustedLocation.X + Width > screenBounds.Right)
                adjustedLocation.X = screenBounds.Right - Width;
            if (adjustedLocation.Y + Height > screenBounds.Bottom)
                adjustedLocation.Y = screenBounds.Bottom - Height;

            Location = adjustedLocation;
        }

        private void UpdateSize()
        {
            var totalHeight = Math.Max(50, _options.Count * 35 + 10); // 35px per item + padding
            Size = new Size(_width, totalHeight);
            _dropdownPanel.Size = Size;
        }

        private void RefreshOptions()
        {
            _dropdownPanel.Controls.Clear();
            
            if (!_options.Any())
            {
                var emptyLabel = new Label
                {
                    Text = "No options available",
                    Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    Location = new Point(10, 10),
                    AutoSize = true,
                    BackColor = Color.Transparent
                };
                _dropdownPanel.Controls.Add(emptyLabel);
                UpdateSize();
                return;
            }

            int y = 5;
            foreach (var option in _options)
            {
                var optionPanel = CreateOptionPanel(option, y);
                _dropdownPanel.Controls.Add(optionPanel);
                y += 35;
            }

            UpdateSize();
        }

        private Panel CreateOptionPanel(DropdownOption option, int y)
        {
            var colors = _themeService.CurrentColors;
            
            var panel = new Panel
            {
                Size = new Size(_width - 10, 30),
                Location = new Point(5, y),
                BackColor = Color.Transparent,
                Cursor = option.Disabled ? Cursors.Default : Cursors.Hand
            };

            // Icon
            if (option.Icon != null)
            {
                var iconBox = new PictureBox
                {
                    Image = option.Icon,
                    Size = new Size(16, 16),
                    Location = new Point(8, 7),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.Transparent
                };
                panel.Controls.Add(iconBox);
            }

            // Label
            var label = new Label
            {
                Text = option.Label,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Location = new Point(option.Icon != null ? 30 : 12, 7),
                Size = new Size(panel.Width - (option.Icon != null ? 35 : 17), 16),
                BackColor = Color.Transparent,
                ForeColor = option.Disabled 
                    ? Color.Gray 
                    : option.Danger 
                        ? colors.Error 
                        : colors.TextPrimary
            };

            panel.Controls.Add(label);

            if (!option.Disabled)
            {
                // Hover effects
                panel.MouseEnter += (s, e) => 
                {
                    panel.BackColor = option.Danger 
                        ? Color.FromArgb(30, colors.Error.R, colors.Error.G, colors.Error.B)
                        : Color.FromArgb(30, colors.Primary.R, colors.Primary.G, colors.Primary.B);
                };
                
                panel.MouseLeave += (s, e) => panel.BackColor = Color.Transparent;
                
                label.MouseEnter += (s, e) => 
                {
                    panel.BackColor = option.Danger 
                        ? Color.FromArgb(30, colors.Error.R, colors.Error.G, colors.Error.B)
                        : Color.FromArgb(30, colors.Primary.R, colors.Primary.G, colors.Primary.B);
                };
                
                label.MouseLeave += (s, e) => panel.BackColor = Color.Transparent;

                // Click handlers
                panel.Click += (s, e) => SelectOption(option);
                label.Click += (s, e) => SelectOption(option);
            }

            return panel;
        }

        private void SelectOption(DropdownOption option)
        {
            if (option.Disabled) return;

            OptionSelected?.Invoke(this, option.Value);
            HideDropdown();
        }

        private void OnAnimationTick(object? sender, EventArgs e)
        {
            _animationStep++;
            var progress = Math.Min(1.0f, (float)_animationStep / ANIMATION_STEPS);

            // Scale and fade in effect
            Opacity = progress * 0.95; // Max 95% opacity

            if (_animationStep >= ANIMATION_STEPS)
            {
                _animationTimer.Stop();
                Opacity = 0.95;
            }
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            var isDark = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark;

            // Dropdown panel
            _dropdownPanel.FillColor = colors.Surface;
            _dropdownPanel.BorderColor = colors.Border;

            RefreshOptions();
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