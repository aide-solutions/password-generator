using System.Windows;

namespace Wpf.Views
{
    public interface IPasswordView : IView
    {
        bool? ShowDialog(Window owner);
    }
}