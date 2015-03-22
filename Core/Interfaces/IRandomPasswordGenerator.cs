


namespace Core.Interfaces
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Random password generation interface contract
    /// </summary>
    public interface IRandomPasswordGenerator : IDisposable, IIsDisposed, INotifyPropertyChanged
    {
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
        /// <param name="minStrength"></param>
        /// <returns>A <see cref="T:Core.Password"/> array.</returns>
        Password[] Generate(uint count, uint length, bool forceNumbers, bool forceLowerCase, bool forceUpperCase,
                            bool useSymbols, char[] dictionary, uint minNumbers, uint minLowers, uint minUppers, PasswordStrengthEnum minStrength);

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
        /// <param name="minStrength"></param>
        /// <param name="callback">The callback to use when generation is finished</param>
        void BeginGenerate(uint count, uint length, bool forceNumbers, bool forceLowerCase, bool forceUpperCase,
                            bool useSymbols, char[] dictionary, uint minNumbers, uint minLowers, uint minUppers, PasswordStrengthEnum minStrength, Action<GenerationEventArgs> callback);

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
        void CheckParameters(uint length, bool useSymbols, bool forceNumbers, bool forceLowerCase, bool forceUpperCase, uint minNumbers, uint minLowers, uint minUpper);
        /// <summary>
        /// Cancels an asynchronous generation.
        /// </summary>
        void Cancel();
        /// <summary>
        /// Determines if a generation can be canceled.
        /// </summary>
        bool CanCancel { get; }
        /// <summary>
        /// Determines if the generation has been canceled.
        /// </summary>
        bool Canceled { get; }
        /// <summary>
        /// Determines if a generation is running.
        /// </summary>
        bool IsGenerating { get; }
        /// <summary>
        /// Obtains the number of passwords currently generated.
        /// </summary>
        int GeneratedPasswordsCount { get; }
    }
}