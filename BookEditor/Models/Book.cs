using BookEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookEditor.Models
{
    public enum BookStatus { Unread, Reading, Read }

    public class Book : BaseViewModel
    {
        private string _title = string.Empty;
        private string _author = string.Empty;
        private byte[]? _coverImage;
        private string _description = string.Empty;
        private ObservableCollection<string> _tags = new();
        private int _totalPages;
        private int _currentPage;
        private string _filePath = string.Empty;
        private int _rating;
        private BookStatus _status = BookStatus.Unread;
        private string _genre = string.Empty;

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string Author { get => _author; set => SetProperty(ref _author, value); }
        public byte[]? CoverImage { get => _coverImage; set => SetProperty(ref _coverImage, value); }
        public string Description { get => _description; set => SetProperty(ref _description, value); }
        public ObservableCollection<string> Tags { get => _tags; set => SetProperty(ref _tags, value); }
        public int TotalPages { get => _totalPages; set => SetProperty(ref _totalPages, value); }
        public int CurrentPage { get => _currentPage; set => SetProperty(ref _currentPage, value); }
        public string FilePath { get => _filePath; set => SetProperty(ref _filePath, value); }
        public int Rating { get => _rating; set => SetProperty(ref _rating, value); }
        public BookStatus Status { get => _status; set => SetProperty(ref _status, value); }
        public string Genre { get => _genre; set => SetProperty(ref _genre, value); }
    }
}
