namespace Core
{
    using System;
    using Core.Interfaces;
    using System.ComponentModel;
    using System.Collections.Generic;

    /// <summary>
    /// Base password generator class
    /// </summary>
    public abstract class PasswordGeneratorBase  : IRandomPasswordGenerator
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public abstract event PropertyChangedEventHandler PropertyChanged;

        protected internal bool _disposed = false;
        private IRandomNumberGenerator _rng;
        private IStrengthCalculator _sc;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Core.PasswordGeneratorBase"/> class.
        /// </summary>
        protected PasswordGeneratorBase()
            : this(new DefaultRandomNumberGenerator(), new DefaultStrengthCalculator())
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Core.PasswordGeneratorBase"/> class.
        /// </summary>
        protected PasswordGeneratorBase(IRandomNumberGenerator randomNumberGenerator)
             : this(randomNumberGenerator, new DefaultStrengthCalculator())
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Core.PasswordGeneratorBase"/> class.
        /// </summary>
        protected PasswordGeneratorBase(IRandomNumberGenerator randomNumberGenerator, IStrengthCalculator strengthCalculator)
        {
            if (randomNumberGenerator == null) throw new ArgumentNullException("randomNumberGenerator");
            if (strengthCalculator == null) throw new ArgumentNullException("strengthCalculator");
            _rng = randomNumberGenerator;
            _sc = strengthCalculator;
        }
        /// <summary>
        /// Get a random char inside a char array using the IRandomNumberGenerator instance
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        internal virtual char GetRandomChar(IList<char> dictionary)
        {
            this.ThrowIfDisposed();
            return dictionary[_rng.GetRandomNumber(0, dictionary.Count)];
        }
        /// <summary>
        /// Evaluates the password strength using the IStrengthCalculator instance
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        internal virtual byte GetPasswordStrength(char[] password)
        {
            this.ThrowIfDisposed();
            return _sc == null ? (byte)PasswordStrengthEnum.None : _sc.CalculateStrength(password);
        }
        /// <summary>
        /// Determines if the instance has been disposed
        /// </summary>
        public virtual bool Disposed
        {
            get { return _disposed; }
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            if(_disposed) return;

            if(_rng!=null)_rng.Dispose();
            _rng = null;

            if(_sc!=null && _sc is IDisposable)((IDisposable)_sc).Dispose();
            _sc = null;

            _disposed = true;
            GC.SuppressFinalize(this);
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
        /// <returns>A <see cref="T:Core.Password"/> array.</returns>
        public abstract Password[] Generate(uint count, uint length, bool forceNumbers, bool forceLowerCase, bool forceUpperCase, bool useSymbols, char[] dictionary, uint minNumbers, uint minLowers, uint minUppers, PasswordStrengthEnum minStrength);

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
        /// <param name="callback">The callback to use when generation is finished</param>
        public abstract void BeginGenerate(uint count, uint length, bool forceNumbers, bool forceLowerCase, bool forceUpperCase, bool useSymbols, char[] dictionary, uint minNumbers, uint minLowers, uint minUppers, PasswordStrengthEnum minStrength, Action<GenerationEventArgs> callback);

        /// <summary>
        /// Cancels an asynchronous generation.
        /// </summary>
        public abstract void Cancel();
        /// <summary>
        /// Checks if parameters are correct
        /// </summary>
        /// <param name="length"></param>
        /// <param name="forceNumbers"></param>
        /// <param name="forceLowerCase"></param>
        /// <param name="forceUpperCase"></param>
        /// <param name="minNumbers"></param>
        /// <param name="minLowers"></param>
        /// <param name="minUpper"></param>
        public abstract void CheckParameters(uint length, bool useSymbols, bool forceNumbers, bool forceLowerCase, bool forceUpperCase, uint minNumbers, uint minLowers, uint minUpper);

        /// <summary>
        /// Determines if a generation can be canceled.
        /// </summary>
        public abstract bool CanCancel { get; internal set; }
        /// <summary>
        /// Determines if the generation has been canceled.
        /// </summary>
        public abstract bool Canceled { get; internal set; }
        /// <summary>
        /// Determines if a generation is running.
        /// </summary>
        public abstract bool IsGenerating { get; internal set; }
        /// <summary>
        /// Obtains the number of passwords currently generated.
        /// </summary>
        public abstract int GeneratedPasswordsCount { get; internal set; }
        
    }
}