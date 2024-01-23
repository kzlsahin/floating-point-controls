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

        /// <summary>
        /// Sets maximum allowed decimal places of the controller that creates formatting string as "F.".
        /// </summary>
        /// <remarks>
        /// This doesn't prevent user to exceed this limit. When value property is changed the output text will be affected by this value.
        /// Default value for format string is "F2"
        /// </remarks>
        public int? MaxAllowedDecimalPlaces { get; set; }

        /// <summary>
        /// Sets boolean value to trim trailing zeros after the last meaningful decimal place or decimal seperator.
        /// </summary>
        public bool TrimTrailingZerosAfterDecimal = true;
        /// <summary>
        /// format string used to stringify the Value.
        /// </summary>
        protected string _formatString { get; set; } = "F2";
        /// <summary>
        /// Decimal seperator of the current culture
        /// </summary>
        protected string _decimalSeperator => CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        /// <summary>
        /// Text is modified by Text property, don't handle TextInput
        /// </summary>
        protected bool _localTextUpdate = false;
        /// <summary>
        /// this control is handling TextInput event.
        /// </summary>
        protected bool _inTextInput = false;
        public DoubleInput()
        {
            InitializeComponent();
        }
        /// <summary>
        /// When the text is changed and the text is parsable as Double.
        /// </summary>
        public event EventHandler? InputEntered;

        /// <summary>
        /// Value of the Value property.
        /// </summary>
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
        /// <summary>
        /// Exposes the Text value of the underneath TextBox
        /// </summary>
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
        /// <summary>
        /// Check input preview, accept only allowed characters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void NumericTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!IsCharacterAllowed(e.Text))
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// Character check method.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>true if character is allowed, false otherwise</returns>
        protected bool IsCharacterAllowed(string text)
        {
            // Check if the entered text is a valid numeric value (allow '.' as well)
            char lastEntry = text[text.Length - 1];
            return
                (_decimalSeperator.Contains(lastEntry) && !Text.Contains(lastEntry))
                || (Text.Length == 0 && lastEntry == '-')
                || Char.IsDigit(lastEntry);
        }
        /// <summary>
        /// If not _inTextInput, updates Value according to parsed Text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Updates Text property only if nTextInput is false.
        /// </summary>
        /// <param name="newValue"></param>
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
        private static void OnValuePropertyChanged(object dependencyObject, DependencyPropertyChangedEventArgs e)
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
