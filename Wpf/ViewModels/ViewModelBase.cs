namespace Wpf.ViewModels
{
    using Views;
    using System.ComponentModel;

    public abstract class ViewModelBase<T> : INotifyPropertyChanged where T : IView
    {
        private T _view;

        protected internal T View
        {
            get { return _view; }
            set { _view = value; InvokePropertyChanged("View"); }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public abstract event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="property"></param>
        internal abstract void InvokePropertyChanged(string property);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        protected ViewModelBase()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        protected ViewModelBase(T view)
            : this()
        {
            _view = view;
            _view.DataContext = this;
        }
    }
}