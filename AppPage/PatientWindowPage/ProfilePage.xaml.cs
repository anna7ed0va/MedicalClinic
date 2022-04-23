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

namespace MedicalClinic.AppPage.PatientWindowPage
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private Patient patient;

        public ProfilePage(Patient patient)
        {
            InitializeComponent();

            this.patient = patient;

            SetAccountInfo();
        }

        private void SetAccountInfo()
        {
            using (MClinicEntities3 context = new MClinicEntities3())
            {
                PersonalData data = context.PersonalData.FirstOrDefault(elem => elem.Id == patient.PersonalDataId);
                User user = context.User.FirstOrDefault(elem => elem.Id == patient.UserId);

                LabelLogin.Content = user.Login;
                LabelPassword.Content = user.Password;
                LabelName.Content = data.Name;
                LabelSurname.Content = data.Surname;
                LabelSex.Content = data.Sex;
                LabelDateOfBirth.Content = data.DateOfBirth.Date.ToShortDateString();
                LabelPhone.Content = data.PhoneNumber;
                LabelEmail.Content = data.Email;
            }
        }
    }
}
