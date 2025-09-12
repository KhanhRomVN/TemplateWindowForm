using System.Windows.Forms;
using Core.Interfaces.Services;

namespace Infrastructure.Services
{
    public class RouterService : IRouterService
    {
        private readonly Dictionary<string, Func<UserControl>> _routes = new();
        private readonly Stack<NavigationItem> _backHistory = new();
        private readonly Stack<NavigationItem> _forwardHistory = new();
        private UserControl? _currentView;

        public event EventHandler<NavigatedEventArgs>? Navigated;

        public UserControl? CurrentView => _currentView;
        public bool CanGoBack => _backHistory.Count > 0;
        public bool CanGoForward => _forwardHistory.Count > 0;

        public void RegisterRoute(string routeName, Func<UserControl> controlFactory)
        {
            _routes[routeName] = controlFactory;
        }

        public void RegisterRoute(string routeName, Type controlType)
        {
            if (!typeof(UserControl).IsAssignableFrom(controlType))
                throw new ArgumentException($"Type {controlType.Name} must inherit from UserControl");

            _routes[routeName] = () => (UserControl)Activator.CreateInstance(controlType)!;
        }

        public void NavigateTo(string routeName, object? parameter = null)
        {
            if (!_routes.TryGetValue(routeName, out var controlFactory))
                throw new ArgumentException($"Route '{routeName}' not found");

            // Add current view to back history
            if (_currentView != null)
            {
                _backHistory.Push(new NavigationItem(GetCurrentRouteName(), _currentView, parameter));
            }

            // Clear forward history when navigating to new route
            _forwardHistory.Clear();

            // Create new instance
            var newView = controlFactory();
            _currentView = newView;

            OnNavigated(routeName, newView, parameter);
        }

        public void GoBack()
        {
            if (!CanGoBack) return;

            var backItem = _backHistory.Pop();
            
            // Add current view to forward history
            if (_currentView != null)
            {
                _forwardHistory.Push(new NavigationItem(GetCurrentRouteName(), _currentView, null));
            }

            _currentView = backItem.View;
            OnNavigated(backItem.RouteName, backItem.View, backItem.Parameter);
        }

        public void GoForward()
        {
            if (!CanGoForward) return;

            var forwardItem = _forwardHistory.Pop();
            
            // Add current view to back history
            if (_currentView != null)
            {
                _backHistory.Push(new NavigationItem(GetCurrentRouteName(), _currentView, null));
            }

            _currentView = forwardItem.View;
            OnNavigated(forwardItem.RouteName, forwardItem.View, forwardItem.Parameter);
        }

        public void ClearHistory()
        {
            _backHistory.Clear();
            _forwardHistory.Clear();
        }

        private string GetCurrentRouteName()
        {
            if (_currentView == null) return string.Empty;
            
            var currentType = _currentView.GetType();
            return _routes.FirstOrDefault(kvp => kvp.Value().GetType() == currentType).Key ?? string.Empty;
        }

        private void OnNavigated(string routeName, UserControl view, object? parameter)
        {
            Navigated?.Invoke(this, new NavigatedEventArgs(routeName, view, parameter));
        }

        private record NavigationItem(string RouteName, UserControl View, object? Parameter);
    }
}