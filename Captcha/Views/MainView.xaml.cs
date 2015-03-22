namespace Captcha.Views
{
    using System.Windows;
    using System.Diagnostics;
    using System.Windows.Media;
    using System.Linq;
    using System;

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            cbFonts.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f=>f.Source).Select(f => f.Source);
            cbFonts.SelectedIndex = 0;

            try
            {
                captcha.Generate();
            }
            catch (Exception)
            {
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                captcha.Generate();
                
            }
            catch (System.Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            bool valid = captcha.IsValid(txtBox.Text, false);

            MessageBox.Show( valid ? "Valid":"invalid","captcha generator");

            captcha.Generate();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var filename = ".\\captcha.png";
            captcha.SaveToFile(filename);
            Process.Start(filename);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            captcha.Say();
        }
    }
}
