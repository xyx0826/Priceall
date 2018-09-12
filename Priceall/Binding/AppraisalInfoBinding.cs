using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Priceall.Binding
{
    class AppraisalInfoBinding : INotifyPropertyChanged
    {
        #region Binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public AppraisalInfoBinding()
        {
            Price = "Copy, click. (or Ctrl + Shift + C)";
            SetTypeIcon("searchmarket");
        }

        string _price;

        public string Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }

        public bool RectAppearDisabled
        {
            set
            {
                OnPropertyChanged("RectOpacity");
            }
        }

        public BitmapImage TypeIconImage { get; set; }

        public void SetTypeIcon(string type)
        {
            try
            {
                TypeIconImage = (BitmapImage)Application.Current.FindResource(type);
            }
            catch (ResourceReferenceKeyNotFoundException)
            {
                TypeIconImage = (BitmapImage)Application.Current.FindResource("default");
            }
            OnPropertyChanged("TypeIconImage");
        }
    }
}
