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
        private DataGridView dataGridView;
        private Panel headerPanel;
        private Panel actionPanel;
        private TextBox searchTextBox;
        private Button addButton;
        private Button editButton;
        private Button deleteButton;
        private Label titleLabel;
        private bool isInitialized = false;

        // Events
        public event EventHandler<EventArgs>? AddClicked;
        public event EventHandler<EventArgs>? EditClicked;
        public event EventHandler<EventArgs>? DeleteClicked;
        public event EventHandler<string>? SearchTextChanged;

        // Properties
        public string Title
        {
            get => titleLabel?.Text ?? "";
            set
            {
                if (titleLabel != null)
                    titleLabel.Text = value;
            }
        }

        // Constructor with IThemeService
        public CustomTable(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent();
            InitializeCustomTable();
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
            this.Size = new System.Drawing.Size(800, 600);
            
            this.ResumeLayout(false);
        }

        private void InitializeCustomTable()
        {
            try
            {
                // Header Panel with title and search
                headerPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 60,
                    BackColor = Color.White,
                    Padding = new Padding(10)
                };

                // Title Label
                titleLabel = new Label
                {
                    Text = "Data Table",
                    Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(64, 64, 64),
                    AutoSize = true,
                    Location = new Point(10, 15)
                };

                // Search TextBox
                searchTextBox = new TextBox
                {
                    PlaceholderText = "Search...",
                    Size = new Size(200, 30),
                    Location = new Point(580, 15),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Font = new Font("Segoe UI", 10F)
                };
                searchTextBox.TextChanged += OnSearchTextChanged;

                headerPanel.Controls.Add(titleLabel);
                headerPanel.Controls.Add(searchTextBox);

                // Action Panel with buttons
                actionPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 50,
                    BackColor = Color.FromArgb(248, 249, 250),
                    Padding = new Padding(10)
                };

                // Add Button
                addButton = new Button
                {
                    Text = "Add",
                    Size = new Size(80, 30),
                    Location = new Point(10, 10),
                    BackColor = Color.FromArgb(34, 197, 94),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                addButton.FlatAppearance.BorderSize = 0;
                addButton.Click += (s, e) => AddClicked?.Invoke(this, EventArgs.Empty);

                // Edit Button
                editButton = new Button
                {
                    Text = "Edit",
                    Size = new Size(80, 30),
                    Location = new Point(100, 10),
                    BackColor = Color.FromArgb(59, 130, 246),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                editButton.FlatAppearance.BorderSize = 0;
                editButton.Click += (s, e) => EditClicked?.Invoke(this, EventArgs.Empty);

                // Delete Button
                deleteButton = new Button
                {
                    Text = "Delete",
                    Size = new Size(80, 30),
                    Location = new Point(190, 10),
                    BackColor = Color.FromArgb(239, 68, 68),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                deleteButton.FlatAppearance.BorderSize = 0;
                deleteButton.Click += (s, e) => DeleteClicked?.Invoke(this, EventArgs.Empty);

                actionPanel.Controls.Add(addButton);
                actionPanel.Controls.Add(editButton);
                actionPanel.Controls.Add(deleteButton);

                // DataGridView
                dataGridView = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    ReadOnly = true,
                    RowHeadersVisible = false,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.None,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = Color.White,
                        ForeColor = Color.FromArgb(64, 64, 64),
                        SelectionBackColor = Color.FromArgb(219, 234, 254),
                        SelectionForeColor = Color.FromArgb(30, 64, 175),
                        Font = new Font("Segoe UI", 9F)
                    },
                    ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = Color.FromArgb(243, 244, 246),
                        ForeColor = Color.FromArgb(75, 85, 99),
                        Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                        Alignment = DataGridViewContentAlignment.MiddleLeft
                    },
                    EnableHeadersVisualStyles = false,
                    ColumnHeadersHeight = 40
                };

                this.Controls.Add(dataGridView);
                this.Controls.Add(actionPanel);
                this.Controls.Add(headerPanel);

                isInitialized = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing CustomTable: {ex.Message}");
                throw;
            }
        }

        private void InitializeBasicTable()
        {
            try
            {
                dataGridView = new DataGridView();
                dataGridView.Dock = DockStyle.Fill;
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView.AllowUserToAddRows = false;
                dataGridView.AllowUserToDeleteRows = false;
                dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView.MultiSelect = false;
                dataGridView.ReadOnly = true;
                dataGridView.RowHeadersVisible = false;
                dataGridView.BackgroundColor = Color.White;
                dataGridView.BorderStyle = BorderStyle.None;

                this.Controls.Add(dataGridView);
                isInitialized = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing basic DataGridView: {ex.Message}");
                throw;
            }
        }

        private void OnSearchTextChanged(object? sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                SearchTextChanged?.Invoke(this, textBox.Text);
            }
        }

        public DataGridView DataGridView
        {
            get { return dataGridView; }
        }

        public void SetDataSource(object dataSource)
        {
            try
            {
                if (!isInitialized || dataGridView == null)
                {
                    throw new InvalidOperationException("CustomTable is not properly initialized");
                }

                dataGridView.DataSource = dataSource;
                Application.DoEvents();
                
                System.Diagnostics.Debug.WriteLine($"DataSource set. Columns count: {dataGridView.Columns.Count}");
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
                if (dataGridView == null || dataGridView.SelectedRows.Count == 0)
                    return null;

                var selectedRow = dataGridView.SelectedRows[0];
                if (selectedRow.DataBoundItem is T item)
                {
                    return item;
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
                if (!ValidateForColumnCustomization())
                {
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Starting CustomizeColumns with {columnConfig?.Count ?? 0} configurations");

                if (columnConfig == null || columnConfig.Count == 0)
                {
                    return;
                }

                foreach (var config in columnConfig)
                {
                    try
                    {
                        ProcessColumnConfiguration(config.Key, config.Value);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error processing column '{config.Key}': {ex.Message}");
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CustomizeColumns: {ex.Message}");
                throw;
            }
        }

        private bool ValidateForColumnCustomization()
        {
            return isInitialized && 
                   dataGridView != null && 
                   !dataGridView.IsDisposed && 
                   dataGridView.Columns.Count > 0;
        }

        private void ProcessColumnConfiguration(string columnIdentifier, object settings)
        {
            DataGridViewColumn? column = FindColumn(columnIdentifier);
            
            if (column == null)
            {
                return;
            }

            ApplyColumnSettings(column, settings);
        }

        private DataGridViewColumn? FindColumn(string identifier)
        {
            try
            {
                if (dataGridView.Columns.Contains(identifier))
                {
                    return dataGridView.Columns[identifier];
                }

                DataGridViewColumn? column = dataGridView.Columns
                    .Cast<DataGridViewColumn>()
                    .FirstOrDefault(c => string.Equals(c.HeaderText, identifier, StringComparison.OrdinalIgnoreCase));

                if (column != null)
                {
                    return column;
                }

                if (int.TryParse(identifier, out int index) && 
                    index >= 0 && 
                    index < dataGridView.Columns.Count)
                {
                    return dataGridView.Columns[index];
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error finding column '{identifier}': {ex.Message}");
                return null;
            }
        }

        private void ApplyColumnSettings(DataGridViewColumn column, object settings)
        {
            try
            {
                if (settings is Dictionary<string, object> settingsDict)
                {
                    ApplyDictionarySettings(column, settingsDict);
                }
                else if (settings is int width)
                {
                    SetColumnWidth(column, width);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying settings to column '{column.Name}': {ex.Message}");
                throw;
            }
        }

        private void ApplyDictionarySettings(DataGridViewColumn column, Dictionary<string, object> settings)
        {
            foreach (var setting in settings)
            {
                try
                {
                    ApplySingleSetting(column, setting.Key, setting.Value);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error applying setting '{setting.Key}' to column '{column.Name}': {ex.Message}");
                }
            }
        }

        private void ApplySingleSetting(DataGridViewColumn column, string settingName, object value)
        {
            switch (settingName.ToLower())
            {
                case "width":
                    if (value is int width)
                        SetColumnWidth(column, width);
                    break;

                case "minwidth":
                case "minimumwidth":
                    if (value is int minWidth && minWidth > 0)
                        column.MinimumWidth = minWidth;
                    break;

                case "visible":
                    if (value is bool visible)
                        column.Visible = visible;
                    break;

                case "readonly":
                    if (value is bool readOnly)
                        column.ReadOnly = readOnly;
                    break;

                case "headertext":
                case "header":
                    if (value is string headerText)
                        column.HeaderText = headerText;
                    break;

                case "autosize":
                    if (value is bool autoSize && autoSize)
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    break;
            }
        }

        private void SetColumnWidth(DataGridViewColumn column, int width)
        {
            try
            {
                if (width > 0 && column.DataGridView != null && !column.DataGridView.IsDisposed)
                {
                    int safeWidth = Math.Max(width, column.MinimumWidth);
                    safeWidth = Math.Min(safeWidth, 1000);
                    column.Width = safeWidth;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting width for column {column?.Name}: {ex.Message}");
            }
        }

        private void SetupTheme()
        {
            if (_themeService == null) return;

            var colors = _themeService.CurrentColors;
            var isDark = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark;

            BackColor = colors.Background;
            
            if (headerPanel != null)
            {
                headerPanel.BackColor = colors.Surface;
                if (titleLabel != null)
                    titleLabel.ForeColor = colors.TextPrimary;
                if (searchTextBox != null)
                {
                    searchTextBox.BackColor = colors.Surface;
                    searchTextBox.ForeColor = colors.TextPrimary;
                }
            }

            if (actionPanel != null)
            {
                actionPanel.BackColor = isDark ? colors.Background : Color.FromArgb(248, 249, 250);
            }

            if (dataGridView != null)
            {
                dataGridView.BackgroundColor = colors.Surface;
                dataGridView.DefaultCellStyle.BackColor = colors.Surface;
                dataGridView.DefaultCellStyle.ForeColor = colors.TextPrimary;
                dataGridView.DefaultCellStyle.SelectionBackColor = colors.Primary;
                dataGridView.DefaultCellStyle.SelectionForeColor = colors.OnPrimary;
                
                dataGridView.ColumnHeadersDefaultCellStyle.BackColor = isDark ? colors.Background : Color.FromArgb(243, 244, 246);
                dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = colors.TextSecondary;
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
                if (dataGridView != null && !dataGridView.IsDisposed)
                {
                    dataGridView.Refresh();
                    Application.DoEvents();
                }
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
                if (dataGridView != null && !dataGridView.IsDisposed)
                {
                    dataGridView.DataSource = null;
                    dataGridView.Rows.Clear();
                    dataGridView.Columns.Clear();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing table: {ex.Message}");
            }
        }

        // Note: Dispose method is now only in CustomTable.Designer.cs to avoid duplicate definitions
    }
}