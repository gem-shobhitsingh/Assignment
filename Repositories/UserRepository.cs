using Assignment.View.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.View.Repositories
{
    public class UserRepository : RepositoryBase
    {
        public void Add(UserModel userModel)
        {
            throw new NotImplementedException();
        }

        public bool AuthenticateUser(NetworkCredential credential)
        {
            bool validUser;
            using (var connection = GetConnection())
            using(var command=new  SqlCommand())
            {
                if(connection.State == ConnectionState.Closed) 
                {
                    connection.Open();
                }
                command.Connection = connection;
                command.CommandText = "Select * from Users WHERE Email=@username and Password=@password";
                command.Parameters.Add("@username",SqlDbType.VarChar).Value = credential.UserName;
                command.Parameters.Add("@password",SqlDbType.VarChar).Value = credential.Password;
                validUser = command.ExecuteScalar() == null ? false : true;
            }
                return validUser;
        }

    }
}
