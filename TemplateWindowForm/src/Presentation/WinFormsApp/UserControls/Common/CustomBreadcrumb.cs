using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Interfaces.Services;

namespace Presentation.WinFormsApp.UserControls.Common
{
    public class BreadcrumbItem
    {
        public string Label { get; set; } = string.Empty;
        public string? Route { get; set; }
        public Image? Icon { get; set; }
        public bool IsCurrentPage { get; set; }
        public Action? OnClick { get; set; }
    }

    public partial class CustomBreadcrumb : UserControl
    {
        private readonly IThemeService _themeService;
        private readonly IRouterService? _routerService;
        private List<BreadcrumbItem> _items = new();
        private FlowLayoutPanel _breadcrumbPanel = null!;
        private bool _showHomeIcon = true;

        public List<BreadcrumbItem> Items
        {
            get => _items;
            set
            {
                _items = value;
                UpdateBreadcrumb();
            }
        }

        public bool ShowHomeIcon
        {
            get => _showHomeIcon;
            set
            {
                _showHomeIcon = value;
                UpdateBreadcrumb();
            }
        }

        public CustomBreadcrumb(IThemeService themeService, IRouterService? routerService = null)
        {
            _themeService = themeService;
            _routerService = routerService;
            
            InitializeComponent(); // Use the standard designer method
            InitializeBreadcrumb(); // Then our custom initialization
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeBreadcrumb()
        {
            SuspendLayout();

            // Main container
            Size = new Size(600, 35);
            BackColor = Color.Transparent;
            Padding = new Padding(0);
            Margin = new Padding(0);

            // Breadcrumb panel
            _breadcrumbPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0),
                AutoScroll = true,
                AutoScrollMinSize = new Size(0, 0)
            };

            Controls.Add(_breadcrumbPanel);

            Name = "CustomBreadcrumb";
            ResumeLayout(false);
        }

        private void UpdateBreadcrumb()
        {
            _breadcrumbPanel.Controls.Clear();

            if (!_items.Any()) return;

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var isLast = i == _items.Count - 1;

                // Add separator for non-first items
                if (i > 0)
                {
                    var separator = CreateSeparator();
                    _breadcrumbPanel.Controls.Add(separator);
                }

                // Create breadcrumb item
                var itemControl = CreateBreadcrumbItem(item, isLast, i == 0);
                _breadcrumbPanel.Controls.Add(itemControl);
            }
        }

        private Label CreateSeparator()
        {
            var colors = _themeService.CurrentColors;
            
            return new Label
            {
                Text = "›",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = colors.TextSecondary,
                AutoSize = true,
                Margin = new Padding(8, 6, 8, 0),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        private Control CreateBreadcrumbItem(BreadcrumbItem item, bool isLast, bool isFirst)
        {
            var colors = _themeService.CurrentColors;
            var isClickable = (!string.IsNullOrEmpty(item.Route) || item.OnClick != null) && !item.IsCurrentPage && !isLast;

            var container = new Panel
            {
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0),
                Height = 24
            };

            // Create icon panel
            Panel? iconPanel = null;
            if (item.Icon != null || (isFirst && _showHomeIcon && item.Icon == null))
            {
                iconPanel = new Panel
                {
                    Size = new Size(16, 16),
                    BackColor = Color.Transparent,
                    Margin = new Padding(0, 4, 4, 0)
                };

                var pictureBox = new PictureBox
                {
                    Size = new Size(16, 16),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.Transparent
                };

                if (item.Icon != null)
                {
                    pictureBox.Image = item.Icon;
                }
                else if (isFirst && _showHomeIcon)
                {
                    // Create simple home icon
                    pictureBox.Image = CreateHomeIcon(colors);
                }

                iconPanel.Controls.Add(pictureBox);
            }

            // Create label
            var label = new Label
            {
                Text = item.Label,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 4, 0, 0),
                ForeColor = isLast || item.IsCurrentPage ? colors.TextPrimary : colors.TextSecondary,
                Cursor = isClickable ? Cursors.Hand : Cursors.Default
            };

            // Add click behavior
            if (isClickable)
            {
                label.MouseEnter += (s, e) => label.ForeColor = colors.Primary;
                label.MouseLeave += (s, e) => label.ForeColor = colors.TextSecondary;
                
                label.Click += (s, e) =>
                {
                    if (item.OnClick != null)
                    {
                        item.OnClick();
                    }
                    else if (!string.IsNullOrEmpty(item.Route) && _routerService != null)
                    {
                        try
                        {
                            _routerService.NavigateTo(item.Route);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                        }
                    }
                };
            }

            // Layout items horizontally
            int x = 0;
            if (iconPanel != null)
            {
                iconPanel.Location = new Point(x, 0);
                container.Controls.Add(iconPanel);
                x += iconPanel.Width + iconPanel.Margin.Right;
            }

            label.Location = new Point(x, 0);
            container.Controls.Add(label);

            // Calculate container width
            container.Width = x + label.Width;

            return container;
        }

        private Image CreateHomeIcon(Core.ValueObjects.ColorPalette colors)
        {
            var bitmap = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                // Draw simple house icon
                var pen = new Pen(colors.TextSecondary, 1.5f);
                var brush = new SolidBrush(Color.FromArgb(50, colors.TextSecondary.R, colors.TextSecondary.G, colors.TextSecondary.B));

                // House base
                var houseRect = new Rectangle(3, 8, 10, 6);
                g.FillRectangle(brush, houseRect);
                g.DrawRectangle(pen, houseRect);

                // Roof
                var roofPoints = new[] {
                    new Point(8, 3),
                    new Point(2, 8),
                    new Point(14, 8)
                };
                g.FillPolygon(brush, roofPoints);
                g.DrawPolygon(pen, roofPoints);

                // Door
                var doorRect = new Rectangle(7, 10, 2, 4);
                g.FillRectangle(new SolidBrush(colors.Background), doorRect);
                g.DrawRectangle(pen, doorRect);

                pen.Dispose();
                brush.Dispose();
            }
            return bitmap;
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            BackColor = colors.Background;
            UpdateBreadcrumb();
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
        }
    }
}