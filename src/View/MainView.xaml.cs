using Microsoft.Win32;
using System.Windows;

namespace View
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.DefaultExt = "csv";
            dialog.Filter =
            "CSV (*.csv)|*.csv|All files (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                fileNameTextBox.Text = dialog.FileName;
            }
        }
    }
}