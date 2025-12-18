using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ERP_Fix.Models;

namespace ERP_Fix
{
    public partial class ERPManager
    {
        // Finance Query
        public int GetPricesCount() => prices.Count;
        public List<Prices> GetAllPrices() => prices;
        public int GetPaymentTermsCount() => paymentTerms.Count;
        public List<PaymentTerms> GetAllPaymentTerms() => paymentTerms;

        // Capital
        public void SetOwnCapital(double newOwnCapital)
        {
            ownCapital = Math.Round(newOwnCapital, 2);
            Console.WriteLine($"[INFO] Own capital set to {FormatAmount(ownCapital)}");
        }

        public void AddOwnCapital(double capitalToAdd)
        {
            ownCapital += Math.Round(capitalToAdd, 2);
            Console.WriteLine($"[INFO] Added {FormatAmount(capitalToAdd)} to own capital. Own capital: {FormatAmount(ownCapital)}");
        }

        public void RemoveOwnCapital(double capitalToRemove)
        {
            if (ownCapital >= capitalToRemove)
            {
                ownCapital -= Math.Round(capitalToRemove, 2);
                Console.WriteLine($"[INFO] Removed {FormatAmount(capitalToRemove)} from own capital. Own capital: {FormatAmount(ownCapital)}");
            }
            else
                Console.WriteLine($"[ERROR] Can't remove {FormatAmount(capitalToRemove)} from {FormatAmount(ownCapital)} own capital");
        }

        public void SetCurrency(string cultureName)
        {
            var cultureInfo = CultureInfo.CurrentCulture;

            try
            {
                cultureInfo = new CultureInfo(cultureName);
            }
            catch (Exception)
            {
                Console.WriteLine($"[ERROR] Invalid culture name '{cultureName}'");
                return;
            }

            currentCurrencyFormat = (NumberFormatInfo)cultureInfo.NumberFormat.Clone();
            currentCurrencyFormat.CurrencyDecimalDigits = 2;
        }

        public string FormatAmount(double amount)
        {
            return amount.ToString("C", currentCurrencyFormat);
        }

        // Prices
        public Prices NewPrices(Dictionary<ArticleType, double> priceList)
        {
            Prices generated = new Prices(lastPricesId + 1, priceList);
            prices.Add(generated);
            lastPricesId += 1;
            return generated;
        }

        public void DeletePrices(int id) => DeletePrices(prices.FirstOrDefault(p => p.Id == id));

        public void DeletePrices(Prices? pricesEntry)
        {
            if (pricesEntry == null)
            {
                Console.WriteLine("[ERROR] Prices entry not found.");
                return;
            }
            if (prices.Remove(pricesEntry))
            {
                Console.WriteLine($"[INFO] Prices with ID {pricesEntry.Id} has been deleted.");
            }
            else
            {
                Console.WriteLine($"[ERROR] Prices with ID {pricesEntry.Id} not found.");
            }
        }

        public void ListPrices()
        {
            Console.ForegroundColor = SECTION_INDICATOR_COLOR;
            Console.WriteLine("========= Prices ========");
            Console.ResetColor();
            foreach (Prices price in prices)
            {
                Console.WriteLine($"ID: {price.Id}");
                foreach (var entry in price.PriceList)
                {
                    Console.WriteLine($"ArticleType-ID: {entry.Key.Id}, Name: {entry.Key.Name}, Price: {entry.Value}");
                }
            }
            Console.WriteLine("=========================");
        }

        // Payment Terms
        public PaymentTerms? NewPaymentTerms(string name, object daysUntilDue, double absolutePenalty, int? discountDays = 0, double? discountPercent = 0.00, double? penaltyRate = 0.00)
        {
            int cDaysUntilDue;
            if (daysUntilDue is int)
                cDaysUntilDue = (int)daysUntilDue;
            else if (daysUntilDue is DateTime)
                cDaysUntilDue = Math.Abs((DateTime.Now.Date - (DateTime)daysUntilDue).Days);
            else
            {
                Console.WriteLine($"[ERROR] daysUntilDue must be an int or DateTime => {daysUntilDue.GetType()} is not allowed");
                return null;
            }

            PaymentTerms generated = new PaymentTerms(lastPaymentTermsId + 1, name, cDaysUntilDue, absolutePenalty, discountDays, discountPercent, penaltyRate);
            paymentTerms.Add(generated);
            lastPaymentTermsId += 1;
            return generated;
        }

        public void DeletePaymentTerms(int id) => DeletePaymentTerms(paymentTerms.FirstOrDefault(t => t.Id == id));

        public void DeletePaymentTerms(PaymentTerms? terms)
        {
            if (terms == null)
            {
                Console.WriteLine("[ERROR] Payment terms not found.");
                return;
            }

            if (bills.Any(b => b.PaymentTerms.Id == terms.Id))
            {
                Console.WriteLine($"[ERROR] Cannot delete PaymentTerms {terms.Id} because it is referenced by a Bill.");
                return;
            }

            if (paymentTerms.Remove(terms))
            {
                Console.WriteLine($"[INFO] Payment terms with ID {terms.Id} has been deleted.");
            }
            else
            {
                Console.WriteLine($"[ERROR] Payment terms with ID {terms.Id} not found.");
            }
        }

        public void ListPaymentTerms()
        {
            Console.ForegroundColor = SECTION_INDICATOR_COLOR;
            Console.WriteLine("===== Payment Terms =====");
            Console.ResetColor();
            foreach (PaymentTerms terms in paymentTerms)
            {
                Console.WriteLine($"ID: {terms.Id}, Name: {terms.Name}, Days Until Due: {terms.DaysUntilDue}, Discount Days: {terms.DiscountDays}, Discount Percent: {terms.DiscountPercent}, Penalty Rate: {terms.PenaltyRate}, Absolute Penalty: {terms.AbsolutePenalty}, Using Penalty Rate: {terms.UsingPenaltyRate.ToString()}");
            }
            Console.WriteLine("=========================");
        }
    }
}
