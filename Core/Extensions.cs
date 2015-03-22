namespace Core
{
    using System;
    using System.Linq;

    /// <summary>
    /// Extensions methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Throws an ObjectDisposedException if the instance has been disposed.
        /// </summary>
        /// <param name="disposable">The instance implementing the IIsDisposed interface.</param>
        public static void ThrowIfDisposed(this Interfaces.IIsDisposed disposable )
        {
            if(disposable.Disposed)throw new ObjectDisposedException("The instance is disposed.");
        }
       
    }
}