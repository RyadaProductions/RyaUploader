using System.Windows;
using Stylet;

namespace RyaUploader.ViewModels
{
    public class TrayIconViewModel : Screen
    {
        private Visibility _isShellVisible = Visibility.Visible;
        public Visibility IsShellVisible
        {
            get => _isShellVisible;
            set
            {
                SetAndNotify(ref _isShellVisible, value);
                NotifyOfPropertyChange(nameof(CanShowWindowCommand));
            }
        }

        /// <summary>
        /// Checks if the window is visible and returns false when it isn't. This is used to disable the show option in the context menu.
        /// </summary>
        public bool CanShowWindowCommand => IsShellVisible != Visibility.Visible;

        /// <summary>
        /// Method invoked from the tray icon, sets the current visibility of the shellview to visible
        /// </summary>
        public void ShowWindowCommand()
        {
            IsShellVisible = Visibility.Visible;
        }

        /// <summary>
        /// Method intended to shutdown the application when the user presses exit in the tray icon
        /// </summary>
        public void ExitApplicationCommand()
        {
            Application.Current.Shutdown(0);
        }
    }
}
