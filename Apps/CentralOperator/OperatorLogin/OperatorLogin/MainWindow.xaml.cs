using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using Ge_Mac.DataLayer;
using Ge_Mac.Settings;
using GraphicSurface;
using ErrorLogging;
using RFIDeas_pcProxAPI;

namespace OperatorLogin
{
     
    public partial class MainWindow : Window
    {
        #region properties + fields

        private ErrorLogger errorLogger;

        SqlDataAccess da;
        string connectionKey = @"default";
        private string culture = "en-GB";
        private string timeFormat = "HH:mm  dd MMMM yyyy";
        private CultureInfo cultureInfo;
        private string password = string.Empty;

        private bool allowMinimise = false;

        private bool allowExitBtn = true;

        private int loginTerminalID = 1;
        private int currentDiagID = -1;

        private double DatabaseVersion;
        bool initialising = true;

        private int resetCount = 0;
        private string lastCode = string.Empty;
        private bool muteBeep = false;
        private bool noReader = false;
        private bool readerRequired = true;

        private bool allowAutoLogout = false;

        private bool noDropdownSelection = false;
        private bool allowEdit = true;
        private bool canRead = true;

        private DateTime lastDBRead;

        private DateTime lastLoginOut;
        private int logInOutDelaymS = 1000;

        bool tagEdit = false;

        //lookup tables
        private Operators operators;
        private Machines machines;
        private MachineSubIDs machineSubIDs;

        //visio
        public VisioDiagrams visioDiagrams;
        public VisioBitmaps visioBitmaps;
        public VisioDiagramBitmaps visioDiagramBitmaps;
        public VisioFavourites visioFavourites;
        public VisioMachines visioMachines;
        public VisioStations visioStations;
        public VisioTexts visioTexts;

        public VisioShapes visioShapes;

        //logins
        public TagReaderLocations tagReaderLocations;
        public LoginTerminals loginTerminals;

        public LoginTerminal currentLoginTerminal;
        public LoginTerminalMachines loginTerminalMachines;

        private Tags tags;
        //private TagReaderLog tagReaderLog;
        
        DispatcherTimer readTimer;

        TagEditWindow tagEditWindow;

        private string version_trans = "Version";
        private string operator_trans = "Operator";
        private string tag_trans = "Tag";
        public string tagEditCaption_trans = "Caption";
        private string station_trans = "Station";
        private string exit_trans = "Close";
        private string update_trans = "Update";
        private string add_trans = "Add";
        private string refresh_trans = "Refresh";
        private string password_trans = "Password";
        private string apply_trans = "Store";
        private string tagdata_trans = "Tag Data";
        private string tagid_trans = "Tag ID";
        private string delete_trans = "Delete";
        private string ok_trans = "OK";
        private string cancel_trans = "Cancel";
        private string stop_trans = "Stop";
        private string login_trans = "Login";
        private string logout_trans = "Logout";
        private string connect_error_trans = "Connection Error";
        private string noReader_trans = "No RFID reader found";
        private string unknowntag_trans;
        private string tagnotassignedmsg_trans;
        private string exitWarning_trans;
        private string stationInUse_trans = "Cannot login, Station already in use";
        private string error_trans="Error";

        #endregion


        #region constructor and initialisation
        public MainWindow()
        {
            initialising = true;
            InitializeComponent();
            errorLogger = new ErrorLogger();
            ReadCommandLine();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                cultureInfo = new CultureInfo(culture);
                SqlDataConnection.ReadDbConfiguration(connectionKey);
                da = SqlDataAccess.Singleton;
                ReadSettings();
                ReadOperators();
                ReadMachines();
                ReadVisio();
                ReadLoginTables();
                da.CurrentCultureName = culture;
                btnMinimise.IsEnabled = allowMinimise;
                if (allowMinimise)
                {
                    btnMinimise.Visibility = Visibility.Visible;
                }
                else
                {
                    btnMinimise.Visibility = Visibility.Collapsed;
                }
                cbxOperators.IsEnabled = !noDropdownSelection;
                if (allowEdit)
                {
                    btnTags.Visibility = Visibility.Visible;
                }
                else
                {
                    btnTags.Visibility = Visibility.Collapsed;
                }
                if (allowExitBtn)
                {
                    btnExit.Visibility = Visibility.Visible;
                }
                else
                {
                    btnExit.Visibility = Visibility.Collapsed;
                }
                TranslateApp();
                Assembly assem = Assembly.GetEntryAssembly();
                AssemblyName assemName = assem.GetName();
                Version ver = assemName.Version;
                string versionString = string.Empty;

                lblVersion.Content = version_trans + " " + ver.ToString();

                readTimer = new DispatcherTimer();
                readTimer.Tick += new EventHandler(readTick);
                readTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);

                DisplayVisioFavourite();

                connect_RFIDeas();
                readTimer.Start();
                LabelTime.Content = DateTime.Now.ToString(timeFormat, cultureInfo);
                initialising = false;
            }
            catch (Exception ex)
            {
                LogError(1, "Window_Loaded", ex.Message);
            }
        }

        private void LogError(int severity, string ident, string message)
        {
            errorLogger.AddError(message, ident, severity);
            Debug.WriteLine(ident + ", " + severity.ToString() + ", " + message);
        }

        private void TranslateApp()
        {
            version_trans = da.Translate("About.Version");
            operator_trans = da.Translate("Operator");
            tag_trans = da.Translate("Tag");
            exit_trans = da.Translate("Exit");
            update_trans = da.Translate("Common.Update");
            add_trans = da.Translate("Common.Add");
            refresh_trans = da.Translate("Common.Refresh");
            password_trans = da.Translate("User_Set_Password");
            apply_trans = da.Translate("Common.Apply");
            tagdata_trans = da.Translate("Tag Data");
            tagid_trans = da.Translate("Tag ID");
            delete_trans = da.Translate("MenuEditDelete.Header");
            ok_trans = da.Translate("EditClose.Content");
            cancel_trans = da.Translate("EditCancel.Content");
            stop_trans = da.Translate("Stop");
            lblOperator.Content = operator_trans;
            station_trans = da.Translate("Station");
            lblStation.Content = station_trans;
            btnTags.Content = da.Translate("Tags");
            tagEditCaption_trans = da.Translate("Edit Operator Tags");
            login_trans = da.Translate("Form_Login");
            logout_trans = da.Translate("Form_Logout");
            connect_error_trans = da.Translate("Connection Error");
            noReader_trans = da.Translate("No RFID Reader Found");
            unknowntag_trans = da.Translate("Unknown Tag");
            tagnotassignedmsg_trans = da.Translate("Tag not assigned message");
            exitWarning_trans = da.Translate("Exit Application Now?");
            stationInUse_trans = da.Translate("Cannot login message");
            error_trans=da.Translate("Error");
        }

        private void ReadSettings()
        {
            try
            {
                ApplicationUserSettings appUserSettings = new ApplicationUserSettings();

                DatabaseVersion = appUserSettings.GetAppSettingDouble("DatabaseVersion");
                if (DatabaseVersion < 0) DatabaseVersion = 1.2;
                da.DatabaseVersion = DatabaseVersion;

                password = appUserSettings.GetAppSettingString("Password");
            }
            catch (Exception ex)
            {
                LogError(1, "ReadSettings", ex.Message);
            }
        }
        #endregion
 
        #region Reader Comms

        private void feedback(string aString)
        {
            Debug.WriteLine(aString);
        }

        private bool userFeedback(string aTitle, string feedback, int timeout, bool isOK)
        {
            UserFeedback uf = new UserFeedback();
            uf.ok_trans = ok_trans;
            uf.cancel_trans = cancel_trans;
            uf.stop_trans = stop_trans;
            uf.FeedbackCaption = aTitle;
            uf.Feedback = feedback;
            if (timeout > 0)
            {
                if (isOK)
                    uf.TimeOutMode = UserFeedback.TimeOutModeType.OK;
                else
                    uf.TimeOutMode = UserFeedback.TimeOutModeType.Cancel;
                uf.TimeOutSeconds = timeout;
            }
            else
                uf.TimeOutMode = UserFeedback.TimeOutModeType.None;
            return (bool)uf.ShowDialog();
        }

        const ushort ct_hitag1 = 0xF302;
        const ushort ct_HID = 0xEF04;
        const ushort ct_8104 = 0xFB01;

        private string getCardType(int cardType)
        {
            string aString = "Unknown";
            switch (cardType)
            {
                case ct_hitag1: aString = "Hitag1";
                    break;
                case ct_HID: aString = "HID";
                    break;
                case ct_8104: aString = "8104";
                    break;
                default: aString = "Unknown" + cardType.ToString();
                    break;
            }
            return aString;
        }

        private void connect_RFIDeas()
        {
            if (!noReader)
            {
                Cursor cur = this.Cursor;
                this.Cursor = Cursors.Wait;
                long DeviceID = 0;
                pcProxDLLAPI.USBDisconnect();
                int rc = pcProxDLLAPI.usbConnect();
                if (rc == 1)
                {
                    DeviceID = pcProxDLLAPI.GetDID();
                    feedback("Connected to DeviceID: " + DeviceID.ToString());
                    uint devType = pcProxDLLAPI.GetProduct();
                    if (devType == 1)
                        feedback("Device Name : pcProx");
                    int x = pcProxDLLAPI.GetCardType();
                    if (x > -1)
                    {
                        feedback("Card Type: (" + getCardType(x) + ")");
                    }
                    //readTimer.Start();
                }
                else
                {
                    feedback("No devices found to connect with");
                    string aTitle = connect_error_trans;
                    string aFeedback=noReader_trans;
                    if (readerRequired)
                        userFeedback(aTitle, aFeedback, 0, false);
                    noReader = true;
                }
                this.Cursor = cur;
            }
        }

        private void disconnect()
        {
            Cursor cur = this.Cursor;
            this.Cursor = Cursors.Wait;
            if (readTimer != null && readTimer.IsEnabled)
                readTimer.Stop();
            if (!noReader)
                pcProxDLLAPI.USBDisconnect();
            feedback("Disconnected");

            this.Cursor = cur;
        }
        private char ReverseHexChar(char hexChar)
        {
            char hex = 'X';
            switch (hexChar)
            {
                case '0': hex = '0';
                    break;
                case '1': hex = '8';
                    break;
                case '2': hex = '4';
                    break;
                case '3': hex = 'C';
                    break;
                case '4': hex = '2';
                    break;
                case '5': hex = 'A';
                    break;
                case '6': hex = '6';
                    break;
                case '7': hex = 'E';
                    break;
                case '8': hex = '1';
                    break;
                case '9': hex = '9';
                    break;
                case 'A': hex = '5';
                    break;
                case 'B': hex = 'D';
                    break;
                case 'C': hex = '3';
                    break;
                case 'D': hex = 'B';
                    break;
                case 'E': hex = '7';
                    break;
                case 'F': hex = 'F';
                    break;
                default:
                    hex = '0';
                    break;
            }
            return hex;
        }

        private string ReverseHexString(string hexString)
        {
            string aString = string.Empty;
            string hex = hexString.ToUpper();
            for (int i = 1; i < hexString.Length; i += 2)
            { 
                char c=ReverseHexChar(hex[i]);
                if ((c != 'X') && ((aString.Length > 0) || (c != '0')))
                {
                    aString += c;
                }
                c = ReverseHexChar(hex[i - 1]);
                if ((c != 'X') && ((aString.Length > 0) || (c != '0')))
                {
                    aString += c;
                }

            }
            return aString;
        }

        private void readRFIDdevice()
        {
            if (!noReader && canRead)
            {
                Byte[] Id = new Byte[8];
                int nBits = pcProxDLLAPI.getActiveID(8);

                if (nBits > 0)
                {
                    String tagdata = string.Empty;// nBits.ToString() + " Bit ID: ";
                    int limit = 8;
                    if (nBits <= 32)
                        limit = 4;
                    if (nBits > 32 && nBits <= 40)
                        limit = 6;
                    for (short i = 0; i < limit; i++)
                    {
                        Id[i] = pcProxDLLAPI.getActiveID_byte(i);
                        tagdata = tagdata + String.Format("{0:X2}", Id[i]);
                    }
                    if (tagdata != lastCode)
                    {
                        beep();
                        if (!tagEdit)
                        {
                            this.WindowState = WindowState.Maximized;
                            this.Topmost = true;  // important 
                            this.Topmost = false; // important 
                            this.Focus();         // important    
                        }                        
                        redBackground();
                        lastCode = tagdata;
                        if (tagEditWindow != null)
                        {
                            tagEditWindow.SetTagEdit(tagdata);
                        }
                        Tag tag = tags.GetByTag(tagdata);
                        if (tag != null)
                        {
                            resetCount = 0;

                            int opID = tag.ReferenceID;
                            Operator op = operators.GetById(opID);
                            if (op != null)
                            {
                                cbxOperators.SelectedItem = op;
                            }
                        }
                        else
                        {
                            if (!tagEdit)
                            {
                                errorBeep();
                                string aTitle = unknowntag_trans;
                                string aFeedback = tagnotassignedmsg_trans;
                                userFeedback(aTitle, aFeedback, 20, true);
                            }
                        }
                    }
                    feedback(tagdata);
                }
                else
                {
                    feedback("No_Tag ");
                    lastCode = string.Empty;
                }
            }
        }

        private void beep()
        {
            if (!muteBeep)
                pcProxDLLAPI.BeepNow(1, 0);
        }

        private void redBackground()
        {
            rectRed.Visibility = Visibility.Visible;
        }

        private void blueBackground()
        {
            rectRed.Visibility = Visibility.Collapsed;
        }

        private void errorBeep()
        {
            if (!muteBeep)
                pcProxDLLAPI.BeepNow(2, 1);
        }

        private void CheckDBRead()
        {
            TimeSpan ts = DateTime.Now.Subtract(lastDBRead);
            if (ts.TotalMinutes >= 10)
            {
                try
                {
                    ReloadData();
                }
                catch
                {
                    Debug.WriteLine("Failed Reload");
                }
            }
        }

        private void CheckSubIDsRead()
        {
            try
            {
                if (!machineSubIDs.IsValid)
                {
                    machineSubIDs = da.GetAllMachineSubIDs(true);
                    UpdateSurfaceContent();
                }
            }
            catch (Exception ex)
            {
                LogError(1, "CheckSubIDsRead", ex.Message);
            }
        }

        private void ResizeFonts()
        {
            try
            {
                Title = this.ActualWidth.ToString() + ", " + this.ActualHeight.ToString();
                lblOperator.FontSize = this.ActualHeight / 48;
                lblStation.FontSize = this.ActualHeight / 48;
                lblStnName.FontSize = this.ActualHeight / 48;
                btnTags.FontSize = this.ActualHeight / 48;
                lblTerminalName.FontSize = this.ActualHeight / 56;
                lbxTerminals.FontSize = this.ActualHeight / 56;
                LabelTime.FontSize = this.ActualHeight / 72;
                lblVersion.FontSize = this.ActualHeight / 72;
                cbxOperators.FontSize = this.ActualHeight / 40;
                btnExit.FontSize = this.ActualHeight / 40;
                btnMinimise.FontSize = this.ActualHeight / 36;
            }
            catch (Exception ex)
            {
                LogError(1, "ResizeFonts", ex.Message);
            }
        }

      
        private void readTick(object sender, EventArgs e)
        {
            try
            {
                if (resetCount >= 14)
                {
                    LabelTime.Content = DateTime.Now.ToString(timeFormat, cultureInfo);
                    cbxOperators.SelectedIndex = -1;
                    cbxOperators.SelectedItem = null;
                    resetCount = 0;
                    lblStnName.Content = string.Empty;
                    lblStation.Content = string.Empty;
                    CheckDBRead();
                }
                CheckSubIDsRead();
                blueBackground();
                readRFIDdevice();
                resetCount++;
            }
            catch (Exception ex)
            {
                LogError(1, "ReadTick", ex.Message);
            }
         }

        #endregion

        #region CommandLine

        private void ReadCommandLine()
        {
            string[] commandLine = Environment.GetCommandLineArgs();

            for (int i = 1; i < commandLine.Length; i++)
            {
                string arg = commandLine[i];
                string command;
                string param;

                if (arg.Length >= 3)
                {
                    command = arg.Substring(0, 2);
                    param = arg.Remove(0, 3);
                }
                else
                {
                    command = arg;
                    param = string.Empty;
                }
                try
                {
                    string aString;

                    switch (command.ToLower())
                    {
                        case "-l": //culture
                            culture = param;
                            break;
                        case "-b":
                            muteBeep = true;
                            break;
                        case "+b":
                            muteBeep = false;
                            break;                        
                        case "-c":
                            connectionKey = param;
                            break;
                        case "-d": //date/time format
                            timeFormat = param;
                            break;
                        case "-e":
                            allowEdit = false;
                            break;
                        case "+e":
                            allowEdit = true;
                            break;
                        case "-i": //log in out delay ms
                            aString = param;
                            int x = 1000;
                            if (int.TryParse(aString, out x))
                                logInOutDelaymS = x;
                            break;
                        case "-m":
                            allowMinimise = false;
                            break;
                        case "+m":
                            allowMinimise = true;
                            break;
                        case "-n":
                            noDropdownSelection = false;
                            break;
                        case "+n":
                            noDropdownSelection = true;
                            break;
                        case "-a":
                            allowAutoLogout = false;
                            break;
                        case "+a":
                            allowAutoLogout = true;
                            break;
                        case "-r":
                            readerRequired = false;
                            break;
                        case "+r":
                            readerRequired = true;
                            break;
                        case "-t":
                            int t = 1;
                            aString = param;
                            if (int.TryParse(aString, out t))
                                loginTerminalID = t;
                            break;
                        case "-w": //no frame
                            this.WindowStyle = WindowStyle.None;
                            //this.Cursor = Cursors.None;
                            break;
                        case "+w": //full frame
                            this.WindowStyle = WindowStyle.SingleBorderWindow;
                            this.Title = param;
                            break;
                        case "-x":
                            allowExitBtn = false;
                            break;
                        case "+x":
                            allowExitBtn = true;
                            break;
                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    LogError(1, "ReadCommandLine", ex.Message);
                }
            }
        }

        private void DisplayCommandLine()
        {
            string commandlineInstructions = "Command Line for Operator Login" + Environment.NewLine + Environment.NewLine +
                                             "  -c  Connection key, eg \"-c JEGR_Server\"" + Environment.NewLine;
            MessageBox.Show(this, commandlineInstructions);
        }

        #endregion

        # region Database Read

        private void ReadOperators()
        {
            cbxOperators.ItemsSource = null;
            cbxOperators.Items.Clear();
            operators = da.GetAllActiveOperators(true);
            operators.Sort();
            cbxOperators.ItemsSource = operators;
            cbxOperators.DisplayMemberPath = "ShortDescription";
            cbxOperators.IsReadOnly = true;
        }

        private void ReadMachines()
        {
            machines = da.GetAllMachines();
            machineSubIDs = da.GetAllMachineSubIDs();
        }

        private void ReadVisio()
        {
            visioDiagrams = da.GetAllVisioDiagrams();
            visioBitmaps = da.GetAllVisioBitmaps();
            visioDiagramBitmaps = da.GetAllVisioDiagramBitmaps();
            visioFavourites = da.GetAllVisioFavourites(true);
            visioMachines = da.GetAllVisioMachines(true);
            visioStations = da.GetAllVisioStations(true);
            visioTexts = da.GetAllVisioTexts(true);
        }

        private void ReadLoginTables()
        {
            loginTerminals = da.GetAllLoginTerminals();
            currentLoginTerminal = loginTerminals.GetById(loginTerminalID);
            lblTerminalName.Content = currentLoginTerminal.TerminalName;

            lbxTerminals.ItemsSource = loginTerminals;
            lbxTerminals.DisplayMemberPath = "TerminalName";
            lbxTerminals.SelectedItem = currentLoginTerminal;

            loginTerminalMachines = da.GetAllLoginTerminalMachines();//defunct now?
            tagReaderLocations = da.GetAllTagReaderLocations();
            tags = da.GetAllTags();

            lblStnName.Content = string.Empty;
        }

        private void UpdateSurfaceMachine(VisioMachine vm, Brush fillBrush, bool refresh)
        {
            if (vm != null)
            {
                if (vm.DiagramID == currentDiagID)
                {
                    string mName = "Machine" + vm.VisioMachineID.ToString();
                    VisioItem visioItem = surface.FindVisioItem(mName);
                    if (visioItem == null)
                    {
                        visioItem = new VisioItem();
                        visioItem.ItemName = mName;
                        visioItem.ItemID = vm.VisioMachineID;
                        surface.VisioItems.Add(visioItem);
                    }
                    visioItem.ItemType = 1;
                    visioItem.ShapeID = vm.MachineShape;
                    visioItem.X = vm.X;
                    visioItem.Y = vm.Y;
                    visioItem.Width = vm.Width;
                    visioItem.Height = vm.Height;
                    visioItem.Z = 2;
                    visioItem.Fillbrush = fillBrush;
                    if (refresh)
                        surface.Refresh();
                }
            }
        }

        private void UpdateSurfaceStation(VisioStation vStn, Brush fillBrush, bool refresh)
        {
            if (vStn != null)
            {
                if (vStn.DiagramID == currentDiagID)
                {
                    string sName = "Station" + vStn.StationID.ToString();
                    VisioItem visioItem = surface.FindVisioItem(sName);
                    if (visioItem == null)
                    {
                        visioItem = new VisioItem();
                        visioItem.ItemName = sName;
                        visioItem.ItemID = vStn.StationID;
                        surface.VisioItems.Add(visioItem);
                    }
                    visioItem.ItemType = 1;
                    visioItem.ShapeID = 0;
                    visioItem.X = vStn.X;
                    visioItem.Y = vStn.Y;
                    visioItem.Width = vStn.Width;
                    visioItem.Height = vStn.Height;
                    visioItem.Z = 3;
                    visioItem.Fillbrush = fillBrush;
                    if (refresh)
                        surface.Refresh();
                }
            }
        }

        private void UpdateSurfaceText(VisioText vT, bool refresh)
        {
            if (vT != null)
            {
                if (vT.DiagramID == currentDiagID)
                {
                    string tName = "Text" + vT.VisioTextID.ToString();
                    VisioItem visioItem = surface.FindVisioItem(tName);
                    if (visioItem == null)
                    {
                        visioItem = new VisioItem();
                        visioItem.ItemName = tName;
                        visioItem.ItemID = vT.VisioTextID;
                        surface.VisioItems.Add(visioItem);
                    }
                    visioItem.ItemType = 2;
                    visioItem.Text = vT.StaticText;
                    visioItem.Text = vT.StaticText;
                    visioItem.X = vT.X;
                    visioItem.Y = vT.Y;
                    visioItem.Width = vT.Width;
                    visioItem.Height = vT.Height;
                    visioItem.Z = 2;
                    visioItem.Fillbrush = Brushes.DarkGreen;
                    if (refresh)
                        surface.Refresh();
                }
            }
        }

        private void UpdateSurfaceContent()
        {
            if (visioDiagrams != null)
            {
                if (visioFavourites != null)
                {
                    VisioFavourite vf = visioFavourites.GetById(currentLoginTerminal.FavouriteID);
                    if (vf != null)
                    {
                        if (visioMachines != null)
                        {
                            foreach (VisioMachine vm in visioMachines)
                            {
                                if (vf.DiagramID == vm.DiagramID)
                                {
                                    UpdateSurfaceMachine(vm, Brushes.DarkGreen, false);
                                }
                            }
                        }
                        if (visioStations != null)
                        {
                            foreach (VisioStation vStn in visioStations)
                            {
                                if (vf.DiagramID == vStn.DiagramID)
                                {
                                    Brush fillBrush = Brushes.LightBlue;
                                    MachineSubID msub = machineSubIDs.GetByMachineSubID(vStn.MachineID, vStn.SubID);
                                    if (msub != null)
                                    {
                                        if (msub.Operator_Remote >= 0)
                                        {
                                            fillBrush = Brushes.Orange;
                                            Operator loginOperator = (Operator)cbxOperators.SelectedItem;
                                            if (loginOperator != null)
                                            {
                                                if (loginOperator.idJensen == msub.Operator_Remote)
                                                    fillBrush = Brushes.Red;
                                            }
                                        }
                                    }
                                    UpdateSurfaceStation(vStn, fillBrush, false);
                                }
                            }
                        }
                        if (visioTexts != null)
                        {
                            foreach (VisioText vT in visioTexts)
                            {
                                if (vf.DiagramID == vT.DiagramID)
                                {
                                    UpdateSurfaceText(vT, false);
                                }
                            }
                        }
                    }
                }
            }
            surface.Refresh();
        }

        # endregion

        # region methods

        private void DisplayVisioFavourite()
        {
            try
            {
                if (currentLoginTerminal != null)
                {
                    VisioFavourite vf = visioFavourites.GetById(currentLoginTerminal.FavouriteID);
                    if (vf != null)
                    {
                        VisioDiagram vdiag = visioDiagrams.GetById(vf.DiagramID);
                        if (vdiag != null)
                        {
                            currentDiagID = vf.DiagramID;
                            VisioDiagramBitmap diagbmp = visioDiagramBitmaps.GetByDiagIdZ(vf.DiagramID, 0);
                            if (diagbmp != null)
                            {
                                VisioBitmap vbmp = visioBitmaps.GetById(diagbmp.BitmapID);
                                if (vbmp != null)
                                {
                                    if (vbmp.DiagBmpSource != null)
                                    {
                                        surface.SetBackground(vbmp.DiagBmpSource, vbmp.DiagBmpSource.PixelWidth, vbmp.DiagBmpSource.PixelHeight, vdiag.Width, vdiag.Height);
                                    }
                                }
                                Point tl = new Point(vf.XMin, vf.YMin);
                                Point br = new Point(vf.XMax, vf.YMax);
                                surface.ZoomToLimits(tl, br);
                            }
                        }
                    }
                }
                UpdateSurfaceContent();
            }
            catch (Exception ex)
            {
                LogError(1, "DisplayVisioFavourite", ex.Message);
            }
        }

        private void ResizeVisio()
        {
            try
            {
                if (currentLoginTerminal != null)
                {
                    VisioFavourite vf = visioFavourites.GetById(currentLoginTerminal.FavouriteID);
                    if (vf != null)
                    {
                        VisioDiagram vdiag = visioDiagrams.GetById(vf.DiagramID);
                        if (vdiag != null)
                        {
                            VisioDiagramBitmap diagbmp = visioDiagramBitmaps.GetByDiagIdZ(vf.DiagramID, 0);
                            if (diagbmp != null)
                            {
                                VisioBitmap vbmp = visioBitmaps.GetById(diagbmp.BitmapID);
                                if (vbmp != null)
                                {
                                    Point tl = new Point(vf.XMin, vf.YMin);
                                    Point br = new Point(vf.XMax, vf.YMax);
                                    surface.ZoomToLimits(tl, br);
                                }
                            }
                        }
                    }
                }
                UpdateSurfaceContent();
            }
            catch (Exception ex)
            {
                LogError(1, "ResizeVisio", ex.Message);
            }
        }

        private bool promptConfirm(Operator loginOperator, Machine machine, int subid)
        {
            string feedbackString = string.Empty;
            bool elsewhere = false;
            bool others = false;
            if ((loginOperator != null) && (machine != null) && (subid >= 0))
            {
                //is this operator logged in anywhere?
                MachineSubIDs msubs = machineSubIDs.GetByRemoteOperator(loginOperator.idJensen);
                if (msubs.Count > 0)
                {
                    foreach (MachineSubID ms in msubs)
                    {
                        if ((ms.Operator_Remote == loginOperator.idJensen) && ((ms.Machine_idJensen != machine.idJensen)
                            || (ms.SubID != subid)))
                        {
                            elsewhere = true;
                            Machine m = machines.GetById(ms.Machine_idJensen);
                            if (m != null)
                            {
                                feedbackString += logout_trans + " " + loginOperator.ShortDescAndID + ", " + m.ShortDescAndID + ", "
                                    + ms.SubID.ToString() + Environment.NewLine;
                            }
                        }
                    }
                }
                //is it in or out?
                bool itsLogIn = true;
                MachineSubID msub = machineSubIDs.GetByMachineSubID(machine.idJensen, subid);
                if (msub != null)
                {
                    itsLogIn = msub.Operator_Remote != loginOperator.idJensen;
                }

                //is anyone else logged in?
                if (itsLogIn)
                {
                    if (msub != null)
                    {
                        if (msub.Operator_Remote >= 0)
                        {
                            others = true;
                            Operator op = operators.GetById(msub.Operator_Remote);
                            if (op != null)
                            {
                                feedbackString += logout_trans + " " + op.ShortDescAndID + ", " + machine.ShortDescAndID + ", " + subid.ToString() +
                                   Environment.NewLine;
                            }
                        }
                    }
                }

                if (itsLogIn)
                {
                    feedbackString += login_trans + " " + loginOperator.ShortDescAndID + ", " + machine.ShortDescAndID + ", " + subid.ToString() +
                        Environment.NewLine;
                }
            }
            if (others && !allowAutoLogout)
            {
                feedbackString = stationInUse_trans;
                userFeedback(error_trans, feedbackString, 10, true);
                return false;
            }
            else
            {
                if (others || elsewhere)
                {
                    canRead = false;
                    int timeout = 5;
                    if (others)
                        timeout = 10;

                    string aTitle = login_trans + " / " + logout_trans;
                    bool doit = userFeedback(aTitle, feedbackString, timeout, !others);
                    canRead = true;
                    return doit;
                }
                else
                    return true;
            }
        }

        private bool logInoutTimeoutOK()
        {
            return (lastLoginOut.AddMilliseconds(logInOutDelaymS) < DateTime.Now);
        }

        private void LogInOut(int machineID, int subid)
        {
            try
            {
                machineSubIDs = da.GetAllMachineSubIDs(true);
                //which operator
                Operator loginOperator = (Operator)cbxOperators.SelectedItem;

                //which machine
                Machine machine = machines.GetById(machineID);

                if ((loginOperator != null) && (loginOperator.idJensen>-1) && (machine != null) && (subid >= 0))
                {
                    if (promptConfirm(loginOperator, machine, subid))
                    {
                        //is it in or out?
                        bool itsLogIn = true;
                        MachineSubID msub = machineSubIDs.GetByMachineSubID(machine.idJensen, subid);
                        if (msub != null)
                        {
                            itsLogIn = msub.Operator_Remote != loginOperator.idJensen;
                        }
                        //is this operator logged in anywhere?
                        MachineSubIDs msubs = machineSubIDs.GetByRemoteOperator(loginOperator.idJensen);
                        if (msubs.Count > 0)
                        {
                            //if so log them out
                            foreach (MachineSubID ms in msubs)
                            {
                                LogDataLogInOut(ms.Machine_idJensen, ms.SubID, ms.Operator_Remote, 0);
                                ms.Operator_idJensen = -1;
                                ms.Operator_Remote = -1;
                                UpdateMachineSubid(ms);
                            }
                        }
                        if (itsLogIn)
                        {
                            if (msub != null)
                            {
                                if (msub.Operator_Remote >= 0) //someone else is logged in - log them out!
                                    LogDataLogInOut(msub.Machine_idJensen, msub.SubID, msub.Operator_Remote, 0);

                                msub.Operator_idJensen = -1;
                                msub.Operator_Remote = loginOperator.idJensen;
                                UpdateMachineSubid(msub);
                                LogDataLogInOut(msub.Machine_idJensen, msub.SubID, msub.Operator_Remote, 1);
                            }
                            else
                            {
                                msub = new MachineSubID();
                                msub.RecNum = -1;
                                msub.Machine_idJensen = machine.idJensen;
                                msub.SubID = subid;
                                msub.Operator_idJensen = -1;
                                msub.Customer_idJensen = -1;
                                msub.Article_idJensen = -1;
                                msub.Operator_Remote = loginOperator.idJensen;
                                machineSubIDs.Add(msub);
                                UpdateMachineSubid(msub);
                                LogDataLogInOut(msub.Machine_idJensen, msub.SubID, msub.Operator_Remote, 1);
                            }
                        }
                    }
                    machineSubIDs = da.GetAllMachineSubIDs(true);
                    UpdateSurfaceContent();
                    lastLoginOut = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                LogError(1, "LogInOut", ex.Message);
            }

        }

        private void UpdateMachineSubid(MachineSubID machineSubID)
        {
            try
            {
                SqlDataAccess da = SqlDataAccess.Singleton;
                if (machineSubID.RecNum == -1)
                {
                    da.InsertNewMachineSubID(machineSubID);
                }
                else
                {
                    da.UpdateMachineSubID(machineSubID);
                }
            }
            catch (Exception ex)
            {
                LogError(1, "UpdateMachineSubid", ex.Message);
            }
        }

        private void LogDataLogInOut(int machineID, int subID, int operatorID, int state)
        {
            try
            {
                LogError(10, "Machine: " + machineID.ToString() + ", SubID: " + subID.ToString() 
                    + ", OperatorID: " + operatorID.ToString() + ", State " + state.ToString(), "Operator Logging");
                SqlDataAccess da=SqlDataAccess.Singleton;
                da.GetServerTime();
                RemoteOperatorLog remoteOperatorLog=new RemoteOperatorLog();
                remoteOperatorLog.RemoteOperatorLogID=-1;
                remoteOperatorLog.LogTime=da.ServerTime;
                remoteOperatorLog.MachineID = machineID;
                remoteOperatorLog.OperatorID = operatorID;
                remoteOperatorLog.SubID = subID;
                remoteOperatorLog.State=state;
                da.InsertRemoteOperatorLog(remoteOperatorLog);
            }
            catch (Exception ex)
            {
                LogError(1, "LogDataLogInOut", ex.Message);
            }
            try
            {
                JGLogDataRec ldr = new JGLogDataRec();
                ldr.RecNum = -1;
                ldr.RemoteID = -1;
                ldr.CompanyID = 99;
                ldr.TimeStamp = DateTime.MinValue;
                ldr.MachineID = machineID;
                ldr.PositionID = -1;
                ldr.SubID = subID;
                ldr.SubIDName = string.Empty;
                ldr.RegType = 6;
                ldr.SubRegType = 0;
                ldr.SubRegTypeID = 1;
                ldr.State = state;
                ldr.MessageA = "Operator";
                ldr.MessageB = "Operator";
                ldr.BatchID = -1;
                ldr.SourceID = -1;
                ldr.ProcessCode = -1;
                ldr.ProcessName = string.Empty;
                ldr.CustNo = -1;
                ldr.SortCategoryID = -1;
                ldr.ArtNo = -1;
                ldr.OperatorNo = operatorID;
                ldr.Value = 0;
                ldr.Unit = -1;
                SqlDataAccess da = SqlDataAccess.Singleton;
                da.InsertNewLogSP(ldr);
            }
            catch (Exception ex)
            {
                LogError(1, "LogDataLogInOut", ex.Message);
            }
        }

        private void IdentDisplayItem(VisioItem item)
        {
            try
            {
                if (item != null)
                {
                    if (item.ItemName.Contains("Station"))
                    {
                        VisioStation vStn = visioStations.GetById(item.ItemID);
                        if (vStn != null)
                        {
                            Machine m = machines.GetById(vStn.MachineID);
                            if (m != null)
                            {
                                if (cbxOperators.SelectedItem == null)
                                {
                                    MachineSubID msub = machineSubIDs.GetByMachineSubID(vStn.MachineID, vStn.SubID);
                                    if (msub != null)
                                    {
                                        if (msub.Operator_Remote >= 0)
                                        {
                                            Operator op = operators.GetById(msub.Operator_Remote);
                                            lblStation.Content = operator_trans;
                                            lblStnName.Content = op.ShortDescription;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(1, "IdentDisplayItem", ex.Message);
            }
        }

        private void UnselectItems()
        {
            lblStation.Content = string.Empty;
            lblStnName.Content = string.Empty;
            cbxOperators.SelectedItem = null;
            cbxOperators.SelectedIndex = -1;
        }

        private void SelectDisplayItem(VisioItem item)
        {
            try
            {
                if (item != null)
                {
                    if (item.ItemName.Contains("Station"))
                    {
                        VisioStation vStn = visioStations.GetById(item.ItemID);
                        if (vStn != null)
                        {
                            Machine m = machines.GetById(vStn.MachineID);
                            if (m != null)
                            {
                                if (cbxOperators.SelectedItem != null)
                                {
                                    lblStation.Content = station_trans;
                                    lblStnName.Content = m.ShortDescAndID + " / " + vStn.SubID.ToString();
                                    LogInOut(vStn.MachineID, vStn.SubID);
                                    resetCount = 0;
                                }
                                else
                                {
                                    MachineSubID msub = machineSubIDs.GetByMachineSubID(vStn.MachineID, vStn.SubID);
                                    if (msub != null)
                                    {
                                        if (msub.Operator_Remote >= 0)
                                        {
                                            Operator op = operators.GetById(msub.Operator_Remote);
                                            lblStation.Content = operator_trans;
                                            lblStnName.Content = op.ShortDescription;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(1, "SelectDisplayItem", ex.Message);
            }
        }

        private void ReloadData()
        {
            try
            {
                cbxOperators.IsDropDownOpen = false;
                lastDBRead = DateTime.Now;
                surface.VisioItems.Clear();
                ReadSettings();
                ReadOperators();
                ReadMachines();
                ReadVisio();
                ReadLoginTables();
                DisplayVisioFavourite();
            }
            catch (Exception ex)
            {
                LogError(1, "ReloadData", ex.Message);
            }
        }

        #endregion

        #region events

        private void rectEvents_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void rectEvents_MouseMove(object sender, MouseEventArgs e)
        {
            if (cbxOperators.SelectedIndex < 0)
            {
                try
                {
                    Point thisPoint = surface.CanvasPoint();
                    VisioItem vi = surface.HitTest(thisPoint);
                    if (vi != null)
                        IdentDisplayItem(vi);
                    else
                        UnselectItems();
                    e.Handled = true;
                }
                catch (Exception ex)
                {
                    LogError(1, "rectEvents_MouseMove", ex.Message);
                }
            }
        }

        private void rectEvents_TouchMove(object sender, TouchEventArgs e)
        {
            if (cbxOperators.SelectedIndex < 0)
            {
                try
                {
                    Point thisPoint = surface.CanvasPoint();
                    VisioItem vi = surface.HitTest(thisPoint);
                    if (vi != null)
                        IdentDisplayItem(vi);
                    else
                        UnselectItems();
                    e.Handled = true;
                }
                catch (Exception ex)
                {
                    LogError(1, "rectEvents_TouchMove", ex.Message);
                }
            }
        }

        private void rectEvents_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (logInoutTimeoutOK())
            {
                try
                {
                    if (e.MiddleButton == MouseButtonState.Pressed)
                    {
                        //surface.PanPoint = Mouse.GetPosition(rectEvents);
                    }
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Point thisPoint = surface.CanvasPoint();
                        VisioItem vi = surface.HitTest(thisPoint);
                        if (vi != null)
                            SelectDisplayItem(vi);
                        else
                            UnselectItems();
                        e.Handled = true;
                        //int x = 0; //debug code
                        //int y = 1;
                        //int z = y / x; //division by zero error!               
                    }
                }
                catch (Exception ex)
                {
                    LogError(1, "rectEvents_MouseDown", ex.Message);
                }                
            }
        }

        private void rectEvents_TouchDown(object sender, TouchEventArgs e)
        {
            if (logInoutTimeoutOK())
            {
                try
                {
                    Point thisPoint = surface.CanvasPoint();
                    VisioItem vi = surface.HitTest(thisPoint);
                    if (vi != null)
                        SelectDisplayItem(vi);
                    else
                        UnselectItems();
                    e.Handled = true;
                }
                catch (Exception ex)
                {
                    LogError(1, "rectEvents_TouchDown", ex.Message);
                }
            }
        }
        
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeFonts();
            if (!initialising)
                ResizeVisio();
        }

        private void cbxOperators_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSurfaceContent();
            //lblMachineName.Content = string.Empty;
            lblStnName.Content = string.Empty;
            lblStation.Content = string.Empty;
            resetCount = 0;
        }

        private void btnTags_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tagEditWindow = new TagEditWindow();
                tagEditWindow.Password = password;
                tagEditWindow.SetOperators(operators);
                tagEditWindow.tags = tags;
                tagEditWindow.EditCaption_trans = tagEditCaption_trans;
                tagEditWindow.tag_trans = tag_trans;
                tagEditWindow.operator_trans = operator_trans;
                tagEditWindow.close_trans = exit_trans;
                tagEditWindow.update_trans = update_trans;
                tagEditWindow.add_trans = add_trans;
                tagEditWindow.refresh_trans = refresh_trans;
                tagEditWindow.password_trans = password_trans;
                tagEditWindow.store_trans = apply_trans;
                tagEditWindow.tagdata_trans = tagdata_trans;
                tagEditWindow.tagid_trans = tagid_trans;
                tagEditWindow.delete_trans = delete_trans;
                tagEdit = true;
                tagEditWindow.ShowDialog();
                ReloadData();
                tagEdit = false;
                cbxOperators.Focus();
            }
            catch (Exception ex)
            {
                LogError(1, "btnTags_Click", ex.Message);
            }
        }

 
        private void cbxLoginTerminal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!initialising)
            {
                //currentLoginTerminal = (LoginTerminal)cbxLoginTerminal.SelectedItem;
                //DisplayVisioFavourite();
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnMinimise_Click(object sender, RoutedEventArgs e)
        {
            cbxOperators.Focus();
            this.WindowState = WindowState.Minimized;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (userFeedback(exit_trans, exitWarning_trans, 10, false))
            {
                disconnect();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void lblOperator_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ////test event
            //UserFeedback uf = new UserFeedback();
            //uf.FeedbackCaption = "Hello this is a title";
            //uf.Feedback = "Its all gone horribly wrong";
            //uf.TimeOutMode = UserFeedback.TimeOutModeType.OK;
            //uf.TimeOutSeconds = 10;
            //uf.ShowDialog();
        }

        private void lblTerminalName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lbxTerminals.Visibility = Visibility.Visible;
        }


        private void lbxTerminals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxTerminals.SelectedItem != null)
            {
                LoginTerminal lt = (LoginTerminal)lbxTerminals.SelectedItem;
                loginTerminalID = lt.LoginTerminalID;
                currentLoginTerminal = loginTerminals.GetById(loginTerminalID);
                ReloadData();
            }
            lbxTerminals.Visibility = Visibility.Collapsed;
        }



        #endregion

        private void cbxOperators_DropDownOpened(object sender, EventArgs e)
        {
            resetCount = 0;
        }



    }
}
