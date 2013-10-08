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
    }

    public class DropTarget : DependencyObject
    {
        public static readonly DependencyProperty CanDropProperty =
            DependencyProperty.RegisterAttached("CanDrop", typeof(bool), typeof(DropTarget), new FrameworkPropertyMetadata(true));

        public static void SetCanDrop(UIElement element, bool value)
        {
            element.SetValue(CanDropProperty, value);
        }

        public static bool GetCanDrop(UIElement element)
        {
            return (bool)element.GetValue(CanDropProperty);
        }

        public static readonly DependencyProperty OnDropProperty =
            DependencyProperty.RegisterAttached("OnDrop", typeof (ICommand), typeof (DropTarget), new FrameworkPropertyMetadata(null, OnDropChanged));


        public static readonly DependencyProperty DropDataTypeProperty =
            DependencyProperty.RegisterAttached("DropDataType", typeof (string), typeof (DropTarget),
                new FrameworkPropertyMetadata("NDragDropFormat"));
  

        
        
       public static void SetOnDrop(UIElement element, ICommand command)
        {
            element.SetValue(OnDropProperty, command);
        }

        public static ICommand GetOnDrop(UIElement element)
        {
            return (ICommand)element.GetValue(OnDropProperty);
        }

        public static string GetDropDataType(UIElement element)
        {
            return (string) element.GetValue(DropDataTypeProperty);
        }

        public static void SetDropDataType(UIElement element, string type)
        {
            element.SetValue(DropDataTypeProperty, type);
        }

        private static void OnDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = d as UIElement;
            if (uiElement == null) return;
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
            {
                command.Execute(new DropEventArgs { Context = dragEventArgs.Data.GetData(GetDropDataType(uiElement)) });
            }
        }

        private static void UiElementDragOver(object sender, DragEventArgs e)
        {
            var uiElement = sender as UIElement;
            if (uiElement == null) return;

            if (!GetCanDrop(uiElement))
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }
    }
}
