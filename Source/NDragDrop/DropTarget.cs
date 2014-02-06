// Copyright (c) 2013 Ronald Valkenburg
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
        public static readonly DependencyProperty OnDropProperty = DependencyProperty.RegisterAttached("OnDrop", typeof(ICommand), typeof(DropTarget), new FrameworkPropertyMetadata(null, OnDropChanged));

        public static void SetOnDrop(UIElement element, ICommand command)
        {
            element.SetValue(OnDropProperty, command);
        }

        public static ICommand GetOnDrop(UIElement element)
        {
            return (ICommand)element.GetValue(OnDropProperty);
        }


        public static readonly DependencyProperty OnParameterProperty = DependencyProperty.RegisterAttached("Parameter", typeof(object), typeof(DropTarget), new FrameworkPropertyMetadata(null));

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
            if (uiElement == null)
                return;

            uiElement.AllowDrop = true;
            uiElement.DragOver += UiElementDragOver;
            uiElement.Drop += UiElementOnDrop;
        }

        private static void UiElementOnDrop(object sender, DragEventArgs dragEventArgs)
        {
            var uiElement = sender as UIElement;
            if (uiElement == null) return;
            var command = GetOnDrop(uiElement);
            if (command != null)
                command.Execute(new DropEventArgs
                {
                    Context = dragEventArgs.Data.GetData("NDragDropFormat"),
                    Parameter = GetParameter(uiElement)
                });
        }

        private static void UiElementDragOver(object sender, DragEventArgs e)
        {
            var uiElement = sender as UIElement;
            if (uiElement == null) 
                return;

            var command = GetOnDrop(uiElement);
            if (command != null && !command.CanExecute(null))
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }
    }
}
