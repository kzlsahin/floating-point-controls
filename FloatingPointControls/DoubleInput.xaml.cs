using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FloatingPointControls
{
    /// <summary>
    /// Control for double type inputs and bindable Value property.
    /// </summary>
    public partial class DoubleInput : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(double), typeof(DoubleInput), new PropertyMetadata(0.0, OnValuePropertyChanged));

        public static readonly DependencyProperty MaxDecimalPlaces =
        DependencyProperty.Register("MaxAllowedDecimalPlaces", typeof(double), typeof(DoubleInput), new PropertyMetadata(0.0, OnMaxDecimalPlacesChanged));


        public int? MaxAllowedDecimalPlaces { get; set; }

        public bool TrimTrailingZerosAfterDecimal = true;

        protected string _formatString { get; set; } = "F2";
        protected string _decimalSeperator => CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        protected bool _localTextUpdate = false;
        protected bool _inTextInput = false;
        public DoubleInput()
        {
            InitializeComponent();
        }
        public event EventHandler? InputEntered;
        public event PropertyChangedEventHandler? PropertyChanged;


        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set
            {
                if (Value != value)
                {
                    SetValue(ValueProperty, value);
                }
            }
        }
        // Expose Text property for easy access
        public string Text
        {
            get { return textBox.Text; }
            private set
            {
                if (value != textBox.Text)
                {
                    // prevent event feedback when the TExtBox is updated by this class
                    _localTextUpdate = true;
                    textBox.Text = value;
                    _localTextUpdate = false;

                }
            }
        }
        protected void NumericTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!IsNumericTextAllowed(e.Text))
            {
                e.Handled = true;
            }
        }
        protected bool DecimalPlacesAvailable()
        {
            string decimalPart = Text.Split(_decimalSeperator).Last();
            return decimalPart.Length < MaxAllowedDecimalPlaces;
        }
        protected bool IsNumericTextAllowed(string text)
        {
            // Check if the entered text is a valid numeric value (allow '.' as well)
            char lastEntry = text[text.Length - 1];
            return
                (_decimalSeperator.Contains(lastEntry) && !Text.Contains(lastEntry))
                || (Text.Length == 0 && lastEntry == '-')
                || Char.IsDigit(lastEntry);
        }
        protected void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _inTextInput = true;
            if (_localTextUpdate == false)
            {
                // Text Changed by user input
                // Check if the entered text is a valid numeric value
                if (double.TryParse(Text, out double val))
                {
                    Value = val;
                    InputEntered?.Invoke(this, e);
                }
            }
            _inTextInput = false;
        }
        protected void OnValueChanged(double? newValue)
        {
            if (_inTextInput) 
                return;
            if (newValue.HasValue)
            {
                string txt = newValue.Value.ToString(_formatString);
                if (TrimTrailingZerosAfterDecimal)
                {
                    txt = txt.TrimEnd('0');
                }
                Text = txt;
            }
        }
        protected static void OnValuePropertyChanged(object dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is DoubleInput)
            {
                DoubleInput? di = dependencyObject as DoubleInput;
                if (di is not null)
                {
                    di.OnValueChanged(e.NewValue as double?);
                }
            }
        }

        private static void OnMaxDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int? newValue = e.NewValue as int?;
            FieldInfo? fieldInfo = typeof(DoubleInput).GetField("_formatString");
            if (newValue.HasValue && fieldInfo is not null)
            {
                fieldInfo.SetValue(d, $"F{newValue}");
            }
        }
    }
}
