using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	/// Interaction logic for TimeEdit.xaml
	/// </summary>
	public partial class TimeEdit : UserControl
	{
        #region Values

        private bool initialising;

        private DateTime value = DateTime.Now;
        private DateTime datePart = DateTime.Today;

        public DateTime Value
        {
            get { return this.value; }
            set
            {
                bool hasChanged = this.value == value;
                if (hasChanged)
                {
                    RaiseEvent(new RoutedEventArgs(ValueChangedEvent, this));
                    this.value = value;
                    SetValue();
                }
            }
        }

        public bool Rollover
        {
            get { return numericSpinEditHH.Rollover; }
            set
            {
                numericSpinEditHH.Rollover = value;
                numericSpinEditMM.Rollover = value;
            }
        }

        #endregion

        #region Layout

        public TextAlignment TextAlignment
        {
            get { return numericSpinEditHH.TextAlignment; }
            set
            {
                numericSpinEditHH.TextAlignment = value;
                numericSpinEditMM.TextAlignment = value;
            }
        }

        public bool AllowFontResize
        {
            get { return numericSpinEditHH.AllowFontResize; }
            set
            {
                numericSpinEditHH.AllowFontResize = value;
                numericSpinEditMM.AllowFontResize = value;
            }
        }

        public Control NextControl
        {
            get { return numericSpinEditMM.NextControl; }
            set { numericSpinEditMM.NextControl = value; }
        }
   
        #endregion

        #region Brushes
        public new Brush Background
        {
            get { return numericSpinEditHH.Background; }
            set
            {
                numericSpinEditHH.Background = value;
                numericSpinEditMM.Background = value;
            }
        }

        public Brush Fill
        {
            get { return numericSpinEditHH.Fill; }
            set
            {
                numericSpinEditHH.Fill = value;
                numericSpinEditMM.Fill = value;
            }
        }

        public Brush ForeGround
        {
            get { return numericSpinEditHH.Foreground; }
            set
            {
                numericSpinEditHH.Foreground = value;
                numericSpinEditMM.Foreground = value;
            }
        }

        public Brush Border
        {
            get { return borderRect.Stroke; }
            set { borderRect.Stroke = value; }
        }

        #endregion

        public TimeEdit()
		{
			this.InitializeComponent();
            numericSpinEditHH.MaxChars = 2;
            numericSpinEditMM.MaxChars = 2;
            numericSpinEditHH.NextControl = numericSpinEditMM;
		}

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
         "ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TimeEdit));

        // Provide CLR accessors for the event
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public void SetValue()
        {
            initialising = true;
            try
            {
                numericSpinEditHH.Value = value.Hour;
                numericSpinEditMM.Value = value.Minute;
                datePart = value.Date;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                initialising = false;
            }
        }

        public void GetValue()
        {
            if (!initialising)
            {
                int h = (int)numericSpinEditHH.Value;
                int m = (int)numericSpinEditMM.Value;
                Value = new DateTime(datePart.Year, datePart.Month, datePart.Day, h, m, 0);
            }
        }

        private void numericSpinEditHH_ValueChanged(object sender, RoutedEventArgs e)
        {
            GetValue();
        }

        private void numericSpinEditMM_ValueChanged(object sender, RoutedEventArgs e)
        {
            GetValue();
        }

        private void timeEdit_GotFocus(object sender, RoutedEventArgs e)
        {
            this.numericSpinEditHH.Focus();
            e.Handled = true;
        }

    }
}