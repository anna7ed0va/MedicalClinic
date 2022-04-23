using MedicalClinic.AppPage.PatientWindowPage;
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
using System.Windows.Shapes;

namespace MedicalClinic
{
    /// <summary>
    /// Interaction logic for PatientWindow.xaml
    /// </summary>
    public partial class PatientWindow : Window
    {
        private Patient patient;

        //private ProfilePage profile;
        //private MedcardPage medcard = new MedcardPage();
        //private AppointmentPage appointment;
        //private CheckAppointment checkAppointment;

        public PatientWindow(Patient patient)
        {
            InitializeComponent();

            this.patient = patient;            

            OpenPage(PagesEnum.PROFILE);
        }

        public enum PagesEnum { PROFILE, MEDCARD, CHECKAPPOINTMENT, APPOINTMENT }

        public void OpenPage(PagesEnum page)
        {
            if (page == PagesEnum.PROFILE)
                UserFrame.Navigate(new ProfilePage(patient));
            else if (page == PagesEnum.MEDCARD)
                UserFrame.Navigate(new MedcardPage(patient));
            else if (page == PagesEnum.APPOINTMENT)
                UserFrame.Navigate(new AppointmentRegPage(patient));
            else if (page == PagesEnum.CHECKAPPOINTMENT)
                UserFrame.Navigate(new AppointmentCheckPage(patient));
        }

        private void ProfileClick(object sender, RoutedEventArgs e)
        {
            OpenPage(PagesEnum.PROFILE);
        }

        private void MedcardClick(object sender, RoutedEventArgs e)
        {
            OpenPage(PagesEnum.MEDCARD);
        }

        private void AppointmentClick(object sender, RoutedEventArgs e)
        {
            OpenPage(PagesEnum.APPOINTMENT);
        }

        private void CheckAppointmentClick(object sender, RoutedEventArgs e)
        {
            OpenPage(PagesEnum.CHECKAPPOINTMENT);
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }
    }
}
