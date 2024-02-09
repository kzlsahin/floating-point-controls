using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FloatingPointControls
{
    /// <summary>
    /// Control for double type inputs and bindable Value property.
    /// </summary>
    public partial class DoubleInput : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(double), typeof(DoubleInput), new PropertyMetadata(0.0, OnValuePropertyChanged));

        public static readonly DependencyProperty MaxDecimalPlacesProperty =
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
        protected bool TrimTrailingZerosAfterDecimal = true;
        /// <summary>
        /// format string used to stringify the Value.
        /// </summary>
        protected string FormatString = "F2";
        /// <summary>
        /// Decimal seperator of the current culture
        /// </summary>
        protected static string DecimalSeperator => CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        /// <summary>
        /// Text is modified by Text property, don't handle TextInput
        /// </summary>
        protected bool LocalTextUpdate = false;
        /// <summary>
        /// this control is handling TextInput event.
        /// </summary>
        protected bool InTextInput = false;
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
                    LocalTextUpdate = true;
                    textBox.Text = value;
                    LocalTextUpdate = false;

                }
            }
        }
        /// <summary>
        /// Check input preview, accept only allowed characters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!IsCharacterAllowed(e.Text))
            {
                e.Handled = true;
            }
        }
        protected void TextBox_MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                e.Handled = true;
                textBox.SelectAll();
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
            char lastEntry = text[^1];
            return
                (DecimalSeperator.Contains(lastEntry) && !Text.Contains(lastEntry))
                || (Text.Length == 0 && lastEntry == '-')
                || Char.IsDigit(lastEntry);
        }
        /// <summary>
        /// If not _inTextInput, updates Value according to parsed Text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LocalTextUpdate)
            {
                return;
            }
            InTextInput = true;
            // Text Changed by user input
            // Check if the entered text is a valid numeric value
            if (double.TryParse(Text, out double val))
            {
                Value = val;
                InputEntered?.Invoke(this, e);
            }
            InTextInput = false;
        }
        /// <summary>
        /// Updates Text property only if nTextInput is false.
        /// </summary>
        /// <param name="newValue"></param>
        protected void OnValueChanged(double? newValue)
        {
            if (InTextInput)
                return;
            if (newValue.HasValue)
            {
                string txt = newValue.Value.ToString(FormatString);
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
                di?.OnValueChanged(e.NewValue as double?);
            }
        }

        protected static void OnMaxDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double? newValue = e.NewValue as double?;
            int? decNum = (int?)newValue ?? 2;
            var type = typeof(DoubleInput);
            FieldInfo? fieldInfo = type.GetField("FormatString", bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo?.SetValue(d, $"F{decNum}");
        }
    }
}
