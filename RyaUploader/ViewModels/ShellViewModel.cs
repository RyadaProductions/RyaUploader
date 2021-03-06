using Stylet;

namespace RyaUploader.ViewModels
{
    public class ShellViewModel : Screen
    {
        public MainViewModel CurrentViewModel { get; set; }
        public TrayIconViewModel TrayIconViewModel { get; set; }
        public TitleBarViewModel TitleBarViewModel { get; set; }

        public ShellViewModel(MainViewModel main, TrayIconViewModel trayIconViewModel, TitleBarViewModel titleBarViewModel)
        {
            CurrentViewModel = main;
            TrayIconViewModel = trayIconViewModel;
            TitleBarViewModel = titleBarViewModel;
        }
    }
}
