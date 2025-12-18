using System;
using Spire.Barcode;
using System.Drawing;

namespace ERP_Fix.Models
{
    public class Article : ArticleSimilar
    {
        public int Id { get; }
        public long ScannerId { get; set; }
        public ArticleType Type { get; }
        public int Stock { get; set; }

        public Article(int id, ArticleType type, int stock, long scannerId)
        {
            Id = id;
            Type = type;
            Stock = stock;
            ScannerId = scannerId;
        }

        public string GenerateBarCode()
        {
            string name = Type.Name + "-" + ScannerId + ".png";

            BarcodeSettings bs = new BarcodeSettings();
            bs.Type = BarCodeType.Code128;
            bs.Data = ScannerId.ToString();

            BarCodeGenerator bg = new BarCodeGenerator(bs);
            if (OperatingSystem.IsWindows())
            {
                bg.GenerateImage().Save(name);
            }
            else
            {
                // On non-Windows platforms, System.Drawing may not be supported. Skip saving.
            }

            return name;
        }
    }
}
