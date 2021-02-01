using System.Windows;
using Stylet;

namespace RyaUploaderV2.ViewModels
{
    public class TitleBarViewModel : Screen
    {
        private readonly IWindowManagerConfig _window;
        private readonly TrayIconViewModel _trayIconViewModel;

        public TitleBarViewModel(IWindowManagerConfig windowManager, TrayIconViewModel trayIconViewModel)
        {
            _window = windowManager;
            _trayIconViewModel = trayIconViewModel;
        }

        public void MoveWindow()
        {
            var window = _window.GetActiveWindow();
            window.DragMove();
        }
        
        public void CloseWindow()
        {
            _trayIconViewModel.IsShellVisible = Visibility.Hidden;
        }

    }
}
