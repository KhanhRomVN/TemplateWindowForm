using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;
using Core.ValueObjects;

namespace Presentation.WinFormsApp.UserControls
{
    public partial class HomePage : UserControl
    {
        private readonly IThemeService _themeService;
        private Panel _heroPanel = null!;
        private Panel _statsPanel = null!;
        private Panel _quickActionsPanel = null!;
        private Label _welcomeLabel = null!;
        private Label _subtitleLabel = null!;
        private Label _descriptionLabel = null!;
        private Button _getStartedButton = null!;
        private Button _learnMoreButton = null!;

        public HomePage(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent();
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Main container with proper layout
            var mainContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(0, 20, 0, 20),
                BackColor = Color.Transparent
            };

            mainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 50F)); // Hero section
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 25F)); // Stats section
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 25F)); // Quick actions section

            // Hero Section
            _heroPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(60, 40, 60, 40),
                Margin = new Padding(0, 0, 0, 20),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Welcome content with modern typography
            var heroContent = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 1,
                BackColor = Color.Transparent
            };

            heroContent.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Welcome
            heroContent.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Subtitle
            heroContent.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Description
            heroContent.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Spacer
            heroContent.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            _welcomeLabel = new Label
            {
                Text = "Welcome to Template App",
                Font = new Font("Segoe UI", 32F, FontStyle.Bold), // Fixed: proper Font constructor
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Height = 70,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            _subtitleLabel = new Label
            {
                Text = "Modern • Fast • Beautiful",
                Font = new Font("Segoe UI", 16F, FontStyle.Regular), // Fixed: proper Font constructor
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Height = 35,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 10, 0, 20)
            };

            _descriptionLabel = new Label
            {
                Text = "Experience the power of modern Windows Forms applications with beautiful UI components, theming support, and intuitive navigation.",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular), // Fixed: proper Font constructor
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Height = 60,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Margin = new Padding(40, 0, 40, 30)
            };

            // Action buttons
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Anchor = AnchorStyles.None,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 20, 0, 0)
            };

            _getStartedButton = new Button
            {
                Text = "Get Started",
                Size = new Size(140, 45),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold), // Fixed: proper Font constructor
                Margin = new Padding(10),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            _learnMoreButton = new Button
            {
                Text = "Learn More",
                Size = new Size(140, 45),
                Font = new Font("Segoe UI", 12F, FontStyle.Regular), // Fixed: proper Font constructor
                Margin = new Padding(10),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            buttonPanel.Controls.Add(_getStartedButton);
            buttonPanel.Controls.Add(_learnMoreButton);

            // Center the button panel
            var buttonContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Height = 70
            };
            buttonPanel.Left = (buttonContainer.Width - buttonPanel.Width) / 2;
            buttonPanel.Anchor = AnchorStyles.None;
            buttonContainer.Controls.Add(buttonPanel);

            heroContent.Controls.Add(_welcomeLabel, 0, 0);
            heroContent.Controls.Add(_subtitleLabel, 0, 1);
            heroContent.Controls.Add(_descriptionLabel, 0, 2);
            heroContent.Controls.Add(new Panel { BackColor = Color.Transparent }, 0, 3); // Spacer
            heroContent.Controls.Add(buttonContainer, 0, 4);

            _heroPanel.Controls.Add(heroContent);

            // Stats Section
            _statsPanel = CreateStatsPanel();

            // Quick Actions Section
            _quickActionsPanel = CreateQuickActionsPanel();

            mainContainer.Controls.Add(_heroPanel, 0, 0);
            mainContainer.Controls.Add(_statsPanel, 0, 1);
            mainContainer.Controls.Add(_quickActionsPanel, 0, 2);

            Controls.Add(mainContainer);

            Name = "HomePage";
            Size = new Size(800, 600);
            BackColor = Color.Transparent;
            
            ResumeLayout(false);
        }

        private Panel CreateStatsPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 20),
                Padding = new Padding(30, 20, 30, 20),
                BorderStyle = BorderStyle.FixedSingle
            };

            var statsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            statsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            statsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            statsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            // Create stat cards
            var stat1 = CreateStatCard("Active Users", "1,234", "👥");
            var stat2 = CreateStatCard("Total Tools", "56", "🔧");
            var stat3 = CreateStatCard("Success Rate", "98.5%", "📈");

            statsLayout.Controls.Add(stat1, 0, 0);
            statsLayout.Controls.Add(stat2, 1, 0);
            statsLayout.Controls.Add(stat3, 2, 0);

            panel.Controls.Add(statsLayout);
            return panel;
        }

        private Panel CreateStatCard(string title, string value, string icon)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                BackColor = Color.Transparent
            };

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 24F), // Fixed: proper Font constructor
                Size = new Size(40, 40),
                Location = new Point(10, 10),
                BackColor = Color.Transparent
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold), // Fixed: proper Font constructor
                Location = new Point(60, 8),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11F, FontStyle.Regular), // Fixed: proper Font constructor
                Location = new Point(60, 35),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            card.Controls.AddRange(new Control[] { iconLabel, valueLabel, titleLabel });
            return card;
        }

        private Panel CreateQuickActionsPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 25, 30, 25),
                BorderStyle = BorderStyle.FixedSingle
            };

            var titleLabel = new Label
            {
                Text = "Quick Actions",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold), // Fixed: proper Font constructor
                AutoSize = true,
                Location = new Point(0, 0),
                BackColor = Color.Transparent
            };

            var actionsLayout = new FlowLayoutPanel
            {
                Location = new Point(0, 40),
                Size = new Size(panel.Width - 60, panel.Height - 65),
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            // Quick action buttons
            var viewToolsBtn = CreateQuickActionButton("View Tools", "Manage your tools", "🔧");
            var settingsBtn = CreateQuickActionButton("Settings", "Customize app", "⚙️");
            var helpBtn = CreateQuickActionButton("Help", "Get support", "❓");

            actionsLayout.Controls.AddRange(new Control[] { viewToolsBtn, settingsBtn, helpBtn });

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(actionsLayout);
            return panel;
        }

        private Button CreateQuickActionButton(string title, string subtitle, string icon)
        {
            var button = new Button
            {
                Size = new Size(160, 80),
                Margin = new Padding(5),
                Font = new Font("Segoe UI", 10F, FontStyle.Regular), // Fixed: proper Font constructor
                Text = $"{icon}\n{title}\n{subtitle}",
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            button.FlatAppearance.BorderSize = 1;
            return button;
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            var isDark = _themeService.CurrentTheme == Core.Enums.ThemeType.Dark;

            BackColor = colors.Background;

            // Hero panel
            _heroPanel.BackColor = colors.Surface;

            // Text colors
            _welcomeLabel.ForeColor = colors.TextPrimary;
            _subtitleLabel.ForeColor = colors.Primary;
            _descriptionLabel.ForeColor = colors.TextSecondary;

            // Buttons
            _getStartedButton.BackColor = colors.Primary;
            _getStartedButton.ForeColor = colors.OnPrimary;
            _getStartedButton.FlatAppearance.BorderColor = colors.Primary;

            _learnMoreButton.BackColor = Color.Transparent;
            _learnMoreButton.ForeColor = colors.Primary;
            _learnMoreButton.FlatAppearance.BorderColor = colors.Primary;

            // Stats panel
            _statsPanel.BackColor = colors.Surface;

            // Update stat cards
            UpdateStatCardColors(_statsPanel, colors);

            // Quick actions panel
            _quickActionsPanel.BackColor = colors.Surface;

            // Update quick action colors
            UpdateQuickActionColors(_quickActionsPanel, colors);
        }

        private void UpdateStatCardColors(Panel parent, ColorPalette colors)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TableLayoutPanel layout)
                {
                    foreach (Control card in layout.Controls)
                    {
                        if (card is Panel cardPanel)
                        {
                            foreach (Control cardControl in cardPanel.Controls)
                            {
                                if (cardControl is Label label)
                                {
                                    if (label.Font.Bold)
                                        label.ForeColor = colors.TextPrimary;
                                    else
                                        label.ForeColor = colors.TextSecondary;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateQuickActionColors(Panel parent, ColorPalette colors)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Label label)
                {
                    label.ForeColor = colors.TextPrimary;
                }
                else if (control is FlowLayoutPanel flow)
                {
                    foreach (Control btn in flow.Controls)
                    {
                        if (btn is Button button)
                        {
                            button.BackColor = Color.Transparent;
                            button.ForeColor = colors.TextSecondary;
                            button.FlatAppearance.BorderColor = colors.Border;
                        }
                    }
                }
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