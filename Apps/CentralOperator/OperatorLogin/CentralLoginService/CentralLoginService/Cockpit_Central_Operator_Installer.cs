using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Xml;


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

            // Get service name from parameters
            string serviceName = Context.Parameters["servicename"];
            if (string.IsNullOrEmpty(serviceName))
            {
                serviceName = "Cockpit_Central_Operator";
            }

            // Update service installer with the service name
            serviceInstaller.ServiceName = serviceName;
            serviceInstaller.DisplayName = serviceName.Replace("_", " ");

            // Save the service name so we can use it in Commit phase
            savedState["servicename"] = serviceName;
            savedState["useshiftmanagement"] = Context.Parameters["useshiftmanagement"] ?? "false";
        }

        protected override void OnCommitted(IDictionary savedState)
        {
            base.OnCommitted(savedState);

            // NOW update the config file - all files have been deployed
            try
            {
                string targetDir = Context.Parameters["targetdir"];
                string serviceName = savedState["servicename"] as string;
                string useShiftManagement = savedState["useshiftmanagement"] as string;

                if (!string.IsNullOrEmpty(targetDir))
                {
                    UpdateConfigFile(targetDir, serviceName, useShiftManagement);
                }
            }
            catch (Exception ex)
            {
                // Log but don't fail the installation
                Context.LogMessage($"Warning: Could not update config file: {ex.Message}");
            }
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            base.OnBeforeUninstall(savedState);

            // Use service name from parameters for uninstall
            string serviceName = Context.Parameters["servicename"];
            if (string.IsNullOrEmpty(serviceName))
            {
                serviceName = "Cockpit_Central_Operator";
            }

            serviceInstaller.ServiceName = serviceName;
        }

        private void UpdateConfigFile(string targetDir, string serviceName, string useShiftManagement)
        {
            string configPath = Path.Combine(targetDir, "CentralLoginService.exe.config");

            if (!File.Exists(configPath))
            {
                Context.LogMessage($"Config file not found at: {configPath}");
                return;
            }

            var doc = new XmlDocument();
            doc.Load(configPath);

            // Update applicationSettings for both ServiceName and UseShiftManagement
            UpdateApplicationSetting(doc, "ServiceName", serviceName);
            UpdateApplicationSetting(doc, "UseShiftManagement", useShiftManagement);

            doc.Save(configPath);
            Context.LogMessage($"Updated config file: ServiceName={serviceName}, UseShiftManagement={useShiftManagement}");
        }

        private void UpdateApplicationSetting(XmlDocument doc, string settingName, string value)
        {
            // Navigate to applicationSettings/CentralLoginService.Properties.Settings
            var appSettings = doc.SelectSingleNode("//applicationSettings/CentralLoginService.Properties.Settings");

            if (appSettings == null)
            {
                Context.LogMessage($"Warning: applicationSettings section not found in config file");
                return;
            }

            // Look for existing setting
            var setting = appSettings.SelectSingleNode($"setting[@name='{settingName}']");

            if (setting != null)
            {
                // Update existing setting value
                var valueNode = setting.SelectSingleNode("value");
                if (valueNode != null)
                {
                    valueNode.InnerText = value;
                }
            }
            else
            {
                // Create new setting element
                var newSetting = doc.CreateElement("setting");
                newSetting.SetAttribute("name", settingName);
                newSetting.SetAttribute("serializeAs", "String");

                var valueElement = doc.CreateElement("value");
                valueElement.InnerText = value;

                newSetting.AppendChild(valueElement);
                appSettings.AppendChild(newSetting);
            }
        }
    }
}