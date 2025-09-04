using System;
using System.Collections.Generic;
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
using MicroSlider;

namespace NumericEdits
{
	/// <summary>
	/// Interaction logic for NumericSpinEdit.xaml
	/// </summary>
	public partial class NumericSpinEdit : UserControl
    {
        #region Values
        public double Value
        {
            get { return numericEdit.Value; }
            set
            {
                numericEdit.Value = value;
                uSlider.Value = value;
            }
        }

        public double Minimum
        {
            get { return numericEdit.Minimum; }
            set
            {
                numericEdit.Minimum = value;
                uSlider.Minimum = value;
            }
        }

        public double Maximum
        {
            get { return numericEdit.Maximum; }
            set
            {
                numericEdit.Maximum = value;
                uSlider.Maximum = value;
            }
        }

        public bool IsInteger
        {
            get { return numericEdit.IsInteger; }
            set { numericEdit.IsInteger = value; }
        }

        public bool Rollover
        {
            get { return numericEdit.Rollover; }
            set
            {
                numericEdit.Rollover = value;
                uSlider.Rollover = value;
            }
        }

        public string FormatString
        {
            get { return numericEdit.FormatString; }
            set { numericEdit.FormatString = value; }
        }
        #endregion

        public double ScrollIncrement
        {
            get { return uSlider.ScrollIncrement; }
            set
            {
                uSlider.ScrollIncrement = value;
                numericEdit.ScrollIncrement = value;
            }
        }

        #region Layout

        public double SpinColumnPercent
        {
            get
            {
                GridLength gl = SpinColumn.Width;
                return gl.Value;
            }
            set
            {
                GridLength gl = new GridLength(100 - value, GridUnitType.Star);
                EditColumn.Width = gl;
                gl = new GridLength(value, GridUnitType.Star);
                SpinColumn.Width = gl;
            }
        }

        public TextAlignment TextAlignment
        {
            get { return numericEdit.TextAlignment; }
            set { numericEdit.TextAlignment = value; }
        }

        public int MaxChars
        {
            get { return numericEdit.MaxChars; }
            set { numericEdit.MaxChars = value; }
        }

        public Control NextControl
        {
            get { return numericEdit.NextControl; }
            set { numericEdit.NextControl = value; }
        }

        #endregion

        #region Brushes
        public new Brush Background
        {
            get { return numericEdit.Background; }
            set { numericEdit.Background = value; }
        }

        public Brush Fill
        {
            get { return uSlider.Background; }
            set { uSlider.Background = value; }
        }

        public Brush ForeGround
        {
            get { return numericEdit.Foreground; }
            set { numericEdit.Foreground = value; }
        }

        public Brush Border
        {
            get { return borderRect.Stroke; }
            set { borderRect.Stroke = value; }
        }

        public bool AllowFontResize
        {
            get { return numericEdit.AllowFontResize; }
            set { numericEdit.AllowFontResize = value; }
        }

        #endregion

        public NumericSpinEdit()
		{
			this.InitializeComponent();
            this.Maximum = 100;
            this.Minimum = 0;

		}

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
         "ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NumericSpinEdit));

        // Provide CLR accessors for the event
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        private void uSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            Value = uSlider.Value;
        }

        private void numericEdit_ValueChanged(object sender, RoutedEventArgs e)
        {
            Value = numericEdit.Value;
            RaiseEvent(new RoutedEventArgs(ValueChangedEvent, this));
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            this.numericEdit.Focus();
            e.Handled = true;
        }



	}
}