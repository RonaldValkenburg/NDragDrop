// Copyright (c) 2013 Ronald Valkenburg
// This software is licensed under the MIT License (see LICENSE file for details)
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;

namespace NDragDrop
{
    public interface ISystemMouse
    {
        Point Position { get; }
        event MousePositionChangedEventHandler PositionChanged;
    }

    public class MousePositionChangedEventArgs : EventArgs
    {
        public Point Position { get; set; }
    }

    public delegate void MousePositionChangedEventHandler(object sender, MousePositionChangedEventArgs eventArgs);

    public class SystemMouse : ISystemMouse
    {
        private Point _position;
        private Timer _timer;

        public SystemMouse()
        {
            _timer = new Timer(TimerElapsed, null, 0, 20);
        }

        public event MousePositionChangedEventHandler PositionChanged;

        public Point Position
        {
            get { return _position; }
            private set
            {
                if (value.Equals(_position)) return;

                _position = value;
                if (PositionChanged != null)
                {
                    PositionChanged.BeginInvoke(this, new MousePositionChangedEventArgs { Position = _position }, null, null);
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(UInt16 virtualKeyCode);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out Point lpPoint);

        private void TimerElapsed(object state)
        {
            Point currentPosition;
            if (GetCursorPos(out currentPosition))
            {
                Position = currentPosition;
            }
        }
    }
}
