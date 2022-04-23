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

namespace MedicalClinic.AppPage.DoctorWindowPage
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private Doctor doctor;

        public ProfilePage(Doctor doctor)
        {
            InitializeComponent();

            this.doctor = doctor;

            SetAccountInfo();
        }

        private void SetAccountInfo()
        {
            using (MClinicEntities3 context = new MClinicEntities3 ())
            {
                PersonalData data = context.PersonalData.FirstOrDefault(elem => elem.Id == doctor.PersonalDataId);
                User user = context.User.FirstOrDefault(elem => elem.Id == doctor.UserId);

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
