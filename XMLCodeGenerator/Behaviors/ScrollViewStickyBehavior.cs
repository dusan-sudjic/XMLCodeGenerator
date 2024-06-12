using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XMLCodeGenerator.Behaviors
{
    public class ScrollViewStickyBehavior : Behavior<ScrollViewer>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ScrollChanged += OnScrollChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.ScrollChanged -= OnScrollChanged;
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            FindAndAdjustTextBlockPositions(AssociatedObject);
        }

        private void FindAndAdjustTextBlockPositions(ScrollViewer scrollView)
        {
            if (scrollView == null || scrollView.Content == null)
                return;

            double verticalOffset = scrollView.VerticalOffset;

            // Get the top position of the scroll viewer's viewport
            double viewportTop = verticalOffset;

            FrameworkElement content = scrollView.Content as FrameworkElement;
            if (content != null)
            {
                // Find and adjust the position of text blocks within the content
                AdjustTextBlockPositionsInVisualTree(content, scrollView, viewportTop);
            }
        }

        private void AdjustTextBlockPositionsInVisualTree(FrameworkElement element, ScrollViewer scrollView, double viewportTop)
        {
            if (element == null || scrollView == null)
                return;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child is TextBlock textBlock && (bool)textBlock.GetValue(IsTopVisibleProperty))
                {
                    // Calculate the distance between the text block and the top of the scroll viewer's viewport
                    double distanceFromViewportTop = CalculateDistanceFromViewportTop(textBlock, scrollView);

                    // Adjust the margin of the text block to keep it aligned with the top of the viewport
                    double newMarginTop = viewportTop + distanceFromViewportTop;
                    textBlock.Margin = new Thickness(textBlock.Margin.Left, newMarginTop, textBlock.Margin.Right, textBlock.Margin.Bottom);
                }
                else if (child is FrameworkElement frameworkElement)
                {
                    // Recursively search for text blocks in the visual tree
                    AdjustTextBlockPositionsInVisualTree(frameworkElement, scrollView, viewportTop);
                }
            }
        }

        private double CalculateDistanceFromViewportTop(TextBlock textBlock, ScrollViewer scrollView)
        {
            // Calculate the distance between the text block's top border and the top border of the scroll viewer's viewport
            GeneralTransform transform = textBlock.TransformToAncestor(scrollView);
            Point textBlockTopLeft = transform.Transform(new Point(0, 0));
            double distanceFromViewportTop = textBlockTopLeft.Y;
            return distanceFromViewportTop;
        }

        public static readonly DependencyProperty IsTopVisibleProperty =
            DependencyProperty.RegisterAttached("IsTopVisible", typeof(bool), typeof(ScrollViewStickyBehavior), new PropertyMetadata(false));

        public static bool GetIsTopVisible(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsTopVisibleProperty);
        }

        public static void SetIsTopVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsTopVisibleProperty, value);
        }
    }
}
