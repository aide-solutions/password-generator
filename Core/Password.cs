namespace Core
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Diagnostics;

    /// <summary>
    /// 
    /// Class representing a password
    /// </summary>
    public sealed class Password : INotifyPropertyChanged, IDisposable
    {
        private char[] _value;
        private PasswordStrengthEnum _strength = PasswordStrengthEnum.NotDefined;
        private Func<char[],byte> _calculateStrengthDelegate;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Core.Password"/> class.
        /// </summary>
        public Password()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Core.Password"/> class.
        /// </summary>
        public Password(char[] value)
        {
            _value = value;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Core.Password"/> class.
        /// </summary>
        public Password(char[] value, Func<char[], byte> calculateStrengthDelegate)
            : this(value)
        {
            _calculateStrengthDelegate = calculateStrengthDelegate;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Core.Password"/> class.
        /// </summary>
        /// <exception cref="ArgumentException">strength has the NotDefined value.</exception>
        public Password(char[] value, PasswordStrengthEnum strength)
            :this(value)
        {
            if(strength == PasswordStrengthEnum.NotDefined)throw new ArgumentException();
            _strength = strength;
        }
        /// <summary>
        /// Determines if the strength has been calculated.
        /// </summary>
        public bool StrengthCalculated
        {
            get { return _strength != PasswordStrengthEnum.NotDefined; }
        }
        /// <summary>
        /// Obtains or defines a char array representing the password value.
        /// </summary>
        public char[] Value
        {
            get { return _value; }
            set
            {
                _value = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Value"));
            }
        }
        /// <summary>
        /// Obtains or defines the password strength.
        /// </summary>
        /// <exception cref="ArgumentException">value has the NotDefined value.</exception>
        public PasswordStrengthEnum Strength
        {
            get
            {
                if (_strength == PasswordStrengthEnum.NotDefined)
                    Task.Factory.StartNew(CalculateStrength);

                return _strength;
            }
            set
            {
                if (value == PasswordStrengthEnum.NotDefined) throw new ArgumentException();
                _strength = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Strength"));
            }
        }
        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="e">A PropertyChangedEventArgs</param>
        private void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }
        /// <summary>
        /// Invokes the CalculateStrengthDelegate function if any.
        /// </summary>
        internal void CalculateStrength()
        {
            try
            {
                if (_calculateStrengthDelegate != null)
                    Strength = (PasswordStrengthEnum)_calculateStrengthDelegate(Value);
            }
            catch (Exception)
            {
                Strength = PasswordStrengthEnum.None;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            _value = null;
            _calculateStrengthDelegate = null;
            _strength = PasswordStrengthEnum.NotDefined;

            GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            return new string(Value);
        }
    }
}