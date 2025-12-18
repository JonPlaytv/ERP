using System;

namespace ERP_Fix.Models
{
    public class Employee : Person
    {
        public Section worksIn { get; set; }

        public Employee(int id, string name, Section worksIn, string Street, string City, string PostalCode, string Country, string Email, string PhoneNumber)
        {
            Id = id;
            Name = name;
            Type = PersonType.Employee;
            this.worksIn = worksIn;

            Information = new PersonInformation(
                new Address(Street, City, PostalCode, Country),
                new ContactInformation(Email, PhoneNumber)
            );
        }
    }
}
