using MedicalClinic.Model;
using MedicalClinic.Windows;
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

namespace MedicalClinic.AppPage.ManagerWindowPage
{
    /// <summary>
    /// Interaction logic for DoctorsPage.xaml
    /// </summary>
    public partial class DoctorsPage : Page
    {
        public DoctorsPage()
        {
            InitializeComponent();

            ProcessingPage();
        }

        private void ProcessingPage()
        {
            if (IsAnyDoctors())
            {
                DoctorsAvailability(true);
                SetDoctorsListItemSource();                
            }
            else
            {
                DoctorsAvailability(false);
            }
        }

        private bool IsAnyDoctors()
        {
            using (var context = new MClinicEntities3())
            {
                return context.Doctor.Any();
            }
        }

        private void DoctorsAvailability(bool availability)
        {
            DoctorsTable.Visibility = availability ? Visibility.Visible : Visibility.Hidden;
            DoctorsTable.IsEnabled = availability;

            ErrorDoctors.Visibility = availability ? Visibility.Hidden : Visibility.Visible;
            ErrorDoctors.IsEnabled = !availability;
        }

        private void SetDoctorsListItemSource()
        {
            Doctors.Items.Clear();

            using (var context = new MClinicEntities3())
            {
                foreach(var doc in context.Doctor)
                {
                    string nameSurname = $"{doc.PersonalData.Surname} {doc.PersonalData.Name}";
                    string speciality = doc.Speciality.Name;
                    string login = doc.User.Login;
                    string absentDate;
                    if (doc.AbsentData == null)
                        absentDate = "(отсутствует)";
                    else
                        absentDate = ((DateTime)doc.AbsentData).ToShortDateString();

                    Doctors.Items.Add(new ElemOfList(doc.Id, nameSurname, speciality, login, absentDate));
                }
            }
        }

        private void DoctorsClick(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = (sender as ListView);

            if (lv.SelectedValue == null)
                return;

            int doctorsId = (lv.SelectedItem as ElemOfList).DoctorsId;

            new DoctorChangeWindow(doctorsId).ShowDialog();

            SetDoctorsListItemSource();
        }
    }

    class ElemOfList
    {
        public int DoctorsId { get; private set; }
        public string NameOfDoctor { get; private set; }
        public string Speciality { get; private set; }
        public string Login { get; private set; }
        public string AbsentDate { get; private set; }
        public ElemOfList(int DoctorsId, string NameOfDoctor, string Speciality, string Login, string AbsentDate)
        {
            this.DoctorsId = DoctorsId;
            this.NameOfDoctor = NameOfDoctor;
            this.Speciality = Speciality;
            this.Login = Login;
            this.AbsentDate = AbsentDate;
        }
    }
}
