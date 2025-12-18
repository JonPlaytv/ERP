using System;

namespace ERP_Fix.Models
{
    public class Company
    {
        public string Name { get; set; }
        public Address Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public BankInformation BankInfo { get; set; }

        public Company(string name, Address address, string email, string phoneNumber, BankInformation bankInfo)
        {
            Name = name;
            Address = address;
            Email = email;
            PhoneNumber = phoneNumber;
            BankInfo = bankInfo;
        }
    }
}
