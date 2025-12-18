using System;

namespace ERP_Fix.Models
{
    public class Customer : Person
    {
        public Customer(int id, string name, string Street, string City, string PostalCode, string Country, string Email, string PhoneNumber)
        {
            Id = id;
            Name = name;
            Type = PersonType.Customer;
            Information = new PersonInformation(
                new Address(Street, City, PostalCode, Country),
                new ContactInformation(Email, PhoneNumber)
            );
        }
    }
}
