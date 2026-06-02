using BookEditor.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookEditor.Controls
{
    public partial class RatingControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(int), typeof(RatingControl), new PropertyMetadata(0, OnValueChanged));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly", typeof(bool), typeof(RatingControl), new PropertyMetadata(false, OnReadOnlyChanged));

        public static readonly DependencyProperty StarSizeProperty = DependencyProperty.Register(
            "StarSize", typeof(double), typeof(RatingControl), new PropertyMetadata(20.0));

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public double StarSize
        {
            get => (double)GetValue(StarSizeProperty);
            set => SetValue(StarSizeProperty, value);
        }

        public ObservableCollection<StarItem> Stars { get; } = new();
        public ICommand RateCommand { get; }

        public Cursor CursorType => IsReadOnly ? Cursors.Arrow : Cursors.Hand;

        // Добавляем переменную для хранения значения под курсором
        private int _hoverValue = 0;

        public RatingControl()
        {
            InitializeComponent();
            // Poprawiona komenda: używamy Convert.ToInt32, aby uniknąć błędów rzutowania (p is int)
            RateCommand = new RelayCommand(p => {
                if (!IsReadOnly && p != null)
                {
                    Value = Convert.ToInt32(p);
                }
            });
            UpdateStars();
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RatingControl)d).UpdateStarsVisuals();

        private static void OnReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (RatingControl)d;
            control.PropertyChanged?.Invoke(control, new PropertyChangedEventArgs(nameof(CursorType)));
        }

        // Изменяем UpdateStars: теперь он только один раз создает коллекцию
        private void UpdateStars()
        {
            if (Stars.Count == 0)
            {
                for (int i = 1; i <= 5; i++)
                {
                    Stars.Add(new StarItem { Value = i });
                }
            }
            UpdateStarsVisuals();
        }

        // Добавляем метод для обновления ТОЛЬКО иконок (без пересоздания списка)
        private void UpdateStarsVisuals()
        {
            int effectiveValue = _hoverValue > 0 ? _hoverValue : Value;
            foreach (var star in Stars)
            {
                star.Icon = star.Value <= effectiveValue ? "★" : "☆";
            }
        }

        // Обработчик наведения мыши
        private void Star_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IsReadOnly) return;
            if (sender is TextBlock tb && tb.DataContext is StarItem star)
            {
                _hoverValue = star.Value;
                UpdateStarsVisuals();
            }
        }

        // Обработчик ухода мыши
        private void Star_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsReadOnly) return;
            _hoverValue = 0;
            UpdateStarsVisuals();
        }
    }

    // Делаем StarItem уведомляющим, чтобы интерфейс видел изменение свойства Icon
    public class StarItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public int Value { get; set; }

        private string _icon = "☆";
        public string Icon
        {
            get => _icon;
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Icon)));
                }
            }
        }
    }
}