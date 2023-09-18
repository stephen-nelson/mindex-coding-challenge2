using CodeChallenge.Extensions;
using CodeChallenge.Models;
using CodeChallenge.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public ReportingStructure GetReportingStructure(Employee employee)
        {
            ReportingStructure structure = null;

            if (employee != null)
            {
                structure = new ReportingStructure()
                {
                    Employee = employee,
                    NumberOfReports = CalculateNumberOfReportingEmployees(employee)
                };
            }

            return structure;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        private static int CalculateNumberOfReportingEmployees(Employee employee)
        {
            Queue<Employee> queue = new Queue<Employee>();
            queue.EnqueueAll(employee.DirectReports);
            int numReports = 0;

            while (queue.Count > 0)
            {
                Employee e = queue.Dequeue();
                numReports++;
                queue.EnqueueAll(e.DirectReports);
            }
            
            return numReports;
        }

        public Compensation GetCompensationByEmployeeId(string id)
        {
            Compensation compensation;

            if (!String.IsNullOrEmpty(id))
            {
                compensation = _employeeRepository.GetCompensationByEmployeeId(id);
            }
            else
            {
                compensation = null;
            }

            return compensation;
        }

        public Compensation Create(Compensation compensation)
        {
            Compensation created = null;

            if (compensation != null)
            {
                Employee employee = _employeeRepository.GetById(compensation.Employee?.EmployeeId);

                if (employee != null)
                {
                    compensation.Employee = employee;

                    created = _employeeRepository.Add(compensation);
                    _employeeRepository.SaveAsync().Wait();
                }
            }

            return created;
        }
    }
}
