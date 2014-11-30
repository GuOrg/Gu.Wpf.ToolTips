namespace Gu.Wpf.ToolTips.Demo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Net.Mime;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media;
    using Gu.Wpf.ToolTips.Demo.Annotations;

    public class ViewModel : INotifyPropertyChanged
    {
        private string _value = "Value from viewmodel";

        public ViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value == _value)
                {
                    return;
                }
                _value = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
