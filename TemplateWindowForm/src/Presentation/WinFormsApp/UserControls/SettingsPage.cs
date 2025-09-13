using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;
using Core.Enums;
using Presentation.WinFormsApp.UserControls.Common;

namespace Presentation.WinFormsApp.UserControls
{
    public partial class SettingsPage : UserControl
    {
        private readonly IThemeService _themeService;
        private Panel _settingsPanel = null!;
        private Label _titleLabel = null!;
        private Label _themeLabel = null!;
        private ComboBox _themeComboBox = null!;
        private CustomButton _applyButton = null!;
        private CustomButton _resetButton = null!;

        public SettingsPage(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent();
            SetupTheme();
            LoadSettings();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Main settings panel
            _settingsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 20, 30, 20),
                BackColor = Color.Transparent
            };

            // Title
            _titleLabel = new Label
            {
                Text = "Settings",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0),
                BackColor = Color.Transparent
            };

            // Theme selection
            _themeLabel = new Label
            {
                Text = "Theme:",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(0, 80),
                BackColor = Color.Transparent
            };

            _themeComboBox = new ComboBox
            {
                Font = new Font("Segoe UI", 11F),
                Size = new Size(200, 35),
                Location = new Point(0, 110),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Add theme options
            _themeComboBox.Items.AddRange(new object[]
            {
                "Light",
                "Dark", 
                "Blue",
                "Green",
                "Purple"
            });

            _themeComboBox.SelectedIndexChanged += OnThemeSelectionChanged;

            // Action buttons
            _applyButton = new CustomButton(_themeService)
            {
                Text = "Apply Changes",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Size = new Size(140, 40),
                Location = new Point(0, 170),
                ButtonStyle = ButtonStyle.Primary
            };
            _applyButton.Click += OnApplySettings;

            _resetButton = new CustomButton(_themeService)
            {
                Text = "Reset to Default",
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                Size = new Size(140, 40),
                Location = new Point(150, 170),
                ButtonStyle = ButtonStyle.Secondary
            };
            _resetButton.Click += OnResetSettings;

            // Add controls to panel
            _settingsPanel.Controls.AddRange(new Control[]
            {
                _titleLabel,
                _themeLabel,
                _themeComboBox,
                _applyButton,
                _resetButton
            });

            Controls.Add(_settingsPanel);

            Name = "SettingsPage";
            Size = new Size(800, 600);
            
            ResumeLayout(false);
        }

        private void LoadSettings()
        {
            // Set current theme selection
            var currentTheme = _themeService.CurrentTheme;
            _themeComboBox.SelectedIndex = (int)currentTheme;
        }

        private void OnThemeSelectionChanged(object? sender, EventArgs e)
        {
            if (_themeComboBox.SelectedIndex >= 0)
            {
                var selectedTheme = (ThemeType)_themeComboBox.SelectedIndex;
                _themeService.SetTheme(selectedTheme);
            }
        }

        private void OnApplySettings(object? sender, EventArgs e)
        {
            MessageBox.Show("Settings applied successfully!", "Settings", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnResetSettings(object? sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to reset all settings to default?", 
                "Reset Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                _themeService.SetTheme(ThemeType.Light);
                _themeComboBox.SelectedIndex = 0;
                MessageBox.Show("Settings have been reset to default.", "Reset Complete", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            
            BackColor = colors.Background;
            _titleLabel.ForeColor = colors.TextPrimary;
            _themeLabel.ForeColor = colors.TextPrimary;
            
            // Style ComboBox
            _themeComboBox.BackColor = colors.Surface;
            _themeComboBox.ForeColor = colors.TextPrimary;
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
                _themeComboBox.SelectedIndexChanged -= OnThemeSelectionChanged;
                _applyButton.Click -= OnApplySettings;
                _resetButton.Click -= OnResetSettings;
            }
            base.Dispose(disposing);
        }
    }
}