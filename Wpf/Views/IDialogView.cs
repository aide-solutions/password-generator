
namespace Wpf.Views
{
    using System.Windows;

    public interface IDialogView : IView
    {
        bool? ShowDialog(Window owner);
        bool? DialogResult { get; set; }
    }
}