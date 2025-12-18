using System;

namespace ERP_Fix.Models
{
    public abstract class Person : ERPItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public PersonType Type { get; set; }
        public PersonInformation? Information { get; set; }
    }
}
