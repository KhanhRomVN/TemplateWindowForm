using Core.Interfaces.Services;
using Presentation.WinFormsApp.Forms;
using Presentation.WinFormsApp.Services;

namespace Presentation.WinFormsApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Get services from container
            var serviceContainer = ServiceContainer.Instance;
            var themeService = serviceContainer.GetService<IThemeService>();
            var routerService = serviceContainer.GetService<IRouterService>();

            // Create and run main form
            var mainForm = new MainForm(themeService, routerService);
            Application.Run(mainForm);
        }
    }
}