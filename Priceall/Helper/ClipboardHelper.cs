using System;
using System.Windows;

namespace Priceall.Helper
{
    static class ClipboardHelper
    {
        /// <summary>
        /// Reads clipboard.
        /// </summary>
        /// <returns>Clipboard content, or empty string if clipboard does not have text.</returns>
        public static string ReadClipboardText()
        {
            if (Clipboard.ContainsText())
            {
                return Clipboard.GetText();
            }
            return String.Empty;
        }
    }
}
