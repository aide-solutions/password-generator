namespace Wpf.Views
{
    /// <summary>
    /// View interface contract
    /// </summary>
    public interface IView
    {
        object DataContext { get; set; }
        void Close();
        string Title { get; set; }
    }
}