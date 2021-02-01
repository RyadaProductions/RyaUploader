using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace RyaUploader.Extensions
{
    public static class WindowExtensions
    {
        /// <summary>
        /// enum to specify what type of accent i want the window to have.
        /// </summary>
        private enum AccentState
        {
            AccentDisabled = 0,
            AccentEnableGradient = 1,
            AccentEnableTransparentgradient = 2,
            AccentEnableBlurbehind = 3,
            AccentInvalidState = 4
        }
        
        [StructLayout(LayoutKind.Sequential)]
        private struct AccentPolicy
        {
            public AccentState AccentState;
            private readonly int _accentFlags;
            private readonly int _gradientColor;
            private readonly int _animationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        private enum WindowCompositionAttribute
        {
            // ...
            WcaAccentPolicy = 19
            // ...
        }

        /// <summary>
        /// Enable the Aeroglass blur that is intended to be used in UWP.
        /// </summary>
        /// <param name="window">the window to enable Aeroglass on</param>
        public static void EnableBlur(this Window window)
        {
            var windowHelper = new WindowInteropHelper(window);

            var accent = new AccentPolicy
            {
                AccentState = AccentState.AccentEnableBlurbehind
            };

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WcaAccentPolicy,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
    }
}
