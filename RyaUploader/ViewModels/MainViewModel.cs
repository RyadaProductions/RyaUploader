using RyaUploaderV2.Models;
using Stylet;

namespace RyaUploaderV2.ViewModels
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
