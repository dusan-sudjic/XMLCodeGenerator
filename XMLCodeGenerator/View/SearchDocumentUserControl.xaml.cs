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
            int selectedTabIndex = MainWindow.SelectedTabIndex;
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
            ItemsControl itemsControl = MainWindow.ItemsControlClasses;
            switch (MainWindow.SelectedTabIndex)
            {
                case 1: { itemsControl = MainWindow.ItemsControlFunctions; break; }
                case 2: { itemsControl = MainWindow.ItemsControlPreprocess; break; }
                case 3: { itemsControl = MainWindow.ItemsControlRewriting; break; }
            }
            scrollToElement(SearchDocumentViewModel.SelectedSearchResult, itemsControl);
        }
        private void scrollToElement(ElementViewModel element, ItemsControl parent)
        {
            for (int i = 0; i < parent.Items.Count; i++)
            {
                var container = parent.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter;
                if (container != null)
                {
                    var elementUserControl = FindVisualChild<ElementUserControl>(container);
                    if (elementUserControl != null)
                    {
                        if (elementUserControl.Element == element)
                        {
                            scrollToElementUserControl(elementUserControl);
                            return;
                        }
                        scrollToElement(element, elementUserControl.itemsControlChildren);
                    }
                }
            }
        }
        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T childOfType)
                {
                    return childOfType;
                }

                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }
        private void scrollToElementUserControl(FrameworkElement targetElement)
        {
            ScrollViewer scrollViewer = null;
            switch (MainWindow.SelectedTabIndex)
            {
                case 0: { scrollViewer = MainWindow.CimClassesScrollViewer; break; }
                case 1: { scrollViewer = MainWindow.CimFunctionsScrollViewer; break; }
                case 2: { scrollViewer = MainWindow.PreprocessScrollViewer; break; }
                case 3: { scrollViewer = MainWindow.RewritingScrollViewer; break; }
            }
            GeneralTransform transform = targetElement.TransformToAncestor(scrollViewer);
            Point targetPosition = transform.Transform(new Point(0, 0));
            double spaceAbove = scrollViewer.ActualHeight / 5;
            double targetOffset = targetPosition.Y + scrollViewer.VerticalOffset - spaceAbove;
            DoubleAnimation verticalAnimation = new DoubleAnimation
            {
                From = scrollViewer.VerticalOffset,
                To = targetOffset,
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                EasingFunction = new QuadraticEase()
            };

            Storyboard.SetTarget(verticalAnimation, scrollViewer);
            Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath(ScrollViewerBehavior.VerticalOffsetProperty));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(verticalAnimation);
            storyboard.Begin();
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
