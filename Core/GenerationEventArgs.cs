
namespace Core
{
    using System;

    public sealed class GenerationEventArgs : System.EventArgs
    {
        private readonly Password[] _passwords;
        private readonly TimeSpan _duration;
        private readonly Exception _exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Core.GenerationEventArgs"/> class.
        /// </summary>
        public GenerationEventArgs(Password[] passwords, TimeSpan duration, Exception exception)
        {
            _passwords = passwords;
            _duration = duration;
            _exception = exception;
        }

        public Password[] Passwords
        {
            get { return _passwords; }
        }

        public TimeSpan Duration
        {
            get { return _duration; }
        }

        public bool HasError
        {
            get { return _exception != null; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }
    }
}