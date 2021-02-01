using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RyaUploaderV2.Models;
using Stylet;

namespace RyaUploaderV2.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        public string BoilerState { get; set; }

        private BoilerClient _client;
        private ShellViewModel _shellViewModel;

        public MainViewModel(BoilerClient client)
        {
            _client = client;
            BoilerState = _client.CurrentState;
        }
    }
}
