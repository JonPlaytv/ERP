using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP_Fix.Models
{
    public class SelfOrder : ERPItem
    {
        public int Id { get; }
        public List<OrderItem> Articles { get; set; }
        public List<OrderItem> Arrived { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public SelfOrder(int id, List<OrderItem> articles)
        {
            Id = id;
            Articles = articles;
            Arrived = new List<OrderItem>();
        }

        public void Arrive(OrderItem item)
        {
            var existing = Articles.FirstOrDefault(a => a.Type == item.Type);
            if (existing == null)
            {
                Console.WriteLine($"[ERROR] Item {item.Type.Name} not found in self-order {Id}");
                return;
            }
            if (item.Stock <= 0)
            {
                Console.WriteLine($"[ERROR] Delivered quantity must be greater than 0.");
                return;
            }
            if (item.Stock > existing.Stock)
            {
                Console.WriteLine($"[WARN] Delivered quantity ({item.Stock}) exceeds remaining stock ({existing.Stock}) in self-order {Id}. Capping to {existing.Stock}.");
                item.Stock = existing.Stock;
            }
            existing.Stock -= item.Stock;

            OrderItem arrivedItem = new OrderItem(item.Id, existing.Type, item.Stock);
            Arrived.Add(arrivedItem);
            if (existing.Stock == 0)
            {
                Articles.Remove(existing);
            }
            Console.WriteLine($"[INFO] Item {item.Type.Name} arrived with quantity {item.Stock}.");

            if (Articles.Count == 0)
            {
                Status = OrderStatus.Completed;
                Console.WriteLine($"[INFO] Self-order {Id} is now completed as all items have arrived.");
            }
        }
    }
}
