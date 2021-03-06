﻿using Priceall.Properties;
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
    internal class ClipboardService
    {
        private IntPtr _hwndNextViewer = IntPtr.Zero;
        private HwndSource _hwndSource;

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

        #region Event
        public event EventHandler ClipboardChanged;

        protected virtual void OnClipboardChanged(EventArgs e)
        {
            ClipboardChanged?.Invoke(this, e);
        }
        #endregion

        /// <summary>
        /// Stores the HwndSource of the listening window and action to do.
        /// </summary>
        /// <param name="hwndSource">HwndSource of the listening window.</param>
        /// <param name="action">Action to invoke when clipboard event occurs.</param>
        public void InitializeListener(HwndSource hwndSource)
        {
            _hwndSource = hwndSource;
            ToggleListener();
            Settings.Default.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "IsUsingAutomaticRefresh")
                    ToggleListener();
            };
        }

        public void ToggleListener()
        {
            if (SettingsService.Get<bool>("IsUsingAutomaticRefresh"))
            {
                _hwndSource.AddHook(OnClipboardChanged);
                _hwndNextViewer = SetClipboardViewer(_hwndSource.Handle);
            }
            else
            {
                _hwndSource.RemoveHook(OnClipboardChanged);
                ChangeClipboardChain(_hwndSource.Handle, _hwndNextViewer);
            }
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
                    // Clipboard content has changed. Fire the event.
                    OnClipboardChanged(EventArgs.Empty);
                    SendMessage(_hwndNextViewer, msg, wParam, lParam);
                    break;
            }

            return IntPtr.Zero;
        }
    }
}
