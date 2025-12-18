using System;

namespace ERP_Fix.Models
{
    public class Section : ERPItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Section(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
