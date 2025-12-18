using System;

namespace ERP_Fix.Models
{
    public class OrderItem : ArticleSimilar
    {
        public int Id { get; }
        public ArticleType Type { get; }
        public int Stock { get; set; }

        public OrderItem(int id, ArticleType type, int stock)
        {
            Id = id;
            Type = type;
            Stock = stock;
        }
    }
}
