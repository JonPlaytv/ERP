using System;
using System.Collections.Generic;

namespace ERP_Fix.DTOs
{
    // =========================
    // DTOs for .erp JSON format
    // =========================
    internal class ERPInstanceSnapshot
    {
        public int SchemaVersion { get; set; }
        public string InstanceName { get; set; } = string.Empty;
        public string FileBaseName { get; set; } = string.Empty;

        public double OwnCapital { get; set; }
        public bool OwnCapitalSet { get; set; }
        public Dictionary<int, int> WantedStockByTypeId { get; set; } = new();

        public DTO_LastIds LastIds { get; set; } = new DTO_LastIds();

        public List<DTO_ArticleType> ArticleTypes { get; set; } = new();
        public List<DTO_Article> Articles { get; set; } = new();
        public List<DTO_StorageSlot> StorageSlots { get; set; } = new();
        public List<DTO_Order> Orders { get; set; } = new();
        public List<DTO_SelfOrder> SelfOrders { get; set; } = new();
        public List<DTO_Prices> Prices { get; set; } = new();
        public List<DTO_Bill> Bills { get; set; } = new();
        public List<DTO_PaymentTerms> PaymentTerms { get; set; } = new();
        public List<DTO_Section> Sections { get; set; } = new();
        public List<DTO_Employee> Employees { get; set; } = new();
        public List<DTO_Customer> Customers { get; set; } = new();
    }

    // =========================
    // DTOs for .erps (secrets) JSON format
    // =========================
    internal class ERPSecretsSnapshot
    {
        public int SchemaVersion { get; set; }
        public string InstanceName { get; set; } = string.Empty;
        public string FileBaseName { get; set; } = string.Empty;
        public string Warning { get; set; } = string.Empty;
        public DTO_Company? Company { get; set; }
    }

    internal class DTO_Company
    {
        public string Name { get; set; } = string.Empty;
        public DTO_Address? Address { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DTO_BankInformation? BankInfo { get; set; }
    }

    internal class DTO_Address
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    internal class DTO_BankInformation
    {
        public string BankName { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty;
        public string BIC { get; set; } = string.Empty;
    }

    internal class DTO_LastIds
    {
        public int LastStockId { get; set; }
        public int LastSlotId { get; set; }
        public int LastArticleTypeId { get; set; }
        public int LastOrderId { get; set; }
        public int LastSelfOrderId { get; set; }
        public int LastPricesId { get; set; }
        public int LastBillId { get; set; }
        public int LastPaymentTermsId { get; set; }
        public int LastSectionId { get; set; }
        public int LastEmployeeId { get; set; }
        public int LastCustomerId { get; set; }
        public int LastOrderItemId { get; set; }
    }

    internal class DTO_ArticleType { public int Id { get; set; } public string Name { get; set; } = string.Empty; }
    internal class DTO_Article { public int Id { get; set; } public int TypeId { get; set; } public int Stock { get; set; } public long ScannerId { get; set; } }
    internal class DTO_StorageSlot { public int Id { get; set; } public List<int> FillArticleIds { get; set; } = new(); }
    internal class DTO_OrderItem { public int Id { get; set; } public int TypeId { get; set; } public int Stock { get; set; } }
    internal class DTO_Order { public int Id { get; set; } public int CustomerId { get; set; } public string Status { get; set; } = string.Empty; public List<DTO_OrderItem> Articles { get; set; } = new(); }
    internal class DTO_SelfOrder { public int Id { get; set; } public string Status { get; set; } = string.Empty; public List<DTO_OrderItem> Articles { get; set; } = new(); public List<DTO_OrderItem> Arrived { get; set; } = new(); }
    internal class DTO_Prices { public int Id { get; set; } public Dictionary<int, double> PriceListByTypeId { get; set; } = new(); }
    internal class DTO_Bill { public int Id { get; set; } public double TotalPrice { get; set; } public int OrderId { get; set; } public int CustomerId { get; set; } public int PaymentTermsId { get; set; } public int PricesId { get; set; } }
    internal class DTO_PaymentTerms { public int Id { get; set; } public string Name { get; set; } = string.Empty; public int DaysUntilDue { get; set; } public int? DiscountDays { get; set; } public double? DiscountPercent { get; set; } public double? PenaltyRate { get; set; } public double AbsolutePenalty { get; set; } public bool UsingPenaltyRate { get; set; } }
    internal class DTO_Section { public int Id { get; set; } public string Name { get; set; } = string.Empty; }
    internal class DTO_Employee { public int Id { get; set; } public string Name { get; set; } = string.Empty; public int WorksInSectionId { get; set; } public string Street { get; set; } = string.Empty; public string City { get; set; } = string.Empty; public string PostalCode { get; set; } = string.Empty; public string Country { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string PhoneNumber { get; set; } = string.Empty; }
    internal class DTO_Customer { public int Id { get; set; } public string Name { get; set; } = string.Empty; public string Street { get; set; } = string.Empty; public string City { get; set; } = string.Empty; public string PostalCode { get; set; } = string.Empty; public string Country { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string PhoneNumber { get; set; } = string.Empty; }
}
