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

                    Book.CurrentPage = value;
                    if (Book.CurrentPage == Book.TotalPages) Book.Status = BookStatus.Read;
                    _mainViewModel.Repository.Save();
                }
            }
        }

        public string CurrentHtmlContent => _pages.Count > 0 && _currentPageIndex > 0 ? ConvertMarkdownToHtml(_pages[_currentPageIndex - 1]) : "";
        public string ProgressText => $"Chapter {CurrentPageIndex} from {Book.TotalPages}";

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
                var chunks = Regex.Split(text, @"(?=^#{1,2} )", RegexOptions.Multiline)
                                  .Where(s => !string.IsNullOrWhiteSpace(s))
                                  .ToList();

                if (!chunks.Any()) chunks.Add(text); 

                _pages = chunks;
                Book.TotalPages = _pages.Count;
                _mainViewModel.Repository.Save();
            }
            else
            {
                _pages.Add("# Error\nFile does not exist or has been moved.");
                Book.TotalPages = 1;
            }
        }

        private string ConvertMarkdownToHtml(string md)
        {
            string html = "<html><body style='font-family:\"Segoe UI\", Arial, sans-serif; font-size:16px; line-height:1.6; color:#333; padding:20px;'>";

            md = Regex.Replace(md, @"\*\*(.+?)\*\*", "<b>$1</b>"); 
            md = Regex.Replace(md, @"\*(.+?)\*", "<i>$1</i>");    
            md = Regex.Replace(md, @"^### (.*?)$", "<h3>$1</h3>", RegexOptions.Multiline); 
            md = Regex.Replace(md, @"^## (.*?)$", "<h2 style='border-bottom:1px solid #ccc;'>$1</h2>", RegexOptions.Multiline); 
            md = Regex.Replace(md, @"^# (.*?)$", "<h1 style='color:#2196F3;'>$1</h1>", RegexOptions.Multiline);
            md = Regex.Replace(md, @"`(.+?)`", "<code>$1</code>");
            md = md.Replace("\r\n", "<br/>").Replace("\n", "<br/>"); 
            html += md + "</body></html>";
            return html;
        }
    }
}
