using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Infrastructure.Services
{
    public interface IErrorHandlerService
    {
        void LogError(string message, Exception exception = null);
        void ShowError(string message, string title = "Error");
        void ShowWarning(string message, string title = "Warning");
        void HandleCriticalError(string message, Exception exception, bool showToUser = true);
    }

    public class ErrorHandlerService : IErrorHandlerService
    {
        private readonly bool _isDebugMode;

        public ErrorHandlerService()
        {
            _isDebugMode = Debugger.IsAttached || Debug.Listeners.Count > 1;
        }

        public void LogError(string message, Exception exception = null)
        {
            var logMessage = $"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            
            if (exception != null)
            {
                logMessage += $"\nException: {exception.Message}";
                logMessage += $"\nStack Trace: {exception.StackTrace}";
            }

            Debug.WriteLine(logMessage);

            // In production, you might want to log to file or send to logging service
            // Example: File.AppendAllText("app_errors.log", logMessage + Environment.NewLine);
        }

        public void ShowError(string message, string title = "Error")
        {
            try
            {
                if (Application.OpenForms.Count > 0)
                {
                    var mainForm = Application.OpenForms[0];
                    if (mainForm.InvokeRequired)
                    {
                        mainForm.Invoke((MethodInvoker)(() => 
                            MessageBox.Show(mainForm, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)));
                    }
                    else
                    {
                        MessageBox.Show(mainForm, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing error message: {ex.Message}");
            }
        }

        public void ShowWarning(string message, string title = "Warning")
        {
            try
            {
                if (Application.OpenForms.Count > 0)
                {
                    var mainForm = Application.OpenForms[0];
                    if (mainForm.InvokeRequired)
                    {
                        mainForm.Invoke((MethodInvoker)(() => 
                            MessageBox.Show(mainForm, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                    }
                    else
                    {
                        MessageBox.Show(mainForm, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing warning message: {ex.Message}");
            }
        }

        public void HandleCriticalError(string message, Exception exception, bool showToUser = true)
        {
            LogError($"CRITICAL ERROR: {message}", exception);

            if (showToUser)
            {
                var userMessage = _isDebugMode 
                    ? $"{message}\n\nTechnical details:\n{exception?.Message}" 
                    : message;

                ShowError(userMessage, "Critical Error");
            }
        }
    }
}