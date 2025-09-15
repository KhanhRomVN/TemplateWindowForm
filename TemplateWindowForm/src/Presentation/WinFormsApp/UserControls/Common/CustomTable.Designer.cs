namespace Presentation.WinFormsApp.UserControls.Common
{
    partial class CustomTable
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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

                    if (searchTextBox != null)
                    {
                        searchTextBox.TextChanged -= OnSearchTextChanged;
                    }

                    if (dataGridView != null && !dataGridView.IsDisposed)
                    {
                        dataGridView.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error disposing CustomTable: {ex.Message}");
                }
                
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        // InitializeComponent is implemented in CustomTable.cs

        #endregion
    }
}