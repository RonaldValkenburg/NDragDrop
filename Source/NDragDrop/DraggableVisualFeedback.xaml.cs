// Copyright (c) 2013 Ronald Valkenburg
// This software is licensed under the MIT License (see LICENSE file for details)
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Point = System.Drawing.Point;

namespace NDragDrop
{
    public partial class DraggableVisualFeedback
    {
        private const int GwlExstyle = -20;
        private const int WsExTransparent = 0x00000020;

        private readonly ISystemMouse _systemMouse;
        private readonly System.Windows.Point _grabPosition;

        public DraggableVisualFeedback()
        {
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            _systemMouse = new SystemMouse();
            _systemMouse.PositionChanged += MousePositionChanged;
        }

        public DraggableVisualFeedback(UIElement uiElement)
            : this()
        {
            _grabPosition = Mouse.GetPosition(uiElement);

            var bounds = VisualTreeHelper.GetDescendantBounds(uiElement);
            var renderTargetBitmap = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);
            
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                var visualBrush = new VisualBrush(uiElement);
                drawingContext.DrawRectangle(visualBrush, null, new Rect(new System.Windows.Point(), bounds.Size));
            }

            renderTargetBitmap.Render(drawingVisual);

            MaxWidth = renderTargetBitmap.PixelWidth;
            MaxHeight = renderTargetBitmap.PixelHeight;
            TheImage.Source = renderTargetBitmap;
        }

        [DllImport("Kernel32")]
        private static extern void SetLastError(uint dwErrCode);

        [DllImport("User32", SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("User32", SetLastError = true, EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongLegacy(IntPtr hWnd, int nIndex);

        [DllImport("User32", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("User32", SetLastError = true, EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLongLegacy(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        private static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
        {
            return IntPtr.Size == 4 ? GetWindowLongLegacy(hWnd, nIndex) : GetWindowLongPtr(hWnd, nIndex);
        }

        private static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            return IntPtr.Size == 4 ? SetWindowLongLegacy(hWnd, nIndex, dwNewLong) : SetWindowLongPtr(hWnd, nIndex, dwNewLong);
        }

        private void OnSourceInitialized(object sender, EventArgs eventArgs)
        {
            var hWnd = new WindowInteropHelper(this).Handle;

            var windowLongPtr = GetWindowLong(hWnd, GwlExstyle);
            if (windowLongPtr == IntPtr.Zero) return;

            windowLongPtr = (IntPtr.Size == 4)
                                ? (IntPtr) (windowLongPtr.ToInt32() | WsExTransparent)
                                : (IntPtr) (windowLongPtr.ToInt64() | WsExTransparent);

            SetLastError(0);

            SetWindowLong(hWnd, GwlExstyle, windowLongPtr);
        }

        private void MousePositionChanged(object sender, MousePositionChangedEventArgs eventargs)
        {
            Dispatcher.BeginInvoke(new Action<Point>(UpdateLocation), eventargs.Position);
        }

        private void UpdateLocation(Point location)
        {
            Left = location.X -_grabPosition.X;
            Top = location.Y - _grabPosition.Y;
        }
    }
}
