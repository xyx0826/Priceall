using System.Collections.Generic;
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
    /// Class for converting between VkCode and CsCode (C# key code).
    /// </summary>
    public static class KeyInfo
    {
        static readonly Dictionary<string, KeyCode> _keyInfo
            = new Dictionary<string, KeyCode>()
        {
            {"Back", new KeyCode(8, 2)},
            {"Tab", new KeyCode(9, 3)},
            {"LineFeed", new KeyCode(10, 4)},
            {"Clear", new KeyCode(12, 5)},
            {"Return", new KeyCode(13, 6)},
            {"Enter", new KeyCode(13, 6)},
            {"Pause", new KeyCode(19, 7)},
            {"Capital", new KeyCode(20, 8)},
            {"CapsLock", new KeyCode(20, 8)},
            {"Escape", new KeyCode(27, 13)},
            {"Space", new KeyCode(32, 18)},
            {"Prior", new KeyCode(33, 19)},
            {"PageUp", new KeyCode(33, 19)},
            {"Next", new KeyCode(34, 20)},
            {"PageDown", new KeyCode(34, 20)},
            {"End", new KeyCode(35, 21)},
            {"Home", new KeyCode(36, 22)},
            {"Left", new KeyCode(37, 23)},
            {"Up", new KeyCode(38, 24)},
            {"Right", new KeyCode(39, 25)},
            {"Down", new KeyCode(40, 26)},
            {"Select", new KeyCode(41, 27)},
            {"Print", new KeyCode(42, 28)},
            {"Execute", new KeyCode(43, 29)},
            {"Snapshot", new KeyCode(44, 30)},
            {"PrintScreen", new KeyCode(44, 30)},
            {"Insert", new KeyCode(45, 31)},
            {"Delete", new KeyCode(46, 32)},
            {"Help", new KeyCode(47, 33)},
            {"D0", new KeyCode(48, 34)},
            {"D1", new KeyCode(49, 35)},
            {"D2", new KeyCode(50, 36)},
            {"D3", new KeyCode(51, 37)},
            {"D4", new KeyCode(52, 38)},
            {"D5", new KeyCode(53, 39)},
            {"D6", new KeyCode(54, 40)},
            {"D7", new KeyCode(55, 41)},
            {"D8", new KeyCode(56, 42)},
            {"D9", new KeyCode(57, 43)},
            {"A", new KeyCode(65, 44)},
            {"B", new KeyCode(66, 45)},
            {"C", new KeyCode(67, 46)},
            {"D", new KeyCode(68, 47)},
            {"E", new KeyCode(69, 48)},
            {"F", new KeyCode(70, 49)},
            {"G", new KeyCode(71, 50)},
            {"H", new KeyCode(72, 51)},
            {"I", new KeyCode(73, 52)},
            {"J", new KeyCode(74, 53)},
            {"K", new KeyCode(75, 54)},
            {"L", new KeyCode(76, 55)},
            {"M", new KeyCode(77, 56)},
            {"N", new KeyCode(78, 57)},
            {"O", new KeyCode(79, 58)},
            {"P", new KeyCode(80, 59)},
            {"Q", new KeyCode(81, 60)},
            {"R", new KeyCode(82, 61)},
            {"S", new KeyCode(83, 62)},
            {"T", new KeyCode(84, 63)},
            {"U", new KeyCode(85, 64)},
            {"V", new KeyCode(86, 65)},
            {"W", new KeyCode(87, 66)},
            {"X", new KeyCode(88, 67)},
            {"Y", new KeyCode(89, 68)},
            {"Z", new KeyCode(90, 69)},
            {"Lwin", new KeyCode(91, 70)},
            {"Rwin", new KeyCode(92, 71)},
            {"Apps", new KeyCode(93, 72)},
            {"Sleep", new KeyCode(95, 73)},
            {"NumPad0", new KeyCode(96, 74)},
            {"NumPad1", new KeyCode(97, 75)},
            {"NumPad2", new KeyCode(98, 76)},
            {"NumPad3", new KeyCode(99, 77)},
            {"NumPad4", new KeyCode(100, 78)},
            {"NumPad5", new KeyCode(101, 79)},
            {"NumPad6", new KeyCode(102, 80)},
            {"NumPad7", new KeyCode(103, 81)},
            {"NumPad8", new KeyCode(104, 82)},
            {"NumPad9", new KeyCode(105, 83)},
            {"Multiply", new KeyCode(106, 84)},
            {"Add", new KeyCode(107, 85)},
            {"Separator", new KeyCode(108, 86)},
            {"Subtract", new KeyCode(109, 87)},
            {"Decimal", new KeyCode(110, 88)},
            {"Devide", new KeyCode(111, 89)},
            {"F1", new KeyCode(112, 90)},
            {"F2", new KeyCode(113, 91)},
            {"F3", new KeyCode(114, 92)},
            {"F4", new KeyCode(115, 93)},
            {"F5", new KeyCode(116, 94)},
            {"F6", new KeyCode(117, 95)},
            {"F7", new KeyCode(118, 96)},
            {"F8", new KeyCode(119, 97)},
            {"F9", new KeyCode(120, 98)},
            {"F10", new KeyCode(121, 99)},
            {"F11", new KeyCode(122, 100)},
            {"F12", new KeyCode(123, 101)},
            {"F13", new KeyCode(124, 102)},
            {"F14", new KeyCode(125, 103)},
            {"F15", new KeyCode(126, 104)},
            {"F16", new KeyCode(127, 105)},
            {"F17", new KeyCode(128, 106)},
            {"F18", new KeyCode(129, 107)},
            {"F19", new KeyCode(130, 108)},
            {"F20", new KeyCode(131, 109)},
            {"F21", new KeyCode(132, 110)},
            {"F22", new KeyCode(133, 111)},
            {"F23", new KeyCode(134, 112)},
            {"F24", new KeyCode(135, 113)},
            {"NumLock", new KeyCode(144, 114)},
            {"Scroll", new KeyCode(145, 115)},
            {"LShiftKey", new KeyCode(160, 116)},
            {"RShiftKey", new KeyCode(161, 117)},
            {"LControlKey", new KeyCode(162, 118)},
            {"RControlKey", new KeyCode(163, 119)},
            {"Lmenu", new KeyCode(164, 120)},
            {"Rmenu", new KeyCode(165, 121)},
            {"BrowserBack", new KeyCode(166, 122)},
            {"BrowserForward", new KeyCode(167, 123)},
            {"BrowserRefresh", new KeyCode(168, 124)},
            {"BrowserStop", new KeyCode(169, 125)},
            {"BrowserSearch", new KeyCode(170, 126)},
            {"BrowserFavorites", new KeyCode(171, 127)},
            {"BrowserHome", new KeyCode(172, 128)},
            {"VolumeMute", new KeyCode(173, 129)},
            {"VolumeDown", new KeyCode(174, 130)},
            {"Volumeup", new KeyCode(175, 131)},
            {"MediaNextTrack", new KeyCode(176, 132)},
            {"MediaPreviousTrack", new KeyCode(177, 133)},
            {"MediaStop", new KeyCode(178, 134)},
            {"MediaPlayPause", new KeyCode(179, 135)},
            {"LaunchMail", new KeyCode(180, 136)},
            {"SelectMedia", new KeyCode(181, 137)},
            {"LaunchApplication1", new KeyCode(182, 138)},
            {"LaunchApplication2", new KeyCode(183, 139)},
            {"OemSemicolon", new KeyCode(186, 140)},
            {"Oem1", new KeyCode(186, 140)},
            {"Oemplus", new KeyCode(187, 141)},
            {"Oemcomma", new KeyCode(188, 142)},
            {"Oemminus", new KeyCode(189, 143)},
            {"OemPeriod", new KeyCode(190, 144)},
            {"OemQuestion", new KeyCode(191, 145)},
            {"Oem2", new KeyCode(191, 145)},
            {"Oemtilde", new KeyCode(192, 146)},
            {"Oem3", new KeyCode(192, 146)},
            {"OemOpenBrackets", new KeyCode(219, 149)},
            {"Oem4", new KeyCode(219, 149)},
            {"OemPipe", new KeyCode(220, 150)},
            {"Oem5", new KeyCode(220, 150)},
            {"OemColoseBrackets", new KeyCode(221, 151)},
            {"Oem6", new KeyCode(221, 151)},
            {"OemQuotes", new KeyCode(222, 152)},
            {"Oem7", new KeyCode(222, 152)},
            {"Oem8", new KeyCode(223, 153)},
            {"OemBlackslash", new KeyCode(226, 154)},
            {"Oem102", new KeyCode(226, 154)},
            {"Play", new KeyCode(250, 167)},
            {"Zoom", new KeyCode(251, 168)},
            {"Pa1", new KeyCode(253, 170)},
            {"OemClear", new KeyCode(254, 171)}
        };

        public static Key GetKey(int vkCode)
        {
            foreach(var key in _keyInfo)
            {
                if (key.Value.VkCode == vkCode)
                    return (Key)key.Value.CsCode;
            }
            return Key.None;
        }
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
