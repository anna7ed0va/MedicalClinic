using MedicalClinic.AppPage.DoctorWindowPage;
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
    /// Interaction logic for DoctorWindow.xaml
    /// </summary>
    public partial class DoctorWindow : Window
    {
        private Doctor doctor;
       
        public DoctorWindow(Doctor doctor)
        {
            InitializeComponent();

            this.doctor = doctor;            

            OpenPage(PagesEnum.PROFILE);
        }

        public enum PagesEnum { PROFILE, APPOINTMENT }

        public void OpenPage(PagesEnum page)
        {
            if (page == PagesEnum.PROFILE)
                UserFrame.Navigate(new ProfilePage(doctor));
            else if(page == PagesEnum.APPOINTMENT)
                UserFrame.Navigate(new AppointmentListPage(doctor));

        }

        private void ProfileClick(object sender, RoutedEventArgs e)
        {
            OpenPage(PagesEnum.PROFILE);
        }

        private void AppointmentListClick(object sender, RoutedEventArgs e)
        {
            OpenPage(PagesEnum.APPOINTMENT);
        }        

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }
    }
}
