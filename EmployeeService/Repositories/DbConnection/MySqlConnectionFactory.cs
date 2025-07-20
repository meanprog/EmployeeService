using System.Data.Common;
using Microsoft.Extensions.Configuration;

namespace EmployeeService.Repositories.DbConnection
{
    public class MySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public MySqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public System.Data.Common.DbConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("MySQL connection string is not configured");
            
            return new MySql.Data.MySqlClient.MySqlConnection(connectionString);
        }
    }
}