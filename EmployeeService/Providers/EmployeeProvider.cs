using EmployeeService.Models;
using EmployeeService.Repositories;
using EmployeeService.UpdateDtos;

namespace EmployeeService.Providers;

public class EmployeeProvider : IEmployeeProvider
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<EmployeeProvider> _logger;

    public EmployeeProvider(
        IEmployeeRepository employeeRepository,
        ILogger<EmployeeProvider> logger)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    public async Task<int> AddEmployeeAsync(Employee employee)
    {
        try
        {
            _logger.LogInformation("Добавление нового сотрудника");
            return await _employeeRepository.AddEmployeeAsync(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при добавлении сотрудника");
            throw;
        }
    }
    

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        _logger.LogInformation("Удаление сотрудника с ID {EmployeeId}", id);
        return await _employeeRepository.DeleteEmployeeAsync(id);
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByCompanyAsync(int companyId)
    {
        _logger.LogInformation("Получение сотрудников компании {CompanyId}", companyId);
        return await _employeeRepository.GetEmployeesByCompanyAsync(companyId);
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int companyId, string departmentName)
    {
        _logger.LogInformation(
            "Получение сотрудников отдела {DepartmentName} в компании {CompanyId}", 
            departmentName, companyId);
        return await _employeeRepository.GetEmployeesByDepartmentAsync(companyId, departmentName);
    }
    
    public async Task<bool> UpdateEmployeeAsync(int id, EmployeeUpdateDto updateDto)
    {
        try
        {
            _logger.LogInformation("Обновление сотрудника с ID {EmployeeId}", id);
        
            if (!await _employeeRepository.ExistsAsync(id))
            {
                _logger.LogWarning("Сотрудник с ID {EmployeeId} не найден", id);
                return false;
            }

            return await _employeeRepository.UpdateEmployeeAsync(id, updateDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении сотрудника с ID {EmployeeId}", id);
            throw;
        }
    }
}