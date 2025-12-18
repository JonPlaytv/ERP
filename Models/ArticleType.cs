using System;

namespace ERP_Fix.Models
{
    public class ArticleType : ERPItem
    {
        public int Id { get; }
        public string Name { get; }

        public ArticleType(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
