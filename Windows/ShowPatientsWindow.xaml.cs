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

namespace MedicalClinic.Windows
{
    /// <summary>
    /// Interaction logic for ShowPatientsWindow.xaml
    /// </summary>
    public partial class ShowPatientsWindow : Window
    {     
        public List<int> AppointmentsId { get; private set; }

        public ShowPatientsWindow(List<int> appointmentsId, int doctorsId)
        {
            InitializeComponent();

            AppointmentsId = appointmentsId;

            PageInitialState(doctorsId);      
            ProcessingPage();
        }

        private void PageInitialState(int doctorsId)
        {
            using(var context = new MClinicEntities3 ())
            {
                Doctor doctor = context.Doctor.FirstOrDefault(doc => doc.Id == doctorsId);

                NameOfDoctor.Text = $"{doctor.PersonalData.Surname} {doctor.PersonalData.Name}";
                DoctorsSpeciality.Text = doctor.Speciality.Name;
            }
        }

        private void ProcessingPage()
        {
            using (var context = new MClinicEntities3()) { 
                foreach(int id in AppointmentsId)
                {
                    Appointment appointment = context.Appointment.FirstOrDefault(app => app.Id == id);

                    string nameSurname = $"{appointment.Patient.PersonalData.Surname} {appointment.Patient.PersonalData.Name}";
                    string phone = appointment.Patient.PersonalData.PhoneNumber;
                    DateTime date = appointment.DateTimeOfMeeting;

                    Patients.Items.Add(new ElemOfList(nameSurname, phone, date));
                }
            }
        }

        public void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    class ElemOfList
    {
        public string NameOfPatient { get; private set; }
        public string PhoneNumber { get; private set; }
        public DateTime DateOfMeeting { get; private set; }

        public ElemOfList(string NameOfPatient, string PhoneNumber, DateTime DateOfMeeting)
        {
            this.NameOfPatient = NameOfPatient;
            this.PhoneNumber = PhoneNumber;
            this.DateOfMeeting = DateOfMeeting;
        }
    }
}
