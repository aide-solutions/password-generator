namespace Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Interfaces;
    using System.Linq;

    /// <summary>
    /// Default password generator implementation
    /// </summary>
    public sealed class DefaultPasswordGenerator : PasswordGeneratorBase
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public override event PropertyChangedEventHandler PropertyChanged;

        private bool _isAsync;
        private bool _isGenerating;
        private int _generatedPasswords;
        private bool _canceled;
        private CancellationTokenSource _cancellation;
        readonly char[] _simpleDictionary = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        readonly char[] _complexDictionary = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890<>&_-@=+*/$%&!:;?.*^[](){}\\|`#~".ToCharArray();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Core.DefaultPasswordGenerator"/> class.
        /// </summary>
        public DefaultPasswordGenerator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Core.DefaultPasswordGenerator"/> class.
        /// </summary>
        public DefaultPasswordGenerator(IRandomNumberGenerator randomNumberGenerator) : base(randomNumberGenerator)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Core.DefaultPasswordGenerator"/> class.
        /// </summary>
        public DefaultPasswordGenerator(IRandomNumberGenerator randomNumberGenerator, IStrengthCalculator strengthCalculator) : base(randomNumberGenerator, strengthCalculator)
        {
        }

        /// <summary>
        /// Checks if parameters are correct
        /// </summary>
        /// <param name="length"></param>
        /// <param name="useSymbols"></param>
        /// <param name="forceNumbers"></param>
        /// <param name="forceLowerCase"></param>
        /// <param name="forceUpperCase"></param>
        /// <param name="minNumbers"></param>
        /// <param name="minLowers"></param>
        /// <param name="minUppers"></param>
        public override void CheckParameters(uint length,bool useSymbols, bool forceNumbers, bool forceLowerCase, bool forceUpperCase, uint minNumbers, uint minLowers, uint minUppers)
        {
            if (length < 1)
                throw new ArgumentException("Password length must be greater than 0.");

            // Check minimal required password length
            if (forceNumbers || forceUpperCase || forceLowerCase)
            {
                uint minPassLenght = 0;
                if (forceNumbers)
                    minPassLenght += minNumbers;
                if (forceUpperCase)
                    minPassLenght += minUppers;
                if (forceLowerCase)
                    minPassLenght += minLowers;
                if (useSymbols)
                    minPassLenght++;

                if (length < minPassLenght)
                    throw new ArgumentException("Password length invalid. The minimal required length is " + minPassLenght);
            }
        }

        /// <summary>
        /// Generates random passwords
        /// </summary>
        /// <param name="count">The password(s) count to generate</param>
        /// <param name="length">The password(s) length</param>
        /// <param name="forceNumbers">Force usage of numbers</param>
        /// <param name="forceLowerCase">Force usage of lower case letters</param>
        /// <param name="forceUpperCase">Force usage of upper case letters</param>
        /// <param name="useSymbols">Allow symbols</param>
        /// <param name="dictionary">A custom dictionary to use</param>
        /// <param name="minNumbers">The minimum numbers count the password must have</param>
        /// <param name="minLowers">The minimum lower case letters the password(s) must have</param>
        /// <param name="minUppers">The minimum upper case letters the password(s) must have</param>
        /// <param name="minStrength">The minimum password strength or PasswordStrength.NotDefined</param>
        /// <returns>A <see cref="T:Core.Password"/> array.</returns>
        public override Password[] Generate(uint count, uint length, bool forceNumbers, bool forceLowerCase, bool forceUpperCase, bool useSymbols, char[] dictionary, uint minNumbers, uint minLowers, uint minUppers, PasswordStrengthEnum minStrength)
        {
            if (IsGenerating) return null;
            CheckParameters(length,useSymbols, forceNumbers, forceLowerCase, forceUpperCase, minNumbers, minLowers, minUppers);
            
            IsGenerating = true;
            CanCancel = false;
            Canceled = false;
            GeneratedPasswordsCount = 0;

            if (dictionary == null)
                dictionary = useSymbols ? _complexDictionary : _simpleDictionary;

            var passwords = new Password[count];
            for (int i = 0; i < passwords.Length; i++)
                passwords[i] = GeneratePassword(length, forceNumbers, forceLowerCase, forceUpperCase, useSymbols,dictionary, minNumbers, minLowers, minUppers, minStrength);

            IsGenerating = false;

            return passwords;
        }

        /// <summary>
        /// Starts an asynchronous password generation
        /// </summary>
        /// <param name="count">The password(s) count to generate</param>
        /// <param name="length">The password(s) length</param>
        /// <param name="forceNumbers">Force usage of numbers</param>
        /// <param name="forceLowerCase">Force usage of lower case letters</param>
        /// <param name="forceUpperCase">Force usage of upper case letters</param>
        /// <param name="useSymbols">Allow symbols</param>
        /// <param name="dictionary">A custom dictionary to use</param>
        /// <param name="minNumbers">The minimum numbers count the password must have</param>
        /// <param name="minLowers">The minimum lower case letters the password(s) must have</param>
        /// <param name="minUppers">The minimum upper case letters the password(s) must have</param>
        /// <param name="minStrength">The minimum password strength or PasswordStrength.NotDefined</param>
        /// <param name="callback">The callback to use when generation is finished</param>
        public override void BeginGenerate(uint count, uint length, bool forceNumbers, bool forceLowerCase, bool forceUpperCase, bool useSymbols, char[] dictionary, uint minNumbers, uint minLowers, uint minUppers, PasswordStrengthEnum minStrength, Action<GenerationEventArgs> callback)
        {
            BeginGenerate(count,length,forceNumbers, forceLowerCase, forceUpperCase, useSymbols, dictionary, minNumbers, minLowers, minUppers, minStrength, callback,null);
        }


        public void BeginGenerate(uint count, uint length, bool forceNumbers, bool forceLowerCase, bool forceUpperCase, bool useSymbols, char[] dictionary, uint minNumbers, uint minLowers, uint minUppers, PasswordStrengthEnum minStrength, Action<GenerationEventArgs> callback, Action<Password> passwordCallback)
        {
            if (IsGenerating) return;
            IsGenerating = true;

            try
            {
                CheckParameters(length, useSymbols, forceNumbers, forceLowerCase, forceUpperCase, minNumbers, minLowers, minUppers);
            }
            catch (Exception e)
            {
                IsGenerating = false;
                if (callback != null)
                    callback.Invoke(new GenerationEventArgs(null, TimeSpan.Zero, e));
                return;
            }

            Exception exp = null;

            if (dictionary == null)
                dictionary = useSymbols ? _complexDictionary : _simpleDictionary;

            if (_cancellation != null) _cancellation.Dispose();
            _cancellation = new CancellationTokenSource();

            GeneratedPasswordsCount = 0;
            Canceled = false;
            CanCancel = true;

            var tasks = new Task<Password>[count];
            var tf = new TaskFactory<Password>(_cancellation.Token);

            var st = new Stopwatch();
            st.Start();

            for (uint i = 0; i < tasks.Length; i++)
                tasks[i] = tf.StartNew(() => GeneratePassword(length, forceNumbers, forceLowerCase, forceUpperCase, useSymbols, dictionary, minNumbers, minLowers, minUppers, minStrength, passwordCallback), _cancellation.Token);
            try
            {
                Task.WaitAll(tasks, _cancellation.Token);
            }
            catch (OperationCanceledException e)
            {
                _cancellation.Dispose();
                _cancellation = null;
                exp = e;

                Canceled = true;
            }
            finally { st.Stop(); }

            CanCancel = false;

            var passwords = new Password[count];
            if (exp == null)
                for (var i = 0; i < passwords.Length; i++)
                {
                    if (tasks[i].Result != null)
                        passwords[i] = tasks[i].Result;

                    tasks[i].Dispose();
                }

            if (callback != null)
                callback.Invoke(new GenerationEventArgs(passwords, st.Elapsed, exp));

            IsGenerating = false;
        }
        
        /// <summary>
        /// Cancels an asynchronous generation.
        /// </summary>
        public override void Cancel()
        {
            if(CanCancel && _cancellation!=null)
                _cancellation.Cancel();
        }

        /// <summary>
        /// Determines if a generation can be canceled.
        /// </summary>
        public override bool CanCancel
        {
            get { return _isAsync; }
            internal set { _isAsync = value; InvokePropertyChanged("CanCancel"); }
        }

        /// <summary>
        /// Determines if the generation has been canceled.
        /// </summary>
        public override bool Canceled
        {
            get { return _canceled; }
            internal set { _canceled = value; InvokePropertyChanged("Canceled"); }
        }

        /// <summary>
        /// Determines if a generation is running.
        /// </summary>
        public override bool IsGenerating
        {
            get { return _isGenerating; }
            internal set { _isGenerating = value; InvokePropertyChanged("IsGenerating"); }
        }

        /// <summary>
        /// Obtains the number of passwords currently generated.
        /// </summary>
        public override int GeneratedPasswordsCount
        {
            get { return _generatedPasswords; }
            internal set
            {
                _generatedPasswords = value; 
                InvokePropertyChanged("GeneratedPasswordsCount");
            }
        }
        
        /// <summary>
        /// Raises the PropertyChanged Event
        /// </summary>
        /// <param name="property"></param>
        private void InvokePropertyChanged(string property)
        {
            if(PropertyChanged != null)
                PropertyChanged(this,new PropertyChangedEventArgs(property));
        }
        /// <summary>
        /// Password generation method
        /// </summary>
        /// <param name="length"></param>
        /// <param name="forceNumbers"></param>
        /// <param name="forceLowerCase"></param>
        /// <param name="forceUpperCase"></param>
        /// <param name="useSymbols"></param>
        /// <param name="dictionary"></param>
        /// <param name="minNumbers"></param>
        /// <param name="minLowers"></param>
        /// <param name="minUppers"></param>
        /// <param name="minStrength"></param>
        /// <returns></returns>
        private Password GeneratePassword(uint length, bool forceNumbers, bool forceLowerCase, bool forceUpperCase, bool useSymbols, IList<char> dictionary, uint minNumbers, uint minLowers, uint minUppers, PasswordStrengthEnum minStrength)
        {
            var pass = new char[length];
            for (var i = 0; i < pass.Length; i++)
            {
                if(_canceled) break;

                pass[i] = GetRandomChar(dictionary);
                
                // Checks if previous char is the same, if it is, invalidates it
                if (i > 0 && pass[i] == pass[i - 1]) 
                    i--;

                if (i != (length - 1))
                    continue; // loop while not at the end of the char array

                
                // Check password validity
                if (!IsPasswordValid(pass, length, minStrength, forceNumbers, forceUpperCase, forceLowerCase, minNumbers, minLowers, minUppers))
                {
                    Thread.Sleep(TimeSpan.FromTicks(10000)); // Make a quick pause to lower cpu usage
                    i = -1; // Restart main loop
                }
                else
                {
                    return new Password(pass, base.GetPasswordStrength);
                }
            }

            return null; // returns null if canceled
        }

        private Password GeneratePassword(uint length, bool forceNumbers, bool forceLowerCase, bool forceUpperCase, bool useSymbols, IList<char> dictionary, uint minNumbers, uint minLowers, uint minUppers, PasswordStrengthEnum minStrength, Action<Password> callback)
        {
            var p = GeneratePassword(length, forceNumbers, forceLowerCase, forceUpperCase, useSymbols, dictionary,
                                     minNumbers, minLowers, minUppers, minStrength);
            if (callback != null && !_canceled)
                callback(p);

            return p;
        }

        /// <summary>
        /// Checks if a password meets all criterias
        /// </summary>
        /// <param name="password"></param>
        /// <param name="length"></param>
        /// <param name="minStrength"></param>
        /// <param name="forceNumbers"></param>
        /// <param name="forceUpper"></param>
        /// <param name="forceLower"></param>
        /// <param name="minNumbers"></param>
        /// <param name="minLowers"></param>
        /// <param name="minUppers"></param>
        /// <returns></returns>
        private bool IsPasswordValid(char[] password, uint length, PasswordStrengthEnum minStrength, bool forceNumbers, bool forceUpper, bool forceLower, uint minNumbers, uint minLowers, uint minUppers)
        {
            if (password == null ) return false;
            if (password.Count() != length) return false;
            if (forceNumbers && (uint)password.Count((c) => char.IsNumber(c)) < minNumbers) return false;
            if (forceUpper && (uint)password.Count((c) => char.IsUpper(c)) < minUppers) return false;
            if (forceLower && (uint)password.Count((c) => char.IsLower(c)) < minLowers) return false;
            if (minStrength!= PasswordStrengthEnum.NotDefined)
            {
                var strength = base.GetPasswordStrength(password);
                if ((PasswordStrengthEnum)strength < minStrength) return false;
            }
            return true;
        }
    }
}