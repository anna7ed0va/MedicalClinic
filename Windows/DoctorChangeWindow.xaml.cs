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
    /// Interaction logic for DoctorChangeWindow.xaml
    /// </summary>
    public partial class DoctorChangeWindow : Window
    {
        public int DoctorsId { get; private set; }

        public DoctorChangeWindow(int doctorsId)
        {
            InitializeComponent();

            DoctorsId = doctorsId;

            PageInitialState();
        }

        private void PageInitialState()
        {
            DateText.DisplayDateStart = DateTime.Now.AddDays(1);
            DateText.DisplayDateEnd = DateTime.Now.AddYears(3);
        }

        public void SetDateClick(object sender, RoutedEventArgs e)
        {
            DateTime? date = DateText.SelectedDate;

            if(date != null)
            {
                if (IsAnyAppointments((DateTime)date))
                    DeleteAppointments((DateTime)date);
                SetDateOfAbsent((DateTime)date);

                MessageBox.Show("Дата была успешно добавлена");
            }
            else
            {
                MessageBox.Show("Вы не выбрали дату");
            }
        }

        private bool IsAnyAppointments(DateTime dateEnd)
        {
            using (var context = new MClinicEntities3 ())
            {
                DateTime dateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                dateStart = dateStart.AddDays(1);

                return context.Appointment.Any(app => app.DateTimeOfMeeting > dateStart &&
                                               app.DateTimeOfMeeting < dateEnd);
            }
        }

        private void DeleteAppointments(DateTime dateEnd)
        {
            using (var context = new MClinicEntities3 ())
            {
                DateTime dateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                dateStart = dateStart.AddDays(1);

                List<Appointment> appointmnetsToDelete = context.Appointment
                                                                .Where(app => app.DateTimeOfMeeting > dateStart &&
                                                                              app.DateTimeOfMeeting < dateEnd).ToList();
                
                List<int> appointmentsId = appointmnetsToDelete.Select(app => app.Id).ToList();

                ShowPatients(appointmentsId);

                context.Appointment.RemoveRange(appointmnetsToDelete);
                context.SaveChanges();
            }
        }

        private void ShowPatients(List<int> appointmentsId)
        {
            new ShowPatientsWindow(appointmentsId, DoctorsId).ShowDialog();
        }

        private void SetDateOfAbsent(DateTime date)
        {
            using (var context = new MClinicEntities3 ())
            {
                context.Doctor.FirstOrDefault(doc => doc.Id == DoctorsId).AbsentData = date;
                context.SaveChanges();
            }
        }

        public void DeleteDateClick(object sender, RoutedEventArgs e)
        {
            using (var context = new MClinicEntities3())
            {
                context.Doctor.FirstOrDefault(doc => doc.Id == DoctorsId).AbsentData = null;
                context.SaveChanges();
            }

            MessageBox.Show("Дата была успешно добавлена");
        }

        public void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}