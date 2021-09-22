using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Priceall.Services
{
    internal abstract class JsonSetting
    {
        /// <summary>
        /// Setting name.
        /// </summary>
        public readonly string Name;

        protected JsonSetting(string name)
        {
            Name = name;
        }
    }

    internal class JsonSetting<T> : JsonSetting where T : struct
    {
        /// <summary>
        /// Backing setting value.
        /// </summary>
        protected T _value;

        /// <summary>
        /// Delegate to be called to persist a new value into the store.
        /// </summary>
        protected Action<T> _valueSetter;

        /// <summary>
        /// Setting default value.
        /// </summary>
        public readonly T DefaultValue;

        public JsonSetting(string name, T defaultValue, T value, Action<T> valueSetter) : base(name)
        {
            DefaultValue = defaultValue;
            _value = value;
            _valueSetter = valueSetter;
        }

        protected JsonSetting(JsonSetting<T> setting) : base(setting.Name)
        {
            DefaultValue = setting.DefaultValue;
            _value = setting._value;
            _valueSetter = setting._valueSetter;
        }

        /// <summary>
        /// Setting current value.
        /// </summary>
        public virtual T Value
        {
            get => _value;
            set
            {
                _value = value;
                _valueSetter(value);
            }
        }
    }

    /// <summary>
    /// A settings store using JSON as storage.
    /// </summary>
    internal static class JsonSettingsService
    {
        private const string FileName = "Priceall.json";

        private static string _settingsPath;

        private static JObject _settings;

        private static Dictionary<string, JsonSetting> _settingObjects;

        /// <summary>
        /// Loads the settings file if there is one, or create empty settings otherwise.
        /// </summary>
        private static void Initialize()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _settingsPath = Path.Combine(dir, FileName);
            if (File.Exists(_settingsPath))
            {
                _settings = JObject.Parse(File.ReadAllText(_settingsPath));
            }
            else
            {
                _settings = new JObject();
            }

            _settingObjects = new Dictionary<string, JsonSetting>();
        }

        private static void SetSetting<T>(string name, T value) where T : struct
        {
            if (_settings == null)
            {
                throw new Exception(
                    "Attempted to set a setting while the service is not yet initialized.");
            }

            _settings[name] = JToken.FromObject(value);
            File.WriteAllText(_settingsPath, _settings.ToString());
        }

        public static JsonSetting<T> CreateSetting<T>(string name, T defaultValue) where T : struct
        {
            if (_settings == null)
            {
                Initialize();
            }

            // Do we have this setting singleton already?
            if (_settingObjects.TryGetValue(name, out var s))
            {
                if (s is JsonSetting<T> sT)
                {
                    return sT;
                }

                throw new Exception($"A setting named {name} already exists " +
                                    $"and has a different type than {typeof(T)}.");
            }

            // Do we have this key in settings already?
            T value;
            if (_settings.TryGetValue(name, out var v))
            {
                value = v.ToObject<T>();
            }
            else
            {
                value = defaultValue;
            }

            var setting = new JsonSetting<T>(name, defaultValue, value,
                newValue => SetSetting(name, newValue));
            _settingObjects[name] = setting;
            return setting;
        }
    }
}
