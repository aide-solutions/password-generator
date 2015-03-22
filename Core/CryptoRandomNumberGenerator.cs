namespace Core
{
    using System;
    using System.Security.Cryptography;
    using Core.Interfaces;
    /// <summary>
    /// Cryptographic random number generator
    /// </summary>
    public sealed class CryptoRandomNumberGenerator : IRandomNumberGenerator, IIsDisposed
    {
        private bool _disposed = false;
        private RNGCryptoServiceProvider _rng;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Core.CryptoRandomNumberGenerator"/> class.
        /// </summary>
        public CryptoRandomNumberGenerator()
        {
            _rng = new RNGCryptoServiceProvider();
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
        /// <remarks>Not implemented. Always returns 0.</remarks>
        /// <returns>0</returns>
        /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
        public int GetRandomNumber()
        {
            this.ThrowIfDisposed();
            return 0;
        }

        /// <summary>
        /// Obtains a random Int32 number greater or equal to 0.
        /// </summary>
        /// <param name="lowerBound">Minimum value</param>
        /// <param name="upperBound">Maximum value</param>
        /// <returns>An Int32 value</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ObjectDisposedException">The instance has been disposed.</exception>
        public int GetRandomNumber(int lowerBound, int upperBound)
        {
            this.ThrowIfDisposed();

            if (!(lowerBound >= 0 && lowerBound < upperBound))
                throw new ArgumentException();

            uint urndnum;
            var rndnum = new Byte[4];
            if (lowerBound == upperBound - 1)
            {
                return lowerBound;
            }

            var xcludeRndBase = (uint.MaxValue - (uint.MaxValue % (uint)(upperBound - lowerBound)));

            do
            {
                _rng.GetBytes(rndnum);
                urndnum = System.BitConverter.ToUInt32(rndnum, 0);
            } while (urndnum >= xcludeRndBase);

            return (int)(urndnum % (upperBound - lowerBound)) + lowerBound;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if(_disposed) return;
            _rng.Dispose();
            _rng = null;
            _disposed = true;
            GC.SuppressFinalize(this);
        }
        
    }
}