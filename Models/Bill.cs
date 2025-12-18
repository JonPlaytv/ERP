using System;

namespace ERP_Fix.Models
{
    public class Bill : ERPItem
    {
        public int Id { get; }
        public double TotalPrice { get; }
        public Order Order { get; }
        public Customer Customer { get; set; }
        public PaymentTerms PaymentTerms { get; set; }
        public Prices Prices { get; set; }
        public DateOnly BillDate { get; set; }

        public Bill(int id, double totalPrice, Order order, PaymentTerms paymentTerms, Prices prices)
        {
            Id = id;
            TotalPrice = totalPrice;
            Order = order;
            Customer = order.Customer;
            PaymentTerms = paymentTerms;
            BillDate = DateOnly.FromDateTime(DateTime.Now);
            Prices = prices;
        }
    }
}
