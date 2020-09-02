using NetCoreHooks.Contracts;
using NetCoreHooks.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreHooks.Data;

namespace NetCoreHooks.Services
{
    public class EmployeeService : IEmployeeRepository
    {
        private readonly ApplicationDbContext _db;

        public EmployeeService(ApplicationDbContext context)
        {
            _db = context;
        }
        public async Task<IList<Employee>> FindAll()
        {
            var employees = await _db.Employees.ToListAsync();
            return employees;
        }

        public async Task<Employee> FindById(int id)
        {
            var employee = await _db.Employees.FindAsync(id);
            return employee;
        }

        public async Task<Employee> FindByUserName(string userName)
        {
            var employee = await _db.Employees
                .Where(n => n.UserName == userName)
                .FirstOrDefaultAsync();                
            return employee;
        }
    }
}
