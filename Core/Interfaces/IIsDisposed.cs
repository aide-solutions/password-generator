namespace Core.Interfaces
{
    public interface IIsDisposed
    {
        /// <summary>
        /// Determines if the instance has been disposed
        /// </summary>
        bool Disposed { get; }
    }
}