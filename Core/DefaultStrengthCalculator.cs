namespace Core
{
    using System;
    
    /// <summary>
    /// Default password strength calculator
    /// </summary>
    /// <remarks>https://www.microsoft.com/protect/fraud/passwords/checker.aspx</remarks>
    public sealed class DefaultStrengthCalculator : Core.Interfaces.IStrengthCalculator
    {
        private const uint HAS_NUMBER   = 10;
        private const uint HAS_UPPER    = 26;
        private const uint HAS_LOWER    = 26;
        private const uint HAS_SYMBOL   = 16;
        private const uint HAS_OTHER    = 10;

        /// <summary>
        /// Calculates the strength of a password.
        /// </summary>
        /// <param name="password">A char array representing the password value.</param>
        /// <returns>A PasswordStrengthEnum byte value.</returns>
        public byte CalculateStrength(char[] password)
        {
            if (password == null || password.Length == 0) return (byte) PasswordStrengthEnum.NotDefined;

            uint charset = 0;
            bool hasUpper = false, 
                 hasLower = false, 
                 hasSymbol  = false, 
                 hasNumber = false, 
                 hasOther = false;
            
            for(var i = 0; i < password.Length; i++)
            {
                if (char.IsNumber(password[i])) hasNumber = true;
                else if (char.IsSymbol(password[i])) hasSymbol = true;
                else if (char.IsLower(password[i])) hasLower = true;
                else if (char.IsUpper(password[i])) hasUpper = true;
                else hasOther = true;
            }
            

            if (hasNumber)
                charset += HAS_NUMBER;
            if (hasUpper)
                charset += HAS_UPPER;
            if (hasLower)
                charset += HAS_LOWER;
            if (hasSymbol)
                charset += HAS_SYMBOL;
            if (hasOther)
                charset += HAS_OTHER;

            var value = Math.Floor(Math.Log(charset) * (password.Length / Math.Log(2)));

            if (value >= 128) return (byte)PasswordStrengthEnum.Best;
            if (value < 128 && value >= 64) return (byte)PasswordStrengthEnum.Strong;
            if (value < 64 && value >= 56) return (byte)PasswordStrengthEnum.Medium;
            if (value < 56) return (byte)PasswordStrengthEnum.Weak;
            return (byte) PasswordStrengthEnum.None;
        }
    }
}