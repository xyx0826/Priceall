using System.Windows.Input;

namespace Priceall.Hotkey
{
    /// <summary>
    /// Class that holds the virtual-key code and .NET key code for a key.
    /// </summary>
    public class KeyCode
    {
        public KeyCode(int vkCode, int csCode)
        {
            VkCode = vkCode;
            CsCode = csCode;
        }

        public int VkCode;
        public int CsCode;

        public Key ToKey()
        {
            return ((Key)CsCode);
        }

        public override string ToString()
        {
            return ((Key)CsCode).ToString();
        }
    }

    /// <summary>
    /// Keyboard event callback.
    /// </summary>
    public struct KeyboardHook
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }

    /// <summary>
    /// Keyboard event types.
    /// </summary>
    public enum KeyboardMessages
    {
        KeyDown = 0x0100,
        KeyUp = 0x0101,
        SysKeyDown = 0x0104,
        SysKeyUp = 0x0105
    }
}
