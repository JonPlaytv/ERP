using System;

namespace ERP_Fix.Models
{
    public class PaymentTerms : ERPItem
    {
        public int Id { get; }
        public string Name { get; set; }
        public int DaysUntilDue { get; set; }
        public int? DiscountDays { get; set; }
        public double? DiscountPercent { get; set; }

        public double? PenaltyRate { get; set; } // default interest
        public double AbsolutePenalty { get; set; }
        public bool UsingPenaltyRate { get; }

        public PaymentTerms(int id, string name, int daysUntilDue, double absolutePenalty, int? discountDays = null, double? discountPercent = null, double? penaltyRate = null)
        {
            Id = id;
            Name = name;
            DaysUntilDue = daysUntilDue;
            DiscountDays = discountDays;
            DiscountPercent = discountPercent;
            PenaltyRate = penaltyRate;
            AbsolutePenalty = absolutePenalty;
            UsingPenaltyRate = penaltyRate.HasValue && penaltyRate.Value > 0;
        }

        public static DateOnly GetDueDate(DateOnly InvoiceDate, int DaysUntilDue)
        {
            return InvoiceDate.AddDays(DaysUntilDue);
        }

        public static DateTime? GetDiscountDate(DateTime invoiceDate, int? discountDays)
        {
            return discountDays.HasValue ? invoiceDate.AddDays(discountDays.Value) : null;
        }

        public static double GetDiscountAmount(double totalAmount, double? discountPercent)
        {
            return discountPercent.HasValue ? totalAmount * (double)discountPercent : 0;
        }
    }
}
