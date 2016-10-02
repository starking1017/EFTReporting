using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace EFTReporting.Model
{
    public class Payment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private bool _isPaymentSelected;

        public bool IsPaymentSelected
        {
            get { return _isPaymentSelected; }
            set
            {
                _isPaymentSelected = value;
                NotifyPropertyChanged("IsPaymentSelected");
            }
        }

        public long? Date { get; set; }
        public string Reference { get; set; }
        public double? Amount { get; set; }
        public string CheckNo { get; set; }
    }
}