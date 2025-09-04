using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ge_Mac.DataLayer;

namespace Ge_Mac.Settings
{
    public class AppUserSettings
    {
        private string userName;
        private string appName;

        public AppUserSettings()
        {
            userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().ToString();
        }





    }
}
