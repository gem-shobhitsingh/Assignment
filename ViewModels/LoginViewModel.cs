using Assignment.View.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Assignment.View.Repositories;
using System.Net;
using System.Threading;
using System.Security.Principal;
using Assignment.ViewModel;
using Assignment.View.View;
using System.Windows;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace Assignment.View.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        //Fields
        private string _email;
        private SecureString _password;
        private string _errorMessage;
        private bool _isViewVisible = true;

        private UserRepository userRepository;

        //Property
        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        public SecureString Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
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

        //-> Commands
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        //Constructor
        public LoginViewModel()
        {
            userRepository = new UserRepository();
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand);
            LoginCommand = new RelayCommand(ExecuteLoginCommand);
            RegisterCommand = new RelayCommand(ExecuteRegisterCommand);
            ForgotPasswordCommand = new RelayCommand(ExecuteForgotPasswordCommand);
            //RecoverPasswordCommand = new ViewModelCommand(p => ExecuteRecoverPasswordCommand("", ""));
        }

        private bool CanExecuteLoginCommand(object obj)
        {
            // Ensure both fields are filled before enabling the button
            return !string.IsNullOrWhiteSpace(Email) && Password?.Length > 0;
        }
        private void ExecuteRegisterCommand(object parameter)
        {
            // Navigate to RegisterView
            var registerView = new RegisterView();
            registerView.Show();

            // Optionally close the current login window
            Application.Current.Windows[0].Close();
        }

        private void ExecuteForgotPasswordCommand(object parameter)
        {
            // Navigate to RegisterView
            var ForgotPasswordView = new ForgotPasswordView();
            ForgotPasswordView.Show();

            Application.Current.Windows[0].Close();
        }

        private void ExecuteLoginCommand(object obj)
        {
            using (var conn = new SqlConnection(@"Server=(localdb)\LocalInstance;Database=Assignment;Integrated Security=True;Encrypt=False"))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password";
                using (var command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@Password", ConvertToUnsecureString(Password)); // Ensure this is the same way you stored passwords

                    int userCount = (int)command.ExecuteScalar();
                    if (userCount > 0)
                    {
                        // User exists, redirect to UserListView
                        var userListView = new UserListView(); // Make sure to create this view
                        userListView.Show();
                        Application.Current.MainWindow.Close(); // Close login window if applicable
                    }
                    else
                    {
                        // Invalid credentials
                        ErrorMessage = "Invalid email or password.";
                    }
                }
            }
        }

        //private void ExecuteLoginCommand(object parameter)
        //{
        //    using (var connection = new SqlConnection(@"Server=(localdb)\LocalInstance;Database=Assignment;Integrated Security=True;Encrypt=False"))
        //    {
        //        connection.Open();
        //        var query = "SELECT FirstName FROM Users WHERE Email = @Email AND Password = @Password";
        //        using (var command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@Email", Email);
        //            command.Parameters.AddWithValue("@Password", Password);

        //            object result = command.ExecuteScalar();
        //            if (result is string firstName && !string.IsNullOrEmpty(firstName))
        //            {
        //                if (!string.IsNullOrEmpty(firstName))
        //                {
        //                    // Pass FirstName to UserListView
        //                    var userListView = new UserListView();
        //                    var userListViewModel = new UserListViewModel(firstName);
        //                    userListView.DataContext = userListViewModel;

        //                    userListView.Show();
        //                    Application.Current.MainWindow.Close();
        //                }
        //                else
        //                {
        //                    ErrorMessage = "Invalid credentials!";
        //                }
        //            }

        //        }
        //    }
        //}

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
    }
}
