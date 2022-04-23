using MedicalClinic.Model;
using MedicalClinic.AppPage.ManagerWindowPage;
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
    /// Interaction logic for ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        private Manager manager;

        public ManagerWindow(Manager manager)
        {
            InitializeComponent();

            this.manager = manager;

            OpenPage(PagesEnum.PATIENTREG);
        }

        public enum PagesEnum { PATIENTREG, DOCTORREG, DOCTOR }

        public void OpenPage(PagesEnum page)
        {
            if (page == PagesEnum.PATIENTREG)
                UserFrame.Navigate(new PatientRegPage());
            else if (page == PagesEnum.DOCTORREG)
                UserFrame.Navigate(new DoctorRegPage());
            else if (page == PagesEnum.DOCTOR)
                UserFrame.Navigate(new DoctorsPage());
        }

        private void AddPatientClick(object sender, RoutedEventArgs e)
        {
            OpenPage(PagesEnum.PATIENTREG);
        }
        
        private void AddDoctorClick(object sender, RoutedEventArgs e)
        {
            OpenPage(PagesEnum.DOCTORREG);
        }

        private void DoctorClick(object sender, RoutedEventArgs e)
        {
            OpenPage(PagesEnum.DOCTOR);
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }
    }
}
