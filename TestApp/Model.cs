using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MarineParamCalculatorDataBindings
{
    public class Model : INotifyPropertyChanged
    {
        double _b = 5;
        bool _Calculating = false;
        public double B
        {
            get => _b;
            set
            {
                if(_b != value)
                {
                    _b = value;
                    RenewDelta();
                    OnPropertyChanged();
                }                
            }
        }
        double _l = 5;
        public double L
        {
            get => _l;
            set
            {
                if(_l != value)
                {
                    _l = value;
                    RenewDelta();
                    OnPropertyChanged();
                }                
            }
        }
        double _t = 5;
        public double T
        {
            get => _t;
            set
            {
                if(_t != value)
                {
                    _t = value;
                    RenewDelta();
                    OnPropertyChanged();
                }                
            }
        }
        double _cb = 1;
        public double Cb
        {
            get => _cb;
            set
            {
                if(value != _cb)
                {
                    _cb = value;
                    RenewDelta();
                    OnPropertyChanged();
                }                
            }
        }
        double _delta = 125;
        public double Delta
        {
            get => _delta;
            set
            {
                if (_delta != value)
                {
                    _delta = value;
                    RenewCb();
                    OnPropertyChanged();
                }                
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RenewDelta()
        {
            if (!_Calculating)
            {
                _Calculating = true;
                Delta = Cb * B * T * L;
                _Calculating = false;
            }            
        }
        public void RenewCb()
        {
            if (!_Calculating)
            {
                _Calculating = true;
                Cb = Delta / (B * T * L);
                _Calculating = false;
            }            
        }

        public override string ToString()
        {
            return $"B:{B,-11:f} L:{L,-11:f} T:{T,-11:f} Cb:{Cb,-11:f} Delta:{Delta,-11:f}";
        }

        internal void Parse(string data)
        {
            
            Dictionary<string,double> keyValuePairs = new Dictionary<string,double>();
            string[] items = data.Split(' ');
            foreach(var item in items)
            {
                string[] pairs = item.Split(':');
                if(pairs.Length < 2 )
                {
                    continue;
                }
                string key = pairs[0];
                if (double.TryParse(pairs[1], out double value))
                {
                    keyValuePairs.Add(key, value);
                }
            }

            if (keyValuePairs.TryGetValue("B", out double B_value))
                B = B_value;
            if (keyValuePairs.TryGetValue("L", out double L_value))
                L = L_value;
            if (keyValuePairs.TryGetValue("T", out double T_value))
                T = T_value;
            
        }
    }
}
