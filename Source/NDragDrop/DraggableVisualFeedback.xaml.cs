// Copyright (c) 2014 Ronald Valkenburg
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
    public partial class DraggableVisualFeedback : Window
    {
        private const int GwlExstyle = -20;
        private const int WsExTransparent = 0x00000020;

        private readonly ISystemMouse _systemMouse;
        private readonly System.Windows.Point _grabPosition;
        private readonly Rect _bounds;
        private readonly DrawingVisual _drawingVisual;

        private Matrix _fromScreenTransform = Matrix.Identity;
        private Matrix _toScreenTransform = Matrix.Identity;

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
            _bounds = VisualTreeHelper.GetDescendantBounds(uiElement);
            _drawingVisual = new DrawingVisual();
            using (var drawingContext = _drawingVisual.RenderOpen())
            {
                var visualBrush = new VisualBrush(uiElement);
                drawingContext.DrawRectangle(visualBrush, null, new Rect(new System.Windows.Point(), _bounds.Size));
            }
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
            InitDeviceTransforms();
            RenderBitmap();

            var hWnd = new WindowInteropHelper(this).Handle;

            var windowLongPtr = GetWindowLong(hWnd, GwlExstyle);
            if (windowLongPtr == IntPtr.Zero) return;

            windowLongPtr = (IntPtr.Size == 4)
                                ? (IntPtr)(windowLongPtr.ToInt32() | WsExTransparent)
                                : (IntPtr)(windowLongPtr.ToInt64() | WsExTransparent);

            SetLastError(0);

            SetWindowLong(hWnd, GwlExstyle, windowLongPtr);
        }

        private void InitDeviceTransforms()
        {
            PresentationSource source = PresentationSource.FromVisual(this);
            if (source == null) return;
            if (source.CompositionTarget == null) return;

            _fromScreenTransform = source.CompositionTarget.TransformFromDevice;
            _toScreenTransform = source.CompositionTarget.TransformToDevice;
        }

        private void RenderBitmap()
        {
            var resolution = new Vector(96.0, 96.0) * _toScreenTransform;
            var pixelSize = new Vector(_bounds.Width, _bounds.Height) * _toScreenTransform;

            var renderTargetBitmap = new RenderTargetBitmap(
                (int)pixelSize.X, (int)pixelSize.Y,
                resolution.X, resolution.Y, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(_drawingVisual);

            MaxWidth = _bounds.Width;
            MaxHeight = _bounds.Height;
            TheImage.Source = renderTargetBitmap;
        }

        private void MousePositionChanged(object sender, MousePositionChangedEventArgs eventargs)
        {
            Dispatcher.BeginInvoke(new Action<Point>(UpdateLocation), eventargs.Position);
        }

        private void UpdateLocation(Point location)
        {
            Vector locationVec = new Vector(location.X, location.Y);
            Vector transformedLocation = locationVec * _fromScreenTransform;

            Left = transformedLocation.X - _grabPosition.X;
            Top = transformedLocation.Y - _grabPosition.Y;
        }
    }
}
