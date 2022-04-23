using MedicalClinic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for DoctorRegPage.xaml
    /// </summary>
    public partial class DoctorRegPage : Page
    {
        public DoctorRegPage()
        {
            InitializeComponent();

            SignOutInitialState();
        }

        private void SignOutInitialState()
        {
            DateText.DisplayDateStart = DateTime.Now.AddYears(-100);
            DateText.DisplayDateEnd = DateTime.Now;

            using(var context = new MClinicEntities3())
            {
                foreach(var spec in context.Speciality.OrderBy(spec => spec.Name))
                {
                    Speciality.Items.Add(spec);
                }
            }
        }

        public void RegClick(object sender, RoutedEventArgs e)
        {
            string login = LoginText.Text.Trim();
            string pass = PassText.Password.Trim();
            string passRep = PassRepText.Password.Trim();
            string name = NameText.Text.Trim();
            string surname = SurnameText.Text.Trim();
            string speciality = Speciality.Text.Trim();            
            string sex = SexText.Text;
            DateTime? date = DateText.SelectedDate;
            string email = EmailText.Text.Trim();
            string phone = PhoneText.Text.Trim();

            string message = CheckRegValues(login, pass, passRep, name, surname, speciality, sex, date, email, phone);
            if (!String.IsNullOrEmpty(message))
            {
                MessageBox.Show(message);
            }
            else
            {
                int specialityId = (Speciality.SelectedItem as Speciality).Id;
                AddPatient(login, pass, name, surname, specialityId, sex, date, email, phone);
                MessageBox.Show("Вы успешно зарегестрированы");
                ClearAllFields();
            }
        }

        private void ClearAllFields()
        {
            LoginText.Text = null;
            PassText.Password = null;
            PassRepText.Password = null;
            NameText.Text = null;
            SurnameText.Text = null;
            SexText.Text = null;
            DateText.SelectedDate = null;
            EmailText.Text = null;
            PhoneText.Text = null;
            Speciality.Text = null;
        }

        private void AddPatient(string login, string pass, string name, string surname, int specialityId, string sex, DateTime? date, string email, string phone)
        {
            using (MClinicEntities3 context = new MClinicEntities3())
            {
                Doctor doctor = new Doctor();
                doctor.SpecialityId = specialityId; 

                PersonalData patientPersonalData = new PersonalData();
                patientPersonalData.Name = name;
                patientPersonalData.Surname = surname;
                patientPersonalData.Sex = sex;
                patientPersonalData.DateOfBirth = (DateTime)date;
                patientPersonalData.Email = email;
                patientPersonalData.PhoneNumber = phone;

                context.PersonalData.Add(patientPersonalData);
                context.SaveChanges();

                User patientUser = new User();
                patientUser.Login = login;
                patientUser.Password = pass;

                context.User.Add(patientUser);
                context.SaveChanges();                
               
                doctor.User = patientUser;
                doctor.PersonalData = patientPersonalData;

                doctor.UserId = patientUser.Id;
                doctor.PersonalData.Id = patientPersonalData.Id;

                context.Doctor.Add(doctor);
                context.SaveChanges();                
            }
        }

        private string CheckRegValues(string login, string pass, string passRep, string name, string surname, string speciality, string sex, DateTime? date, string email, string phone)
        {
            string message = "";

            Regex rusSymbRegex = new Regex("^[А-Яа-я]+$");
            Regex emailRegex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            Regex loginSybmRegex = new Regex(@"^[a-zA-Z0-9]+$");

            if (login.Length < 8 || login.Length > 20)
                message = "В логине должно быть 8 или более символов";
            else if (!loginSybmRegex.IsMatch(login))
                message = "Логин должен содержать только латинские буквы и цифры";
            else if (IsLoginAlreadyExists(login))
                message = "Такой логин уже существует. Придумайте новый";
            else if (pass.Length < 8 || pass.Length > 20)
                message = "В пароле должно быть 8 или более символов";
            else if (!loginSybmRegex.IsMatch(pass))
                message = "Пароль должен содержать только латинские буквы и цифры";
            else if (passRep != pass)
                message = "Повторение пароля не совпадает с паролем";
            else if (name.Length == 0)
                message = "Поле пустое";
            else if (!rusSymbRegex.IsMatch(name))
                message = "Имя должно содержать только русские буквы";
            else if (name.Length > 25)
                message = "Имя должно быть менее 25 символов";
            else if (surname.Length == 0)
                message = "Поле с фамилией пустое";
            else if (!rusSymbRegex.IsMatch(surname))
                message = "Фамилия должна содержать только русские буквы";
            else if (surname.Length > 40)
                message = "Фамилия должна быть менее 40 символов";
            else if (speciality.Length == 0)
                message = "Специальность не выбрана";
            else if (sex.Length == 0)
                message = "Пол не выбран";
            else if (date == null)
                message = "Дата рождения не выбрана";
            else if (!emailRegex.IsMatch(email))
                message = "Неверно указан Email";
            else if (IsEmailAlreadyExists(email))
                message = "Такой email уже был зарегистрирован";
            else if (phone.Length == 0 || phone.Length < 10)
                message = "Номер неправильно записан";
            else if (IsPhoneAlreadyExists(phone))
                message = "Такой номер уже зарегистрирован";
            return message;
        }

        private bool IsLoginAlreadyExists(string login)
        {
            using (MClinicEntities3 context = new MClinicEntities3())
            {
                return context.User.Any(user => user.Login == login);
            }
        }

        private bool IsEmailAlreadyExists(string email)
        {
            using (MClinicEntities3 context = new MClinicEntities3())
            {
                return context.PersonalData.Any(user => user.Email == email);
            }
        }

        private bool IsPhoneAlreadyExists(string phone)
        {
            using (MClinicEntities3 context = new MClinicEntities3())
            {
                return context.PersonalData.Any(user => user.PhoneNumber == phone);
            }
        }

        private void PhoneTextPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }
    }
}
