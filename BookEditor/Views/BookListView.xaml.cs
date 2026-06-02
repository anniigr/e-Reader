using BookEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookEditor.Views
{
    public partial class BookListView : UserControl
    {
        public BookListView()
        {
            InitializeComponent();
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is BookListViewModel vm && vm.ViewDetailsCommand.CanExecute(null))
            {
                vm.ViewDetailsCommand.Execute(null);
            }
        }
    }
}
