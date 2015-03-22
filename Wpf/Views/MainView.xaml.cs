using System;
using System.Windows.Threading;

namespace Wpf.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window, IMainView
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new ViewModels.MainViewModel(this);
        }
      
    }
}
