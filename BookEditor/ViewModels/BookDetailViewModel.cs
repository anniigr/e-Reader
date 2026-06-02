using BookEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookEditor.ViewModels
{
    public class BookDetailViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        public Book Book { get; }

        public bool HasDescription => !string.IsNullOrWhiteSpace(Book.Description);
        public bool HasTags => Book.Tags.Any();
        public bool HasProgress => Book.TotalPages > 0;

        public bool CanRead => !string.IsNullOrWhiteSpace(Book.FilePath);
        public bool CanContinueReading => CanRead && Book.CurrentPage > 1;

        public ICommand GoBackCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand ReadFromBeginningCommand { get; }
        public ICommand ContinueReadingCommand { get; }

        public BookDetailViewModel(MainViewModel mainViewModel, Book book)
        {
            _mainViewModel = mainViewModel;
            Book = book;

            // Подписка на изменения книги, чтобы обновлять UI (например, после чтения или редактирования)
            Book.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(HasDescription));
                OnPropertyChanged(nameof(HasProgress));
                OnPropertyChanged(nameof(CanRead));
                OnPropertyChanged(nameof(CanContinueReading));
            };
            Book.Tags.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasTags));

            GoBackCommand = new RelayCommand(() => _mainViewModel.CurrentPage = new BookListViewModel(_mainViewModel));

            EditCommand = new RelayCommand(() =>
            {
                var editVm = new BookEditViewModel(Book, b => { _mainViewModel.Repository.Save(); });
                var window = new Views.BookEditWindow(editVm);
                window.ShowDialog();
            });

            ReadFromBeginningCommand = new RelayCommand(() => StartReading(1));
            ContinueReadingCommand = new RelayCommand(() => StartReading(Book.CurrentPage));
        }

        private void StartReading(int startPage)
        {
            if (Book.Status == BookStatus.Unread) Book.Status = BookStatus.Reading;
            _mainViewModel.Repository.Save();
            _mainViewModel.CurrentPage = new ReaderViewModel(_mainViewModel, Book, startPage);
        }
    }
}

