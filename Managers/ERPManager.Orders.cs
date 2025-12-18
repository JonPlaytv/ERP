using System;
using System.Collections.Generic;
using System.Linq;
using ERP_Fix.Models;

namespace ERP_Fix
{
    public partial class ERPManager
    {
        // Queries
        public int GetOrderCount() => orders.Count;
        public List<Order> GetAllOrders() => orders;
        public int GetSelfOrderCount() => selfOrders.Count;
        public List<SelfOrder> GetAllSelfOrders() => selfOrders;
        public int GetBillCount() => bills.Count;
        public List<Bill> GetAllBills() => bills;

        // Logic
        public Dictionary<ArticleType, int> SuggestOrders()
        {
            Dictionary<ArticleType, int> generated = new Dictionary<ArticleType, int>();

            foreach (ArticleType type in articleTypes)
            {
                List<Article> articles = GetArticlesByType(type);

                int fullStock = 0;
                foreach (Article article in articles)
                    fullStock += article.Stock;

                if (wantedStock.ContainsKey(type) && fullStock < wantedStock[type])
                {
                    Console.WriteLine($"Suggested Order: {wantedStock[type] - fullStock} of article type {type.Name} ({type.Id})");
                    generated[type] = wantedStock[type] - fullStock;
                }
                else if (!wantedStock.ContainsKey(type) && fullStock < WANTED_STOCK_DEFAULT)
                {
                    Console.WriteLine($"Suggested Order: {WANTED_STOCK_DEFAULT - fullStock} of article type {type.Name} ({type.Id}). (Used default as no suitable wantedStock entry was found)");
                    generated[type] = WANTED_STOCK_DEFAULT - fullStock;
                }
            }
            return generated;
        }

        public OrderItem NewOrderItem(int typeId, int stock, bool toList = true)
        {
            ArticleType? articleType = FindArticleType(typeId);
            if (articleType == null)
            {
                throw new ArgumentException($"Article type with ID {typeId} does not exist.");
            }

            int uniqueNumber = GenerateSequentialId();
            OrderItem generated = new OrderItem(uniqueNumber, articleType, stock);

            return generated;
        }

        // Orders
        public Order NewOrder(List<OrderItem> orderArticles, Customer customer)
        {
            Order generated = new Order(lastOrderId + 1, orderArticles, customer);
            orders.Add(generated);
            lastOrderId += 1;
            return generated;
        }

        public void DeleteOrder(int id) => DeleteOrder(orders.FirstOrDefault(o => o.Id == id));

        public void DeleteOrder(Order? order)
        {
            if (order == null)
            {
                Console.WriteLine("[ERROR] Order not found.");
                return;
            }

            if (bills.Any(b => b.Order.Id == order.Id))
            {
                Console.WriteLine($"[ERROR] Cannot delete Order {order.Id} because it is referenced by a Bill.");
                return;
            }

            if (orders.Remove(order))
            {
                Console.WriteLine($"[INFO] Order with ID {order.Id} has been deleted.");
            }
            else
            {
                Console.WriteLine($"[ERROR] Order with ID {order.Id} not found.");
            }
        }

        public void ListOrders(bool showFullNotPending = false)
        {
            Console.ForegroundColor = SECTION_INDICATOR_COLOR;
            Console.WriteLine("========= Orders ========");
            Console.ResetColor();
            foreach (Order order in orders)
            {
                Console.Write($"ID: {order.Id}, From: {order.Customer.Name}, Status: ");
                if (order.Status == OrderStatus.Pending) Console.ForegroundColor = ConsoleColor.Blue;
                else if (order.Status == OrderStatus.Completed) Console.ForegroundColor = ConsoleColor.Green;
                else if (order.Status == OrderStatus.Cancelled) Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(order.Status);
                Console.ResetColor();
                if (order.Status == OrderStatus.Pending || showFullNotPending)
                {
                    foreach (OrderItem item in order.Articles)
                    {
                        StorageSlot? slot = FindStorageSlot(item);
                        string slot_id = (slot == null) ? "unsorted" : slot.Id.ToString();
                        Console.WriteLine($"ArticleType-ID: {item.Type.Id}, Article-ID: {item.Id}, Name: {item.Type.Name}, Stock: {item.Stock}, In Slot {slot_id}");
                    }
                }
            }
            Console.WriteLine("=========================");
        }

        public Order? NewestOrder() => orders.OrderBy(o => o.Id).FirstOrDefault();

        public void FinishOrder(Order order) => order.Status = OrderStatus.Completed;

        public void CancelOrder(Order order)
        {
            order.Status = OrderStatus.Cancelled;
            Console.WriteLine($"[INFO] Order with ID {order.Id} has been cancelled.");
        }

        // SelfOrders
        public SelfOrder NewSelfOrder(List<OrderItem> orderArticles)
        {
            SelfOrder generated = new SelfOrder(lastSelfOrderId + 1, orderArticles);
            selfOrders.Add(generated);
            lastSelfOrderId += 1;
            return generated;
        }

        public void DeleteSelfOrder(int id) => DeleteSelfOrder(selfOrders.FirstOrDefault(s => s.Id == id));

        public void DeleteSelfOrder(SelfOrder? selfOrder)
        {
            if (selfOrder == null)
            {
                Console.WriteLine("[ERROR] Self order not found.");
                return;
            }
            if (selfOrders.Remove(selfOrder))
            {
                Console.WriteLine($"[INFO] Self order with ID {selfOrder.Id} has been deleted.");
            }
            else
            {
                Console.WriteLine($"[ERROR] Self order with ID {selfOrder.Id} not found.");
            }
        }

        public void ListSelfOrders(bool showFullNotPending = false)
        {
            Console.ForegroundColor = SECTION_INDICATOR_COLOR;
            Console.WriteLine("======= Self Orders =======");
            Console.ResetColor();
            foreach (SelfOrder selfOrder in selfOrders)
            {
                Console.Write($"ID: {selfOrder.Id}, Status: ");
                if (selfOrder.Status == OrderStatus.Pending) Console.ForegroundColor = ConsoleColor.Blue;
                else if (selfOrder.Status == OrderStatus.Completed) Console.ForegroundColor = ConsoleColor.Green;
                else if (selfOrder.Status == OrderStatus.Cancelled) Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(selfOrder.Status);
                Console.ResetColor();

                if (selfOrder.Status == OrderStatus.Pending || showFullNotPending)
                {
                    foreach (OrderItem item in selfOrder.Articles)
                    {
                        StorageSlot? slot = FindStorageSlot(item);
                        string slot_id = (slot == null) ? "unsorted" : slot.Id.ToString();
                        Console.WriteLine($"ArticleType-ID: {item.Type.Id}, Article-ID: {item.Id}, Name: {item.Type.Name}, Stock: {item.Stock}, In Slot {slot_id}");
                    }
                }
            }
            Console.WriteLine("=========================");
        }

        public void FinishSelfOrder(SelfOrder selfOrder) => selfOrder.Status = OrderStatus.Completed;

        public void CancelSelfOrder(SelfOrder selfOrder)
        {
            selfOrder.Status = OrderStatus.Cancelled;
            Console.WriteLine($"[INFO] Order with ID {selfOrder.Id} has been cancelled.");
        }

        // Bills
        public Bill? NewBill(Order order, Prices prices, PaymentTerms terms)
        {
            double totalPrice = 0;

            foreach (OrderItem item in order.Articles)
            {
                if (!prices.PriceList.ContainsKey(item.Type))
                {
                    Console.WriteLine($"[ERROR] No price found for ArticleType {item.Type.Name}");
                    return null;
                }
                double price = prices.PriceList[item.Type];
                totalPrice += price * item.Stock;
            }

            totalPrice = Math.Round(totalPrice, 2);
            Bill generated = new Bill(lastBillId + 1, totalPrice, order, terms, prices);
            bills.Add(generated);
            lastBillId += 1;
            return generated;
        }

        public void DeleteBill(int id) => DeleteBill(bills.FirstOrDefault(b => b.Id == id));

        public void DeleteBill(Bill? bill)
        {
            if (bill == null)
            {
                Console.WriteLine("[ERROR] Bill not found.");
                return;
            }
            if (bills.Remove(bill))
            {
                Console.WriteLine($"[INFO] Bill with ID {bill.Id} has been deleted.");
            }
            else
            {
                Console.WriteLine($"[ERROR] Bill with ID {bill.Id} not found.");
            }
        }

        public double CalculateOrderTotal(Order order, Prices prices)
        {
            double total = 0;
            foreach (var item in order.Articles)
            {
                if (!prices.PriceList.TryGetValue(item.Type, out var unit))
                {
                    throw new InvalidOperationException($"No price found for ArticleType {item.Type.Name}.");
                }
                total += unit * item.Stock;
            }
            return Math.Round(total, 2);
        }

        public void ListBills()
        {
            Console.ForegroundColor = SECTION_INDICATOR_COLOR;
            Console.WriteLine("========= Bills =========");
            Console.ResetColor();
            foreach (Bill bill in bills)
            {
                Console.WriteLine($"ID: {bill.Id}, Total Price: {FormatAmount(bill.TotalPrice)}, From: {bill.Customer.Name}, Terms: {bill.PaymentTerms.Name}");

                foreach (OrderItem item in bill.Order.Articles)
                {
                    StorageSlot? slot = FindStorageSlot(item);
                    string slot_id = (slot == null) ? "unsorted" : slot.Id.ToString();
                    Console.WriteLine($"ArticleType-ID: {item.Type.Id}, Article-ID: {item.Id}, Name: {item.Type.Name}, Stock: {item.Stock}, In Slot {slot_id}");
                }
            }
            Console.WriteLine("=========================");
        }
    }
}
