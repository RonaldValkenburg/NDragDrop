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

        public Draggable(UIElement uiElement, object context)
        {
            _uiElement = uiElement;
            _context = context;
            uiElement.MouseDown += uiElement_MouseDown;
            uiElement.PreviewMouseMove += uiElement_MouseMove;
        }

        private void uiElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void uiElement_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(null);
            var diff = _startPoint - mousePosition;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                DragDrop.DoDragDrop(_uiElement, new DataObject("NDragDropFormat", _context), DragDropEffects.Move);
            }
        }
    }
}