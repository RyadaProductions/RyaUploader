using Stylet;

namespace RyaUploader.ViewModels
{
    public class MainViewModel : Screen
    {
        public BoilerClientViewModel Client { get; set; }

        public MainViewModel(BoilerClientViewModel client)
        {
            Client = client;
        }
    }
}
