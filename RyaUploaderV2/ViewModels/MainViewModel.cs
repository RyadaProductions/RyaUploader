using RyaUploaderV2.Models;
using Stylet;

namespace RyaUploaderV2.ViewModels
{
    public class MainViewModel : Screen
    {
        public string BoilerState => _client.CurrentState;

        private readonly BoilerClient _client;

        public MainViewModel(BoilerClient client)
        {
            _client = client;
        }
    }
}
