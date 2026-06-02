using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BookEditor.Controls
{
    public partial class TagControl : UserControl
    {
        public static readonly DependencyProperty TagsProperty = DependencyProperty.Register(
            "Tags", typeof(ObservableCollection<string>), typeof(TagControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly", typeof(bool), typeof(TagControl), new PropertyMetadata(true));

        public static readonly DependencyProperty RemoveTagCommandProperty = DependencyProperty.Register(
            "RemoveTagCommand", typeof(ICommand), typeof(TagControl), new PropertyMetadata(null));

        public static readonly DependencyProperty ChipBackgroundProperty = DependencyProperty.Register(
            "ChipBackground", typeof(Brush), typeof(TagControl), new PropertyMetadata(Brushes.Gray));

        public ObservableCollection<string> Tags
        {
            get => (ObservableCollection<string>)GetValue(TagsProperty);
            set => SetValue(TagsProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public ICommand RemoveTagCommand
        {
            get => (ICommand)GetValue(RemoveTagCommandProperty);
            set => SetValue(RemoveTagCommandProperty, value);
        }

        public Brush ChipBackground
        {
            get => (Brush)GetValue(ChipBackgroundProperty);
            set => SetValue(ChipBackgroundProperty, value);
        }

        public TagControl()
        {
            InitializeComponent();
        }
    }
}
