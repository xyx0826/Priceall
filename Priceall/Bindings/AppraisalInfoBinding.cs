﻿using Priceall.Helpers;
using Priceall.Properties;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Priceall.Bindings
{
    class AppraisalInfoBinding : INotifyPropertyChanged
    {
        #region Binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Refresh()
        {
            OnPropertyChanged(null);
        }

        public void RefreshPriceColor()
        {
            OnPropertyChanged("PriceFontBrush");
        }
        #endregion

        public AppraisalInfoBinding()
        {
            Settings.Default.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                OnPropertyChanged(e.PropertyName);

                // Possible price tag color change: refresh the whole thing
                if (e.PropertyName == "IsUsingConditionalColors"
                || e.PropertyName == "PriceColor"
                || e.PropertyName == "LowerColor"
                || e.PropertyName == "UpperColor")
                {
                    OnPropertyChanged("PriceFontBrush");
                }
            };

            Price = "Priceall";
            SetTypeIcon("searchmarket");
        }

        #region Price tag
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
        #endregion

        #region Price font brush
        bool? _priceLowerOrHigher;

        public bool? PriceLowerOrHigher
        {
            get
            {
                return _priceLowerOrHigher;
            }
            set
            {
                _priceLowerOrHigher = value;
                RefreshPriceColor();
            }
        }

        public SolidColorBrush PriceFontBrush
        {
            get
            {
                if (Settings.Default.IsUsingConditionalColors)
                {
                    if (PriceLowerOrHigher == true)
                        return ColorHelper
                            .ConvertSettingToColorBrush("LowerColor");
                    else if (PriceLowerOrHigher == false)
                        return ColorHelper
                            .ConvertSettingToColorBrush("UpperColor");
                }
                return ColorHelper
                    .ConvertSettingToColorBrush("PriceColor");
            }
        }
        #endregion

        #region Type icon image
        public BitmapImage TypeIconImage { get; set; }

        public void SetTypeIcon(string type)
        {
            try
            {
                TypeIconImage = (BitmapImage)Application.Current.FindResource(type);
            }
            catch (Exception)   // ResourceReferenceKeyNotFoundException
            {
                TypeIconImage = (BitmapImage)Application.Current.FindResource("searchmarket");
            }
            OnPropertyChanged("TypeIconImage");
        }
        #endregion
    }
}
