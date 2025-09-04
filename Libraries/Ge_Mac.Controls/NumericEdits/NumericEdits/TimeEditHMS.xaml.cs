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
	/// Interaction logic for TimeEditHMS.xaml
	/// </summary>
	public partial class TimeEditHMS : UserControl
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

        #endregion

        #region Layout

        public TextAlignment TextAlignment
        {
            get { return numericSpinEditHH.TextAlignment; }
            set
            {
                numericSpinEditHH.TextAlignment = value;
                numericSpinEditMM.TextAlignment = value;
                numericSpinEditSS.TextAlignment = value;
            }
        }

        public bool AllowFontResize
        {
            get { return numericSpinEditHH.AllowFontResize; }
            set
            {
                numericSpinEditHH.AllowFontResize = value;
                numericSpinEditMM.AllowFontResize = value;
                numericSpinEditSS.AllowFontResize = value;
            }
        }

        public Control NextControl
        {
            get { return numericSpinEditSS.NextControl; }
            set { numericSpinEditSS.NextControl = value; }
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
                numericSpinEditSS.Background = value;
            }
        }

        public Brush Fill
        {
            get { return numericSpinEditHH.Fill; }
            set 
            {
                numericSpinEditHH.Fill = value;
                numericSpinEditMM.Fill = value;
                numericSpinEditSS.Fill = value;
            }
        }

        public Brush ForeGround
        {
            get { return numericSpinEditHH.Foreground; }
            set 
            {
                numericSpinEditHH.Foreground = value;
                numericSpinEditMM.Foreground = value;
                numericSpinEditSS.Foreground = value;
            }
        }

        public Brush Border
        {
            get { return borderRect.Stroke; }
            set { borderRect.Stroke = value; }
        }

        #endregion

		public TimeEditHMS()
		{
			this.InitializeComponent();
            numericSpinEditHH.MaxChars = 2;
            numericSpinEditMM.MaxChars = 2;
            numericSpinEditSS.MaxChars = 2;
            numericSpinEditHH.NextControl = numericSpinEditMM;
            numericSpinEditMM.NextControl = numericSpinEditSS;
		}

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
         "ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TimeEditHMS));

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
                numericSpinEditSS.Value = value.Second;
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
                int s = (int)numericSpinEditSS.Value;
                Value = new DateTime(datePart.Year, datePart.Month, datePart.Day, h, m, s);
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

        private void numericSpinEditSS_ValueChanged(object sender, RoutedEventArgs e)
        {
            GetValue();
        }

        private void timeEditHMS_GotFocus(object sender, RoutedEventArgs e)
        {
            numericSpinEditHH.Focus();
            e.Handled = true;
        }
	}
}