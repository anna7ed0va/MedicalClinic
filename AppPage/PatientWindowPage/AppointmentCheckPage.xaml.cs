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
    /// Interaction logic for AppointmentCheckPage.xaml
    /// </summary>
    public partial class AppointmentCheckPage : Page
    {
        private Patient patient;

        public AppointmentCheckPage(Patient patient)
        {
            InitializeComponent();

            this.patient = patient;

            ProcessingPage();
        }

        private void ProcessingPage()
        {
            if (IsAnyAppointment())
            {
                AppointmentsAvailability(true);
                SetDataGridItemSource();
            }
            else
            {
                AppointmentsAvailability(false);
            }
        }

        private bool IsAnyAppointment()
        {
            using (var context = new MClinicEntities3())
            {
                DateTime date = DateTime.Now.AddHours(-1);
                return context.Appointment.Any(app => app.PatientId == patient.Id && app.DateTimeOfMeeting > date);
            }
        }

        private void AppointmentsAvailability(bool availability)
        {
            AppointmentsTable.Visibility = availability ? Visibility.Visible : Visibility.Hidden;
            AppointmentsTable.IsEnabled = availability;

            ErrorAppointments.Visibility = availability ? Visibility.Hidden : Visibility.Visible;
            ErrorAppointments.IsEnabled = !availability;
        }

        private void SetDataGridItemSource()
        {
            List<ElemOfGrid> pageElems = new List<ElemOfGrid>();

            using (var context = new MClinicEntities3 ())
            {
                DateTime date = DateTime.Now.AddHours(-1);

                List<Appointment> appointments = context.Appointment.Where(app => app.PatientId == patient.Id && app.DateTimeOfMeeting > date).ToList();

                foreach (var app in appointments)
                {
                    Doctor doctor = context.Doctor.FirstOrDefault(doc => doc.Id == app.DoctorsId);
                    string nameSurname = $"{doctor.PersonalData.Surname} {doctor.PersonalData.Name}";
                    string spec = doctor.Speciality.Name;
                    pageElems.Add(new ElemOfGrid(app.Id, nameSurname, spec, app.Description, app.DateTimeOfMeeting));
                }
            }

            Appointments.ItemsSource = pageElems;
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            List<int> idOfAppointments = (Appointments.ItemsSource as List<ElemOfGrid>).Where(app => app.IsSelected == true)
                                                                                          .Select(app => app.IdOfAppointment)
                                                                                          .ToList();

            if (idOfAppointments.Count != 0)
            {
                List<Appointment> rangeToRemove = new List<Appointment>();
                using (var context = new MClinicEntities3())
                {
                    foreach (var id in idOfAppointments)
                    {
                        rangeToRemove.Add(context.Appointment.FirstOrDefault(app => app.Id == id));
                    }

                    context.Appointment.RemoveRange(rangeToRemove);
                    context.SaveChanges();

                }
                SetDataGridItemSource();

                MessageBox.Show("Запись(и) были успешно отменены");
            }
            else
            {
                MessageBox.Show("Вы не выбрали записи, которые нужно отменить");
            }
        }

        private void RemoveAppointmentsFromTable(List<int> idOfAppointments)
        {
            foreach (var id in idOfAppointments)
            {
                ElemOfGrid elemToRemove = (Appointments.ItemsSource as List<ElemOfGrid>).FirstOrDefault(app => app.IdOfAppointment == id);
                if (elemToRemove != null)
                    Appointments.Items.Remove(elemToRemove);
            }
        }
    }

    class ElemOfGrid
    {
        public int IdOfAppointment { get; private set; }
        public bool IsSelected { get; set; }
        public string NameOfDoctor { get; private set; }
        public string SpeciallityOfDoctor { get; private set; }
        public string Description { get; private set; }
        public DateTime DateOfMeeting { get; private set; }

        public ElemOfGrid(int IdOfAppointment, string NameOfDoctor, string SpeciallityOfDoctor, string Description, DateTime DateOfMeeting)
        {
            this.IdOfAppointment = IdOfAppointment;
            IsSelected = false;
            this.NameOfDoctor = NameOfDoctor;
            this.SpeciallityOfDoctor = SpeciallityOfDoctor;
            this.Description = Description;
            this.DateOfMeeting = DateOfMeeting;
        }
    }
}
