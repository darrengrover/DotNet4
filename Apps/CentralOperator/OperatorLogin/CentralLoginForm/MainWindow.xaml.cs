using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TPL.TagReaders;
using System.Collections;

namespace CentralLoginUI
{

    public partial class MainWindow : Window
    {
        private TagReaderServiceFramework tagReaderServiceFramework;
        private ServiceState currentState = ServiceState.Stopped;
        private SynchronizationContext uiSyncContext;

        public MainWindow()
        {
            InitializeComponent();
            tagReaderServiceFramework = new TagReaderServiceFramework(Properties.Settings.Default.JEGRConnection, Feedback);
            tagReaderServiceFramework.UseOperatorPermissions = Properties.Settings.Default.UseOperatorPermissions;
            SetState(currentState);
            uiSyncContext = SynchronizationContext.Current;
        }

        private void feedback(string info)
        {
            //tagReaderServiceFramework.DBFeedback(info);
            txtFeedback.Text = MaxStringLen("> " + DateTime.Now + " : " + info + txtFeedback.Text);
        }

        private string MaxStringLen(string String)
        {
            string RetString = "";

            if (String.Length <= 65536)
            {
                RetString = String;
            }
            else
            {
                RetString = String.Substring(0, 4096);
            }
            return RetString;
        }

        public void Feedback(string info)
        {
            uiSyncContext.Post(delegate { feedback(info); }, null);            
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            tagReaderServiceFramework.DebugMode = (Boolean)chkDebug.IsChecked;
            tagReaderServiceFramework.DBDebugMode = true;
            tagReaderServiceFramework.AutoLogout = Properties.Settings.Default.AutoLogout;
            tagReaderServiceFramework.AutoLogoutTimes = Properties.Settings.Default.AutoLogoutTime;
            SetState(tagReaderServiceFramework.Start());
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            SetState(tagReaderServiceFramework.Stop());
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            SetState(tagReaderServiceFramework.Pause());
        }

        private void btnResume_Click(object sender, RoutedEventArgs e)
        {
            SetState(tagReaderServiceFramework.Resume());
        }

        private void SetState(ServiceState State)
        {
            switch (State)
            {
                case ServiceState.Stopped:
                    btnStart.IsEnabled = true;
                    btnStop.IsEnabled = false;
                    btnResume.IsEnabled = false;
                    btnPause.IsEnabled = false;
                    currentState = ServiceState.Stopped;
                    break;

                case ServiceState.Running:
                    btnStart.IsEnabled = false;
                    btnStop.IsEnabled = true;
                    btnResume.IsEnabled = false;
                    btnPause.IsEnabled = true;
                    currentState = ServiceState.Running;
                    break;

                case ServiceState.Paused:
                    btnStart.IsEnabled = false;
                    btnStop.IsEnabled = true;
                    btnPause.IsEnabled = false;
                    btnResume.IsEnabled = true;
                    currentState = ServiceState.Paused;
                    break;

                default:
                    break;
            }
        }

        private void btnPing_Click(object sender, RoutedEventArgs e)
        {
            tagReaderServiceFramework.Ping();
        }

        private void chkDebug_Checked(object sender, RoutedEventArgs e)
        {
            if (tagReaderServiceFramework != null)
                tagReaderServiceFramework.DebugMode = (Boolean)chkDebug.IsChecked;
        }

        private void btnAutoLogout_Click(object sender, RoutedEventArgs e)
        {
            if (tagReaderServiceFramework != null)
            {
                //tagReaderServiceFramework.AutoLogout = true;
                //tagReaderServiceFramework.AutoLogoutTime = DateTime.Now.AddMinutes(5);
                tagReaderServiceFramework.TagReaders.AutoLogoutNow();
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtFeedback.Text = string.Empty;
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            if (tagReaderServiceFramework != null)
                tagReaderServiceFramework.TagReaders.ReloadData(true);
        }

    }
}
