namespace Gu.Wpf.ToolTips.Demo
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Wpf.ToolTips.Demo.Annotations;

    public class ViewModel : INotifyPropertyChanged
    {
        private string _value = "Value from viewmodel";

        public ViewModel()
        {
            throw new NotImplementedException("Change color of i");
            throw new NotImplementedException("Template text/button");
            
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
