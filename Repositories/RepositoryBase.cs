using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Assignment.View.Repositories
{
    public abstract class RepositoryBase
    {
        private readonly string connectionString;
        public RepositoryBase() 
        {
            connectionString = @"Data Source=(localdb)\LocalInstance;Initial Catalog=Assignment;Integrated Security=True;";

        }
        protected SqlConnection GetConnection() 
        {
            return new SqlConnection(connectionString);
        }
    }
}
