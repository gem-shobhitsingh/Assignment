using Assignment.View.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using Assignment.View.Model;
using Assignment.ViewModel;


namespace Assignment.View.ViewModel
{
    public class UserListViewModel : ViewModelBase
    {
        private ObservableCollection<UserDisplayModel> _data;
        public ObservableCollection<UserDisplayModel> Data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged(nameof(Data));
            }
        }

        private bool _isViewVisible = true;
        public bool IsViewVisible
        {
            get => _isViewVisible;
            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }
        private string _SearchText;
        public string SearchText
        {
            get => _SearchText;
            set
            {
                _SearchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }


        public ICommand LogoutCommand { get; }
        public ICommand SearchCommand { get; }

        public UserListViewModel()
        {
            LogoutCommand = new ViewModelCommand(p=>Logout());
            SearchCommand = new ViewModelCommand(p => displayResult());
            DisplayData();
        }

        
        private void DisplayData()
        {
            var dataList = new ObservableCollection<UserDisplayModel>();

            using (var connection = new SqlConnection(@"Server=(localdb)\LocalInstance;Database=Assignment;Integrated Security=True;Encrypt=False"))
            using (var command = new SqlCommand())
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string query = "SELECT FirstName,LastName,Gender,DOB,Email FROM Users";
                command.CommandText = query;
                command.Connection = connection;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dataItem = new UserDisplayModel();
                        {
                            dataItem.FirstName = reader.GetString(0);
                            dataItem.LastName = reader.GetString(1);
                            dataItem.Gender = reader.GetString(2);
                            dataItem.DOB = reader.GetDateTime(3);
                            dataItem.Email = reader.GetString(4);
                        };
                        dataList.Add(dataItem);
                    }
                }
            }
            Data = dataList;
        }

        private void displayResult()
        {
            var filteredData = new ObservableCollection<UserDisplayModel>();
            string search = SearchText.Trim();

            string sqlQuery = "SELECT FirstName,LastName,Gender,DOB,Email FROM Users WHERE " +
            "FirstName LIKE @search OR LastName LIKE @search OR " +
            "Gender LIKE @search OR DOB LIKE @search OR Email LIKE @search";

            if(string.IsNullOrEmpty(search))
            {
                string query = "SELECT FirstName,LastName,Gender,DOB,Email FROM Users";
            }


            using (SqlConnection connection = new SqlConnection(@"Server=(localdb)\LocalInstance;Database=Assignment;Integrated Security=True;Encrypt=False"))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.CommandText = sqlQuery;
                command.Connection = connection;
                command.Parameters.AddWithValue("@search", "%" + search + "%");



                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dataItem = new UserDisplayModel();
                        {
                            dataItem.FirstName = reader.GetString(0);
                            dataItem.LastName = reader.GetString(1);
                            dataItem.Gender = reader.GetString(2);
                            dataItem.DOB = reader.GetDateTime(3);
                            dataItem.Email = reader.GetString(4);
                        };



                        filteredData.Add(dataItem);
                    }
                }



            }
            Data = filteredData;
        }
        private void Logout()
        {
            IsViewVisible = false;
            var loginView = new LoginView();
            loginView.Show();
            Application.Current.Windows[0].Close();

        }

    }
}
