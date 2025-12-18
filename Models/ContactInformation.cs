using System;

namespace ERP_Fix.Models
{
    public class ContactInformation : ERPItem
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public ContactInformation(string email, string phoneNumber)
        {
            Email = email;
            PhoneNumber = phoneNumber;
        }
    }
}
