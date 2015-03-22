

namespace Wpf.ViewModels
{
    using System.ComponentModel;
    using Views;
    using Core;
    using System.Windows;

    public sealed class PasswordViewModel : ViewModelBase<IPasswordView>
    {
        public override event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public PasswordViewModel(IPasswordView view) : base(view)
        {
            View.Title = "Password evaluation";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public PasswordViewModel()
        {
        }
        
        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="property"></param>
        internal override void InvokePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if(handler!=null)handler(this, new PropertyChangedEventArgs(property));
        }
    }
}