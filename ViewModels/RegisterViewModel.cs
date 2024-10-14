using Assignment.View.Model;
using Assignment.View.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Assignment.View.View;
using System.Windows;
using System.Text.RegularExpressions;
using Assignment.ViewModel;
using System.Configuration;
using System.Windows.Markup;
using System.Runtime.InteropServices;

namespace Assignment.View.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        //Fields
        private string _FirstName;
        private string _LastName;
        private string _Email;
        private string _Gender;
        private DateTime _DateOfBirth;
        private SecureString _Password;
        private SecureString _ConfirmPassword;
        private string _errorMessage;
        private bool _isViewVisible = true;
        private UserRepository _userRepository;


        //Property
        public string FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                _FirstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }
        public string LastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                _LastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }
        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                _Email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        public string Gender
        {
            get
            {
                return _Gender;
            }
            set
            {
                _Gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }
        public DateTime DateOfBirth
        {
            get
            {
                return _DateOfBirth;
            }
            set
            {
                _DateOfBirth = value;
                OnPropertyChanged(nameof(DateOfBirth));
            }
        }

        public SecureString Password
        {
            get => _Password;
            set
            {
                _Password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public SecureString ConfirmPassword
        {
            get => _ConfirmPassword;
            set
            {
                _ConfirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        public bool IsViewVisible
        {
            get
            {
                return _isViewVisible;
            }
            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }

        //Commands
        public ICommand RegisterCommand { get; }

        public RegisterViewModel()
        {
            DateOfBirth = new DateTime(1980, 01, 01);
            _userRepository = new UserRepository();
            RegisterCommand = new ViewModelCommand(ExecuteRegisterCommand);

        }


        private string formattedString(string Gender)
        {
            int GenderIndex = Gender.IndexOf(' ');

            string GenderResult = Gender.Substring(GenderIndex + 1, Gender.Length - GenderIndex - 1);

            return GenderResult;
        }

        public static string ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException(nameof(securePassword));

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        private void ExecuteRegisterCommand(object obj)
        {
            var myCredential = new NetworkCredential(Email, Password);
            var myCredential2 = new NetworkCredential(Email, ConfirmPassword);


            bool registerValidation = true;

            if (!string.Equals(myCredential.Password, myCredential2.Password))
            {
                ErrorMessage = "Password is not same as Confirm password";
                registerValidation = false;
            }

            if (Password.Length < 6 || Password.Length > 16)
            {
                ErrorMessage = "Invalid password format";
                registerValidation = false;
            }

            if (string.IsNullOrEmpty(Email) && IsValidEmail(Email))
            {
                ErrorMessage = "Email is required";
                registerValidation = false;
            }

            if (Gender.Contains("Please Select"))
            {
                ErrorMessage = "Option not selected";
                registerValidation = false;
            }

            if (string.IsNullOrEmpty(LastName))
            {
                ErrorMessage = "LastName is required";
                registerValidation = false;
            }

            if (string.IsNullOrEmpty(FirstName))
            {
                ErrorMessage = "FirstName is required";
                registerValidation = false;
            }

            if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName) && Gender.Contains("Please Select") && string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(myCredential.Password) && string.IsNullOrEmpty(myCredential2.Password))
            {
                ErrorMessage = "All fields are empty";
                registerValidation = false;
            }

            if (registerValidation)
            {

            using (var conn = new SqlConnection(@"Server=(localdb)\LocalInstance;Database=Assignment;Integrated Security=True;Encrypt=False")) 
            using (var command = new SqlCommand())
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
               string addData = "INSERT INTO Users (FirstName, LastName, Gender, Email, DOB, Password) VALUES (@firstName, @lastName, @gender, @Email, @DateOfBirth, @Password)";
                string unsecurePassword = ConvertToUnsecureString(Password);
                command.Connection = conn;
                command.CommandText = addData;
                command.Parameters.AddWithValue("@firstName", FirstName);
                command.Parameters.AddWithValue("@lastName", LastName);
                command.Parameters.AddWithValue("@gender", formattedString(Gender));
                command.Parameters.AddWithValue("@Email", Email);
                command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                command.Parameters.AddWithValue("@Password", unsecurePassword);
                command.ExecuteNonQuery();
            conn.Close();
                    // After successful registration
                    IsViewVisible = false;
                    var loginview = new LoginView();
                    loginview.Show();
                    Application.Current.MainWindow.Close();
                    MessageBox.Show("User Registered Successfully", "Congratulations", MessageBoxButton.OK, MessageBoxImage.Information);

                }
            }

        }
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }
        
    }
}


