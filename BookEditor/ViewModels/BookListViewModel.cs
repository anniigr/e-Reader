using BookEditor.Models;
using BookEditor.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookEditor.ViewModels
{
    public class BookListViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;

        public ObservableCollection<Book> FilteredBooks { get; } = new();

        private Book? _selectedBook;
        public Book? SelectedBook
        {
            get => _selectedBook;
            set
            {
                if (SetProperty(ref _selectedBook, value))
                {
                    ((RelayCommand)EditBookCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)DeleteBookCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)ViewDetailsCommand).RaiseCanExecuteChanged();
                }
            }
        }

        // Filters
        private string _searchText = string.Empty;
        public string SearchText { get => _searchText; set { SetProperty(ref _searchText, value); ApplyFilters(); } }

        private string _filterStatus = "Wszystkie";
        public string FilterStatus { get => _filterStatus; set { SetProperty(ref _filterStatus, value); ApplyFilters(); } }
        public ObservableCollection<string> StatusOptions { get; } = new() { "Wszystkie", "Nieprzeczytane", "W trakcie", "Przeczytane" };

        private int _filterRating = 0;
        public int FilterRating { get => _filterRating; set { SetProperty(ref _filterRating, value); ApplyFilters(); } }
        public ObservableCollection<int> RatingOptions { get; } = new() { 0, 1, 2, 3, 4, 5 };

        private string _sortBy = "Tytuł";
        public string SortBy { get => _sortBy; set { SetProperty(ref _sortBy, value); ApplyFilters(); } }
        public ObservableCollection<string> SortOptions { get; } = new() { "Tytuł", "Autor", "Ocena" };

        public ICommand ClearFiltersCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand DeleteBookCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand AddBookCommand { get; } // Дублируем для удобства кнопки на панели

        public BookListViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _mainViewModel.Repository.OnRepositoryChanged += ApplyFilters;

            ClearFiltersCommand = new RelayCommand(ClearFilters,
                () => !string.IsNullOrEmpty(SearchText) || FilterStatus != "Wszystkie" || FilterRating != 0 || SortBy != "Tytuł");

            AddBookCommand = new RelayCommand(AddBook);
            EditBookCommand = new RelayCommand(EditBook, () => SelectedBook != null);
            DeleteBookCommand = new RelayCommand(DeleteBook, () => SelectedBook != null);
            ViewDetailsCommand = new RelayCommand(ViewDetails, () => SelectedBook != null);

            ApplyFilters();
        }

        private void ClearFilters()
        {
            SearchText = string.Empty;
            FilterStatus = "Wszystkie";
            FilterRating = 0;
            SortBy = "Tytuł";
        }

        private void ApplyFilters()
        {
            var result = _mainViewModel.Repository.Books.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
                result = result.Where(b => b.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                           b.Author.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            if (FilterStatus == "Nieprzeczytane") result = result.Where(b => b.Status == BookStatus.Unread);
            else if (FilterStatus == "W trakcie") result = result.Where(b => b.Status == BookStatus.Reading);
            else if (FilterStatus == "Przeczytane") result = result.Where(b => b.Status == BookStatus.Read);

            if (FilterRating > 0) result = result.Where(b => b.Rating >= FilterRating);

            result = SortBy switch
            {
                "Autor" => result.OrderBy(b => b.Author),
                "Ocena" => result.OrderByDescending(b => b.Rating),
                _ => result.OrderBy(b => b.Title)
            };

            FilteredBooks.Clear();
            foreach (var book in result) FilteredBooks.Add(book);

            ((RelayCommand)ClearFiltersCommand).RaiseCanExecuteChanged();
        }

        private void AddBook()
        {
            var editVm = new BookEditViewModel(null, book =>
            {
                _mainViewModel.Repository.AddBook(book);
                _mainViewModel.StatusMessage = "Book added.";
            });
            var window = new BookEditWindow(editVm);
            window.ShowDialog();
        }

        private void EditBook()
        {
            if (SelectedBook == null) return;
            var editVm = new BookEditViewModel(SelectedBook, book =>
            {
                _mainViewModel.Repository.Save();
                _mainViewModel.StatusMessage = "Book edited.";
                ApplyFilters();
            });
            var window = new BookEditWindow(editVm);
            window.ShowDialog();
        }

        private void DeleteBook()
        {
            if (SelectedBook != null)
            {
                _mainViewModel.Repository.RemoveBook(SelectedBook);
                _mainViewModel.StatusMessage = "Book removed.";
                SelectedBook = null;
            }
        }

        private void ViewDetails()
        {
            if (SelectedBook != null)
            {
                // Переход на детальный вид. (BookDetailViewModel будет реализован в следующей части)
                _mainViewModel.CurrentPage = new BookDetailViewModel(_mainViewModel, SelectedBook);
            }
        }
    }
}
