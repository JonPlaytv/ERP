using System;
using System.Collections.Generic;

namespace ERP_Fix.Models
{
    public class StorageSlot : ERPItem
    {
        public int Id { get; }
        public List<Article> Fill { get; set; }

        public StorageSlot(int id, List<Article> fill)
        {
            Id = id;
            Fill = fill;
        }
    }
}
