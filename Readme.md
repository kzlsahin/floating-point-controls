# Floating Poin Controls for WPF

There is no built-in control for floating-point inputs in Windows Presentation Foundation (WPF). Using TextBox for managing floating-point inputs can be a challange for a developer. If all you need is a floating-point input with seamless WPF compatibility, this package is for you.

There is only a DoubleInput control currently.
## Features:
- Accepts only digits, negative sign (-) at the beginning and decimal seperator of the culture information only once.
- Does not response and change the text if any char is entered other than the accepted ones.
- Fires event when value is changed,
- Value changes only when the text is parsable as a floating-point number.
- suitible to be used with data binding even with TwoWay binding mode.
- Style properties such as, text size, text weight, alignment shall be accessible for customizable visual styling.
- Maximum number of decimal places can bes set by XAML file.
- Can be set to trim trailing zeros after last meaningful deciaml digit or decimal seperator.
- Whole text is selected when the control is double-clicked.

## Example:

[WPF Sample With Circular Dependent Inputs](https://github.com/kzlsahin/Workbench/tree/master/MarineParamCalculatorDataBindings)

```XML
<!--xmlns:controls="clr-namespace:FloatingPointControls;assembly=FloatingPointControl" -->
<controls:DoubleInput 
            Grid.Column="1" 
            Value="{Binding T,  Mode=TwoWay}"
            Style="{StaticResource DoubleInputStyle}" />
```