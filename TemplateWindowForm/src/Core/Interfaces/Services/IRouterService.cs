using System.Windows.Forms;

namespace Core.Interfaces.Services
{
    public interface IRouterService
    {
        event EventHandler<NavigatedEventArgs>? Navigated;
        
        UserControl? CurrentView { get; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        
        void RegisterRoute(string routeName, Type controlType);
        void RegisterRoute(string routeName, Func<UserControl> controlFactory);
        void NavigateTo(string routeName, object? parameter = null);
        void GoBack();
        void GoForward();
        void ClearHistory();
    }

    public class NavigatedEventArgs : EventArgs
    {
        public string RouteName { get; }
        public UserControl View { get; }
        public object? Parameter { get; }

        public NavigatedEventArgs(string routeName, UserControl view, object? parameter = null)
        {
            RouteName = routeName;
            View = view;
            Parameter = parameter;
        }
    }
}