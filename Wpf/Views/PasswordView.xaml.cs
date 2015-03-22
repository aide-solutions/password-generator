namespace Wpf.Views
{
    using System;
    using System.Windows;
    using Core;

    /// <summary>
    /// Interaction logic for PasswordView.xaml
    /// </summary>
    public partial class PasswordView : Window, IPasswordView
    {
        private readonly DefaultStrengthCalculator _strengthCalc = new DefaultStrengthCalculator();

        public PasswordView()
        {
            InitializeComponent();
            txtStrength.Text = Enum.GetName(typeof (PasswordStrengthEnum), PasswordStrengthEnum.NotDefined);
            passwordBox.Focus();
        }

      
        public bool? ShowDialog(Window owner)
        {
            this.Owner = owner;
            this.WindowStartupLocation = owner == null
                                             ? WindowStartupLocation.CenterScreen
                                             : WindowStartupLocation.CenterOwner;

            return this.ShowDialog();
        }

        public void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var strength = (PasswordStrengthEnum)_strengthCalc.CalculateStrength(passwordBox.Password.ToCharArray());
            txtStrength.Text = Enum.GetName(typeof(PasswordStrengthEnum),strength);
        }
    }
}
