using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XMLCodeGenerator.Behaviors;
using XMLCodeGenerator.ViewModel;

namespace XMLCodeGenerator.View
{
    public partial class SearchDocumentUserControl : UserControl, INotifyPropertyChanged
    {
        public SearchDocumentViewModel SearchDocumentViewModel { get; set; }
        public DocumentViewModel Document { get => MainWindow.Document; }
        public SearchDocumentUserControl()
        {
            InitializeComponent();
            SearchDocumentViewModel = Document.SearchDocumentViewModel;
            DataContext = SearchDocumentViewModel;
        }
        private void SearchDocument_Click(object sender, RoutedEventArgs e)
        {
            int selectedTabIndex = Document.CurrentlyDisplayedTab;
            SearchDocumentViewModel.SearchDocument(selectedTabIndex);
        }
        private void UpArrow_Click(object sender, RoutedEventArgs e)
        {
            SearchDocumentViewModel.UpArrowClicked();
        }
        private void DownArrow_Click(object sender, RoutedEventArgs e)
        {
            SearchDocumentViewModel.DownArrowClicked();
        }

        private void SearchDocument_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchDocumentViewModel.SelectedSearchResult == null)
                return;
            MainWindow.ScrollToElement(SearchDocumentViewModel.SelectedSearchResult, false);
        }
        private void ResetSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchDocumentViewModel.ResetSearch();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
