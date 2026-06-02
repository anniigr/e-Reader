using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookEditor.ViewModels
{
    public class WelcomeViewModel : BaseViewModel
    {
        public ICommand CreateRepoCommand { get; }
        public ICommand OpenRepoCommand { get; }

        public WelcomeViewModel(MainViewModel mainViewModel)
        {
            CreateRepoCommand = mainViewModel.CreateRepoCommand;
            OpenRepoCommand = mainViewModel.OpenRepoCommand;
        }
    }
}
