using MedicalClinic.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MedicalClinic.AppPage.MainWindowPage
{
    /// <summary>
    /// Interaction logic for LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        public MainWindow mainWindow;

        public LogInPage(MainWindow _mainWindow)
        {
            InitializeComponent();

            mainWindow = _mainWindow;
        }

        public void EnterClick(object sender, RoutedEventArgs e)
        {
            string login = LoginText.Text;
            string password = PasswordText.Password;

            if (login.Length == 0 || password.Length == 0)
            {
                MessageBox.Show("Логин и пароль должны быть заполнены");
            }
            else
            {
                using (MClinicEntities3 context = new MClinicEntities3 ())
                {
                    Doctor doctor = context.Doctor.FirstOrDefault(user => user.User.Login == login && user.User.Password == password);
                    Patient patient = context.Patient.FirstOrDefault(user => user.User.Login == login && user.User.Password == password);
                    Manager manager = context.Manager.FirstOrDefault(user => user.User.Login == login && user.User.Password == password);


                    if (patient != null)
                    {
                        new PatientWindow(patient).Show();
                        mainWindow.Close();
                    }
                    else if (doctor != null)
                    {
                        new DoctorWindow(doctor).Show();
                        mainWindow.Close();
                    }
                    else if(manager != null)
                    {
                        new ManagerWindow(manager).Show();
                        mainWindow.Close();
                    }
                    else
                    {
                        MessageBox.Show("Не существует такого пользователя");
                    }
                }
            }
        }
    }
}
