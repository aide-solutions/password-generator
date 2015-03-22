

namespace Core.Interfaces
{
    using System;
    /// <summary>
    /// Random number generator contract
    /// </summary>
    public interface IRandomNumberGenerator : IDisposable
    {
        /// <summary>
        /// Obtains a random Int32 number.
        /// </summary>
        /// <returns>An Int32 value</returns>
        int GetRandomNumber();
        /// <summary>
        /// Obtains a random Int32 number.
        /// </summary>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns>An Int32 value</returns>
        int GetRandomNumber(int lowerBound, int upperBound);
    }
}