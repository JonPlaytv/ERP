using System;
using System.Collections.Generic;

namespace ERP_Fix.Models
{
    public class Order : ERPItem
    {
        public int Id { get; }
        public List<OrderItem> Articles { get; set; }
        public Customer Customer { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public Order(int id, List<OrderItem> articles, Customer customer)
        {
            Id = id;
            Articles = articles;
            Customer = customer;
        }
    }
}
