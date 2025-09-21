using Core.Interfaces.Services;
using Infrastructure.Services;
using Presentation.WinFormsApp.UserControls;

namespace Presentation.WinFormsApp.Services
{
    public class ServiceContainer
    {
        private readonly Dictionary<Type, object> _services = new();
        private readonly Dictionary<Type, Func<object>> _factories = new();
        private static ServiceContainer? _instance;

        public static ServiceContainer Instance => _instance ??= new ServiceContainer();

        private ServiceContainer()
        {
            RegisterServices();
        }

        private void RegisterServices()
        {
            try
            {
                // Register core services as singletons
                var themeService = new ThemeService();
                var routerService = new RouterService();
                var errorHandlerService = new ErrorHandlerService();
                
                RegisterSingleton<IThemeService>(themeService);
                RegisterSingleton<IRouterService>(routerService);
                RegisterSingleton<IErrorHandlerService>(errorHandlerService);
                
                // Setup routes with proper error handling
                SetupRoutes(routerService, themeService);
                
                System.Diagnostics.Debug.WriteLine("ServiceContainer: All services registered successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ServiceContainer initialization error: {ex.Message}");
                throw;
            }
        }

        private void SetupRoutes(RouterService routerService, IThemeService themeService)
        {
            try
            {
                // Register the 3 main routes as specified in your application
                routerService.RegisterRoute("Home", () => 
                {
                    try
                    {
                        return new HomePage(themeService);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error creating HomePage: {ex.Message}");
                        throw;
                    }
                });
                
                routerService.RegisterRoute("Tool", () => 
                {
                    try
                    {
                        return new ToolPage(themeService);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error creating ToolPage: {ex.Message}");
                        throw;
                    }
                });
                
                routerService.RegisterRoute("Settings", () => 
                {
                    try
                    {
                        return new SettingsPage(themeService);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error creating SettingsPage: {ex.Message}");
                        throw;
                    }
                });
                
                System.Diagnostics.Debug.WriteLine("ServiceContainer: All routes registered successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Route setup error: {ex.Message}");
                throw;
            }
        }

        public void RegisterSingleton<T>(T implementation) where T : class
        {
            if (implementation == null)
                throw new ArgumentNullException(nameof(implementation));
                
            _services[typeof(T)] = implementation;
            System.Diagnostics.Debug.WriteLine($"ServiceContainer: Registered singleton {typeof(T).Name}");
        }

        public void RegisterFactory<T>(Func<T> factory) where T : class
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
                
            _factories[typeof(T)] = () => factory();
            System.Diagnostics.Debug.WriteLine($"ServiceContainer: Registered factory {typeof(T).Name}");
        }

        public T GetService<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered. Available services: {string.Join(", ", _services.Keys.Select(k => k.Name))}");
        }

        public T CreateComponent<T>() where T : class
        {
            if (_factories.TryGetValue(typeof(T), out var factory))
            {
                return (T)factory();
            }

            throw new InvalidOperationException($"Component factory for type {typeof(T).Name} is not registered. Available factories: {string.Join(", ", _factories.Keys.Select(k => k.Name))}");
        }

        public T? GetService<T>(Type serviceType) where T : class
        {
            if (_services.TryGetValue(serviceType, out var service))
            {
                return (T)service;
            }

            return null;
        }

        public bool IsServiceRegistered<T>() where T : class
        {
            return _services.ContainsKey(typeof(T));
        }

        public bool IsFactoryRegistered<T>() where T : class
        {
            return _factories.ContainsKey(typeof(T));
        }

        public void UnregisterService<T>() where T : class
        {
            _services.Remove(typeof(T));
            System.Diagnostics.Debug.WriteLine($"ServiceContainer: Unregistered service {typeof(T).Name}");
        }

        public void Clear()
        {
            _services.Clear();
            _factories.Clear();
            System.Diagnostics.Debug.WriteLine("ServiceContainer: All services and factories cleared");
        }

        public IEnumerable<Type> GetRegisteredServices()
        {
            return _services.Keys;
        }

        public IEnumerable<Type> GetRegisteredFactories()
        {
            return _factories.Keys;
        }
    }
}