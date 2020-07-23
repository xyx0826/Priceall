using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Priceall.Hotkey
{
    public class KeyCombo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("key")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Key Key { get; set; }

        [JsonProperty("modifier_keys")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ModifierKeys ModifierKeys { get; set; }

        [JsonProperty("all_keys", ItemConverterType = typeof(StringEnumConverter))]
        public List<Key> AllKeys { get; private set; }

        public KeyCombo()
        {
            Name = String.Empty;
            Key = Key.None;
            ModifierKeys = ModifierKeys.None;
            AllKeys = new List<Key>();
        }

        public KeyCombo(string name, Key key, ModifierKeys modifierKeys)
        {
            Name = name;
            Key = key;
            ModifierKeys = modifierKeys;
            AllKeys = new List<Key>() { Key };
        }

        public void Clear()
        {
            Key = Key.None;
            ModifierKeys = ModifierKeys.None;
            AllKeys.Clear();
        }

        public bool KeysEqual(List<Key> keys)
        {
            if (AllKeys.Count != keys.Count) return false;
            foreach (Key key in AllKeys) keys.Remove(key);
            return keys.Count == 0;
        }

        public override string ToString()
        {
            if (Key == Key.None) return "N/A";

            var str = new StringBuilder();

            if (ModifierKeys.HasFlag(ModifierKeys.Control))
                str.Append("Ctrl + ");
            if (ModifierKeys.HasFlag(ModifierKeys.Shift))
                str.Append("Shift + ");
            if (ModifierKeys.HasFlag(ModifierKeys.Alt))
                str.Append("Alt + ");
            if (ModifierKeys.HasFlag(ModifierKeys.Windows))
                str.Append("Win + ");

            str.Append(Key);

            return str.ToString();
        }

        public static KeyCombo Empty
        {
            get
            {
                return new KeyCombo();
            }
        }
    }

    public static class KeyComboUtils
    {
        public static string ConvertToSettingValue(KeyCombo keyCombo)
        {
            return JsonConvert.SerializeObject(keyCombo);
        }
        
        public static KeyCombo ConvertFromSettingValue(string settingValue)
        {
            try
            {
                return JsonConvert.DeserializeObject<KeyCombo>(settingValue);
            }
            catch (JsonReaderException)
            {
                return KeyCombo.Empty;
            }
        }
    }
}
