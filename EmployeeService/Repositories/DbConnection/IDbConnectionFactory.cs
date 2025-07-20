

namespace EmployeeService.Repositories.DbConnection
{
    public interface IDbConnectionFactory
    {
        System.Data.Common.DbConnection CreateConnection();
    }
    
}