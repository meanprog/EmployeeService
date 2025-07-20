using EmployeeService.Models;
using EmployeeService.UpdateDtos;

namespace EmployeeService.Providers;

public interface IEmployeeProvider
{
    Task<int> AddEmployeeAsync(Employee employee);
    Task<bool> DeleteEmployeeAsync(int id);
    Task<IEnumerable<Employee>> GetEmployeesByCompanyAsync(int companyId);
    Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int companyId, string departmentName);
    Task<bool> UpdateEmployeeAsync(int id, EmployeeUpdateDto updateDto);
}