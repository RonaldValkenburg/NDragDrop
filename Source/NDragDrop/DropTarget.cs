// Copyright (c) 2014 Ronald Valkenburg
// This software is licensed under the MIT License (see LICENSE file for details)
using System;
using System.Windows;
using System.Windows.Input;

namespace NDragDrop
{
    public class DropEventArgs : EventArgs
    {
        public object Context { get; set; }
        public object Parameter { get; set; }
    }

    public class DropTarget : DependencyObject
    {
        public static readonly DependencyProperty OnDropProperty =
            DependencyProperty.RegisterAttached("OnDrop", typeof(ICommand), typeof(DropTarget), new FrameworkPropertyMetadata(null, OnDropChanged));

        public static void SetOnDrop(UIElement element, ICommand command)
        {
            element.SetValue(OnDropProperty, command);
        }

        public static ICommand GetOnDrop(UIElement element)
        {
            return (ICommand)element.GetValue(OnDropProperty);
        }

        public static readonly DependencyProperty DropDataTypeProperty =
            DependencyProperty.RegisterAttached("DropDataType", typeof(string), typeof(DropTarget), new FrameworkPropertyMetadata("NDragDropFormat"));

        public static string GetDropDataType(UIElement element)
        {
            return (string)element.GetValue(DropDataTypeProperty);
        }

        public static void SetDropDataType(UIElement element, string type)
        {
            element.SetValue(DropDataTypeProperty, type);
        }

        public static readonly DependencyProperty OnParameterProperty =
            DependencyProperty.RegisterAttached("Parameter", typeof(object), typeof(DropTarget), new FrameworkPropertyMetadata(null));

        public static void SetParameter(UIElement element, object parameter)
        {
            element.SetValue(OnParameterProperty, parameter);
        }

        public static object GetParameter(UIElement element)
        {
            return element.GetValue(OnParameterProperty);
        }

        private static void OnDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = d as UIElement;
            if (uiElement == null) return;

            uiElement.AllowDrop = true;
            uiElement.DragOver += UiElementDragOver;
            uiElement.Drop += UiElementOnDrop;
        }

        private static void UiElementOnDrop(object sender, DragEventArgs e)
        {
            var uiElement = sender as UIElement;
            if (uiElement == null) return;

            var command = GetOnDrop(uiElement);
            if (command == null) return;

            var dropEventArgs = CreateDropEventArgs(uiElement, e);
            command.Execute(dropEventArgs);

            e.Handled = true;
        }

        private static void UiElementDragOver(object sender, DragEventArgs e)
        {
            var uiElement = sender as UIElement;
            if (uiElement == null) return;

            var command = GetOnDrop(uiElement);
            if (command != null)
            {
                var dropEventArgs = CreateDropEventArgs(uiElement, e);

                if (!command.CanExecute(dropEventArgs))
                {
                    e.Effects = DragDropEffects.None;
                }
            }

            e.Handled = true;
        }

        private static DropEventArgs CreateDropEventArgs(UIElement sender, DragEventArgs e)
        {
            return new DropEventArgs
            {
                Context = e.Data.GetData(GetDropDataType(sender)),
                Parameter = GetParameter(sender)
            };
        }
    }
}
