using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Priceall.Services
{
    /// <summary>
    /// Services class for clipboard interaction.
    /// Provides text retrieval from clipboard or update event subscription.
    /// </summary>
    class ClipboardService
    {
        IntPtr _hwndNextViewer = IntPtr.Zero;
        HwndSource _hwndSource;
        Action _clipboardAction;
        
        #region P/Invoke
        internal const int WM_DRAWCLIPBOARD = 0x0308;

        internal const int WM_CHANGECBCHAIN = 0x030D;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        #endregion

        /// <summary>
        /// Stores the HwndSource of the listening window and action to do.
        /// </summary>
        /// <param name="hwndSource">HwndSource of the listening window.</param>
        /// <param name="action">Action to invoke when clipboard event occurs.</param>
        public void InitializeListener(HwndSource hwndSource, Action action)
        {
            _hwndSource = hwndSource;
            _clipboardAction = action;
        }

        /// <summary>
        /// Reads clipboard.
        /// </summary>
        /// <returns>Clipboard content, or empty string if clipboard does not have text.</returns>
        public string ReadClipboardText()
        {
            if (Clipboard.ContainsText())
            {
                return Clipboard.GetText();
            }
            return String.Empty;
        }
        
        /// <summary>
        /// Starts processing clipboard messages by hooking up the specified window.
        /// </summary>
        public void StartListener()
        {
            _hwndSource.AddHook(OnClipboardChanged);
            _hwndNextViewer = SetClipboardViewer(_hwndSource.Handle);
        }

        /// <summary>
        /// Stops listening to clipboard messages and remove the app from queue.
        /// </summary>
        public void StopListener()
        {
            _hwndSource.RemoveHook(OnClipboardChanged);
            ChangeClipboardChain(_hwndSource.Handle, _hwndNextViewer);
        }

        /// <summary>
        /// Fired when a clipboard message occurs.
        /// </summary>
        private IntPtr OnClipboardChanged(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_CHANGECBCHAIN:
                    if (wParam == _hwndNextViewer)
                    {
                        // clipboard viewer chain changed, need to fix it. 
                        _hwndNextViewer = lParam;
                    }
                    else if (_hwndNextViewer != IntPtr.Zero)
                    {
                        // pass the message to the next viewer. 
                        SendMessage(_hwndNextViewer, msg, wParam, lParam);
                    }
                    break;

                case WM_DRAWCLIPBOARD:
                    _clipboardAction.Invoke();
                    SendMessage(_hwndNextViewer, msg, wParam, lParam);
                    break;
            }

            return IntPtr.Zero;
        }
    }
}
