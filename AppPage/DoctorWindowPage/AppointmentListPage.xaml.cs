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

namespace MedicalClinic.AppPage.DoctorWindowPage
{
    /// <summary>
    /// Interaction logic for AppointmentListPage.xaml
    /// </summary>
    public partial class AppointmentListPage : Page
    {
        Doctor doctor;
        public AppointmentListPage(Doctor doctor)
        {
            InitializeComponent();

            this.doctor = doctor;

            ProcessingPage();
        }

        private void ProcessingPage()
        {
            if (IsAnyAppointmentsForToday())
            {
                AppointmentsAvailability(true);
                SetAppointmentListItemSource();
                SetTodaysDate();
            }
            else
            {
                AppointmentsAvailability(false);
            }
        }

        private bool IsAnyAppointmentsForToday()
        {
            using(var context = new MClinicEntities3())
            {
                return context.Appointment.Any(app => app.DateTimeOfMeeting.Year == DateTime.Now.Year &&
                                                      app.DateTimeOfMeeting.Month == DateTime.Now.Month &&
                                                      app.DateTimeOfMeeting.Day == DateTime.Now.Day);
            }
        }

        private void AppointmentsAvailability(bool availability)
        {
            AppointmentsTable.Visibility = availability ? Visibility.Visible : Visibility.Hidden;
            AppointmentsTable.IsEnabled = availability;

            ErrorAppointments.Visibility = availability ? Visibility.Hidden : Visibility.Visible;
            ErrorAppointments.IsEnabled = !availability;
        }

        private void SetAppointmentListItemSource()
        {
            using(var context = new MClinicEntities3())
            {
                string date = DateTime.Now.ToShortDateString();

                List<Appointment> appointments = context.Appointment.Where(app => app.DateTimeOfMeeting.Year == DateTime.Now.Year &&
                                                                                  app.DateTimeOfMeeting.Month == DateTime.Now.Month &&
                                                                                  app.DateTimeOfMeeting.Day == DateTime.Now.Day)
                                                                    .ToList();

                foreach(var app in appointments)
                {
                    Patient patient = context.Patient.FirstOrDefault(pat => pat.Id == app.PatientId);

                    string nameSurname = $"{patient.PersonalData.Surname} {patient.PersonalData.Name}";
                    Appointments.Items.Add(new ElemOfGrid(app.Id, nameSurname, app.Description, app.DateTimeOfMeeting.ToShortTimeString()));
                }
            }
        }

        private void SetTodaysDate()
        {
            TodaysDate.Content = DateTime.Now.ToShortDateString();
        }

        private void AppointmentsClick(object sender, SelectionChangedEventArgs e)
        {
            if(Appointments.SelectedValue == null)
            {
                return;
            }

            var lv = sender as ListView;

            int appointmentId = (lv.SelectedItem as ElemOfGrid).AppointmentId;

            IsConclusionExists(appointmentId, out int? conclusionId);

            if(conclusionId == null)
            {
                new ConclusionAddWindow(appointmentId).ShowDialog();
                Appointments.SelectedItem = null;
            }
            else
            {
                MessageBox.Show("Заключение уже было создано");
                new ConclusionResultWindow(appointmentId).ShowDialog();
                Appointments.SelectedItem = null;
            }
        }

        bool IsConclusionExists(int appointmentId, out int? conclusionId)
        {
            conclusionId = null; 
            using (var context = new MClinicEntities3())
            {
                var appointment = context.Appointment.FirstOrDefault(app => app.Id == appointmentId);

                var conclusion = context.Conclusion.FirstOrDefault(conc => conc.AppoinmentId == appointmentId);

                if(conclusion != null)
                {
                    conclusionId = conclusion.Id;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    class ElemOfGrid
    {
        public int AppointmentId { get; private set; }
        public string NameOfPatient { get; private set; }       
        public string Description { get; private set; }
        public string TimeOfMeeting { get; private set; }

        public ElemOfGrid(int IdOfAppointment, string NameOfPatient, string Description, string TimeOfMeeting)
        {
            this.AppointmentId = IdOfAppointment;
            this.NameOfPatient = NameOfPatient;            
            this.Description = String.IsNullOrEmpty(Description) ? "(необязательно)" : Description;
            this.TimeOfMeeting = TimeOfMeeting;
        }
    }
}
