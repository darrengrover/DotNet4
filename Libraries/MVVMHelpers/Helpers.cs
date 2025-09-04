using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;


namespace MVVMHelpers
{

    public static class PropertySupport
    {
        public static String ExtractPropertyName<T>(Expression<Func<T>> propertyExpresssion)
        {
            if (propertyExpresssion == null)
            {
                throw new ArgumentNullException("propertyExpresssion");
            }

            var memberExpression = propertyExpresssion.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpresssion");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpresssion");
            }

            var getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
            {
                throw new ArgumentException("The referenced property is a static property.", "propertyExpresssion");
            }

            return memberExpression.Member.Name;
        }
    }

    public class ObservableObject : INotifyPropertyChanged
    {
        #region Notify

        [XmlIgnore]

        private bool hasChanged = false;
        public bool HasChanged
        {
            get { return hasChanged; }
            set
            {
                hasChanged = value;
                deleteUnwrittenRecord = false;
            }
        }
        //public bool HasChanged { get; set; }

        private bool deleteUnwrittenRecord = false;

        public bool DeleteUnwrittenRecord
        {
            get { return deleteUnwrittenRecord; }
            set { deleteUnwrittenRecord = value; }
        }

        private bool deleteUnchangedRecord = false;

        public bool DeleteUnchangedRecord
        {
            get { return deleteUnchangedRecord; }
            set { deleteUnchangedRecord = value; }
        }

        public bool AssignNotify(ref bool item, bool value, string notify)
        {
            deleteUnwrittenRecord = false;
            if (item != value)
            {
                deleteUnchangedRecord = false; //???
                item = value;
                RaisePropertyChanged(notify);
            }
            return value;
        }

        public DateTime AssignNotify(ref DateTime item, DateTime value, string notify)
        {
            deleteUnwrittenRecord = false;
            if (item != value)
            {
                item = value;
                RaisePropertyChanged(notify);
            }
            return value;
        }

        public TimeSpan AssignNotify(ref TimeSpan item, TimeSpan value, string notify)
        {
            deleteUnwrittenRecord = false;
            if (item != value)
            {
                item = value;
                RaisePropertyChanged(notify);
            }
            return value;
        }

        public DateTime? AssignNotify(ref DateTime? item, DateTime? value, string notify)
        {
            deleteUnwrittenRecord = false;
            if (item != value)
            {
                item = value;
                RaisePropertyChanged(notify);
            }
            return value;
        }

        public int AssignNotify(ref int item, int value, string notify)
        {
            deleteUnwrittenRecord = false;
            if (item != value)
            {
                item = value;
                RaisePropertyChanged(notify);
            }
            return value;
        }

        public int? AssignNotify(ref int? item, int? value, string notify)
        {
            deleteUnwrittenRecord = false;
            if (item != value)
            {
                item = value;
                RaisePropertyChanged(notify);
            }
            return value;
        }

        public Int64 AssignNotify(ref Int64 item, Int64 value, string notify)
        {
            deleteUnwrittenRecord = false;
            if (item != value)
            {
                item = value;
                RaisePropertyChanged(notify);
            }
            return value;
        }

        public int AssignNotify(ref int item, Int64 value, string notify)
        {
            deleteUnwrittenRecord = false;
            if (item != value)
            {
                item = (int)value;
                RaisePropertyChanged(notify);
            }
            return (int)value;
        }

        public short AssignNotify(ref short item, short value, string notify)
        {
            deleteUnwrittenRecord = false;
            if (item != value)
            {
                item = value;
                RaisePropertyChanged(notify);
            }
            return value;
        }

        public double AssignNotify(ref double item, double value, string notify)
        {
            deleteUnwrittenRecord = false;
            if (item != value)
            {
                item = value;
                RaisePropertyChanged(notify);
            }
            return value;
        }

        public decimal AssignNotify(ref decimal item, decimal value, string notify)
        {
            deleteUnwrittenRecord = false;
            if (item != value)
            {
                item = value;
                RaisePropertyChanged(notify);
            }
            return value;
        }

        public string AssignNotify(ref string item, string value, string notify)
        {
            deleteUnwrittenRecord = false;
            if (item != value)
            {
                item = value;
                RaisePropertyChanged(notify);
            }
            if (item == null) item = string.Empty;
            return value;
        }
        public Brush AssignNotify(ref Brush item, Brush value, string notify)
        {
            deleteUnwrittenRecord = false;
            if (item != value)
            {
                item = value;
                RaisePropertyChanged(notify);
            }
            return value;
        }
        public byte[] AssignNotify(ref byte[] item, byte[] value, string notify)
        {
            deleteUnwrittenRecord = false;
            if (item != value)
            {
                item = value;
                RaisePropertyChanged(notify);
            }
            return value;
        }

        public Visibility AssignNotify(ref Visibility item, Visibility value, string notify)
        {
            deleteUnwrittenRecord = false;
            if (item != value)
            {
                item = value;
                RaisePropertyChanged(notify);
            }
            return value;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void Notify(string info)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }

        public virtual void Notify(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)
        {
            var propertyName = PropertySupport.ExtractPropertyName(propertyExpresssion);
            this.RaisePropertyChanged(propertyName);
        }

        public void RaisePropertyChanged(String propertyname)
        {
            HasChanged = true;
            VerifyPropertyName(propertyname);
            Notify(propertyname);
            AfterPropertyChanged(propertyname);
            Notify("HasChanged");
        }

        public void RaisePropertyChanged(params string[] propertyNames)
        {
            HasChanged = true;
            foreach (string info in propertyNames)
            {
                VerifyPropertyName(info);
                Notify(info);
                AfterPropertyChanged(info);
            }
            Notify("HasChanged");
        }

        public void VerifyPropertyName(String propertyName)
        {
            if (Debugger.IsAttached)//running in ide
            {
                if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                {
                    Debug.WriteLine("Invalid property name: " + this.GetType().ToString() + "." + propertyName);
                }
            }
        }

        public virtual void AfterPropertyChanged(String propertyName) //does nothing but can be overriden
        { }

        #endregion

        //[XmlIgnore]
        //public bool DesignTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(
        //new DependencyObject());
        [XmlIgnore]
        public bool DesignTime
        {
            get
            {
                if (_designTime == null)
                {
                    _designTime = GetInDesignMode();
                }
                return (bool)_designTime;
            }
        }
        private bool? _designTime;
        private bool GetInDesignMode()
        {
            return System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }
    }

    public class RelayCommand<T> : ICommand
    {

        #region Declarations

        readonly Predicate<T> _canExecute;
        readonly Action<T> _execute;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class and the command can always be executed.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {

            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        [DebuggerStepThrough]
        public Boolean CanExecute(Object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public void Execute(Object parameter)
        {
            _execute((T)parameter);
        }

        #endregion
    }

    public class RelayCommand : ICommand
    {

        #region Declarations

        readonly Func<Boolean> _canExecute;
        readonly Action _execute;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class and the command can always be executed.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Func<Boolean> canExecute)
        {

            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        [DebuggerStepThrough]
        public Boolean CanExecute(Object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public void Execute(Object parameter)
        {
            _execute();
        }

        #endregion
    }

    public static class StringExtensions
    {
        public static string FormatTimeSince(this DateTime theDate)
        {
            var defaultDate = new DateTime();
            if (theDate == defaultDate) return string.Empty;
            TimeSpan ts = DateTime.Now.Subtract(theDate);
            if (ts.TotalSeconds < 60)
            {
                return string.Format("{0} sec", ts.Seconds);
            }
            if (ts.TotalMinutes < 60)
            {
                return string.Format("{0} min", ts.Minutes);
            }
            if (ts.TotalHours >= 1 && ts.TotalHours < 2)
            {
                return string.Format("{0} hr", ts.Hours);
            }
            if (ts.TotalHours < 24)
            {
                return string.Format("{0} hrs", ts.Hours);
            }
            if (ts.TotalDays >= 1 && ts.TotalDays < 2)
            {
                return string.Format("{0} day", ts.Days);
            }
            return string.Format("{0} days", ts.Days);
        }
    }

    public enum DialogImage { Asterisk, Error, Exclamation, Hand, Information, None, Question, Stop, Warning }
    public enum DialogButton { OK, OKCancel, YesNo, YesNoCancel }
    public enum DialogResponse { Cancel, No, None, OK, Yes }

    /// <summary>
    /// This is a very bare bones implementation of an IDialog service.
    /// There should be many more overloads for showing messages to make programming easier and to provide support for TaskDialog.
    /// You should also have File Open, File Save, Folder Browswer operations.
    /// </summary>    
    public interface IDialogService
    {
        DialogResponse ShowException(String message, DialogImage image = DialogImage.Error);
        DialogResponse ShowMessage(String message, String caption, DialogButton button, DialogImage image);
    }

    /// <summary>
    /// This is a very bare bones implementation of a Dialog service.
    /// MessageBox is a bummer way to display messages in WPF.  Use TaskDialog or a TaskDialog replacment for a nicer experiece.
    /// You should also have File Open, File Save, Folder Browswer operations.
    /// </summary>
    public class ModalDialogService : IDialogService
    {

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ModalDialogService"/> class.
        /// </summary>
        public ModalDialogService() { }

        #endregion

        #region Methods

        static MessageBoxButton GetButton(DialogButton button)
        {

            switch (button)
            {

                case DialogButton.OK: return MessageBoxButton.OK;

                case DialogButton.OKCancel: return MessageBoxButton.OKCancel;

                case DialogButton.YesNo: return MessageBoxButton.YesNo;

                case DialogButton.YesNoCancel: return MessageBoxButton.YesNoCancel;
            }
            throw new ArgumentOutOfRangeException("button", "Invalid button");
        }

        static MessageBoxImage GetImage(DialogImage image)
        {

            switch (image)
            {

                case DialogImage.Asterisk: return MessageBoxImage.Asterisk;

                case DialogImage.Error: return MessageBoxImage.Error;

                case DialogImage.Exclamation: return MessageBoxImage.Exclamation;

                case DialogImage.Hand: return MessageBoxImage.Hand;

                case DialogImage.Information: return MessageBoxImage.Information;

                case DialogImage.None: return MessageBoxImage.None;

                case DialogImage.Question: return MessageBoxImage.Question;

                case DialogImage.Stop: return MessageBoxImage.Stop;

                case DialogImage.Warning: return MessageBoxImage.Warning;
            }
            throw new ArgumentOutOfRangeException("image", "Invalid image");
        }

        static DialogResponse GetResponse(MessageBoxResult result)
        {

            switch (result)
            {

                case MessageBoxResult.Cancel: return DialogResponse.Cancel;

                case MessageBoxResult.No: return DialogResponse.No;

                case MessageBoxResult.None: return DialogResponse.None;

                case MessageBoxResult.OK: return DialogResponse.OK;

                case MessageBoxResult.Yes: return DialogResponse.Yes;
            }
            throw new ArgumentOutOfRangeException("result", "Invalid result");
        }

        /// <summary>
        /// Shows the exception in a MessageBox.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="image">The image.</param>
        /// <returns><see cref="DialogResponse"/>.OK</returns>
        public DialogResponse ShowException(String message, DialogImage image = DialogImage.Error)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, GetImage(image));
            return DialogResponse.OK;
        }

        /// <summary>
        /// Shows the message in a MessageBox.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="image">The image.</param>
        /// <returns><see cref="DialogResponse"/></returns>
        public DialogResponse ShowMessage(String message, String caption, DialogButton button, DialogImage image)
        {
            return GetResponse(MessageBox.Show(message, caption, GetButton(button), GetImage(image)));
        }

        #endregion
    }

    public class EventCommand
    {
        public static DependencyProperty CommandProperty =
           DependencyProperty.RegisterAttached("Command",
           typeof(ICommand),
           typeof(EventCommand));

        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(EventCommand.CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(CommandProperty);
        }

        public static DependencyProperty EventNameProperty =
           DependencyProperty.RegisterAttached("Name",
           typeof(string),
           typeof(EventCommand),
           new FrameworkPropertyMetadata(NameChanged));

        public static void SetName(DependencyObject target, string value)
        {
            target.SetValue(EventCommand.EventNameProperty, value);
        }

        public static string GetName(DependencyObject target)
        {
            return (string)target.GetValue(EventNameProperty);
        }

        private static void NameChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = target as UIElement;

            if (element != null)
            {
                // If we're putting in a new command and there wasn't one already hook the event
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    EventInfo eventInfo = element.GetType().GetEvent((string)e.NewValue);

                    Delegate d = Delegate.CreateDelegate(eventInfo.EventHandlerType,
                      typeof(EventCommand).GetMethod("Handler",
                      BindingFlags.NonPublic | BindingFlags.Static));

                    eventInfo.AddEventHandler(element, d);
                }
                // If we're clearing the command and it wasn't already null unhook the event
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    EventInfo eventInfo = element.GetType().GetEvent((string)e.OldValue);

                    Delegate d = Delegate.CreateDelegate(eventInfo.EventHandlerType,
                                 typeof(EventCommand).GetMethod("Handler"));

                    eventInfo.RemoveEventHandler(element, d);
                }
            }
        }

        static void Handler(object sender, EventArgs e)
        {
            UIElement element = (UIElement)sender;
            ICommand command = (ICommand)element.GetValue(EventCommand.CommandProperty);

            var src = Tuple.Create(sender, e);

            if (command != null && command.CanExecute(src) == true)
                command.Execute(src);
        }
    }

    public class EnumToBoolConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter.Equals(value)) return true; else return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return parameter;
        }
        #endregion
    }

    public class BoolToVisibleOrHidden : IValueConverter
    {
        #region Constructors
        public BoolToVisibleOrHidden() { }
        #endregion

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool bValue = (bool)value;
            if (bValue)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;

            if (visibility == Visibility.Visible)
                return true;
            else
                return false;
        }
        #endregion
    }


}
