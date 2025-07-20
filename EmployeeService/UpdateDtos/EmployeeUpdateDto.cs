namespace EmployeeService.UpdateDtos;

public class EmployeeUpdateDto
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Phone { get; set; }
    public int? CompanyId { get; set; }
    public PassportUpdateDto? Passport { get; set; }
    public DepartmentUpdateDto? Department { get; set; }
}
