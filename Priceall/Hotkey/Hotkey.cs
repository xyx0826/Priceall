using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;

namespace Priceall.Hotkey
{
    /// <summary>
    /// Class representing a hotkey definition.
    /// </summary>
    public class Hotkey
    {
        /// <summary>
        /// Name (tag) of this hotkey.
        /// </summary>
        public string Name;
        /// <summary>
        /// Key composition of this hotkey.
        /// </summary>
        public Key[] Keys;
        /// <summary>
        /// Whether this hotkey is active.
        /// </summary>
        public bool IsActive;
        /// <summary>
        /// Action to invoke when this hotkey is pressed.
        /// </summary>
        static Action Action;

        /// <summary>
        /// Creates a hotkey of the given keypresses.
        /// </summary>
        /// <param name="name">The name for the hotkey.</param>
        /// <param name="keys">The key combo for the hotkey.</param>
        /// <param name="action">The action to be executed on hotkey press.</param>
        /// <param name="isActive">Whether the hotkey should be set to active.</param>
        public Hotkey(string name, Key[] keys, Action action, bool isActive = true)
        {
            Name = name;
            Keys = keys;
            IsActive = isActive;
            Action = action;
            Array.Sort(Keys);
        }

        /// <summary>
        /// Creates a hotkey using a csv-formatted key combo.
        /// </summary>
        /// <param name="name">The name for the hotkey.</param>
        /// <param name="keys">The key combo for the hotke, in CSV format.</param>
        /// <param name="action">The action to be executed on hotkey press.</param>
        /// <param name="isActive">Whether the hotkey should be set to active.</param>
        public Hotkey(string name, string keys, Action action, bool isActive = true)
        {
            Name = name;
            Keys = DeserializeKeyCombo(keys);
            IsActive = isActive;
            Action = action;
            Array.Sort(Keys);
        }

        /// <summary>
        /// Determines whether the input key combination matches this hotkey's sequence.
        /// </summary>
        /// <param name="pressedKeys">Pressed keys on the keyboard.</param>
        /// <returns>Whether the input keys match this hotkey's sequence exactly.</returns>
        public bool IsKeyHit(Key[] pressedKeys)
        {
            // Check for length
            if (pressedKeys.Length != Keys.Length) return false;
            Array.Sort(pressedKeys);
            return pressedKeys.SequenceEqual(Keys);
        }

        /// <summary>
        /// Invokes this hotkey's accociated action if it is currently active.
        /// </summary>
        public void Invoke()
        {
            if (IsActive) Action.Invoke();
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
                if (Enum.TryParse(key, out Key keyEnum))
                    keyList.Add(keyEnum);
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
