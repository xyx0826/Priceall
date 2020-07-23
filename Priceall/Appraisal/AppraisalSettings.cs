using System;
using System.ComponentModel;

namespace Priceall.Appraisal
{
    abstract class AppraisalSettings
    {
        public string Name { get; protected set; }
    }

    /// <summary>
    /// A custom settings supported by an appraisal service.
    /// </summary>
    /// <typeparam name="T">The type of the settings value.</typeparam>
    class AppraisalSettings<T> : AppraisalSettings, INotifyPropertyChanged where T : struct
    {

        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                _onSet(value);
                OnPropertyChanged(nameof(Value));
            }
        }

        private Action<T> _onSet;

        public AppraisalSettings(string name, Action<T> onSet, T value = default)
        {
            _onSet = onSet;
            Name = name;
            Value = value;
        }

        #region Binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
