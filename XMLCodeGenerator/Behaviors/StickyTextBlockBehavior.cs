using System.Windows;

namespace XMLCodeGenerator.Behaviors
{
    public static class StickyTextBlockBehavior
    {
        public static readonly DependencyProperty IsTopVisibleProperty =
            DependencyProperty.RegisterAttached("IsTopVisible", typeof(bool), typeof(StickyTextBlockBehavior), new PropertyMetadata(false));

        public static bool GetIsTopVisible(UIElement element)
        {
            return (bool)element.GetValue(IsTopVisibleProperty);
        }

        public static void SetIsTopVisible(UIElement element, bool value)
        {
            element.SetValue(IsTopVisibleProperty, value);
        }
    }
}
