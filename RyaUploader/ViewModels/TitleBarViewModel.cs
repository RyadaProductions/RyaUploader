using System.Windows;
using Stylet;

namespace RyaUploader.ViewModels
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

        /// <summary>
        /// Method invoked when the user is holding leftmouse button over the whole UserControl to be able to drag the window.
        /// </summary>
        public void MoveWindow()
        {
            var window = _window.GetActiveWindow();
            window.DragMove();
        }

        /// <summary>
        /// Method invoked when the user clicks the close button on the window.
        /// This hides the window from the user but keeps it running in the background.
        /// </summary>
        public void CloseWindow()
        {
            _trayIconViewModel.IsShellVisible = Visibility.Hidden;
        }

    }
}
