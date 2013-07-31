using System.Collections.Generic;
using System.Windows;

namespace NDragDrop
{
    public class DragSource : DependencyObject
    {
        // TODO: Add opacitiy attached property
        private static readonly Dictionary<UIElement, Draggable> Draggables = new Dictionary<UIElement, Draggable>();

        public static readonly DependencyProperty ContextProperty =
            DependencyProperty.RegisterAttached("Context", typeof(object), typeof(DragSource), new FrameworkPropertyMetadata(null, ContextChanged));

        public static void SetContext(UIElement element, object value)
        {
            element.SetValue(ContextProperty, value);
        }

        public static object GetContext(UIElement element)
        {
            return element.GetValue(ContextProperty);
        }

        private static void ContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = d as UIElement;
            if (uiElement == null) return;
            Draggables.Add(uiElement, new Draggable(uiElement, e.NewValue));
        }
    }
}
