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
    /// Interaction logic for AppointmentRegPage.xaml
    /// </summary>
    public partial class AppointmentRegPage : Page
    {
        private Patient patient;

        private List<Doctor> doctors;

        public AppointmentRegPage(Patient patient)
        {
            InitializeComponent();

            this.patient = patient;

            PageProcessing();
        }

        private void PageProcessing()
        {
            InitialState();
            if (doctors.Count != 0)
                FillAppointmentPage(doctors);
        }

        private void InitialState()
        {
            if (TryFindDoctors())
            {
                ErrorState.Visibility = Visibility.Hidden;
                WorkState.Visibility = Visibility.Visible;
            }
            else
            {
                WorkState.Visibility = Visibility.Hidden;
                ErrorState.Visibility = Visibility.Visible;
            }
        }

        private bool TryFindDoctors()
        {
            doctors = new List<Doctor>();
            using (MClinicEntities3 context = new MClinicEntities3())
            {
                if (context.Doctor.Count() == 0)
                    return false;
                foreach (var doctor in context.Doctor)
                {
                    if (doctor.AbsentData != null)
                    {
                        var timeSpan = (DateTime)doctor.AbsentData - DateTime.Now;

                        if (timeSpan.TotalDays < 31)
                            doctors.Add(doctor);
                    }
                    else
                        doctors.Add(doctor);
                }

                if (doctors.Count == 0)
                    return false;
                else
                    return true;
            }
        }

        private void FillAppointmentPage(List<Doctor> doctors)
        {
            this.doctors = doctors;

            FillSpecialitiesField();
        }

        private void FillSpecialitiesField()
        {
            SpecialitiesField.Items.Clear();

            using (MClinicEntities3 context = new MClinicEntities3())
            {
                List<int> specId = doctors.Select(elem => elem.SpecialityId).Distinct().ToList();

                foreach (var spec in context.Speciality.OrderBy(spec => spec.Name))
                {
                    if (specId.Any(elem => elem == spec.Id))
                        SpecialitiesField.Items.Add(spec.Name);
                }
            }
        }

        private void ComboBoxSpecialityItemSelect(object sender, RoutedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            if (cb.Items.Count == 0)
                return;

            FillDoctorsField(cb.SelectedItem.ToString());

            MainBlock.IsEnabled = true;
            DTPickersBlockAvailability(false);
            ConfirmAppointmentButtonAvailability(false);
        }

        private void FillDoctorsField(string selectedItem)
        {
            DoctorsField.Items.Clear();

            using (MClinicEntities3 context = new MClinicEntities3())
            {
                int speciality = context.Speciality.ToList().Find(spec => spec.Name == selectedItem).Id;

                foreach (var doctor in doctors)
                {
                    if (speciality == doctor.SpecialityId)
                    {
                        DoctorsField.Items.Add(context.PersonalData.FirstOrDefault(data => data.Id == doctor.PersonalDataId));
                    }
                }
            }
        }

        private void ComboBoxDoctorItemSelect(object sender, RoutedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            if (cb.Items.Count == 0)
                return;

            PersonalData doctorsPersonalData = cb.SelectedItem as PersonalData;
            using (MClinicEntities3 context = new MClinicEntities3())
            {
                Doctor selectedDoctor = context.Doctor.FirstOrDefault(doc => doc.PersonalDataId == doctorsPersonalData.Id);
                FillDateTimePicker(selectedDoctor);
            }


            DTPickersBlockAvailability(true);
            TEOfAppointmentFieldAvailability(false, false);
            ConfirmAppointmentButtonAvailability(false);
        }

        private void DTPickersBlockAvailability(bool availablility)
        {
            DTPickersBlock.Visibility = availablility ? Visibility.Visible : Visibility.Hidden;
            DTPickersBlock.IsEnabled = availablility;
        }

        private void FillDateTimePicker(Doctor doctor)
        {
            DateOfAppointment.SelectedDate = null;

            if (doctor.AbsentData != null)
                DateOfAppointment.DisplayDateStart = doctor.AbsentData;
            else
                DateOfAppointment.DisplayDateStart = DateTime.Now.AddDays(1);

            DateOfAppointment.DisplayDateEnd = DateTime.Now.AddMonths(1);

            int span = (DateOfAppointment.DisplayDateEnd - DateOfAppointment.DisplayDateStart).Value.Days;
            DateTime day = (DateTime)DateOfAppointment.DisplayDateStart;
            while (span > 0)
            {
                if (day.DayOfWeek == DayOfWeek.Sunday || day.DayOfWeek == DayOfWeek.Saturday)
                    DateOfAppointment.BlackoutDates.Add(new CalendarDateRange(day));

                if (day.DayOfWeek == DayOfWeek.Sunday)
                {
                    day = day.AddDays(6);
                    span -= 6;
                }
                else
                {
                    day = day.AddDays(1);
                    span--;
                }
            }
        }

        private void DateOfAppointmentItemSelect(object sender, SelectionChangedEventArgs e)
        {
            var dp = sender as DatePicker;

            if (dp.SelectedDate == null)
                return;

            int selectedDoctorId;

            using (MClinicEntities3 context = new MClinicEntities3())
            {
                PersonalData doctorsPersData = (DoctorsField.SelectedItem as PersonalData);
                selectedDoctorId = context.Doctor.FirstOrDefault(doc => doc.PersonalDataId == doctorsPersData.Id).Id;
            }

            FillTimePicker((DateTime)dp.SelectedDate, selectedDoctorId);
        }

        private void FillTimePicker(DateTime selectedDate, int selectedDoctorId)
        {
            TimeOfAppointment.ItemsSource = null;

            var timeOfAppointments = ListOfTimeOfAppointment(selectedDoctorId, selectedDate);

            if (timeOfAppointments.Count != 0)
            {
                TimeOfAppointment.ItemsSource = timeOfAppointments;
                TEOfAppointmentFieldAvailability(true, false);
                ConfirmAppointmentButtonAvailability(true);
            }
            else
                TEOfAppointmentFieldAvailability(false, true);
        }

        private void TEOfAppointmentFieldAvailability(bool timeAvailability, bool errorAvailability)
        {
            TimeOfAppointmentField.Visibility = timeAvailability ? Visibility.Visible : Visibility.Hidden;
            ErrorTimeApplointment.Visibility = errorAvailability ? Visibility.Visible : Visibility.Hidden;

            TimeOfAppointmentField.IsEnabled = timeAvailability;
            ErrorTimeApplointment.IsEnabled = errorAvailability;
        }

        private List<string> ListOfTimeOfAppointment(int selectedDoctorId, DateTime selectedDate)
        {
            var timeOfAppointments = new List<string>();

            using (MClinicEntities3 context = new MClinicEntities3())
            {
                List<Appointment> appointments = context.Appointment
                                                        .Where(app => app.DoctorsId == selectedDoctorId &&
                                                                      app.DateTimeOfMeeting.Year == selectedDate.Year &&
                                                                      app.DateTimeOfMeeting.Month == selectedDate.Month &&
                                                                      app.DateTimeOfMeeting.Day == selectedDate.Day)
                                                        .OrderBy(app => app.DateTimeOfMeeting)
                                                        .ToList();

                DateTime time = new DateTime(1, 1, 1, 9, 0, 0);

                while (time.Hour <= 18)
                {
                    if (!appointments.Any(app => app.DateTimeOfMeeting.Hour == time.Hour))
                    {
                        timeOfAppointments.Add($"{time.Hour}:00");
                    }
                    time = time.AddHours(1);
                }
            }

            return timeOfAppointments;
        }

        private void ConfirmAppointmentButtonAvailability(bool availability)
        {
            ConfirmAppointmentButton.IsEnabled = availability;
            ConfirmAppointmentButton.Visibility = availability ? Visibility.Visible : Visibility.Hidden;
        }

        private void ConfirmAppointmentButtonClick(object sender, RoutedEventArgs e)
        {
            int doctorsId;
            int patientId = patient.Id;
            string description = DescriptionOfAppointment.Text;
            DateTime? dateOfMeeting = DateOfAppointment.SelectedDate;
            string timeOfMeeting = TimeOfAppointment.SelectedItem == null ? "" : TimeOfAppointment.SelectedItem.ToString();

            using (MClinicEntities3 context = new MClinicEntities3())
            {
                PersonalData doctorsPersData = (DoctorsField.SelectedItem as PersonalData);
                doctorsId = context.Doctor
                                   .FirstOrDefault(doc => doc.PersonalDataId == doctorsPersData.Id).Id;
            }

            string errorMessage = CheckAppointmentPageValues(patientId, (DateTime)dateOfMeeting, timeOfMeeting);

            if (!String.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage);
            }
            else
            {
                AddAppointment(doctorsId, patientId, description, (DateTime)dateOfMeeting, timeOfMeeting);
                MessageBox.Show("Регистрация на встречу прошла успешно");
                ClearAppointmentPage();
            }
        }

        private string CheckAppointmentPageValues(int patientId, DateTime dateOfMeeting, string timeOfMeeting)
        {
            string message = "";
            if (String.IsNullOrEmpty(timeOfMeeting))
            {
                message = "Выберите время записи";
                return message;
            }

            using (MClinicEntities3 context = new MClinicEntities3())
            {
                DateTime checkDateTime = dateOfMeeting.AddHours(ParseTimeStringToInt(timeOfMeeting));
                if (context.Appointment
                          .FirstOrDefault(app => app.PatientId == patientId
                          && app.DateTimeOfMeeting == checkDateTime) != null)
                {
                    message = "У вас уже есть запись на указанное время";
                }

                int doctorsPersonalDataId = (DoctorsField.SelectedItem as PersonalData).Id;
                DateTime? absentDate = context.Doctor.FirstOrDefault(doc => doc.PersonalDataId == doctorsPersonalDataId).AbsentData;
                if(absentDate != null)
                {
                    if(dateOfMeeting < absentDate)
                    {
                        message = "К сожалению, доктор не принимает в этот день, он будет принимать с" + ((DateTime)absentDate).ToShortDateString();
                    }
                }
            }

            return message;
        }

        private void AddAppointment(int doctorsId, int patientId, string description, DateTime dateOfMeeting, string timeOfMeeting)
        {
            using (MClinicEntities3 context = new MClinicEntities3())
            {

                Appointment appointment = new Appointment();

                appointment.DoctorsId = doctorsId;
                appointment.PatientId = patientId;
                appointment.Description = description;
                appointment.DateOfRegistration = DateTime.Now;
                appointment.DateTimeOfMeeting = dateOfMeeting.AddHours(ParseTimeStringToInt(timeOfMeeting));

                context.Appointment.Add(appointment);
                context.SaveChanges();
            }
        }

        private int ParseTimeStringToInt(string time)
        {
            return int.Parse(time.Substring(0, time.IndexOf(':')));
        }

        private void ClearAppointmentPage()
        {
            doctors = null;
            MainBlock.IsEnabled = false;
            DTPickersBlock.Visibility = Visibility.Hidden;
            DoctorsField.Items.Clear();
            PageProcessing();
            DescriptionOfAppointment.Text = null;
        }
    }
}
