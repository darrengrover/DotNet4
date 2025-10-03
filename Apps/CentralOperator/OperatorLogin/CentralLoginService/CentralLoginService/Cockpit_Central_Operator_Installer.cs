using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.ServiceProcess;


namespace CentralLoginService
{
    [RunInstaller(true)]
    public partial class Cockpit_Central_Operator_Installer : Installer
    {
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller serviceProcessInstaller;
        public Cockpit_Central_Operator_Installer()
        {
            InitializeComponent();
            serviceProcessInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            //# Service Account Information

            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            //# Service Information
            serviceInstaller.StartType = ServiceStartMode.Automatic;          
            serviceInstaller.Description = "Interfaces with RFIdeas card readers";

            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
        protected override void OnBeforeInstall(IDictionary savedState)
        {
            base.OnBeforeInstall(savedState);
            ConfigureService();
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);
            ConfigureService();
        }
        private void ConfigureService()
        {
            // Get parameters from custom action
            string serviceName = Context.Parameters["servicename"];
            string useShiftManagement = Context.Parameters["useshiftmanagement"];

            // Use defaults if not provided
            if (string.IsNullOrEmpty(serviceName))
            {
                serviceName = "Cockpit_Central_Operator";
            }

            if (string.IsNullOrEmpty(useShiftManagement))
            {
                useShiftManagement = "false";
            }

            // Update service installer
            serviceInstaller.ServiceName = serviceName;
            serviceInstaller.DisplayName = serviceName.Replace("_", " ");

            // Save settings to config file
            try
            {
                string targetDir = Context.Parameters["targetdir"];
                if (!string.IsNullOrEmpty(targetDir))
                {
                    UpdateConfigFile(targetDir, serviceName, useShiftManagement);
                }
            }
            catch (Exception ex)
            {
                Context.LogMessage($"Warning: Could not update config file: {ex.Message}");
            }
        }

        private void UpdateConfigFile(string targetDir, string serviceName, string useShiftManagement)
        {
            string configPath = Path.Combine(targetDir, "CentralLoginService.exe.config");

            if (File.Exists(configPath))
            {
                var doc = new System.Xml.XmlDocument();
                doc.Load(configPath);

                // Update appSettings
                var appSettings = doc.SelectSingleNode("//appSettings");
                if (appSettings != null)
                {
                    UpdateOrAddSetting(doc, appSettings, "ServiceName", serviceName);
                    UpdateOrAddSetting(doc, appSettings, "UseShiftManagement", useShiftManagement);
                }

                doc.Save(configPath);
            }
        }

        private void UpdateOrAddSetting(System.Xml.XmlDocument doc, System.Xml.XmlNode appSettings, string key, string value)
        {
            var setting = appSettings.SelectSingleNode($"add[@key='{key}']");
            if (setting != null)
            {
                setting.Attributes["value"].Value = value;
            }
            else
            {
                var newSetting = doc.CreateElement("add");
                newSetting.SetAttribute("key", key);
                newSetting.SetAttribute("value", value);
                appSettings.AppendChild(newSetting);
            }
        }
    }
}
