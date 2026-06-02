using BookEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookEditor.Views
{
    public partial class BookEditWindow : Window
    {
        public BookEditWindow(BookEditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.CloseRequested += () =>
            {
                DialogResult = true;
            };
        }
    }
}
