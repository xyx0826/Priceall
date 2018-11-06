using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using System.Diagnostics;

namespace Priceall.Hotkey
{
    /// <summary>
    /// Class representing a hotkey definition.
    /// </summary>
    public class Hotkey
    {
        public string Name;
        public Key[] Keys;
        static Action Action;
        
        /// <summary>
        /// Creates a hotkey of the given keypresses.
        /// </summary>
        /// <param name="name">The name for the hotkey.</param>
        /// <param name="keys">The key combo for the hotkey.</param>
        /// <param name="action">The action to be executed on hotkey press.</param>
        public Hotkey(string name, Key[] keys, Action action)
        {
            Name = name;
            Keys = keys;
            Action = action;
            Array.Sort(Keys);
        }

        /// <summary>
        /// Creates a hotkey using a csv-formatted key combo.
        /// </summary>
        /// <param name="name">The name for the hotkey.</param>
        /// <param name="keys">The key combo for the hotke, in CSV format.</param>
        /// <param name="action">The action to be executed on hotkey press.</param>
        public Hotkey(string name, string keys, Action action)
        {
            Name = name;
            Keys = DeserializeKeyCombo(keys);
            Action = action;
            Array.Sort(Keys);
        }

        public bool IsKeyHit(Key[] pressedKeys)
        {
            // Check for length
            if (pressedKeys.Length != Keys.Length) return false;
            Array.Sort(pressedKeys);
            return pressedKeys.SequenceEqual(Keys);
        }

        public void Invoke()
        {
            Action.Invoke();
        }

        /// <summary>
        /// Deserializes a CSV-formatted key combo def into a list of .NET keys.
        /// </summary>
        /// <param name="keys">Key combo in CSV format.</param>
        /// <returns>An array of keys.</returns>
        Key[] DeserializeKeyCombo(string keys)
        {
            var keyList = new List<Key>();
            foreach (var key in keys.Split(','))
            {
                if (Int32.TryParse(key, out int keyCode))
                {
                    keyList.Add((Key)keyCode);
                }
            }
            return keyList.ToArray();
        }

        /// <summary>
        /// Converts the key definition of this hotkey to comma-separated form.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name},{String.Join(",", Keys)}";
        }
    }
}
