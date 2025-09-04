using System;
using System.Diagnostics;
using System.Windows.Forms;
using VBAS = Microsoft.VisualBasic.ApplicationServices;
using System.IO;

namespace Ge_Mac.LoggingAndExceptions
{
    public static class ExceptionHandler
    {
        public static void Handle(Exception ex)
        {
            Show(ex);

            Log(ex);
        }

        /// <summary>
        /// Writes the Exception to the Trace window
        /// </summary>
        /// <param name="ex"></param>
        public static void Show(Exception ex)
        {
            const string exFormat =
                "RAIL:============================================================================" +
                "\r\nError: {0}\r\n" +
                "=================================================================================" +
                "\r\nTarget: {3}\r\nSource: {1}\r\n\r\nStackTrace:\r\n{2}\r\n\r\nInnerException:\r\n{4}";
            string exMessage = string.Format(exFormat, ex.Message, ex.Source, ex.StackTrace, ex.TargetSite, ex.InnerException);
            Trace.WriteLine(exMessage);
        }

        public static void Log(Exception logex)
        {
            string dirpath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),  "AppLog");

            try
            {
                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            string filename = Path.Combine(dirpath, "ExplorerLog " + DateTime.Today.ToString("yy-MM-dd") + ".txt");

            TextWriter tw = null;

            try
            {
                // create a writer and open the file
                tw = new StreamWriter(filename, true);

                const string exFormat = "Error: {0}; Target: {3}; Source: {1}; StackTrace: {2}";
                string exMessage = string.Format(exFormat, logex.Message, logex.Source, logex.StackTrace, logex.TargetSite);

                // write a line of text to the file
                tw.WriteLine(exMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                tw.Close();
            }
        }

        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;

                Handle(ex);

                MessageBox.Show("A serious error has occured, the application will now close.\r\n"
                    + "If this continues to happen please contact support with the following information:\n\n" +
                      ex.Message + ex.StackTrace,
                      "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            finally
            {
                Application.Exit();
            }
        }
    }
}
