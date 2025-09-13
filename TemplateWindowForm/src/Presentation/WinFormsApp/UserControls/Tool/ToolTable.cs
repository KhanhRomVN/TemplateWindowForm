using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces.Services;
using Presentation.WinFormsApp.UserControls.Common;
using Presentation.WinFormsApp.Models;

namespace Presentation.WinFormsApp.UserControls.Tool
{
    public partial class ToolTable : UserControl
    {
        private readonly IThemeService _themeService;
        private CustomTable _customTable = null!;
        private List<ToolModel> _allTools = new();
        private List<ToolModel> _filteredTools = new();

        public ToolTable(IThemeService themeService)
        {
            _themeService = themeService;
            InitializeComponent();
            LoadData();
            SetupTheme();
            
            _themeService.ThemeChanged += OnThemeChanged;
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Custom Table
            _customTable = new CustomTable(_themeService)
            {
                Dock = DockStyle.Fill,
                Title = "Tools Management"
            };

            // Subscribe to table events
            _customTable.AddClicked += OnAddTool;
            _customTable.EditClicked += OnEditTool;
            _customTable.DeleteClicked += OnDeleteTool;
            _customTable.SearchTextChanged += OnSearchTextChanged;

            Controls.Add(_customTable);

            Name = "ToolTable";
            Size = new Size(800, 600);
            
            ResumeLayout(false);
        }

        private void LoadData()
        {
            _allTools = ToolModel.GetFakeData();
            _filteredTools = _allTools.ToList();
            _customTable.SetDataSource(_filteredTools);
        }

        private void OnAddTool(object? sender, EventArgs e)
        {
            MessageBox.Show("Add Tool functionality will be implemented here.", "Add Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnEditTool(object? sender, EventArgs e)
        {
            var selectedTool = _customTable.GetSelectedItem<ToolModel>();
            if (selectedTool != null)
            {
                MessageBox.Show($"Edit Tool: {selectedTool.Name}\nThis functionality will be implemented here.", "Edit Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select a tool to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OnDeleteTool(object? sender, EventArgs e)
        {
            var selectedTool = _customTable.GetSelectedItem<ToolModel>();
            if (selectedTool != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{selectedTool.Name}'?", 
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    _allTools.Remove(selectedTool);
                    _filteredTools.Remove(selectedTool);
                    _customTable.SetDataSource(_filteredTools);
                    
                    MessageBox.Show($"Tool '{selectedTool.Name}' has been deleted.", "Tool Deleted", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a tool to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OnSearchTextChanged(object? sender, string searchText)
        {
            _filteredTools = ToolModel.SearchTools(_allTools, searchText);
            _customTable.SetDataSource(_filteredTools);
        }

        private void SetupTheme()
        {
            var colors = _themeService.CurrentColors;
            BackColor = colors.Background;
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
                
                _customTable.AddClicked -= OnAddTool;
                _customTable.EditClicked -= OnEditTool;
                _customTable.DeleteClicked -= OnDeleteTool;
                _customTable.SearchTextChanged -= OnSearchTextChanged;
            }
            base.Dispose(disposing);
        }
    }
}