using EmployeeService.Models;
using EmployeeService.Providers;
using EmployeeService.UpdateDtos;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeProvider _employeeProvider;

    public EmployeesController(IEmployeeProvider employeeProvider)
    {
        _employeeProvider = employeeProvider;
    }

    [HttpPost]
    public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _employeeProvider.AddEmployeeAsync(employee);
        return Ok(new { Id = id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var result = await _employeeProvider.DeleteEmployeeAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("company/{companyId}")]
    public async Task<IActionResult> GetEmployeesByCompany(int companyId)
    {
        var employees = await _employeeProvider.GetEmployeesByCompanyAsync(companyId);
        return Ok(employees);
    }

    [HttpGet("company/{companyId}/department/{departmentName}")]
    public async Task<IActionResult> GetEmployeesByDepartment(int companyId, string departmentName)
    {
        var employees = await _employeeProvider.GetEmployeesByDepartmentAsync(companyId, departmentName);
        return Ok(employees);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeUpdateDto employeeUpdate)
    {
        var result = await _employeeProvider.UpdateEmployeeAsync(id, employeeUpdate);
        return result ? NoContent() : NotFound();
    }
}