using System;

namespace ERP_Fix.Models
{
    public class PersonInformation : ERPItem
    {
        public Address Address { get; set; }
        public ContactInformation ContactInformation { get; set; }

        public PersonInformation(Address address, ContactInformation contactInformation)
        {
            Address = address;
            ContactInformation = contactInformation;
        }
    }
}
