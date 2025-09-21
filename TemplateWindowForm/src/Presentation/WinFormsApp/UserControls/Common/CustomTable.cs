using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Interfaces.Services;

namespace Presentation.WinFormsApp.UserControls.Common
{
    public partial class CustomTable : UserControl
    {
        private readonly IThemeService? _themeService;
        private DataGridView _dataGridView = null!;
        private Panel _headerPanel = null!;
        private Panel _actionPanel = null!;
        private Panel _footerPanel = null!;
        private TextBox _searchTextBox = null!;
        private Button _addButton = null!;
        private Button _editButton = null!;
        private Button _deleteButton = null!;
        private Button _refreshButton = null!;
        private Label _titleLabel = null!;
        private Label _recordCountLabel = null!;
        private ComboBox _pageSizeComboBox = null!;
        private Button _prevPageButton = null!;
        private Button _nextPageButton = null!;
        private Label _pageInfoLabel = null!;
        private bool _isInitialized = false;
        
        // Pagination properties
        private int _currentPage = 1;
        private int _pageSize = 25;
        private int _totalRecords = 0;
        private object? _originalDataSource;

        // Events
        public event EventHandler<EventArgs>? AddClicked;
        public event EventHandler<EventArgs>? EditClicked;
        public event EventHandler<EventArgs>? DeleteClicked;
        public event EventHandler<EventArgs>? RefreshClicked;
        public event EventHandler<string>? SearchTextChanged;

        // Properties
        public string Title
        {
            get => _titleLabel?.Text ?? "";
            set
            {
                if (_titleLabel != null)
                    _titleLabel.Text = value;
            }
        }

        public bool ShowActions { get; set; } = true;
        public bool ShowPagination { get; set; } = true;
        public bool ShowSearch { get; set; } = true;

        // Constructor with IThemeService
        public CustomTable(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent();
            InitializeModernTable();
            SetupTheme();
            
            if (_themeService != null)
                _themeService.ThemeChanged += OnThemeChanged;
        }

        // Parameterless constructor for backward compatibility
        public CustomTable()
        {
            _themeService = null;
            InitializeComponent();
            InitializeBasicTable();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "CustomTable";
            this.Size = new System.Drawing.Size(1000, 700);
            this.BackColor = Color.White;
            
            this.ResumeLayout(false);
        }

        private void InitializeModernTable()
        {
            try
            {
                // Header Panel
                _headerPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 80,
                    Padding = new Padding(20, 15, 20, 15),
                    BackColor = Color.White
                };

                // Title Label
                _titleLabel = new Label
                {
                    Text = "Data Management",
                    Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(31, 41, 55),
                    AutoSize = true,
                    Location = new Point(0, 20),
                    BackColor = Color.Transparent
                };

                // Search Box
                if (ShowSearch)
                {
                    _searchTextBox = new TextBox
                    {
                        Size = new Size(300, 30),
                        Location = new Point(660, 20),
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                        Font = new Font("Segoe UI", 10F),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.FromArgb(249, 250, 251)
                    };
                    _searchTextBox.TextChanged += OnSearchTextChanged;
                    _headerPanel.Controls.Add(_searchTextBox);
                }

                _headerPanel.Controls.Add(_titleLabel);

                // Action Panel
                if (ShowActions)
                {
                    _actionPanel = new Panel
                    {
                        Dock = DockStyle.Top,
                        Height = 70,
                        Padding = new Padding(20, 10, 20, 10),
                        BackColor = Color.FromArgb(248, 250, 252)
                    };

                    // Action Buttons
                    var buttonY = 15;
                    var buttonHeight = 40;

                    _addButton = CreateActionButton("Add New", Color.FromArgb(34, 197, 94), 0, buttonY, buttonHeight);
                    _addButton.Click += (s, e) => AddClicked?.Invoke(this, EventArgs.Empty);

                    _editButton = CreateActionButton("Edit", Color.FromArgb(59, 130, 246), 110, buttonY, buttonHeight);
                    _editButton.Click += (s, e) => EditClicked?.Invoke(this, EventArgs.Empty);

                    _deleteButton = CreateActionButton("Delete", Color.FromArgb(239, 68, 68), 210, buttonY, buttonHeight);
                    _deleteButton.Click += (s, e) => DeleteClicked?.Invoke(this, EventArgs.Empty);

                    _refreshButton = CreateActionButton("Refresh", Color.FromArgb(107, 114, 128), 320, buttonY, buttonHeight);
                    _refreshButton.Click += (s, e) => RefreshClicked?.Invoke(this, EventArgs.Empty);

                    // Record Count Label
                    _recordCountLabel = new Label
                    {
                        Text = "0 records",
                        Font = new Font("Segoe UI", 9F),
                        ForeColor = Color.FromArgb(107, 114, 128),
                        AutoSize = true,
                        Location = new Point(850, 25),
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                        BackColor = Color.Transparent
                    };

                    _actionPanel.Controls.AddRange(new Control[] {
                        _addButton, _editButton, _deleteButton, _refreshButton, _recordCountLabel
                    });
                }

                // Modern DataGridView
                _dataGridView = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AllowUserToResizeRows = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    ReadOnly = true,
                    RowHeadersVisible = false,
                    EnableHeadersVisualStyles = false,
                    GridColor = Color.FromArgb(243, 244, 246),
                    CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                    RowTemplate = { Height = 50 },
                    
                    // Header styling
                    ColumnHeadersHeight = 55,
                    ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = Color.FromArgb(249, 250, 251),
                        ForeColor = Color.FromArgb(75, 85, 99),
                        Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                        Alignment = DataGridViewContentAlignment.MiddleLeft,
                        Padding = new Padding(15, 0, 15, 0),
                        SelectionBackColor = Color.FromArgb(249, 250, 251)
                    },

                    // Default cell styling
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = Color.White,
                        ForeColor = Color.FromArgb(31, 41, 55),
                        Font = new Font("Segoe UI", 10F),
                        Alignment = DataGridViewContentAlignment.MiddleLeft,
                        Padding = new Padding(15, 0, 15, 0),
                        SelectionBackColor = Color.FromArgb(219, 234, 254),
                        SelectionForeColor = Color.FromArgb(30, 64, 175),
                        WrapMode = DataGridViewTriState.False
                    },

                    // Alternating row style
                    AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = Color.FromArgb(249, 250, 251),
                        ForeColor = Color.FromArgb(31, 41, 55),
                        Font = new Font("Segoe UI", 10F),
                        Alignment = DataGridViewContentAlignment.MiddleLeft,
                        Padding = new Padding(15, 0, 15, 0),
                        SelectionBackColor = Color.FromArgb(219, 234, 254),
                        SelectionForeColor = Color.FromArgb(30, 64, 175)
                    }
                };

                // Add hover effect
                _dataGridView.CellMouseEnter += OnCellMouseEnter;
                _dataGridView.CellMouseLeave += OnCellMouseLeave;
                _dataGridView.SelectionChanged += OnSelectionChanged;

                // Footer Panel for Pagination
                if (ShowPagination)
                {
                    _footerPanel = new Panel
                    {
                        Dock = DockStyle.Bottom,
                        Height = 60,
                        Padding = new Padding(20, 10, 20, 10),
                        BackColor = Color.FromArgb(248, 250, 252)
                    };

                    // Page size selector
                    _pageSizeComboBox = new ComboBox
                    {
                        Size = new Size(100, 35),
                        Location = new Point(0, 12),
                        Font = new Font("Segoe UI", 9F),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    _pageSizeComboBox.Items.AddRange(new object[] { "10", "25", "50", "100" });
                    _pageSizeComboBox.SelectedIndex = 1; // Default to 25
                    _pageSizeComboBox.SelectedIndexChanged += OnPageSizeChanged;

                    var pageSizeLabel = new Label
                    {
                        Text = "Show:",
                        Font = new Font("Segoe UI", 9F),
                        ForeColor = Color.FromArgb(107, 114, 128),
                        Location = new Point(110, 20),
                        AutoSize = true,
                        BackColor = Color.Transparent
                    };

                    // Pagination controls
                    _prevPageButton = new Button
                    {
                        Size = new Size(80, 35),
                        Location = new Point(650, 12),
                        Text = "Previous",
                        Font = new Font("Segoe UI", 9F),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.White,
                        ForeColor = Color.FromArgb(107, 114, 128),
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                        Enabled = false
                    };
                    _prevPageButton.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
                    _prevPageButton.Click += OnPrevPageClicked;

                    _nextPageButton = new Button
                    {
                        Size = new Size(80, 35),
                        Location = new Point(900, 12),
                        Text = "Next",
                        Font = new Font("Segoe UI", 9F),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.White,
                        ForeColor = Color.FromArgb(107, 114, 128),
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };
                    _nextPageButton.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
                    _nextPageButton.Click += OnNextPageClicked;

                    _pageInfoLabel = new Label
                    {
                        Text = "Page 1 of 1",
                        Font = new Font("Segoe UI", 9F),
                        ForeColor = Color.FromArgb(107, 114, 128),
                        Location = new Point(740, 20),
                        Size = new Size(150, 20),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                        BackColor = Color.Transparent
                    };

                    _footerPanel.Controls.AddRange(new Control[] {
                        _pageSizeComboBox, pageSizeLabel, _prevPageButton, _nextPageButton, _pageInfoLabel
                    });
                }

                // Assemble the table
                this.Controls.Add(_dataGridView);
                if (ShowPagination) this.Controls.Add(_footerPanel);
                if (ShowActions) this.Controls.Add(_actionPanel);
                this.Controls.Add(_headerPanel);

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing Modern CustomTable: {ex.Message}");
                throw;
            }
        }

        private void InitializeBasicTable()
        {
            try
            {
                _dataGridView = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.None
                };

                this.Controls.Add(_dataGridView);
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing basic DataGridView: {ex.Message}");
                throw;
            }
        }

        private Button CreateActionButton(string text, Color color, int x, int y, int height)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(100, height),
                Location = new Point(x, y),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        // Event Handlers
        private void OnSearchTextChanged(object? sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                SearchTextChanged?.Invoke(this, textBox.Text);
                ApplySearch(textBox.Text);
            }
        }

        private void OnCellMouseEnter(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _dataGridView.Rows.Count)
            {
                var row = _dataGridView.Rows[e.RowIndex];
                if (!row.Selected)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(243, 244, 246);
                }
            }
        }

        private void OnCellMouseLeave(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _dataGridView.Rows.Count)
            {
                var row = _dataGridView.Rows[e.RowIndex];
                if (!row.Selected)
                {
                    row.DefaultCellStyle.BackColor = e.RowIndex % 2 == 0 
                        ? Color.White 
                        : Color.FromArgb(249, 250, 251);
                }
            }
        }

        private void OnSelectionChanged(object? sender, EventArgs e)
        {
            if (ShowActions)
            {
                bool hasSelection = _dataGridView.SelectedRows.Count > 0;
                if (_editButton != null) _editButton.Enabled = hasSelection;
                if (_deleteButton != null) _deleteButton.Enabled = hasSelection;
            }
        }

        private void OnPageSizeChanged(object? sender, EventArgs e)
        {
            if (_pageSizeComboBox?.SelectedItem != null)
            {
                _pageSize = int.Parse(_pageSizeComboBox.SelectedItem.ToString()!);
                _currentPage = 1;
                ApplyPagination();
            }
        }

        private void OnPrevPageClicked(object? sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                ApplyPagination();
            }
        }

        private void OnNextPageClicked(object? sender, EventArgs e)
        {
            var totalPages = (int)Math.Ceiling((double)_totalRecords / _pageSize);
            if (_currentPage < totalPages)
            {
                _currentPage++;
                ApplyPagination();
            }
        }

        // Public Methods
        public DataGridView DataGridView => _dataGridView;

        public void SetDataSource(object dataSource)
        {
            try
            {
                if (!_isInitialized || _dataGridView == null)
                {
                    throw new InvalidOperationException("CustomTable is not properly initialized");
                }

                _originalDataSource = dataSource;
                _dataGridView.DataSource = dataSource;
                
                // Calculate total records
                if (dataSource is DataTable dt)
                    _totalRecords = dt.Rows.Count;
                else if (dataSource is System.Collections.IList list)
                    _totalRecords = list.Count;
                else
                    _totalRecords = 0;

                UpdateRecordCount();
                UpdatePaginationControls();
                
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting DataSource: {ex.Message}");
                throw;
            }
        }

        public T? GetSelectedItem<T>() where T : class
        {
            try
            {
                if (_dataGridView?.SelectedRows.Count > 0)
                {
                    var selectedRow = _dataGridView.SelectedRows[0];
                    return selectedRow.DataBoundItem as T;
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting selected item: {ex.Message}");
                return null;
            }
        }

        public void CustomizeColumns(Dictionary<string, object> columnConfig)
        {
            try
            {
                foreach (DataGridViewColumn column in _dataGridView.Columns)
                {
                    if (columnConfig.ContainsKey(column.Name))
                    {
                        var config = (Dictionary<string, object>)columnConfig[column.Name];
                        
                        if (config.ContainsKey("width"))
                            column.Width = (int)config["width"];
                        
                        if (config.ContainsKey("headertext"))
                            column.HeaderText = (string)config["headertext"];
                        
                        if (config.ContainsKey("readonly"))
                            column.ReadOnly = (bool)config["readonly"];
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error customizing columns: {ex.Message}");
            }
        }

        private void ApplySearch(string searchText)
        {
            // Implement search logic based on data source type
            // This is a simplified version - you can enhance it based on your needs
        }

        private void ApplyPagination()
        {
            // Implement pagination logic
            UpdatePaginationControls();
        }

        private void UpdateRecordCount()
        {
            if (_recordCountLabel != null)
            {
                _recordCountLabel.Text = $"{_totalRecords} records";
            }
        }

        private void UpdatePaginationControls()
        {
            if (!ShowPagination) return;

            var totalPages = Math.Max(1, (int)Math.Ceiling((double)_totalRecords / _pageSize));
            
            if (_pageInfoLabel != null)
                _pageInfoLabel.Text = $"Page {_currentPage} of {totalPages}";
            
            if (_prevPageButton != null)
                _prevPageButton.Enabled = _currentPage > 1;
            
            if (_nextPageButton != null)
                _nextPageButton.Enabled = _currentPage < totalPages;
        }

        private void SetupTheme()
        {
            if (_themeService == null) return;

            var colors = _themeService.CurrentColors;
            var isDark = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark;

            BackColor = colors.Background;

            // Update panel colors
            if (_headerPanel != null)
            {
                _headerPanel.BackColor = colors.Surface;
                if (_titleLabel != null)
                    _titleLabel.ForeColor = colors.TextPrimary;
            }

            if (_actionPanel != null)
            {
                _actionPanel.BackColor = isDark ? colors.Background : Color.FromArgb(248, 250, 252);
                if (_recordCountLabel != null)
                    _recordCountLabel.ForeColor = colors.TextSecondary;
            }

            if (_footerPanel != null)
            {
                _footerPanel.BackColor = isDark ? colors.Background : Color.FromArgb(248, 250, 252);
            }

            // Update DataGridView colors
            if (_dataGridView != null)
            {
                _dataGridView.BackgroundColor = colors.Surface;
                
                // Update cell styles for theme
                var headerStyle = _dataGridView.ColumnHeadersDefaultCellStyle;
                headerStyle.BackColor = isDark ? colors.Background : Color.FromArgb(249, 250, 251);
                headerStyle.ForeColor = colors.TextSecondary;
                headerStyle.SelectionBackColor = headerStyle.BackColor;

                var defaultStyle = _dataGridView.DefaultCellStyle;
                defaultStyle.BackColor = colors.Surface;
                defaultStyle.ForeColor = colors.TextPrimary;
                defaultStyle.SelectionBackColor = colors.Primary;
                defaultStyle.SelectionForeColor = colors.OnPrimary;

                var alternatingStyle = _dataGridView.AlternatingRowsDefaultCellStyle;
                alternatingStyle.BackColor = isDark ? ControlPaint.Dark(colors.Surface, 0.02f) : Color.FromArgb(249, 250, 251);
                alternatingStyle.ForeColor = colors.TextPrimary;
                alternatingStyle.SelectionBackColor = colors.Primary;
                alternatingStyle.SelectionForeColor = colors.OnPrimary;

                _dataGridView.GridColor = colors.Border;
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

        public void RefreshTable()
        {
            try
            {
                _dataGridView?.Refresh();
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing table: {ex.Message}");
            }
        }

        public void ClearTable()
        {
            try
            {
                if (_dataGridView != null && !_dataGridView.IsDisposed)
                {
                    _dataGridView.DataSource = null;
                    _totalRecords = 0;
                    UpdateRecordCount();
                    UpdatePaginationControls();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing table: {ex.Message}");
            }
        }

        // Fixed: Removed duplicate Dispose method and used the inherited one
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (_themeService != null)
                    {
                        _themeService.ThemeChanged -= OnThemeChanged;
                    }

                    if (_searchTextBox != null)
                    {
                        _searchTextBox.TextChanged -= OnSearchTextChanged;
                    }

                    if (_dataGridView != null)
                    {
                        _dataGridView.CellMouseEnter -= OnCellMouseEnter;
                        _dataGridView.CellMouseLeave -= OnCellMouseLeave;
                        _dataGridView.SelectionChanged -= OnSelectionChanged;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error disposing CustomTable: {ex.Message}");
                }
            }
            base.Dispose(disposing);
        }
    }
}