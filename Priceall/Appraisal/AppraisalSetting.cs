using System;
using System.ComponentModel;
using Priceall.Services;

namespace Priceall.Appraisal
{
    /// <summary>
    /// A custom setting supported by an appraisal service.
    /// </summary>
    /// <typeparam name="T">The type of the setting value.</typeparam>
    internal class AppraisalSetting<T> : JsonSetting<T>, INotifyPropertyChanged where T : struct
    {
        private Action<T> _onSet;

        public AppraisalSetting(JsonSetting<T> setting, Action<T> onSet = null) : base(setting)
        {
            _onSet = onSet;
        }

        public override T Value
        {
            get => base.Value;
            set
            {
                // Updates backing setting, fires IPropertyChanged, calls optional action
                base.Value = value;
                _onSet?.Invoke(value);
                OnPropertyChanged(nameof(Value));
            }
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
