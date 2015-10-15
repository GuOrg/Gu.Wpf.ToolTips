namespace Gu.Wpf.ToolTips.Demo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Wpf.ToolTips.Demo.Annotations;

    public class ViewModel : INotifyPropertyChanged
    {
        private string _value = "Value from viewmodel";

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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
