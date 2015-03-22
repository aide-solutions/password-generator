

namespace Wpf.Views
{
    using System;
    using System.Windows;
    /// <summary>
    /// Interaction logic for DialogView.xaml
    /// </summary>
    public partial class DialogView : Window, IDialogView
    {
        public DialogView()
        {
            InitializeComponent();
        }

        public bool? ShowDialog(Window owner)
        {
            this.Owner = owner;
            this.WindowStartupLocation = owner == null
                                             ? System.Windows.WindowStartupLocation.CenterScreen
                                             : System.Windows.WindowStartupLocation.CenterOwner;

            return this.ShowDialog();
        }
    }
}
