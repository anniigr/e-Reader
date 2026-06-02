using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookEditor.Models;

namespace BookEditor.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;

        public int TotalBooks => _mainViewModel.Repository.Books.Count;

        public int ReadOrReadingBooks => _mainViewModel.Repository.Books
            .Count(b => b.Status == BookStatus.Read || b.Status == BookStatus.Reading);

        public string AverageRating
        {
            get
            {
                var ratedBooks = _mainViewModel.Repository.Books.Where(b => b.Rating > 0).ToList();
                return ratedBooks.Any() ? $"{ratedBooks.Average(b => b.Rating):F1} ★" : "—";
            }
        }

        public string PopularGenre
        {
            get
            {
                var tags = _mainViewModel.Repository.Books.SelectMany(b => b.Tags).ToList();
                if (!tags.Any()) return "—";
                return tags.GroupBy(t => t).OrderByDescending(g => g.Count()).First().Key;
            }
        }

        public string StatusMessage => _mainViewModel.StatusMessage;

        public StatisticsViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _mainViewModel.Repository.OnRepositoryChanged += UpdateStatistics;
            _mainViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.StatusMessage))
                    OnPropertyChanged(nameof(StatusMessage));
            };
        }

        private void UpdateStatistics()
        {
            OnPropertyChanged(nameof(TotalBooks));
            OnPropertyChanged(nameof(ReadOrReadingBooks));
            OnPropertyChanged(nameof(AverageRating));
            OnPropertyChanged(nameof(PopularGenre));
        }
    }
}
