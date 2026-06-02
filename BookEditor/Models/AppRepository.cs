using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace BookEditor.Models
{
    public class AppRepository
    {
        private string _currentFilePath = string.Empty;
        public ObservableCollection<Book> Books { get; private set; } = new();

        public event Action? OnRepositoryChanged;

        public void Create(string filePath)
        {
            _currentFilePath = filePath;
            Books = new ObservableCollection<Book>();
            AttachEvents();
            Save();
            OnRepositoryChanged?.Invoke();
        }

        public void Load(string filePath)
        {
            _currentFilePath = filePath;
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var loaded = JsonSerializer.Deserialize<ObservableCollection<Book>>(json);
                if (loaded != null) Books = loaded;
            }
            AttachEvents();
            OnRepositoryChanged?.Invoke();
        }

        public void AddBook(Book book)
        {
            book.PropertyChanged += (s, e) => Save();
            book.Tags.CollectionChanged += (s, e) => Save();
            Books.Add(book);
        }

        public void RemoveBook(Book book)
        {
            Books.Remove(book);
        }

        private void AttachEvents()
        {
            Books.CollectionChanged += (s, e) => Save();
            foreach (var book in Books)
            {
                book.PropertyChanged += (s, e) => Save();
                book.Tags.CollectionChanged += (s, e) => Save();
            }
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(_currentFilePath)) return;
            var json = JsonSerializer.Serialize(Books, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_currentFilePath, json);
            OnRepositoryChanged?.Invoke();
        }
    }
}
