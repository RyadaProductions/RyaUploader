using RyaUploader.Models;
using Stylet;

namespace RyaUploader.ViewModels
{
    public class MainViewModel : Screen
    {
        public BoilerClient Client { get; set; }

        public MainViewModel(BoilerClient client)
        {
            Client = client;
        }
    }
}
