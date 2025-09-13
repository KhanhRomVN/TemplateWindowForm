using Core.Interfaces.Services;
using Infrastructure.Services;
using Presentation.WinFormsApp.UserControls;

namespace Presentation.WinFormsApp.Services
{
    public class ServiceContainer
    {
        private readonly Dictionary<Type, object> _services = new();
        private static ServiceContainer? _instance;

        public static ServiceContainer Instance => _instance ??= new ServiceContainer();

        private ServiceContainer()
        {
            RegisterServices();
        }

        private void RegisterServices()
        {
            // Register services as singletons
            var themeService = new ThemeService();
            var routerService = new RouterService();
            
            RegisterSingleton<IThemeService>(themeService);
            RegisterSingleton<IRouterService>(routerService);
            
            // Register routes with proper DI
            SetupRoutes(routerService, themeService);
        }

        private void SetupRoutes(RouterService routerService, IThemeService themeService)
        {
            // Register routes with factory functions for proper DI
            routerService.RegisterRoute("Home", () => new HomePage(themeService));
            routerService.RegisterRoute("Tool", () => new ToolPage(themeService));
            routerService.RegisterRoute("Settings", () => new SettingsPage(themeService));
        }

        public void RegisterSingleton<T>(T implementation) where T : class
        {
            _services[typeof(T)] = implementation;
        }

        public T GetService<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered.");
        }

        public T? GetService<T>(Type serviceType) where T : class
        {
            if (_services.TryGetValue(serviceType, out var service))
            {
                return (T)service;
            }

            return null;
        }
    }
}