// Copyright (c) 2013 Ronald Valkenburg
// This software is licensed under the MIT License (see LICENSE file for details)
using System.Windows;

namespace NDragDrop
{
    public class DragSource : DependencyObject
    {
        public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached("Context", typeof(object), typeof(DragSource), new FrameworkPropertyMetadata(null, ContextChanged));

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
// ReSharper disable ObjectCreationAsStatement
            new Draggable(uiElement, e.NewValue);
// ReSharper restore ObjectCreationAsStatement
        }
    }
}
