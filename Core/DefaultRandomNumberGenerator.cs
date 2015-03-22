namespace Core
{
    using System;
    using Interfaces;

    /// <summary>
    /// A default random number generator
    /// </summary>
    public sealed class DefaultRandomNumberGenerator
        : IRandomNumberGenerator, IDisposable, IIsDisposed
    {
        private Random _rnd = null;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Core.DefaultRandomNumberGenerator"/> class.
        /// </summary>
        public DefaultRandomNumberGenerator()
        {
            _rnd = new Random();
        }
        /// <summary>
        /// Determines if the instance has been disposed
        /// </summary>
        public bool Disposed
        {
            get { return _disposed; }
        }
        /// <summary>
        /// Obtains a random Int32 number.
        /// </summary>
        /// <returns>An Int32 value</returns>
        public int GetRandomNumber()
        {
            this.ThrowIfDisposed();
            return _rnd.Next();
        }
        /// <summary>
        /// Obtains a random Int32 number.
        /// </summary>
        /// <param name="lowerBound"></param>
        /// <param name="upperBound"></param>
        /// <returns>An Int32 value</returns>
        public int GetRandomNumber(int lowerBound, int upperBound)
        {
            this.ThrowIfDisposed();
            return _rnd.Next(lowerBound, upperBound);
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if(_disposed) return;
            _rnd = null;
            _disposed = true;

            GC.SuppressFinalize(this);
        }

       
    }
}