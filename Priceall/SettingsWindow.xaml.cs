using System.Windows;
using System.ComponentModel;
using Priceall.Properties;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.Windows.Input;

namespace Priceall
{
    /// <summary>
    /// SettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {
        #region Binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        static readonly Regex _numberRegex = new Regex("[^0-9]+");

        //public List<KeyDef> ModifierKeys { get; set; }
        //public List<KeyDef> VirtualKeys { get; set; }

        public SettingsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Binds data contexts as soon as window is loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //ModifierKeys = new List<KeyDef>();
            //VirtualKeys = new List<KeyDef>();

            //foreach (var keyValue in Enum.GetValues(typeof(ModifierKeys)))
            //{
            //    ModifierKeys.Add(new KeyDef(Enum.GetName(typeof(ModifierKeys), keyValue), (int)keyValue));
            //}

            //foreach (var keyValue in Enum.GetValues(typeof(Key)))
            //{
            //    VirtualKeys.Add(new KeyDef(Enum.GetName(typeof(Key), keyValue), (int)keyValue));
            //}

            DataContext = this;
        }

        /// <summary>
        /// Cancels window closing, reflect latest settings and hide.
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            OnPropertyChanged(null);
            Hide();
        }

        #region Settings values
        public int MaxStringLength
        {
            get { return Settings.Default.MaxStringLength; }
            set
            {
                if (value != 0)
                {
                    Settings.Default.MaxStringLength = value;
                    Settings.Default.Save();
                }
            }
        }

        public string AppVersion => Assembly.GetEntryAssembly().GetName().Version.ToString();
        #endregion

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _numberRegex.IsMatch(e.Text);
        }

        private void OpenGithubPage(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }

        //public class KeyDef
        //{
        //    public KeyDef(string name, int value)
        //    {
        //        Name = name;
        //        Value = value;
        //    }
        //    public string Name { get; set; }
        //    public int Value { get; set; }
        //}
    }
}
