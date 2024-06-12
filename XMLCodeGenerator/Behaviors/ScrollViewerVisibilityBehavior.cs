using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace XMLCodeGenerator.Behaviors
{
    public class ScrollViewerVisibilityBehavior : Behavior<FrameworkElement>
    {
        private ScrollViewer scrollViewer;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= OnLoaded;
            scrollViewer = FindParent<ScrollViewer>(AssociatedObject);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged += OnScrollChanged;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged -= OnScrollChanged;
            }
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            CheckVisibility();
        }

        private void CheckVisibility()
        {
            if (scrollViewer == null || AssociatedObject == null)
                return;

            var transform = AssociatedObject.TransformToAncestor(scrollViewer);
            var elementBounds = transform.TransformBounds(new Rect(AssociatedObject.RenderSize));
            var viewportBounds = new Rect(new Point(0, 0), scrollViewer.RenderSize);

            IsWithinViewport = viewportBounds.IntersectsWith(elementBounds);
        }

        public static readonly DependencyProperty IsWithinViewportProperty =
            DependencyProperty.Register("IsWithinViewport", typeof(bool), typeof(ScrollViewerVisibilityBehavior), new PropertyMetadata(false));

        public bool IsWithinViewport
        {
            get { return (bool)GetValue(IsWithinViewportProperty); }
            set { 
                SetValue(IsWithinViewportProperty, value); 
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
                return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }
}
