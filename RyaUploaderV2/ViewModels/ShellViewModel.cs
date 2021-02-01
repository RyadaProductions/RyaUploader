using Stylet;

namespace RyaUploaderV2.ViewModels
{
    public class ShellViewModel : Screen
    {
        public object CurrentViewModel { get; set; }

        public ShellViewModel(MainViewModel main)
        {
            CurrentViewModel = main;
        }
    }
}
