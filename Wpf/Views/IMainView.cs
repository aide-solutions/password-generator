using System.Windows.Threading;
namespace Wpf.Views
{
    public interface IMainView : IView
    {
        Dispatcher Dispatcher { get; }
    }
}