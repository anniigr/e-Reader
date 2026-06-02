using BookEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookEditor.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public AppRepository Repository { get; set; } = new();

        private BaseViewModel _currentPage;
        public BaseViewModel CurrentPage { get => _currentPage; set => SetProperty(ref _currentPage, value); }

        private string _statusMessage = "No Repository opened";
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

        public ICommand CreateRepoCommand { get; }
        public ICommand OpenRepoCommand { get; }
        public StatisticsViewModel Statistics { get; }

        public MainViewModel()
        {

            CreateRepoCommand = new RelayCommand(() =>
            {
                var dlg = new Microsoft.Win32.SaveFileDialog { Filter = "BookEditor Repo|*.librepo" };
                if (dlg.ShowDialog() == true)
                {
                    Repository.Create(dlg.FileName);
                    CurrentPage = new BookListViewModel(this);
                    StatusMessage = "Repository created.";
                }
            });

            OpenRepoCommand = new RelayCommand(() =>
            {
                var dlg = new Microsoft.Win32.OpenFileDialog { Filter = "BookEditor Repo|*.librepo" };
                if (dlg.ShowDialog() == true)
                {
                    Repository.Load(dlg.FileName);
                    CurrentPage = new BookListViewModel(this);
                    StatusMessage = "Repository loaded.";
                }
            });

            Statistics = new StatisticsViewModel(this);

            CurrentPage = new WelcomeViewModel(this);
        }
    }
}
