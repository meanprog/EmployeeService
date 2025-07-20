
using System.Data.Common;
using Dapper;
using EmployeeService.Models;
using EmployeeService.Repositories.DbConnection;
using EmployeeService.UpdateDtos;

namespace EmployeeService.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public EmployeeRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<int> AddEmployeeAsync(Employee employee)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            var departmentId = await connection.QueryFirstOrDefaultAsync<int?>(
                @"SELECT Id FROM Departments 
                WHERE CompanyId = @CompanyId AND Name = @Name AND Phone = @Phone",
                new { employee.CompanyId, employee.Department.Name, employee.Department.Phone },
                transaction);

            if (departmentId == null)
            {
                departmentId = await connection.ExecuteScalarAsync<int>(
                    @"INSERT INTO Departments (CompanyId, Name, Phone) 
                    VALUES (@CompanyId, @Name, @Phone);
                    SELECT LAST_INSERT_ID();",
                    new { employee.CompanyId, employee.Department.Name, employee.Department.Phone },
                    transaction);
            }
            
            var passportId = await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO Passports (Type, Number) 
                VALUES (@Type, @Number);
                SELECT LAST_INSERT_ID();",
                employee.Passport,
                transaction);
            
            var employeeId = await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO Employees (Name, Surname, Phone, CompanyId, DepartmentId, PassportId) 
                VALUES (@Name, @Surname, @Phone, @CompanyId, @DepartmentId, @PassportId);
                SELECT LAST_INSERT_ID();",
                new
                {
                    employee.Name,
                    employee.Surname,
                    employee.Phone,
                    employee.CompanyId,
                    DepartmentId = departmentId,
                    PassportId = passportId
                },
                transaction);

            transaction.Commit();
            return employeeId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>(
            "SELECT COUNT(1) FROM Employees WHERE Id = @Id",
            new { Id = id });
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var affectedRows = await connection.ExecuteAsync(
            @"DELETE FROM Employees WHERE Id = @Id",
            new { Id = id });

        return affectedRows > 0;
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByCompanyAsync(int companyId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var query = @"
            SELECT e.Id, e.Name, e.Surname, e.Phone, e.CompanyId,
                   p.Type, p.Number,
                   d.Name, d.Phone
            FROM Employees e
            JOIN Passports p ON e.PassportId = p.Id
            JOIN Departments d ON e.DepartmentId = d.Id
            WHERE e.CompanyId = @CompanyId";

        var employees = await connection.QueryAsync<Employee, Passport, Department, Employee>(
            query,
            (employee, passport, department) =>
            {
                employee.Passport = passport;
                employee.Department = department;
                return employee;
            },
            new { CompanyId = companyId },
            splitOn: "Type,Name");

        return employees;
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int companyId, string departmentName)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        var query = @"
            SELECT e.Id, e.Name, e.Surname, e.Phone, e.CompanyId,
                   p.Type, p.Number,
                   d.Name, d.Phone
            FROM Employees e
            JOIN Passports p ON e.PassportId = p.Id
            JOIN Departments d ON e.departmentId = d.Id
            WHERE e.CompanyId = @CompanyId AND d.Name = @DepartmentName";

        var employees = await connection.QueryAsync<Employee, Passport, Department, Employee>(
            query,
            (employee, passport, department) =>
            {
                employee.Passport = passport;
                employee.Department = department;
                return employee;
            },
            new { CompanyId = companyId, DepartmentName = departmentName },
            splitOn: "Type,Name");

        return employees;
    }

    public async Task<bool> UpdateEmployeeAsync(int id, EmployeeUpdateDto updateDto)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            if (!await connection.ExecuteScalarAsync<bool>(
                    "SELECT COUNT(1) FROM Employees WHERE Id = @Id",
                    new { Id = id },
                    transaction))
                return false;
            
            var employeeUpdated = await UpdateEmployeeCoreFields(connection, transaction, id, updateDto);
            
            if (updateDto.Passport != null)
                await UpdatePassportData(connection, transaction, id, updateDto.Passport);

            if (updateDto.Department != null)
                await UpdateDepartmentData(connection, transaction, id, updateDto.Department);

            await transaction.CommitAsync();
            return employeeUpdated;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<bool> UpdateEmployeeCoreFields(
        System.Data.Common.DbConnection connection,
        DbTransaction transaction,
        int id,
        EmployeeUpdateDto updateDto)
    {
        var fields = new List<string>();
        var parameters = new DynamicParameters();
        parameters.Add("Id", id);

        if (updateDto.Name != null)
        {
            fields.Add("Name = @Name");
            parameters.Add("Name", updateDto.Name);
        }

        if (updateDto.Surname != null)
        {
            fields.Add("Surname = @Surname");
            parameters.Add("Surname", updateDto.Surname);
        }

        if (updateDto.Phone != null)
        {
            fields.Add("Phone = @Phone");
            parameters.Add("Phone", updateDto.Phone);
        }

        if (updateDto.CompanyId != null)
        {
            fields.Add("CompanyId = @CompanyId");
            parameters.Add("CompanyId", updateDto.CompanyId);
        }

        if (fields.Count == 0)
            return false;

        var query = $"UPDATE Employees SET {string.Join(", ", fields)} WHERE Id = @Id";
        return await connection.ExecuteAsync(query, parameters, transaction) > 0;
    }

    private async Task UpdatePassportData(
        System.Data.Common.DbConnection connection,
        DbTransaction transaction,
        int employeeId,
        PassportUpdateDto passportDto)
    {
        var passportId = await connection.ExecuteScalarAsync<int>(
            "SELECT PassportId FROM Employees WHERE Id = @EmployeeId",
            new { EmployeeId = employeeId },
            transaction);

        var fields = new List<string>();
        var parameters = new DynamicParameters();
        parameters.Add("Id", passportId);

        if (passportDto.Type != null)
        {
            fields.Add("Type = @Type");
            parameters.Add("Type", passportDto.Type);
        }

        if (passportDto.Number != null)
        {
            fields.Add("Number = @Number");
            parameters.Add("Number", passportDto.Number);
        }

        if (fields.Count > 0)
        {
            await connection.ExecuteAsync(
                $"UPDATE Passports SET {string.Join(", ", fields)} WHERE Id = @Id",
                parameters,
                transaction);
        }
    }

    private async Task UpdateDepartmentData(
        System.Data.Common.DbConnection connection,
        DbTransaction transaction,
        int employeeId,
        DepartmentUpdateDto departmentDto)
    {
        var current = await connection.QueryFirstOrDefaultAsync<Employee>(
            "SELECT CompanyId, DepartmentId FROM Employees WHERE Id = @Id",
            new { Id = employeeId },
            transaction);

        var companyId = departmentDto.CompanyId ?? current.CompanyId;

        var departmentId = await connection.QueryFirstOrDefaultAsync<int?>(
            "SELECT Id FROM Departments WHERE CompanyId = @CompanyId AND Name = @Name",
            new { CompanyId = companyId, departmentDto.Name },
            transaction) ?? await connection.ExecuteScalarAsync<int>(
            @"INSERT INTO Departments (CompanyId, Name, Phone) 
        VALUES (@CompanyId, @Name, @Phone);
        SELECT LAST_INSERT_ID()",
            new
            {
                CompanyId = companyId,
                departmentDto.Name,
                departmentDto.Phone
            },
            transaction);

        await connection.ExecuteAsync(
            "UPDATE Employees SET DepartmentId = @DepartmentId WHERE Id = @EmployeeId",
            new { DepartmentId = departmentId, EmployeeId = employeeId },
            transaction);
    }
}