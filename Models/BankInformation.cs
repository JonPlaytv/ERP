using System;

namespace ERP_Fix.Models
{
    public class BankInformation
    {
        public string BankName { get; set; }
        public string IBAN { get; set; }
        public string BIC { get; set; }

        public BankInformation(string bankName, string iban, string bic)
        {
            BankName = bankName;
            IBAN = iban;
            BIC = bic;
        }
    }
}
