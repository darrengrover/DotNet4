using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NumericEdits
{
	/// <summary>
	/// Interaction logic for MainControl.xaml
	/// </summary>
	public partial class NumericEdit
	{
        private string lastText = string.Empty;
        private int selStart = 0;
        private bool updating = false;

        private double minimum = double.NegativeInfinity;

        public double Minimum
        {
            get { return minimum; }
            set { minimum = value; }
        }
        private double maximum = double.PositiveInfinity;

        public double Maximum
        {
            get { return maximum; }
            set { maximum = value; }
        }

        private double value = 0;
        public double Value
        {
            get { return this.value; }
            set
            {
                bool hasChanged = false;
                double aValue = LimitValue(value);
                hasChanged |= aValue != this.value;
                this.value = aValue;
                if (hasChanged)
                {
                    SetText();
                    this.txtNumeric.Focus();
                    RaiseEvent(new RoutedEventArgs(ValueChangedEvent, this));
                    hasChanged = false;
                }
            }
        }

        private void SetText()
        {
            int selstart = txtNumeric.SelectionStart;
            bool oldUpdating = updating;
            updating = true;
            txtNumeric.Text = value.ToString(formatString);
            txtNumeric.SelectionStart = selstart;
            txtNumeric.SelectionLength = 0;
            updating = oldUpdating;
        }

        private bool allowFontResize = false;
        public bool AllowFontResize
        {
            get { return allowFontResize; }
            set { allowFontResize = value; }
        }

        private int maxChars = 0;

        public int MaxChars
        {
            get { return maxChars; }
            set { maxChars = value; }
        }

        private Control nextControl = null;

        public Control NextControl
        {
            get { return nextControl; }
            set { nextControl = value; }
        }

        private static bool isInteger = true;
        public bool IsInteger
        {
            get { return isInteger; }
            set { isInteger = value; }
        }

        private double scrollIncrement = 1;
        public double ScrollIncrement
        {
            get { return scrollIncrement; }
            set { scrollIncrement = value; }
        }

        private bool rollover = false;

        public bool Rollover
        {
            get { return rollover; }
            set { rollover = value; }
        }

        private string formatString = "0";

        public string FormatString
        {
            get { return formatString; }
            set { formatString = value; }
        }

        public new Brush Background
        {
            get { return txtNumeric.Background; }
            set { txtNumeric.Background = value; }
        }

        public Brush ForeGround
        {
            get { return txtNumeric.Foreground; }
            set { txtNumeric.Foreground = value; }
        }

        public Brush Border
        {
            get { return borderRect.Stroke; }
            set { borderRect.Stroke = value; }
        }

        public TextAlignment TextAlignment
        {
            get { return txtNumeric.TextAlignment; }
            set { txtNumeric.TextAlignment = value; }
        }

        public NumericEdit()
		{
			this.InitializeComponent();
		}

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
         "ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericEdit));

        // Provide CLR accessors for the event
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        private double LimitValue(double aValue)
        {
            if (aValue > maximum)
                if (rollover && (minimum != double.NaN) && (minimum != double.NegativeInfinity)) aValue = minimum;
                else aValue = maximum;
            if (aValue < minimum)
                if (rollover && (maximum != double.NaN) && (maximum != double.PositiveInfinity)) aValue = maximum;
                else aValue = minimum;
            if (IsInteger)
                return (int)aValue;
            else
                return aValue;
        }

        private void CheckValue()
        {
            Value = LimitValue(value);
        }

        private void CheckMinimum()
        {
            if (minimum > maximum)
                minimum = maximum;
            CheckValue();
        }

        private void CheckMaximum()
        {
            if (maximum < minimum)
                maximum = minimum;
            CheckValue();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
        }

        private static string ValidateValue(string value, double min, double max)
        {
            string aString = string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                value = value.Trim();
                if (isInteger)
                {
                    try
                    {
                        Convert.ToInt64(value);
                        aString = value;
                    }
                    catch
                    {
                        aString = string.Empty;
                    }
                }
                else
                {
                    try
                    {
                        Convert.ToDouble(value);
                        aString = value;
                    }
                    catch
                    {
                        aString = string.Empty;
                    }
                }
            }
            return aString;
        }

        private static double ValidateLimits(double min, double max, double value)
        {
            if (!min.Equals(double.NaN))
            {
                if (value < min)
                    return min;
            }
            if (!max.Equals(double.NaN))
            {
                if (value > max)
                    return max;
            }
            return value;
        }

        private void txtNumeric_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!updating)
            {
                updating = true;
                string aString = txtNumeric.Text;
                bool isValid = true;
                bool overRange = false;
                Int64 x = 0;
                double d = 0;
                if ((aString != string.Empty) && (aString != "-") && (aString != ".") && (aString != "-."))
                {
                    if (isInteger)
                    {
                        isValid = Int64.TryParse(aString, out x);
                        if (isValid)
                        {
                            overRange = (x > maximum) || (x < minimum);
                            isValid &= !overRange;
                        }
                        if (isValid || overRange)
                            Value = x;
                    }
                    else
                    {
                        isValid = double.TryParse(aString, out d);
                        if (isValid)
                        {
                            overRange = (d <= maximum) || (d >= minimum);
                            isValid &= !overRange;
                        }
                        if (isValid || overRange)
                            Value = d;
                    }
                }
                if (overRange)
                {
                    lastText = value.ToString(formatString);
                    selStart = lastText.Length;
                    txtNumeric.Text = lastText;
                    txtNumeric.SelectionStart = selStart;
                }
                else
                {
                    if (isValid)
                    {
                        lastText = txtNumeric.Text;
                        selStart = txtNumeric.SelectionStart;
                    }
                    else
                    {
                        txtNumeric.Text = lastText;
                        txtNumeric.SelectionStart = selStart;
                        txtNumeric.SelectionLength = 0;
                    }
                }
                updating = false;
                if ((txtNumeric.Text.Length >= maxChars) && (nextControl != null))
                {
                    nextControl.Focus();
                }
            }
        }

        private void txtNumeric_SelectionChanged(object sender, RoutedEventArgs e)
        {
            selStart = txtNumeric.SelectionStart;
        }

        private void txtNumeric_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double increment = scrollIncrement;
            if (e.Delta > 0)
                increment = -scrollIncrement;
            Value += increment;
        }

        private void numericEdit_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (allowFontResize)
            {
                double fontSize = 12 * this.ActualHeight / 24;
                if (fontSize < 8) fontSize = 8;
                txtNumeric.FontSize = fontSize;
            }
        }

        private void txtNumeric_GotFocus(object sender, RoutedEventArgs e)
        {
            updating = true;
            txtNumeric.SelectionStart = 0;
            txtNumeric.SelectionLength = txtNumeric.Text.Length;
            updating = false;
            e.Handled = true;
        }

        private void numericEdit_GotFocus(object sender, RoutedEventArgs e)
        {
            this.txtNumeric.Focus();
            e.Handled = true;
        }

	}
}