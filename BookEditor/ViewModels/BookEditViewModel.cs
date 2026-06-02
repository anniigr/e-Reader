using BookEditor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;

namespace BookEditor.ViewModels
{
    public class BookEditViewModel : BaseViewModel
    {
        private readonly Book? _originalBook;
        private readonly Action<Book> _onSaveCallback;

        private string _title = string.Empty;
        public string Title { get => _title; set { SetProperty(ref _title, value); ((RelayCommand)SaveCommand)?.RaiseCanExecuteChanged(); } }

        private string _author = string.Empty;
        public string Author { get => _author; set => SetProperty(ref _author, value); }

        private byte[]? _coverImage;
        public byte[]? CoverImage { get => _coverImage; set => SetProperty(ref _coverImage, value); }

        private string _description = string.Empty;
        public string Description { get => _description; set => SetProperty(ref _description, value); }

        private string _newTag = string.Empty;
        public string NewTag { get => _newTag; set => SetProperty(ref _newTag, value); }

        public ObservableCollection<string> Tags { get; } = new();

        private string _filePath = string.Empty;
        public string FilePath { get => _filePath; set => SetProperty(ref _filePath, value); }

        public ICommand BrowseCoverCommand { get; }
        public ICommand BrowseBookCommand { get; }
        public ICommand AddTagCommand { get; }
        public ICommand RemoveTagCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action? CloseRequested;

        // Конструктор
        public BookEditViewModel(Book? book, Action<Book> onSaveCallback)
        {
            _originalBook = book;
            _onSaveCallback = onSaveCallback;

            if (book != null)
            {
                Title = book.Title;
                Author = book.Author;
                CoverImage = book.CoverImage;
                Description = book.Description;
                FilePath = book.FilePath;
                foreach (var tag in book.Tags) Tags.Add(tag);
            }

            BrowseCoverCommand = new RelayCommand(BrowseCover);
            BrowseBookCommand = new RelayCommand(BrowseBook);
            AddTagCommand = new RelayCommand(AddTag);
            RemoveTagCommand = new RelayCommand(RemoveTag);

            SaveCommand = new RelayCommand(Save, () => !string.IsNullOrWhiteSpace(Title));
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke());
        }

        private void BrowseCover()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog { Filter = "Image Files|*.png;*.jpg;*.jpeg" };
            if (dlg.ShowDialog() == true)
            {
                CoverImage = File.ReadAllBytes(dlg.FileName);
            }
        }

        private void BrowseBook()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog { Filter = "Markdown Files|*.md|All Files|*.*" };
            if (dlg.ShowDialog() == true)
            {
                FilePath = dlg.FileName;
            }
        }

        private void AddTag()
        {
            if (!string.IsNullOrWhiteSpace(NewTag) && !Tags.Contains(NewTag.Trim()))
            {
                Tags.Add(NewTag.Trim());
                NewTag = string.Empty;
            }
        }

        private void RemoveTag(object? tag)
        {
            if (tag is string t) Tags.Remove(t);
        }

        private void Save()
        {
            var bookToSave = _originalBook ?? new Book();
            bookToSave.Title = Title;
            bookToSave.Author = Author;
            bookToSave.CoverImage = CoverImage;
            bookToSave.Description = Description;
            bookToSave.FilePath = FilePath;

            bookToSave.Tags.Clear();
            foreach (var tag in Tags) bookToSave.Tags.Add(tag);

            _onSaveCallback(bookToSave);
            CloseRequested?.Invoke();
        }
    }
}
