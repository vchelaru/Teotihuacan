using System;
using System.Runtime.InteropServices;

namespace Teotihuacan.Utilities
{
    internal sealed class User32
    {
        [DllImport("user32.dll")]
        internal static extern void SetWindowPos(IntPtr Hwnd, IntPtr Level, int X, int Y, int W, int H, uint Flags);

        internal static void SetWindowOnTop(IntPtr windowHandle, int windowWitdh, int windowHeight)
        {
            //SetWindowPos((uint)this.Window.Handle, -1, 0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0);
            SetWindowPos(
                windowHandle, WinApi.HWND_TOPMOST, 0, 0, windowWitdh, windowHeight, 
                0 //(uint)WinApi.SetWindowPosFlags.ShowWindow
            );
        }

        internal static void UnsetWindowOnTop(IntPtr windowHandle, int windowWitdh, int windowHeight)
        {
            //SetWindowPos((uint)this.Window.Handle, -1, 0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0);
            SetWindowPos(
                windowHandle, WinApi.HWND_NOTOPMOST, 0, 0, windowWitdh, windowHeight,
                0 //(uint)WinApi.SetWindowPosFlags.ShowWindow
            );
        }
    }
}
