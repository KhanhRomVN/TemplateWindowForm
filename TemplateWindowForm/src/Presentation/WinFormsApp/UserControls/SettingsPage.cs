using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;
using Core.Enums;
using Core.ValueObjects;
using Presentation.WinFormsApp.UserControls.Common;

namespace Presentation.WinFormsApp.UserControls
{
    public partial class SettingsPage : UserControl
    {
        private readonly IThemeService _themeService;
        private Panel _settingsContainer = null!;
        private Panel _themeSection = null!;
        private Panel _generalSection = null!;
        private Label _titleLabel = null!;
        private Label _themeSectionTitle = null!;
        private Label _generalSectionTitle = null!;
        private CustomButton _lightThemeButton = null!;
        private CustomButton _darkThemeButton = null!;
        private CustomButton _blueThemeButton = null!;
        private CustomButton _greenThemeButton = null!;
        private CustomButton _purpleThemeButton = null!;
        private CheckBox _startMinimizedCheckBox = null!;
        private CheckBox _autoUpdateCheckBox = null!;
        private CheckBox _notificationsCheckBox = null!;
        private Label _currentThemeLabel = null!;

        public SettingsPage(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent();
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Main container with scroll support
            _settingsContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(40, 30, 40, 30)
            };

            // Title
            _titleLabel = new Label
            {
                Text = "Settings",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0),
                Margin = new Padding(0, 0, 0, 30),
                BackColor = Color.Transparent
            };

            // Theme Section
            _themeSection = CreateSection("Theme Settings", 60);
            _themeSectionTitle = _themeSection.Controls.OfType<Label>().First();

            // Current theme display
            _currentThemeLabel = new Label
            {
                Text = $"Current Theme: {_themeService.CurrentTheme}",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(0, 40),
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 20)
            };

            // Theme buttons layout
            var themeButtonsPanel = new FlowLayoutPanel
            {
                Location = new Point(0, 80),
                Size = new Size(_themeSection.Width - 40, 100),
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            _lightThemeButton = new CustomButton(_themeService)
            {
                Text = "Light",
                Size = new Size(100, 40),
                ButtonStyle = ButtonStyle.Outline,
                Margin = new Padding(0, 0, 10, 10)
            };
            _lightThemeButton.Click += (s, e) => ChangeTheme(ThemeType.Light);

            _darkThemeButton = new CustomButton(_themeService)
            {
                Text = "Dark",
                Size = new Size(100, 40),
                ButtonStyle = ButtonStyle.Outline,
                Margin = new Padding(0, 0, 10, 10)
            };
            _darkThemeButton.Click += (s, e) => ChangeTheme(ThemeType.Dark);

            _blueThemeButton = new CustomButton(_themeService)
            {
                Text = "Blue",
                Size = new Size(100, 40),
                ButtonStyle = ButtonStyle.Outline,
                Margin = new Padding(0, 0, 10, 10)
            };
            _blueThemeButton.Click += (s, e) => ChangeTheme(ThemeType.Blue);

            _greenThemeButton = new CustomButton(_themeService)
            {
                Text = "Green",
                Size = new Size(100, 40),
                ButtonStyle = ButtonStyle.Outline,
                Margin = new Padding(0, 0, 10, 10)
            };
            _greenThemeButton.Click += (s, e) => ChangeTheme(ThemeType.Green);

            _purpleThemeButton = new CustomButton(_themeService)
            {
                Text = "Purple",
                Size = new Size(100, 40),
                ButtonStyle = ButtonStyle.Outline,
                Margin = new Padding(0, 0, 10, 10)
            };
            _purpleThemeButton.Click += (s, e) => ChangeTheme(ThemeType.Purple);

            themeButtonsPanel.Controls.AddRange(new Control[] {
                _lightThemeButton, _darkThemeButton, _blueThemeButton, _greenThemeButton, _purpleThemeButton
            });

            _themeSection.Controls.Add(_currentThemeLabel);
            _themeSection.Controls.Add(themeButtonsPanel);

            // General Section
            _generalSection = CreateSection("General Settings", 200);
            _generalSectionTitle = _generalSection.Controls.OfType<Label>().First();

            // General settings checkboxes
            _startMinimizedCheckBox = new CheckBox
            {
                Text = "Start application minimized",
                Font = new Font("Segoe UI", 11F),
                AutoSize = true,
                Location = new Point(0, 40),
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 15)
            };

            _autoUpdateCheckBox = new CheckBox
            {
                Text = "Enable automatic updates",
                Font = new Font("Segoe UI", 11F),
                AutoSize = true,
                Location = new Point(0, 70),
                BackColor = Color.Transparent,
                Checked = true,
                Margin = new Padding(0, 0, 0, 15)
            };

            _notificationsCheckBox = new CheckBox
            {
                Text = "Show desktop notifications",
                Font = new Font("Segoe UI", 11F),
                AutoSize = true,
                Location = new Point(0, 100),
                BackColor = Color.Transparent,
                Checked = true,
                Margin = new Padding(0, 0, 0, 15)
            };

            _generalSection.Controls.AddRange(new Control[] {
                _startMinimizedCheckBox, _autoUpdateCheckBox, _notificationsCheckBox
            });

            // Add sections to container
            _settingsContainer.Controls.Add(_titleLabel);
            _settingsContainer.Controls.Add(_themeSection);
            _settingsContainer.Controls.Add(_generalSection);

            // Add container to main control
            Controls.Add(_settingsContainer);

            // Update current theme button state
            UpdateThemeButtonStates();

            Name = "SettingsPage";
            Size = new Size(800, 600);
            
            ResumeLayout(false);
        }

        private Panel CreateSection(string title, int top)
        {
            var section = new Panel
            {
                Location = new Point(0, top),
                Size = new Size(720, 150),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20),
                Margin = new Padding(0, 0, 0, 30)
            };

            var sectionTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0),
                BackColor = Color.Transparent
            };

            section.Controls.Add(sectionTitle);
            return section;
        }

        private void ChangeTheme(ThemeType themeType)
        {
            _themeService.SetTheme(themeType);
            _currentThemeLabel.Text = $"Current Theme: {themeType}";
            UpdateThemeButtonStates();
        }

        private void UpdateThemeButtonStates()
        {
            var currentTheme = _themeService.CurrentTheme;

            // Reset all buttons to outline style
            _lightThemeButton.ButtonStyle = ButtonStyle.Outline;
            _darkThemeButton.ButtonStyle = ButtonStyle.Outline;
            _blueThemeButton.ButtonStyle = ButtonStyle.Outline;
            _greenThemeButton.ButtonStyle = ButtonStyle.Outline;
            _purpleThemeButton.ButtonStyle = ButtonStyle.Outline;

            // Set active button to primary style
            switch (currentTheme)
            {
                case ThemeType.Light:
                    _lightThemeButton.ButtonStyle = ButtonStyle.Primary;
                    break;
                case ThemeType.Dark:
                    _darkThemeButton.ButtonStyle = ButtonStyle.Primary;
                    break;
                case ThemeType.Blue:
                    _blueThemeButton.ButtonStyle = ButtonStyle.Primary;
                    break;
                case ThemeType.Green:
                    _greenThemeButton.ButtonStyle = ButtonStyle.Primary;
                    break;
                case ThemeType.Purple:
                    _purpleThemeButton.ButtonStyle = ButtonStyle.Primary;
                    break;
            }
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            var isDark = _themeService.CurrentTheme == ThemeType.Dark;

            BackColor = colors.Background;
            _settingsContainer.BackColor = colors.Background;

            // Title styling
            _titleLabel.ForeColor = colors.TextPrimary;

            // Section styling
            _themeSection.BackColor = colors.Surface;
            _themeSectionTitle.ForeColor = colors.TextPrimary;
            _currentThemeLabel.ForeColor = colors.TextSecondary;

            _generalSection.BackColor = colors.Surface;
            _generalSectionTitle.ForeColor = colors.TextPrimary;

            // Checkbox styling
            _startMinimizedCheckBox.ForeColor = colors.TextPrimary;
            _autoUpdateCheckBox.ForeColor = colors.TextPrimary;
            _notificationsCheckBox.ForeColor = colors.TextPrimary;

            // Section borders
            if (isDark)
            {
                _themeSection.BorderStyle = BorderStyle.FixedSingle;
                _generalSection.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => {
                    SetupTheme();
                    UpdateThemeButtonStates();
                }));
            }
            else
            {
                SetupTheme();
                UpdateThemeButtonStates();
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