using EmployeeService.Models;
using EmployeeService.UpdateDtos;

namespace EmployeeService.Repositories;

public interface IEmployeeRepository
{
    Task<int> AddEmployeeAsync(Employee employee);
    Task<bool> DeleteEmployeeAsync(int id);
    Task<IEnumerable<Employee>> GetEmployeesByCompanyAsync(int companyId);
    Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int companyId, string departmentName);
    Task<bool> UpdateEmployeeAsync(int id, EmployeeUpdateDto employeeUpdate);
    Task<bool> ExistsAsync(int id);
}