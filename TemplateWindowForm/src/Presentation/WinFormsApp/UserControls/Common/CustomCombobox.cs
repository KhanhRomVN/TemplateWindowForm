using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Interfaces.Services;
using Guna.UI2.WinForms;

namespace Presentation.WinFormsApp.UserControls.Common
{
    public class ComboboxOption
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public bool Selected { get; set; } = false;
    }

    public partial class CustomCombobox : UserControl
    {
        private readonly IThemeService _themeService;
        private List<ComboboxOption> _options = new();
        private List<ComboboxOption> _dynamicOptions = new();
        private bool _multiple = false;
        private bool _searchable = false;
        private bool _creatable = false;
        private bool _isDropdownOpen = false;
        private const int SEARCH_THRESHOLD = 10;

        // Controls
        private Label _label = null!;
        private Guna2TextBox _inputTextBox = null!;
        private Guna2Button _dropdownButton = null!;
        private Panel _dropdownPanel = null!;
        private Panel _selectedItemsPanel = null!;
        private FlowLayoutPanel _badgeContainer = null!;

        // Events
        public event EventHandler<List<string>>? SelectionChanged;

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

        public List<ComboboxOption> Options
        {
            get => _options;
            set
            {
                _options = value ?? new List<ComboboxOption>();
                UpdateSearchableState();
                RefreshDropdown();
            }
        }

        public bool Multiple
        {
            get => _multiple;
            set
            {
                _multiple = value;
                UpdateSearchableState();
                UpdateLayout();
            }
        }

        public bool Searchable
        {
            get => _searchable;
            set
            {
                _searchable = value;
                UpdateInputState();
            }
        }

        public bool Creatable
        {
            get => _creatable;
            set => _creatable = value;
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

        public List<string> SelectedValues
        {
            get
            {
                var allOptions = _options.Concat(_dynamicOptions);
                return allOptions.Where(o => o.Selected).Select(o => o.Value).ToList();
            }
            set
            {
                var allOptions = _options.Concat(_dynamicOptions);
                foreach (var option in allOptions)
                {
                    option.Selected = value.Contains(option.Value);
                }
                UpdateSelectedDisplay();
                UpdateBadges();
            }
        }

        public string SelectedValue
        {
            get => SelectedValues.FirstOrDefault() ?? string.Empty;
            set => SelectedValues = string.IsNullOrEmpty(value) ? new List<string>() : new List<string> { value };
        }

        public CustomCombobox(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeCustomCombobox();
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeCustomCombobox()
        {
            SuspendLayout();

            Size = new Size(300, 80);
            BackColor = Color.Transparent;

            // Label
            _label = new Label
            {
                Text = "Select Option",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(0, 0),
                BackColor = Color.Transparent
            };

            // Input TextBox
            _inputTextBox = new Guna2TextBox
            {
                Location = new Point(0, 25),
                Size = new Size(270, 40),
                Font = new Font("Segoe UI", 10F),
                BorderRadius = 8,
                BorderThickness = 1,
                PlaceholderText = "Please enter/select your option...",
                ReadOnly = true
            };
            _inputTextBox.TextChanged += OnInputTextChanged;
            _inputTextBox.Enter += OnInputFocus;
            _inputTextBox.Leave += OnInputBlur;
            _inputTextBox.KeyDown += OnInputKeyDown;

            // Dropdown Button
            _dropdownButton = new Guna2Button
            {
                Location = new Point(270, 25),
                Size = new Size(30, 40),
                BorderRadius = 8,
                Text = "▼",
                Font = new Font("Segoe UI", 8F),
                UseTransparentBackground = true,
                ShadowDecoration = { Enabled = false }
            };
            _dropdownButton.Click += OnDropdownButtonClick;

            // Dropdown Panel
            _dropdownPanel = new Panel
            {
                Location = new Point(0, 65),
                Size = new Size(300, 200),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false,
                BackColor = Color.White,
                AutoScroll = true
            };

            // Selected Items Panel
            _selectedItemsPanel = new Panel
            {
                Location = new Point(0, 70),
                Size = new Size(300, 0),
                BackColor = Color.Transparent,
                Visible = false,
                AutoSize = true
            };

            _badgeContainer = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 5, 0, 0)
            };
            _selectedItemsPanel.Controls.Add(_badgeContainer);

            Controls.AddRange(new Control[] { _label, _inputTextBox, _dropdownButton, _dropdownPanel, _selectedItemsPanel });

            _dropdownPanel.BringToFront();

            UpdateSearchableState();
            SetupDropdownZOrder();

            Name = "CustomCombobox";
            ResumeLayout(false);
        }

        private void UpdateSearchableState()
        {
            _searchable = _multiple || _options.Count >= SEARCH_THRESHOLD;
            UpdateInputState();
        }

        private void UpdateInputState()
        {
            _inputTextBox.ReadOnly = !_searchable && !_creatable;
        }

        private void UpdateLayout()
        {
            if (_multiple)
            {
                _selectedItemsPanel.Visible = true;
                _selectedItemsPanel.Location = new Point(0, 70);
                Height = Math.Max(120, 70 + _selectedItemsPanel.Height + 10);
            }
            else
            {
                _selectedItemsPanel.Visible = false;
                Height = 80;
            }
        }

        private void SetupDropdownZOrder()
        {
            if (Parent != null)
            {
                Parent.Controls.SetChildIndex(_dropdownPanel, 0);
            }
        }

        private void OnDropdownButtonClick(object? sender, EventArgs e)
        {
            ToggleDropdown();
        }

        private void OnInputFocus(object? sender, EventArgs e)
        {
            if (_searchable || _creatable)
            {
                OpenDropdown();
            }
        }

        private void OnInputBlur(object? sender, EventArgs e)
        {
            var closeTimer = new System.Windows.Forms.Timer { Interval = 150 };
            closeTimer.Tick += (s, args) =>
            {
                closeTimer.Stop();
                closeTimer.Dispose();
                CloseDropdown();
            };
            closeTimer.Start();
        }

        private void OnInputTextChanged(object? sender, EventArgs e)
        {
            if (_isDropdownOpen)
            {
                RefreshDropdown();
            }
        }

        private void OnInputKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && _creatable && !string.IsNullOrWhiteSpace(_inputTextBox.Text))
            {
                CreateNewOption(_inputTextBox.Text.Trim());
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                CloseDropdown();
            }
        }

        private void ToggleDropdown()
        {
            if (_isDropdownOpen)
                CloseDropdown();
            else
                OpenDropdown();
        }

        private void OpenDropdown()
        {
            _isDropdownOpen = true;
            _dropdownPanel.Visible = true;
            _dropdownPanel.BringToFront();
            RefreshDropdown();

            if (!_searchable && !_multiple && !string.IsNullOrEmpty(SelectedValue))
            {
                _inputTextBox.Clear();
            }
        }

        private void CloseDropdown()
        {
            _isDropdownOpen = false;
            _dropdownPanel.Visible = false;
            UpdateSelectedDisplay();
        }

        private void RefreshDropdown()
        {
            _dropdownPanel.Controls.Clear();

            var allOptions = _options.Concat(_dynamicOptions).ToList();
            var filteredOptions = FilterOptions(allOptions);

            int y = 5;
            foreach (var option in filteredOptions)
            {
                var optionPanel = CreateOptionPanel(option, y);
                _dropdownPanel.Controls.Add(optionPanel);
                y += 35;
            }

            if (!filteredOptions.Any())
            {
                var noOptionsLabel = new Label
                {
                    Text = "No options found",
                    Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    Location = new Point(10, 10),
                    AutoSize = true
                };

                if (_creatable && !string.IsNullOrWhiteSpace(_inputTextBox.Text))
                {
                    var createLabel = new Label
                    {
                        Text = $"Press Enter to create \"{_inputTextBox.Text.Trim()}\"",
                        Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                        ForeColor = Color.Blue,
                        Location = new Point(10, 30),
                        AutoSize = true
                    };
                    _dropdownPanel.Controls.Add(createLabel);
                }

                _dropdownPanel.Controls.Add(noOptionsLabel);
            }
        }

        private List<ComboboxOption> FilterOptions(List<ComboboxOption> options)
        {
            var searchText = _inputTextBox.Text?.ToLower() ?? string.Empty;

            if (_multiple)
            {
                options = options.Where(o => !o.Selected).ToList();
            }

            if (_searchable && !string.IsNullOrWhiteSpace(searchText))
            {
                return options.Where(o => 
                    o.Label.ToLower().Contains(searchText) || 
                    o.Value.ToLower().Contains(searchText)
                ).ToList();
            }

            return options;
        }

        private Panel CreateOptionPanel(ComboboxOption option, int y)
        {
            var colors = _themeService.CurrentColors;
            
            var panel = new Panel
            {
                Size = new Size(_dropdownPanel.Width - 10, 30),
                Location = new Point(5, y),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            if (_multiple)
            {
                var checkbox = new CheckBox
                {
                    Size = new Size(16, 16),
                    Location = new Point(5, 7),
                    Checked = option.Selected,
                    BackColor = Color.Transparent
                };
                panel.Controls.Add(checkbox);
            }

            var label = new Label
            {
                Text = option.Label,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Location = new Point(_multiple ? 25 : 10, 7),
                Size = new Size(panel.Width - (_multiple ? 30 : 15), 16),
                BackColor = Color.Transparent,
                ForeColor = option.Selected && !_multiple ? colors.Primary : colors.TextPrimary
            };

            panel.Controls.Add(label);

            panel.MouseEnter += (s, e) => panel.BackColor = Color.FromArgb(30, colors.Primary.R, colors.Primary.G, colors.Primary.B);
            panel.MouseLeave += (s, e) => panel.BackColor = Color.Transparent;
            label.MouseEnter += (s, e) => panel.BackColor = Color.FromArgb(30, colors.Primary.R, colors.Primary.G, colors.Primary.B);
            label.MouseLeave += (s, e) => panel.BackColor = Color.Transparent;

            panel.Click += (s, e) => SelectOption(option);
            label.Click += (s, e) => SelectOption(option);

            return panel;
        }

        private void SelectOption(ComboboxOption option)
        {
            if (_multiple)
            {
                option.Selected = !option.Selected;
                UpdateBadges();
                RefreshDropdown();
            }
            else
            {
                var allOptions = _options.Concat(_dynamicOptions);
                foreach (var opt in allOptions)
                {
                    opt.Selected = opt.Value == option.Value;
                }
                CloseDropdown();
            }

            SelectionChanged?.Invoke(this, SelectedValues);
        }

        private void CreateNewOption(string value)
        {
            if (_dynamicOptions.Any(o => o.Value.Equals(value, StringComparison.OrdinalIgnoreCase)))
                return;

            var newOption = new ComboboxOption
            {
                Value = value,
                Label = char.ToUpper(value[0]) + value.Substring(1),
                Selected = true
            };

            _dynamicOptions.Add(newOption);

            if (!_multiple)
            {
                var allOptions = _options.Concat(_dynamicOptions);
                foreach (var opt in allOptions)
                {
                    opt.Selected = opt.Value == newOption.Value;
                }
                CloseDropdown();
            }
            else
            {
                UpdateBadges();
            }

            _inputTextBox.Clear();
            SelectionChanged?.Invoke(this, SelectedValues);
        }

        private void UpdateSelectedDisplay()
        {
            if (_multiple) return;

            var selected = (_options.Concat(_dynamicOptions)).FirstOrDefault(o => o.Selected);
            if (selected != null)
            {
                _inputTextBox.Text = selected.Label;
            }
            else
            {
                _inputTextBox.Clear();
            }
        }

        private void UpdateBadges()
        {
            if (!_multiple) return;

            _badgeContainer.Controls.Clear();
            var selected = (_options.Concat(_dynamicOptions)).Where(o => o.Selected);

            foreach (var option in selected)
            {
                var badge = CreateBadge(option);
                _badgeContainer.Controls.Add(badge);
            }

            _selectedItemsPanel.Height = _badgeContainer.Height + 10;
            UpdateLayout();
        }

        private Panel CreateBadge(ComboboxOption option)
        {
            var colors = _themeService.CurrentColors;
            
            var badge = new Panel
            {
                BackColor = Color.FromArgb(30, colors.Primary.R, colors.Primary.G, colors.Primary.B),
                Size = new Size(100, 24),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(2)
            };

            var label = new Label
            {
                Text = option.Label,
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                ForeColor = colors.Primary,
                Location = new Point(5, 4),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var closeButton = new Label
            {
                Text = "×",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = colors.Primary,
                Location = new Point(badge.Width - 20, 2),
                Size = new Size(15, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            closeButton.Click += (s, e) => RemoveBadge(option);
            closeButton.MouseEnter += (s, e) => closeButton.ForeColor = colors.Error;
            closeButton.MouseLeave += (s, e) => closeButton.ForeColor = colors.Primary;

            badge.Width = label.PreferredWidth + 25;
            closeButton.Location = new Point(badge.Width - 18, 2);

            badge.Controls.AddRange(new Control[] { label, closeButton });
            
            return badge;
        }

        private void RemoveBadge(ComboboxOption option)
        {
            option.Selected = false;
            UpdateBadges();
            SelectionChanged?.Invoke(this, SelectedValues);
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            var isDark = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark;

            BackColor = colors.Background;
            
            _label.ForeColor = colors.TextPrimary;

            _inputTextBox.BorderColor = colors.Border;
            _inputTextBox.FocusedState.BorderColor = colors.Primary;
            _inputTextBox.FillColor = colors.Surface;
            _inputTextBox.ForeColor = colors.TextPrimary;

            _dropdownButton.FillColor = colors.Surface;
            _dropdownButton.ForeColor = colors.TextSecondary;
            _dropdownButton.BorderColor = colors.Border;
            _dropdownButton.HoverState.FillColor = colors.Primary;
            _dropdownButton.HoverState.ForeColor = colors.OnPrimary;

            _dropdownPanel.BackColor = colors.Surface;
            _dropdownPanel.ForeColor = colors.TextPrimary;

            if (isDark)
            {
                _dropdownPanel.BorderStyle = BorderStyle.FixedSingle;
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