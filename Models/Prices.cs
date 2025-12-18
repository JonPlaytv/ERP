using System;
using System.Collections.Generic;

namespace ERP_Fix.Models
{
    public class Prices : ERPItem
    {
        public int Id { get; }
        public Dictionary<ArticleType, double> PriceList { get; }

        public Prices(int id, Dictionary<ArticleType, double> priceList)
        {
            Id = id;
            PriceList = priceList;
        }
    }
}
