using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BitMoneto
{
    /// <summary>
    /// Logica di interazione per Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            if (GestoreImpostazioni.ControllaPassword(null))
                ApriApplicazione(null);
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string psw = PasswordBox.Password;
            if (GestoreImpostazioni.ControllaPassword(psw))
            {
                ApriApplicazione(psw);
            }
            else
                MessageBox.Show("Password errata");
        }

        private void ApriApplicazione(string password)
        {
            MainWindow mainWindow = new MainWindow(password);
            mainWindow.Show();
            this.Close();
        }
    }
}
