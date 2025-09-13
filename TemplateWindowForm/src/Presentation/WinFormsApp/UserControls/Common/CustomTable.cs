using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;

namespace Presentation.WinFormsApp.UserControls.Common
{
    public partial class CustomTable : UserControl
    {
        private readonly IThemeService _themeService;
        private DataGridView _dataGridView = null!;
        private Panel _headerPanel = null!;
        private Panel _actionPanel = null!;
        private Label _titleLabel = null!;
        private Label _countLabel = null!;
        private TextBox _searchTextBox = null!;
        private Button _addButton = null!;
        private Button _editButton = null!;
        private Button _deleteButton = null!;
        private Button _refreshButton = null!;
        private Label _searchIcon = null!;

        public string Title
        {
            get => _titleLabel.Text;
            set => _titleLabel.Text = value;
        }

        public DataGridView DataGrid => _dataGridView;

        public event EventHandler? AddClicked;
        public event EventHandler? EditClicked;
        public event EventHandler? DeleteClicked;
        public event EventHandler? RefreshClicked;
        public event EventHandler<string>? SearchTextChanged;

        public CustomTable(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent();
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Header Panel with modern design
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                Padding = new Padding(25, 20, 25, 15)
            };

            // Title Section
            _titleLabel = new Label
            {
                Text = "Data Table",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0),
                BackColor = Color.Transparent
            };

            _countLabel = new Label
            {
                Text = "0 items",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(0, 35),
                BackColor = Color.Transparent
            };

            // Search Section with icon
            var searchPanel = new Panel
            {
                Size = new Size(320, 40),
                Location = new Point(0, 65),
                BackColor = Color.Transparent
            };

            _searchIcon = new Label
            {
                Text = "🔍",
                Font = new Font("Segoe UI", 14F),
                Size = new Size(20, 20),
                Location = new Point(15, 10),
                BackColor = Color.Transparent
            };

            _searchTextBox = new TextBox
            {
                Size = new Size(320, 40),
                Location = new Point(0, 0),
                Font = new Font("Segoe UI", 11F),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0)
            };
            _searchTextBox.TextChanged += OnSearchTextChanged;

            // Position search icon over textbox
            searchPanel.Controls.Add(_searchTextBox);
            searchPanel.Controls.Add(_searchIcon);
            _searchIcon.BringToFront();

            // Action Panel for buttons
            _actionPanel = new Panel
            {
                Size = new Size(400, 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Action Buttons with modern styling
            _addButton = CreateActionButton("Add", "➕", Color.FromArgb(34, 197, 94), 0);
            _addButton.Click += (s, e) => AddClicked?.Invoke(this, EventArgs.Empty);

            _editButton = CreateActionButton("Edit", "✏️", Color.FromArgb(59, 130, 246), 1);
            _editButton.Click += (s, e) => EditClicked?.Invoke(this, EventArgs.Empty);

            _deleteButton = CreateActionButton("Delete", "🗑️", Color.FromArgb(239, 68, 68), 2);
            _deleteButton.Click += (s, e) => DeleteClicked?.Invoke(this, EventArgs.Empty);

            _refreshButton = CreateActionButton("Refresh", "🔄", Color.FromArgb(107, 114, 128), 3);
            _refreshButton.Click += (s, e) => RefreshClicked?.Invoke(this, EventArgs.Empty);

            _actionPanel.Controls.AddRange(new Control[] { 
                _addButton, _editButton, _deleteButton, _refreshButton 
            });

            // Add controls to header
            _headerPanel.Controls.Add(_titleLabel);
            _headerPanel.Controls.Add(_countLabel);
            _headerPanel.Controls.Add(searchPanel);
            _headerPanel.Controls.Add(_actionPanel);

            // Enhanced DataGridView
            _dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                GridColor = Color.FromArgb(243, 244, 246),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10F),
                    Padding = new Padding(12, 8, 12, 8),
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(55, 65, 81),
                    SelectionBackColor = Color.FromArgb(239, 246, 255),
                    SelectionForeColor = Color.FromArgb(59, 130, 246)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    Padding = new Padding(12, 12, 12, 12),
                    BackColor = Color.FromArgb(249, 250, 251),
                    ForeColor = Color.FromArgb(75, 85, 99),
                    Alignment = DataGridViewContentAlignment.MiddleLeft
                },
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 50,
                RowTemplate = { Height = 50 },
                BackgroundColor = Color.White,
                EnableHeadersVisualStyles = false,
                Margin = new Padding(25, 0, 25, 25)
            };

            // Add panels to main control
            Controls.Add(_dataGridView);
            Controls.Add(_headerPanel);

            // Handle resize to position buttons correctly
            _headerPanel.Resize += OnHeaderPanelResize;

            Name = "CustomTable";
            Size = new Size(800, 600);
            
            ResumeLayout(false);
        }

        private Button CreateActionButton(string text, string icon, Color color, int index)
        {
            var button = new Button
            {
                Text = $"{icon} {text}",
                Size = new Size(90, 38),
                Location = new Point(index * 95, 6),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = color,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void OnHeaderPanelResize(object? sender, EventArgs e)
        {
            if (_headerPanel.Width > 0)
            {
                _actionPanel.Location = new Point(_headerPanel.Width - _actionPanel.Width - 25, 65);
            }
        }

        private void OnSearchTextChanged(object? sender, EventArgs e)
        {
            var searchText = _searchTextBox.Text;
            SearchTextChanged?.Invoke(this, searchText);
        }

        public void SetDataSource<T>(List<T> data)
        {
            _dataGridView.DataSource = null;
            if (data != null && data.Count > 0)
            {
                _dataGridView.DataSource = data;
                _dataGridView.AutoResizeColumns();
                UpdateItemCount(data.Count);
                SetupDataGridTheme();
            }
            else
            {
                UpdateItemCount(0);
            }
        }

        public T? GetSelectedItem<T>() where T : class
        {
            if (_dataGridView.SelectedRows.Count > 0)
            {
                return _dataGridView.SelectedRows[0].DataBoundItem as T;
            }
            return null;
        }

        private void UpdateItemCount(int count)
        {
            _countLabel.Text = count == 1 ? "1 item" : $"{count} items";
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            var isDark = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark;
            
            BackColor = colors.Background;
            _headerPanel.BackColor = colors.Background;
            
            // Title styling
            _titleLabel.ForeColor = colors.TextPrimary;
            _countLabel.ForeColor = colors.TextSecondary;
            
            // Search box theming
            _searchTextBox.BackColor = colors.Surface;
            _searchTextBox.ForeColor = colors.TextPrimary;
            
            _searchIcon.ForeColor = colors.TextSecondary;
            
            SetupDataGridTheme();
        }

        private void SetupDataGridTheme()
        {
            var colors = _themeService.CurrentColors;
            var isDark = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark;
            
            if (isDark)
            {
                // Dark theme for DataGridView
                _dataGridView.BackgroundColor = colors.Surface;
                _dataGridView.GridColor = Color.FromArgb(63, 63, 70);
                
                _dataGridView.DefaultCellStyle.BackColor = colors.Surface;
                _dataGridView.DefaultCellStyle.ForeColor = colors.OnSurface;
                _dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(30, colors.Primary.R, colors.Primary.G, colors.Primary.B);
                _dataGridView.DefaultCellStyle.SelectionForeColor = colors.Primary;
                
                _dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 27);
                _dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = colors.OnSurface;
                
                _dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(39, 39, 42);
                _dataGridView.AlternatingRowsDefaultCellStyle.ForeColor = colors.OnSurface;
                _dataGridView.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(40, colors.Primary.R, colors.Primary.G, colors.Primary.B);
                _dataGridView.AlternatingRowsDefaultCellStyle.SelectionForeColor = colors.Primary;
            }
            else
            {
                // Light theme for DataGridView (already configured in InitializeComponent)
                _dataGridView.BackgroundColor = colors.Surface;
                _dataGridView.GridColor = Color.FromArgb(229, 231, 235);
                
                _dataGridView.DefaultCellStyle.BackColor = colors.Surface;
                _dataGridView.DefaultCellStyle.ForeColor = colors.OnSurface;
                _dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(239, 246, 255);
                _dataGridView.DefaultCellStyle.SelectionForeColor = Color.FromArgb(59, 130, 246);
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
                _headerPanel.Resize -= OnHeaderPanelResize;
            }
            base.Dispose(disposing);
        }
    }
}