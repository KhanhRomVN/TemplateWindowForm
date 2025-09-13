using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Core.Interfaces.Services;

namespace Presentation.WinFormsApp.UserControls.Common
{
    public partial class CustomTable : UserControl
    {
        private readonly IThemeService _themeService;
        private DataGridView _dataGridView = null!;
        private Panel _headerPanel = null!;
        private Label _titleLabel = null!;
        private CustomButton _addButton = null!;
        private CustomButton _editButton = null!;
        private CustomButton _deleteButton = null!;
        private CustomTextBox _searchTextBox = null!;

        public string Title
        {
            get => _titleLabel.Text;
            set => _titleLabel.Text = value;
        }

        public DataGridView DataGrid => _dataGridView;

        public event EventHandler? AddClicked;
        public event EventHandler? EditClicked;
        public event EventHandler? DeleteClicked;
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

            // Header Panel with rounded corners
            _headerPanel = new RoundedPanel(_themeService)
            {
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(15),
                BorderRadius = 4
            };

            // Title Label
            _titleLabel = new Label
            {
                Text = "Data Table",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(15, 15)
            };

            // Search TextBox
            _searchTextBox = new CustomTextBox(_themeService)
            {
                PlaceholderText = "Search...",
                Font = new Font("Segoe UI", 10F),
                Width = 250,
                Height = 35,
                Location = new Point(15, 40)
            };
            _searchTextBox.TextChanged += (s, e) => {
                var searchText = _searchTextBox.Text == _searchTextBox.PlaceholderText ? string.Empty : _searchTextBox.Text;
                SearchTextChanged?.Invoke(this, searchText);
            };

            // Action Buttons
            _addButton = new CustomButton(_themeService)
            {
                Text = "➕ Add",
                Font = new Font("Segoe UI", 10F),
                Size = new Size(90, 35),
                ButtonStyle = ButtonStyle.Success
            };
            _addButton.Click += (s, e) => AddClicked?.Invoke(this, EventArgs.Empty);

            _editButton = new CustomButton(_themeService)
            {
                Text = "✏️ Edit",
                Font = new Font("Segoe UI", 10F),
                Size = new Size(90, 35),
                ButtonStyle = ButtonStyle.Info
            };
            _editButton.Click += (s, e) => EditClicked?.Invoke(this, EventArgs.Empty);

            _deleteButton = new CustomButton(_themeService)
            {
                Text = "🗑️ Delete",
                Font = new Font("Segoe UI", 10F),
                Size = new Size(90, 35),
                ButtonStyle = ButtonStyle.Error
            };
            _deleteButton.Click += (s, e) => DeleteClicked?.Invoke(this, EventArgs.Empty);

            // Position buttons on the right side
            _addButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _editButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _deleteButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Custom DataGridView with rounded corners
            _dataGridView = new RoundedDataGridView(_themeService)
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.None,
                GridColor = Color.LightGray,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10F),
                    Padding = new Padding(8, 6, 8, 6)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    Padding = new Padding(8, 8, 8, 8)
                },
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 45,
                RowTemplate = { Height = 40 }
            };

            // Add controls to header panel
            _headerPanel.Controls.Add(_titleLabel);
            _headerPanel.Controls.Add(_searchTextBox);
            _headerPanel.Controls.Add(_addButton);
            _headerPanel.Controls.Add(_editButton);
            _headerPanel.Controls.Add(_deleteButton);

            // Add panels to main control
            Controls.Add(_dataGridView);
            Controls.Add(_headerPanel);

            // Handle resize to position buttons correctly
            _headerPanel.Resize += OnHeaderPanelResize;

            Name = "CustomTable";
            Size = new Size(800, 600);
            
            ResumeLayout(false);
        }

        private void OnHeaderPanelResize(object? sender, EventArgs e)
        {
            if (_headerPanel.Width > 0)
            {
                var rightMargin = 15;
                var buttonSpacing = 10;
                
                _deleteButton.Location = new Point(_headerPanel.Width - _deleteButton.Width - rightMargin, 15);
                _editButton.Location = new Point(_deleteButton.Left - _editButton.Width - buttonSpacing, 15);
                _addButton.Location = new Point(_editButton.Left - _addButton.Width - buttonSpacing, 15);
            }
        }

        public void SetDataSource<T>(List<T> data)
        {
            _dataGridView.DataSource = null;
            if (data != null && data.Count > 0)
            {
                _dataGridView.DataSource = data;
                _dataGridView.AutoResizeColumns();
                SetupDataGridTheme();
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

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            
            BackColor = colors.Background;
            _titleLabel.ForeColor = colors.TextPrimary;
            
            SetupDataGridTheme();
        }

        private void SetupDataGridTheme()
        {
            var colors = _themeService.CurrentColors;
            
            _dataGridView.BackgroundColor = colors.Surface;
            _dataGridView.DefaultCellStyle.BackColor = colors.Surface;
            _dataGridView.DefaultCellStyle.ForeColor = colors.OnSurface;
            _dataGridView.DefaultCellStyle.SelectionBackColor = colors.Primary;
            _dataGridView.DefaultCellStyle.SelectionForeColor = colors.OnPrimary;
            
            _dataGridView.ColumnHeadersDefaultCellStyle.BackColor = colors.Secondary;
            _dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = colors.OnSecondary;
            _dataGridView.EnableHeadersVisualStyles = false;
            
            _dataGridView.GridColor = colors.Border;
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

    // Helper class for rounded panel
    public class RoundedPanel : Panel
    {
        private readonly IThemeService _themeService;
        private int _borderRadius = 4;

        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value;
                Invalidate();
            }
        }

        public RoundedPanel(IThemeService themeService)
        {
            _themeService = themeService;
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);
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
            using (var pen = new Pen(colors.Border, 1))
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

            path.AddArc(arcRect, 180, 90);
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();
            return path;
        }
    }

    // Helper class for rounded DataGridView
    public class RoundedDataGridView : DataGridView
    {
        private readonly IThemeService _themeService;
        private int _borderRadius = 4;

        public int BorderRadius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value;
                Invalidate();
            }
        }

        public RoundedDataGridView(IThemeService themeService)
        {
            _themeService = themeService;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_borderRadius > 0)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var colors = _themeService.CurrentColors;
                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                var path = GetRoundedRectanglePath(rect, _borderRadius);

                // Draw border
                using (var pen = new Pen(colors.Border, 1))
                {
                    g.DrawPath(pen, path);
                }

                path.Dispose();
            }
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

            path.AddArc(arcRect, 180, 90);
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}