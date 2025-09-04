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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OperatorLogin
{
    /// <summary>
    /// Interaction logic for UserFeedback.xaml
    /// </summary>
    public partial class UserFeedback : Window
    {
        DispatcherTimer readTimer;
        public enum TimeOutModeType { None, Cancel, OK };
        public string ok_trans = "OK";
        public string cancel_trans = "Cancel";
        public string stop_trans = "Stop";
        public string FeedbackCaption = "Caption";
        public string Feedback = "Feedback";
        private TimeOutModeType timeOutMode = TimeOutModeType.None;
        public TimeOutModeType TimeOutMode
        {
            get { return timeOutMode; }
            set { timeOutMode = value; }
        }
        private int timeOutSeconds = 5;
        private int countDown = 5;
        public int TimeOutSeconds
        {
            get { return timeOutSeconds; }
            set
            {
                timeOutSeconds = value;
                countDown = value;
            }
        }

        public UserFeedback()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            readTimer = new DispatcherTimer();
            if (timeOutMode != TimeOutModeType.None)
            {
                readTimer.Tick += new EventHandler(readTick);
                readTimer.Interval = new TimeSpan(0, 0, 0, 1);
                readTimer.Start();
            }
            else
            {
                btnCancel.Visibility = Visibility.Collapsed;
                btnStop.Visibility = Visibility.Collapsed;
                //btnCancel.Visibility = Visibility.Visible;
                //btnStop.Visibility = Visibility.Visible;
            }
            lblHeader.Content = FeedbackCaption;
            txtFeedback.Text = Feedback;
            PopulateButtons();  
            if (timeOutMode == TimeOutModeType.Cancel)
                btnCancel.Focus();
            else
                btnOK.Focus();
        }

        private void PopulateButtons()
        {
            if (timeOutMode == TimeOutModeType.None)
            {
                btnOK.Content = ok_trans;
                btnCancel.Content = cancel_trans;
                btnStop.Content = stop_trans;
            }
            if (timeOutMode == TimeOutModeType.OK)
            {
                btnOK.Content = ok_trans + " (" + countDown.ToString() + ")";
                btnCancel.Content = cancel_trans;
                btnStop.Content = stop_trans;
            }
            if (timeOutMode == TimeOutModeType.Cancel)
            {
                btnOK.Content = ok_trans;
                btnCancel.Content = cancel_trans + " (" + countDown.ToString() + ")";
                btnStop.Content = stop_trans;
            }
        }


        private void readTick(object sender, EventArgs e)
        {
            countDown--;
            if (countDown > 0)
            {
                PopulateButtons();
            }
            else
            {
                if (timeOutMode == TimeOutModeType.OK)
                {
                    readTimer.Stop();
                    DialogResult = true;
                }
                if (timeOutMode == TimeOutModeType.Cancel)
                {
                    readTimer.Stop();
                    DialogResult = false;
                }
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (readTimer.IsEnabled)
                readTimer.Stop();
            e.Handled = true;
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (readTimer.IsEnabled)
                readTimer.Stop();
            e.Handled = true;
            DialogResult = false;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (readTimer.IsEnabled)
                readTimer.Stop();
            e.Handled = true;
        }

        private void btnStop_TouchDown(object sender, TouchEventArgs e)
        {
            if (readTimer.IsEnabled)
                readTimer.Stop();
            e.Handled = true;
        }

        private void btnOK_TouchDown(object sender, TouchEventArgs e)
        {
            btnOK_Click(sender, e);
        }

        private void btnCancel_TouchDown(object sender, TouchEventArgs e)
        {
            btnCancel_Click(sender, e);
        }

    }
}
