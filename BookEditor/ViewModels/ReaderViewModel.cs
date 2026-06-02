using BookEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;

namespace BookEditor.ViewModels
{
    public class ReaderViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        public Book Book { get; }

        private List<string> _pages = new();

        private int _currentPageIndex;
        public int CurrentPageIndex
        {
            get => _currentPageIndex;
            set
            {
                if (SetProperty(ref _currentPageIndex, value))
                {
                    OnPropertyChanged(nameof(CurrentHtmlContent));
                    OnPropertyChanged(nameof(ProgressText));
                    ((RelayCommand)NextPageCommand)?.RaiseCanExecuteChanged();
                    ((RelayCommand)PrevPageCommand)?.RaiseCanExecuteChanged();

                    // Автосохранение позиции в репозиторий
                    Book.CurrentPage = value;
                    if (Book.CurrentPage == Book.TotalPages) Book.Status = BookStatus.Read;
                    _mainViewModel.Repository.Save();
                }
            }
        }

        public string CurrentHtmlContent => _pages.Count > 0 && _currentPageIndex > 0 ? ConvertMarkdownToHtml(_pages[_currentPageIndex - 1]) : "";
        public string ProgressText => $"Rozdział {CurrentPageIndex} z {Book.TotalPages}";

        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand CloseCommand { get; }

        public ReaderViewModel(MainViewModel mainViewModel, Book book, int startPage)
        {
            _mainViewModel = mainViewModel;
            Book = book;

            LoadMarkdownFile();

            CurrentPageIndex = Math.Clamp(startPage, 1, Book.TotalPages == 0 ? 1 : Book.TotalPages);

            NextPageCommand = new RelayCommand(() => CurrentPageIndex++, () => CurrentPageIndex < Book.TotalPages);
            PrevPageCommand = new RelayCommand(() => CurrentPageIndex--, () => CurrentPageIndex > 1);
            CloseCommand = new RelayCommand(() => _mainViewModel.CurrentPage = new BookDetailViewModel(_mainViewModel, Book));
        }

        private void LoadMarkdownFile()
        {
            if (File.Exists(Book.FilePath))
            {
                string text = File.ReadAllText(Book.FilePath);
                // Разбиваем текст на главы по Markdown-заголовкам 1-го и 2-го уровня
                var chunks = Regex.Split(text, @"(?=^#{1,2} )", RegexOptions.Multiline)
                                  .Where(s => !string.IsNullOrWhiteSpace(s))
                                  .ToList();

                if (!chunks.Any()) chunks.Add(text); // Если заголовков нет, все одним куском

                _pages = chunks;
                Book.TotalPages = _pages.Count;
                _mainViewModel.Repository.Save();
            }
            else
            {
                _pages.Add("# Błąd\nPlik nie istnieje lub został przeniesiony.");
                Book.TotalPages = 1;
            }
        }

        private string ConvertMarkdownToHtml(string md)
        {
            string html = "<html><body style='font-family:\"Segoe UI\", Arial, sans-serif; font-size:16px; line-height:1.6; color:#333; padding:20px;'>";

            md = Regex.Replace(md, @"\*\*(.+?)\*\*", "<b>$1</b>"); // Жирный
            md = Regex.Replace(md, @"\*(.+?)\*", "<i>$1</i>");     // Курсив
            md = Regex.Replace(md, @"^### (.*?)$", "<h3>$1</h3>", RegexOptions.Multiline); // H3
            md = Regex.Replace(md, @"^## (.*?)$", "<h2 style='border-bottom:1px solid #ccc;'>$1</h2>", RegexOptions.Multiline); // H2
            md = Regex.Replace(md, @"^# (.*?)$", "<h1 style='color:#2196F3;'>$1</h1>", RegexOptions.Multiline); // H1

            md = md.Replace("\r\n", "<br/>").Replace("\n", "<br/>"); // Переносы
            html += md + "</body></html>";
            return html;
        }
    }
}
