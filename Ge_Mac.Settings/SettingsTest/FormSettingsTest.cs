using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ge_Mac.DataLayer;
using Ge_Mac.Settings;

namespace SettingsTest
{
    public partial class FormSettingsTest : Form
    {
        ApplicationUserSettings aus;

        public FormSettingsTest()
        {
            InitializeComponent();
            SqlDataConnection.ReadDbConfiguration("laptop");
            aus = new ApplicationUserSettings();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text += aus.AppName + Environment.NewLine;
            textBox1.Text += aus.UserName + Environment.NewLine;
            textBox1.Text += aus.GetAppSettingString("UICulture") + Environment.NewLine; ;
            textBox1.Text += aus.GetAppUserSettingString("UICulture") + Environment.NewLine; ;
            textBox1.Text += aus.GetAppUserSettingString("settingstest", "ge-mac/gcailes", "UICulture") + Environment.NewLine; ;
            textBox1.Text += aus.GetAppUserSettingString("settingstest", "ge-mac/dgrover", "UICulture") + Environment.NewLine; ;
        }
    }
}
