using System.Windows;
using Stylet;

namespace RyaUploaderV2.ViewModels
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

        public bool CanShowWindowCommand => IsShellVisible != Visibility.Visible;

        public void ShowWindowCommand()
        {
            IsShellVisible = Visibility.Visible;
        }

        public void ExitApplicationCommand()
        {
            Application.Current.Shutdown();
        }
    }
}
