using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Interfaces.Services; // Fixed: Added correct using directive
using Presentation.WinFormsApp.UserControls.Common;

namespace Presentation.WinFormsApp.UserControls
{
    public partial class ToolPage : UserControl
    {
        private readonly IThemeService _themeService;
        private CustomTable customTable;
        private bool isDataLoaded = false;
        private bool isDisposing = false;

        public ToolPage(IThemeService themeService)
        {
            _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
            
            try
            {
                InitializeComponent();
                InitializeCustomComponents();
                
                // Use Load event instead of calling LoadData in constructor
                this.Load += ToolPage_Load;
                this.HandleCreated += ToolPage_HandleCreated;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ToolPage constructor: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Show error to user
                ShowErrorMessage($"Failed to initialize ToolPage: {ex.Message}");
                throw;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ToolPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.Name = "ToolPage";
            this.Size = new System.Drawing.Size(1000, 700);
            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            try
            {
                // Initialize CustomTable
                customTable = new CustomTable();
                customTable.Dock = DockStyle.Fill;
                customTable.Name = "customTable";
                
                this.Controls.Add(customTable);
                
                System.Diagnostics.Debug.WriteLine("CustomTable initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing custom components: {ex.Message}");
                throw;
            }
        }

        private void ToolPage_HandleCreated(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("ToolPage handle created");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ToolPage_HandleCreated: {ex.Message}");
            }
        }

        private void ToolPage_Load(object sender, EventArgs e)
        {
            try
            {
                if (isDisposing || IsDisposed)
                {
                    return;
                }

                System.Diagnostics.Debug.WriteLine("ToolPage_Load started");
                
                // Ensure everything is properly initialized
                if (customTable?.DataGridView == null)
                {
                    throw new InvalidOperationException("CustomTable or DataGridView is not initialized");
                }

                // Use BeginInvoke to ensure UI is fully ready
                this.BeginInvoke((MethodInvoker)delegate
                {
                    try
                    {
                        LoadData();
                    }
                    catch (Exception loadEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error in LoadData: {loadEx.Message}");
                        ShowErrorMessage($"Failed to load data: {loadEx.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ToolPage_Load: {ex.Message}");
                ShowErrorMessage($"Failed to load page: {ex.Message}");
            }
        }

        private void LoadData()
        {
            try
            {
                if (isDataLoaded || isDisposing || IsDisposed)
                {
                    System.Diagnostics.Debug.WriteLine("Data already loaded or control is disposing");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("LoadData started");

                // Validate components
                if (!ValidateComponents())
                {
                    throw new InvalidOperationException("Components are not properly initialized");
                }

                // Create and set sample data
                DataTable sampleData = CreateSampleData();
                customTable.SetDataSource(sampleData);

                // Wait a bit for DataGridView to process the data
                Application.DoEvents();
                System.Threading.Thread.Sleep(100);

                // Now customize columns
                CustomizeColumns();

                isDataLoaded = true;
                System.Diagnostics.Debug.WriteLine("LoadData completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadData: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private bool ValidateComponents()
        {
            try
            {
                if (customTable == null)
                {
                    System.Diagnostics.Debug.WriteLine("CustomTable is null");
                    return false;
                }

                if (customTable.DataGridView == null)
                {
                    System.Diagnostics.Debug.WriteLine("DataGridView is null");
                    return false;
                }

                if (customTable.IsDisposed)
                {
                    System.Diagnostics.Debug.WriteLine("CustomTable is disposed");
                    return false;
                }

                if (customTable.DataGridView.IsDisposed)
                {
                    System.Diagnostics.Debug.WriteLine("DataGridView is disposed");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating components: {ex.Message}");
                return false;
            }
        }

        private DataTable CreateSampleData()
        {
            try
            {
                DataTable table = new DataTable();
                
                // Add columns
                table.Columns.Add("ID", typeof(int));
                table.Columns.Add("Name", typeof(string));
                table.Columns.Add("Description", typeof(string));
                table.Columns.Add("Status", typeof(string));
                table.Columns.Add("DateCreated", typeof(DateTime));

                // Add sample rows
                for (int i = 1; i <= 10; i++)
                {
                    table.Rows.Add(
                        i,
                        $"Tool {i}",
                        $"Description for tool {i}",
                        i % 2 == 0 ? "Active" : "Inactive",
                        DateTime.Now.AddDays(-i)
                    );
                }

                System.Diagnostics.Debug.WriteLine($"Sample data created with {table.Rows.Count} rows and {table.Columns.Count} columns");
                return table;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating sample data: {ex.Message}");
                throw;
            }
        }

        private void CustomizeColumns()
        {
            try
            {
                if (!ValidateComponents())
                {
                    System.Diagnostics.Debug.WriteLine("Cannot customize columns: components not valid");
                    return;
                }

                // Wait for DataGridView to be fully ready
                if (customTable.DataGridView.Columns.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("No columns found, waiting for DataGridView to initialize...");
                    
                    // Try waiting a bit more
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(200);
                    
                    if (customTable.DataGridView.Columns.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Still no columns found after waiting");
                        return;
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Customizing columns. Found {customTable.DataGridView.Columns.Count} columns");

                // Define column configuration
                var columnConfig = new Dictionary<string, object>
                {
                    {
                        "ID", new Dictionary<string, object>
                        {
                            { "width", 80 },
                            { "headertext", "ID" },
                            { "readonly", true }
                        }
                    },
                    {
                        "Name", new Dictionary<string, object>
                        {
                            { "width", 150 },
                            { "headertext", "Tool Name" },
                            { "readonly", false }
                        }
                    },
                    {
                        "Description", new Dictionary<string, object>
                        {
                            { "width", 300 },
                            { "headertext", "Description" },
                            { "readonly", false }
                        }
                    },
                    {
                        "Status", new Dictionary<string, object>
                        {
                            { "width", 100 },
                            { "headertext", "Status" },
                            { "readonly", false }
                        }
                    },
                    {
                        "DateCreated", new Dictionary<string, object>
                        {
                            { "width", 150 },
                            { "headertext", "Date Created" },
                            { "readonly", true }
                        }
                    }
                };

                // Apply column customization
                customTable.CustomizeColumns(columnConfig);
                
                System.Diagnostics.Debug.WriteLine("Column customization completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CustomizeColumns: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Don't throw here, just log the error
                // This allows the page to still function even if column customization fails
                ShowErrorMessage($"Warning: Could not customize columns: {ex.Message}");
            }
        }

        private void ShowErrorMessage(string message)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)(() => ShowErrorMessage(message)));
                    return;
                }

                MessageBox.Show(
                    message, 
                    "Tool Page Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing error message: {ex.Message}");
            }
        }

        // Public method to reload data
        public void ReloadData()
        {
            try
            {
                isDataLoaded = false;
                customTable?.ClearTable();
                LoadData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reloading data: {ex.Message}");
                ShowErrorMessage($"Failed to reload data: {ex.Message}");
            }
        }

        // Public method to refresh the table
        public void RefreshTable()
        {
            try
            {
                customTable?.RefreshTable();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing table: {ex.Message}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !isDisposing)
            {
                try
                {
                    isDisposing = true;
                    
                    // Remove event handlers
                    this.Load -= ToolPage_Load;
                    this.HandleCreated -= ToolPage_HandleCreated;
                    
                    // Dispose custom components
                    if (customTable != null && !customTable.IsDisposed)
                    {
                        customTable.Dispose();
                        customTable = null;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error disposing ToolPage: {ex.Message}");
                }
            }
            
            base.Dispose(disposing);
        }
    }
}