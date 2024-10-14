using Assignment.View.Repositories;
using Assignment.View.View;
using Assignment.View.ViewModel;
using Assignment.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Assignment.ViewModels
{
    public class ForgotPasswordViewModel : ViewModelBase
    {
        private string _email;
        private String _password;
        private String _confirmpassword;
        private string _errorMessage;

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
        public String Password
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
        public String ConfirmPassword
        {
            get
            {
                return _confirmpassword;
            }
            set
            {
                _confirmpassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        public ICommand ResetCommand { get; }


        //Constructor
        public ForgotPasswordViewModel()
        {
            //ResetCommand = new RelayCommand(ExecuteResetPasswordCommand);
            ResetCommand = new ViewModelCommand(ExecuteResetPasswordCommand);
        }

        private void ExecuteResetPasswordCommand(object obj)
        {

            if (Password == ConfirmPassword)
            {
                using (var conn = new SqlConnection(@"Server=(localdb)\LocalInstance;Database=Assignment;Integrated Security=True;Encrypt=False"))
                {
                    conn.Open();
                    string query = "UPDATE Users SET Password = @Password WHERE Email = @Email";
                    using (var command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Password", Password);
                        command.Parameters.AddWithValue("@Email", Email);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            var loginView = new LoginView();
                            loginView.Show();
                            Application.Current.Windows[0].Close();
                            MessageBox.Show("Password changed Successfully", "Congratulations", MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                        else
                        {
                            ErrorMessage = "Email not found or password could not be updated.";
                        }
                    }
                }
            }
            else
            {
                ErrorMessage = "Password and Confirm Password do not match.";
            }
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
    }
}
