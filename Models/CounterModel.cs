using System.ComponentModel;
using Microsoft.Maui.Graphics;

namespace Counter.Models
{
    public class CounterModel : INotifyPropertyChanged
    {
        private int _value;
        private Color _counterColor;

        public string Name { get; set; }
        public int InitialValue { get; set; }

        public int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public Color CounterColor
        {
            get => _counterColor;
            set
            {
                if (_counterColor != value)
                {
                    _counterColor = value;
                    OnPropertyChanged(nameof(CounterColor));
                }
            }
        }

        public string CounterColorHex
        {
            get => CounterColor.ToHex();
            set => CounterColor = Color.FromArgb(value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ResetValue()
        {
            Value = InitialValue;
        }
    }
}
