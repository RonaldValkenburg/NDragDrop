// Copyright (c) 2013 Ronald Valkenburg
// This software is licensed under the MIT License (see LICENSE file for details)
using System;
using System.Windows;
using System.Windows.Input;
using Point = System.Windows.Point;

namespace NDragDrop
{
    public class Draggable
    {
        private readonly UIElement _uiElement;
        private readonly object _context;
        private static Point _startPoint;
        private DraggableVisualFeedback _visualFeedback;

        public Draggable(UIElement uiElement, object context)
        {
            _uiElement = uiElement;
            _context = context;
            uiElement.PreviewMouseDown += UiElementPreviewMouseDown;
            uiElement.PreviewMouseMove += UiElementPreviewMouseMove;
        }

        private void UiElementPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void UiElementPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(null);
            var diff = _startPoint - mousePosition;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                AddVisualFeedback();
                DragDrop.DoDragDrop(_uiElement, new DataObject("NDragDropFormat", _context), DragDropEffects.Move);
                RemoveVisualFeedback();
            }
        }

        private void AddVisualFeedback()
        {
            _visualFeedback = new DraggableVisualFeedback(_uiElement);
            _visualFeedback.Show();
        }

        private void RemoveVisualFeedback()
        {
            _visualFeedback.Close();
            _visualFeedback = null;
        }
    }
}