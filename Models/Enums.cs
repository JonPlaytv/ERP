using System;

namespace ERP_Fix.Models
{
    public enum OrderStatus
    {
        Pending,
        Completed,
        Cancelled
    }

    public enum PersonType
    {
        Employee = 0,
        Customer = 1
    }
}
