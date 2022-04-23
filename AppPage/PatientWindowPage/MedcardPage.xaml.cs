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

namespace MedicalClinic.AppPage.PatientWindowPage
{
    /// <summary>
    /// Interaction logic for MedcardPage.xaml
    /// </summary>
    public partial class MedcardPage : Page
    {
        public Patient Patient { get; private set; } 

        public MedcardPage(Patient patient)
        {
            InitializeComponent();

            this.Patient = patient;

            if (IsAnyConclusion())
            {
                ConclusionsAvailability(true);
                FillConclusionList();
            }
            else
            {
                ConclusionsAvailability(false);
            }
        }
        
        private bool IsAnyConclusion()
        {
            using(var context = new MClinicEntities3())
            {
                return context.Conclusion.Any(conc => conc.MedCardId == Patient.MedCardId);
            }
        }

        private void ConclusionsAvailability(bool availability)
        {
            ConclusionsList.Visibility = availability ? Visibility.Visible : Visibility.Hidden;
            ConclusionsList.IsEnabled = availability;

            ErrorConclusions.Visibility = availability ? Visibility.Hidden : Visibility.Visible;
            ErrorConclusions.IsEnabled = !availability;
        }

        private void FillConclusionList()
        {
            using(var context = new MClinicEntities3())
            {
                List<Conclusion> conclusions = context.Conclusion.Where(conc => conc.MedCardId == Patient.MedCardId).ToList();

                foreach(var conclusion in conclusions)
                {
                    string nameSurname = $"{conclusion.Appointment.Doctor.PersonalData.Surname} {conclusion.Appointment.Doctor.PersonalData.Name}";
                    string speciality = conclusion.Appointment.Doctor.Speciality.Name;
                    DateTime date = conclusion.Appointment.DateTimeOfMeeting;

                    ElemOfList elem = new ElemOfList(conclusion.Id, nameSurname, speciality, date);

                    Conclusions.Items.Add(elem);
                }
            }
        }

        private void ConclusionClick(object sender, SelectionChangedEventArgs e)
        {
            if (Conclusions.SelectedValue == null)
            {
                return;
            }

            var lv = sender as ListView;
            ElemOfList elem = lv.SelectedItem as ElemOfList;
            int appointmentId;

            using (var context = new MClinicEntities3())
            {
                appointmentId = context.Conclusion.FirstOrDefault(conc => conc.Id == elem.ConclusionId).AppoinmentId;
            }

            new ConclusionResultWindow(appointmentId).ShowDialog();
            Conclusions.SelectedItem = null;
        }
    }

    class ElemOfList
    {
        public int ConclusionId { get; private set; }
        public string NameOfDoctor { get; private set; }
        public string Speciality { get; private set; }
        public DateTime DateOfMeeting { get; private set; }

        public ElemOfList(int ConclusionId, string NameOfDoctor, string Speciality, DateTime DateOfMeeting)
        {
            this.ConclusionId = ConclusionId;
            this.NameOfDoctor = NameOfDoctor;
            this.Speciality = Speciality;
            this.DateOfMeeting = DateOfMeeting;
        }
    } 
}
