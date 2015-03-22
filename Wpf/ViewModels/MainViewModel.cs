namespace Wpf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Security.AccessControl;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Core;
    using Core.Interfaces;
    using Microsoft.Win32;
    using Views;
    using System.Diagnostics;

    public sealed class MainViewModel : ViewModelBase<IMainView>
    {
        private string _customDictionary;
        private bool _forceUpperCase;
        private bool _forceLowerCase;
        private bool _forceNumbers;
        private bool _useSymbols;
        private bool _useCustomDictionary;
        private int _minUpperCount;
        private int _minLowerCount;
        private int _minNumberCount;
        private PasswordStrengthEnum _minStrength = PasswordStrengthEnum.NotDefined;
        private int _passwordLength;
        private int _passwordCount;
        private TimeSpan _elapsed;
        private readonly ObservableCollection<Password> _passwords;
        private readonly IRandomPasswordGenerator _generator;
        private int _generatedPasswords;
        private double _progress;

        private ICommand _generateCommand = null;
        private ICommand _cancelCommand = null;
        private ICommand _selectCustomDictionaryCommand = null;
        private ICommand _saveResultCommand = null;
        private ICommand _clearResultCommand = null;
        private ICommand _evaluatePasswordCommand = null;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public override event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MainViewModel()
        {
            _passwords = new ObservableCollection<Password>();
            _generator = new DefaultPasswordGenerator(new CryptoRandomNumberGenerator());
            _generator.PropertyChanged += new PropertyChangedEventHandler(OnGeneratorPropertyChanged);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MainViewModel(IMainView view) : this()
        {
            View = view;
            View.Title = "Easy password generator";
        }

        public ICommand GenerateCommand 
        {
            get { return _generateCommand ?? (_generateCommand = new Command<object>(OnGenerateCommand)); }
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new Command<object>(OnCancelCommand)); }
        }

        public ICommand SelectCustomDictionaryCommand
        {
            get { return _selectCustomDictionaryCommand ?? (_selectCustomDictionaryCommand = new Command<object>(OnSelectCustomDictionary)); }
        }

        public ICommand SaveResultsCommand
        {
            get { return _saveResultCommand ?? (_saveResultCommand = new Command<object>(OnSaveResultCommand)); }
        }

        public ICommand ClearResultsCommand
        {
            get { return _clearResultCommand ?? (_clearResultCommand = new Command<object>(OnClearResultsCommand)); }
        }

        public ICommand EvaluatePasswordCommand
        {
            get { return _evaluatePasswordCommand ?? (_evaluatePasswordCommand = new Command<object>(OnEvaluatePasswordCommand)); }
        }

        public ObservableCollection<Password> GeneratedPasswords
        {
            get { return _passwords; }
        }

        public string ElapsedTime
        {
            get { return string.Format("Elapsed time : {0}",_elapsed); }
        }

        public string GeneratedPasswordsCount
        {
            get { return string.Format("Generated passwords : {0}", _passwords.Count); }
        }

        public bool IsGenerating
        {
            get { return _generator != null && _generator.IsGenerating; }
        }

        public bool CanGenerate
        {
            get { return PasswordLength > 0 && PasswordCount > 0 && !IsGenerating; }
        }

        public bool CanCancel
        {
            get { return _generator!=null && _generator.CanCancel; }
        }

        public string[] PasswordStrengthValues
        {
            get { return Enum.GetNames(typeof (PasswordStrengthEnum)); }
        }
        
        public bool UseCustomDictionary
        {
            get { return _useCustomDictionary; }
            set
            {
                _useCustomDictionary = value;
                InvokePropertyChanged("UseCustomDictionary");
                if(value)
                    UseSymbols = false;
            }
        }

        public string CustomDictionary
        {
            get { return _customDictionary; }
            set { _customDictionary = value; InvokePropertyChanged("CustomDictionary"); }
        }

        public bool ForceUpperCase
        {
            get { return _forceUpperCase; }
            set { _forceUpperCase = value; InvokePropertyChanged("ForceUpperCase"); }
        }

        public bool ForceLowerCase
        {
            get { return _forceLowerCase; }
            set { _forceLowerCase = value; InvokePropertyChanged("ForceLowerCase"); }
        }

        public bool ForceNumbers
        {
            get { return _forceNumbers; }
            set { _forceNumbers = value; InvokePropertyChanged("ForceNumbers"); }
        }

        public bool UseSymbols
        {
            get { return _useSymbols; }
            set
            {
                _useSymbols = value; InvokePropertyChanged("UseSymbols");
                if (value)
                    UseCustomDictionary = false;
            }
        }

        public int MinUpperCount
        {
            get { return _minUpperCount; }
            set { _minUpperCount = value; InvokePropertyChanged("MinUpperCount"); }
        }

        public int MinLowerCount
        {
            get { return _minLowerCount; }
            set { _minLowerCount = value; InvokePropertyChanged("MinLowerCount"); }
        }

        public int MinNumberCount
        {
            get { return _minNumberCount; }
            set { _minNumberCount = value; InvokePropertyChanged("MinNumberCount"); }
        }

        public PasswordStrengthEnum MinStrength
        {
            get { return _minStrength; }
            set { _minStrength = value; InvokePropertyChanged("MinStrength"); }
        }

        public int PasswordLength
        {
            get { return _passwordLength; }
            set { _passwordLength = value; InvokePropertyChanged("PasswordLength"); InvokePropertyChanged("CanGenerate"); }
        }

        public int PasswordCount
        {
            get { return _passwordCount; }
            set { _passwordCount = value; InvokePropertyChanged("PasswordCount"); InvokePropertyChanged("CanGenerate"); }
        }

        public double Progress
        {
            get { return _progress; }
            set { _progress = value; InvokePropertyChanged("Progress"); }
        }

        private void ShowError(string message)
        {
            var vm = new DialogViewModel(new DialogView());
            vm.ShowDialog((Window)View, true, false, "Error", message, true);
        }

        private void OnGenerateCommand(object value)
        {
            if (_generator == null || !CanGenerate ) return;

            _generatedPasswords = 0;
            Progress = 0;

            var t = new Task(() => ((DefaultPasswordGenerator)_generator).BeginGenerate(
                (uint)PasswordCount, 
                (uint)PasswordLength,
                ForceNumbers, 
                ForceLowerCase, 
                ForceUpperCase,
                UseSymbols,
                UseCustomDictionary 
                    ? CustomDictionary == null 
                        ? null 
                        : CustomDictionary.ToCharArray() : null, 
                (uint)MinNumberCount,
                (uint)MinLowerCount, (uint)MinUpperCount,
                MinStrength,
                new Action<GenerationEventArgs>(OnGenerationFinished),
                new Action<Password>(OnPasswordGenerated)));

            t.Start();
        }

       

        private void OnCancelCommand(object value)
        {
            if (_generator == null || !CanCancel) return;
            _generator.Cancel();
        }
        
        private void OnPasswordGenerated(Password password)
        {
            _generatedPasswords++;
            Progress = ((double)_generatedPasswords / PasswordCount) * 100;
        }

        private void OnGenerationFinished(GenerationEventArgs e)
        {
            if(e.HasError && e.Exception is OperationCanceledException) return;
            if(e.HasError)
            {
                View.Dispatcher.Invoke(new Delegates.ActionDelegate<string>(ShowError), e.Exception.Message);
            }
            else
            {
                OnClearResultsCommand(null);

                var addDelegate = new Delegates.ActionDelegate<Password>(_passwords.Add);
                foreach (var p in e.Passwords)
                    View.Dispatcher.Invoke(addDelegate, p);

                _elapsed = e.Duration;

                InvokePropertyChanged("GeneratedPasswords");
                InvokePropertyChanged("ElapsedTime");
                InvokePropertyChanged("GeneratedPasswordsCount");
            }
            GC.Collect();
        }

        private void OnSelectCustomDictionary(object value)
        {
            var ofd = new OpenFileDialog
                          {
                              Filter = "All|*.*|Text|*.txt",
                              FilterIndex = 1,
                              Title = "Select a custom dictionary",
                              CheckFileExists = true,
                              CheckPathExists = true,
                              InitialDirectory = Environment.CurrentDirectory
                          };

            var result = ofd.ShowDialog((Window) View);
            if (!result.HasValue || !result.Value) return;

            var content = File.ReadAllLines(ofd.FileName);
            if(string.IsNullOrEmpty(content[0]) || string.IsNullOrWhiteSpace(content[0])) return;

            CustomDictionary = content[0];
        }

        private void OnSaveResultCommand(object value)
        {
            if(_passwords ==null || !_passwords.Any()) return;

            var sfd = new SaveFileDialog()
                          {
                              OverwritePrompt = false,
                              AddExtension = true,
                              Filter = "All|*.*|Text|*.txt",
                              FilterIndex = 1,
                              InitialDirectory = Environment.CurrentDirectory,
                              Title = "Save passwords to"
                          };

            var result = sfd.ShowDialog((Window) View);
            if(!result.HasValue || !result.Value) return;

            try
            {
                WritePasswordsToFile(sfd.FileName, _passwords.ToArray());
            }
            catch (Exception exp)
            {
                ShowError(exp.Message);
            }
        }

        private void OnClearResultsCommand(object value)
        {
            foreach (var password in _passwords)
                password.Dispose();

            View.Dispatcher.Invoke(new Delegates.ActionDelegate(_passwords.Clear));
        }

        private void OnEvaluatePasswordCommand(object value)
        {
            var vm = new PasswordViewModel(new PasswordView());
            vm.View.ShowDialog((Window) View);
        }

        private void OnGeneratorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InvokePropertyChanged("IsGenerating");
            InvokePropertyChanged("CanCancel");
            InvokePropertyChanged("CanGenerate");
        }

        internal override void InvokePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(property));
        }

        private static void WritePasswordsToFile(string filename, IEnumerable<Password> passwords)
        {
            if(string.IsNullOrEmpty(filename)) return;
            using (var fs = new FileStream(filename, FileMode.Append, FileSystemRights.Write, FileShare.Read, 4096, FileOptions.None))
            {
                byte[] buffer;
                foreach (var password in passwords.Where(p => p.Value.Any()))
                {
                    buffer = Encoding.UTF8.GetBytes(string.Format("{0}{1}", password, Environment.NewLine));
                    fs.Write(buffer, 0, buffer.Length);
                }
                fs.Flush(true);
            }
        }
    }

    public static class Delegates
    {
        public delegate void ActionDelegate();
        public delegate void ActionDelegate<in T>(T item);
    }
}