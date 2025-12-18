using System;
using System.Collections.Generic;
using System.Linq;
using ERP_Fix.Models;

namespace ERP_Fix
{
    public partial class ERPManager
    {
        // People Query
        public int GetSectionCount() => sections.Count;
        public List<Section> GetAllSections() => sections;
        public int GetEmployeeCount() => employees.Count;
        public List<Employee> GetAllEmployees() => employees;
        public int GetCustomerCount() => customers.Count;
        public List<Customer> GetAllCustomers() => customers;
        public Company? GetCompany() => company;

        // Finders
        public Section? FindSection(int id) => sections.FirstOrDefault(s => s.Id == id);
        public Customer? FindCustomer(int id) => customers.FirstOrDefault(c => c.Id == id);

        // Company
        public void SetCompany(string companyName, string street, string city, string postalCode, string country, string email, string phoneNumber, string bankName, string iban, string bic)
        {
            company = new Company(companyName, new Address(street, city, postalCode, country), email, phoneNumber, new BankInformation(bankName, iban, bic));
        }

        // Sections
        public Section NewSection(string name)
        {
            Section generated = new Section(lastSectionId + 1, name);
            sections.Add(generated);
            lastSectionId += 1;
            return generated;
        }

        public void DeleteSection(int id) => DeleteSection(sections.FirstOrDefault(s => s.Id == id));

        public void DeleteSection(Section? section)
        {
            if (section == null)
            {
                Console.WriteLine("[ERROR] Section not found.");
                return;
            }

            if (employees.Any(e => e.worksIn.Id == section.Id))
            {
                Console.WriteLine($"[ERROR] Cannot delete Section {section.Id} because it is referenced by one or more Employees.");
                return;
            }

            if (sections.Remove(section))
            {
                Console.WriteLine($"[INFO] Section with ID {section.Id} has been deleted.");
            }
            else
            {
                Console.WriteLine($"[ERROR] Section with ID {section.Id} not found.");
            }
        }

        public void ListSections()
        {
            Console.ForegroundColor = SECTION_INDICATOR_COLOR;
            Console.WriteLine("======= Sections =======");
            Console.ResetColor();
            foreach (Section section in sections)
            {
                Console.WriteLine($"ID: {section.Id}, Name: {section.Name}");
            }
            Console.WriteLine("=========================");
        }

        // Employees
        public Employee NewEmployee(string name, Section worksIn, string Street, string City, string PostalCode, string Country, string Email, string PhoneNumber)
        {
            Employee generated = new Employee(lastEmployeeId + 1, name, worksIn, Street, City, PostalCode, Country, Email, PhoneNumber);
            employees.Add(generated);
            lastEmployeeId += 1;
            return generated;
        }

        public void DeleteEmployee(int id) => DeleteEmployee(employees.FirstOrDefault(e => e.Id == id));

        public void DeleteEmployee(Employee? employee)
        {
            if (employee == null)
            {
                Console.WriteLine("[ERROR] Employee not found.");
                return;
            }
            if (employees.Remove(employee))
            {
                Console.WriteLine($"[INFO] Employee with ID {employee.Id} has been deleted.");
            }
            else
            {
                Console.WriteLine($"[ERROR] Employee with ID {employee.Id} not found.");
            }
        }

        public void ListEmployees()
        {
            Console.ForegroundColor = SECTION_INDICATOR_COLOR;
            Console.WriteLine("======= Employees =======");
            Console.ResetColor();
            foreach (Employee employee in employees)
            {
                Console.WriteLine($"ID: {employee.Id}, Name: {employee.Name}, Works in: {employee.worksIn.Name}");
            }
            Console.WriteLine("=========================");
        }

        // Customers
        public Customer NewCustomer(string name, string Street, string City, string PostalCode, string Country, string Email, string PhoneNumber)
        {
            Customer generated = new Customer(lastCustomerId + 1, name, Street, City, PostalCode, Country, Email, PhoneNumber);
            customers.Add(generated);
            lastCustomerId += 1;
            return generated;
        }

        public void ListCustomers()
        {
            Console.ForegroundColor = SECTION_INDICATOR_COLOR;
            Console.WriteLine("======= Customers =======");
            Console.ResetColor();
            foreach (Customer customer in customers)
            {
                Console.WriteLine($"ID: {customer.Id}, Name: {customer.Name}");
            }
            Console.WriteLine("=========================");
        }

        public void DeleteCustomer(int id) => DeleteCustomer(customers.FirstOrDefault(c => c.Id == id));

        public void DeleteCustomer(Customer? customer)
        {
            if (customer == null)
            {
                Console.WriteLine("[ERROR] Customer not found.");
                return;
            }

            if (orders.Any(o => o.Customer.Id == customer.Id) || bills.Any(b => b.Customer.Id == customer.Id))
            {
                Console.WriteLine($"[ERROR] Cannot delete Customer {customer.Id} because it is referenced by Orders or Bills.");
                return;
            }

            if (customers.Remove(customer))
            {
                Console.WriteLine($"[INFO] Customer with ID {customer.Id} has been deleted.");
            }
            else
            {
                Console.WriteLine($"[ERROR] Customer with ID {customer.Id} not found.");
            }
        }
    }
}
