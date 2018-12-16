using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Priceall.Hotkey.Controls
{
    /// <summary>
    /// HotkeyEditor.xaml 的交互逻辑
    /// </summary>
    public partial class HotkeyEditor : UserControl
    {
        private static readonly Key[] PositionalModKeys = {
            Key.LeftCtrl, Key.RightCtrl,
            Key.LeftShift, Key.RightShift,
            Key.LeftAlt, Key.RightAlt,
            Key.LWin, Key.RWin
        };

        public HotkeyEditor()
        {
            InitializeComponent();
            MyKeyCombo = new KeyCombo();
        }

        #region Dependency Injection
        public static readonly DependencyProperty HotkeyProperty
            = DependencyProperty.Register(
                nameof(MyKeyCombo), typeof(KeyCombo), typeof(HotkeyEditor), 
                new FrameworkPropertyMetadata(
                    default(KeyCombo), 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private KeyCombo MyKeyCombo
        {
            get => (KeyCombo)GetValue(HotkeyProperty);
            set => SetValue(HotkeyProperty, value);
        }
        #endregion

        #region Event fired when key combo is updated
        public static readonly RoutedEvent NewKeyComboEvent = EventManager.RegisterRoutedEvent(
            "NewKeyCombo", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HotkeyEditor));
        
        public event RoutedEventHandler NewKeyCombo
        {
            add { AddHandler(NewKeyComboEvent, value); }
            remove { RemoveHandler(NewKeyComboEvent, value); }
        }
        
        void RaiseNewKeyComboEvent()
        {
            RaiseEvent(
                new NewKeyComboEventArgs(
                    MyKeyCombo.Key, MyKeyCombo.ModifierKeys, 
                    MyKeyCombo.AllKeys.ToArray()));
        }

        private sealed class NewKeyComboEventArgs : RoutedEventArgs
        {
            public NewKeyComboEventArgs(Key key, ModifierKeys modifierKeys, Key[] allKeys)
            {
                Key = key;
                ModifierKeys = modifierKeys;
                AllKeys = allKeys;
            }

            public Key Key { get; }

            public ModifierKeys ModifierKeys { get; }

            public Key[] AllKeys { get; }
        }
        #endregion

        /// <summary>
        /// Handler when a key is pressed when editor has focus.
        /// </summary>
        private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (e.IsRepeat) return;

            var pressedKey = e.Key == Key.System ? e.SystemKey : e.Key;

            // Don't process till a non-modifier key is received
            foreach (Key modKey in PositionalModKeys)
                if (pressedKey == modKey) return;

            // A non-modifier key is pressed; set combo
            var newCombo = new KeyCombo();
            newCombo.Key = pressedKey;
            newCombo.ModifierKeys = Keyboard.Modifiers;
            
            foreach (Key modKey in PositionalModKeys)
                if (Keyboard.IsKeyDown(modKey))
                    newCombo.AllKeys.Add(modKey);

            MyKeyCombo = newCombo;
            RaiseNewKeyComboEvent();
        }

        /// <summary>
        /// Handler when the clear combo button is pressed.
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyKeyCombo = new KeyCombo();
            RaiseNewKeyComboEvent();
        }
    }
}
