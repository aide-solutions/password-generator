
namespace Wpf.ViewModels
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;
    using Views;

    public sealed class DialogViewModel : ViewModelBase<IDialogView>
    {
        public override event PropertyChangedEventHandler PropertyChanged;

        private bool _showOkButton;
        private bool _showCancelButton;
        private string _title;
        private string _message;
        private bool _isError;
        private ICommand _okCommand = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DialogViewModel(IDialogView view) : base(view)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DialogViewModel()
        {
        }

        public ICommand OkCommand
        {
            get { return _okCommand ?? ( _okCommand = new Command<object>(OnOkCommand) ); }
        }



        public bool ShowOkButton
        {
            get { return _showOkButton; }
            set { _showOkButton = value; InvokePropertyChanged("ShowOkButton"); }
        }

        public bool ShowCancelButton
        {
            get { return _showCancelButton; }
            set { _showCancelButton = value; InvokePropertyChanged("ShowCancelButton"); }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; InvokePropertyChanged("Title"); }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; InvokePropertyChanged("Message"); }
        }

        public bool IsError
        {
            get { return _isError; }
            set { _isError = value; InvokePropertyChanged("IsError"); InvokePropertyChanged("IsInfo"); }
        }

        public bool IsInfo
        {
            get { return !_isError; }
        }

        public bool? ShowDialog(Window owner, bool showOk, bool showCancel, string title, string message, bool isError)
        {
            this.ShowOkButton = showOk;
            this.ShowCancelButton = showCancel;
            this.Title = title;
            this.Message = message;
            this.IsError = isError;

            if(IsError)
               System.Media.SystemSounds.Exclamation.Play();

            return View.ShowDialog(owner);
        }

        private void  OnOkCommand(object value)
        {
            View.DialogResult = true;
            View.Close();
        }


        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="property"></param>
        internal override void InvokePropertyChanged(string property)
        {
            var handler = this.PropertyChanged;
            if(handler!=null)handler(this, new PropertyChangedEventArgs(property));
        }
    }
}