using System;
using System.Xml;

namespace ERP_Fix.Utilities
{
    public class BillReader
    {
        public static void xml(string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            XmlNamespaceManager nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsMgr.AddNamespace("rsm", "urn:ferd:CrossIndustryInvoice:invoice:1p0");
            nsMgr.AddNamespace("ram", "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:12");
            nsMgr.AddNamespace("udt", "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:15");

            // Invoice Header
            Console.WriteLine("=== Invoice Header ===");
            var idNode = xmlDoc.SelectSingleNode("//rsm:ExchangedDocument/ram:ID", nsMgr);
            var typeCodeNode = xmlDoc.SelectSingleNode("//rsm:ExchangedDocument/ram:TypeCode", nsMgr);
            var issueDateNode = xmlDoc.SelectSingleNode("//rsm:ExchangedDocument/ram:IssueDateTime/udt:DateTimeString", nsMgr);
            var noteNode = xmlDoc.SelectSingleNode("//rsm:ExchangedDocument/ram:IncludedNote/ram:Content", nsMgr);

            Console.WriteLine($"Invoice Number: {idNode?.InnerText}");
            Console.WriteLine($"Type Code: {typeCodeNode?.InnerText}");
            Console.WriteLine($"Issue Date: {issueDateNode?.InnerText}");
            if (noteNode != null)
                Console.WriteLine($"Note: {noteNode.InnerText}");

            // Seller
            Console.WriteLine("\n=== Seller ===");
            var sellerNode = xmlDoc.SelectSingleNode("//ram:SellerTradeParty", nsMgr);
            Console.WriteLine($"Name: {sellerNode?.SelectSingleNode("ram:Name", nsMgr)?.InnerText}");
            Console.WriteLine($"Street: {sellerNode?.SelectSingleNode("ram:PostalTradeAddress/ram:LineOne", nsMgr)?.InnerText}");
            Console.WriteLine($"ZIP: {sellerNode?.SelectSingleNode("ram:PostalTradeAddress/ram:PostcodeCode", nsMgr)?.InnerText}");
            Console.WriteLine($"City: {sellerNode?.SelectSingleNode("ram:PostalTradeAddress/ram:CityName", nsMgr)?.InnerText}");
            Console.WriteLine($"Country: {sellerNode?.SelectSingleNode("ram:PostalTradeAddress/ram:CountryID", nsMgr)?.InnerText}");
            Console.WriteLine($"VAT ID: {sellerNode?.SelectSingleNode("ram:SpecifiedTaxRegistration/ram:ID", nsMgr)?.InnerText}");

            // Buyer
            Console.WriteLine("\n=== Buyer ===");
            var buyerNode = xmlDoc.SelectSingleNode("//ram:BuyerTradeParty", nsMgr);
            Console.WriteLine($"Name: {buyerNode?.SelectSingleNode("ram:Name", nsMgr)?.InnerText}");
            Console.WriteLine($"Street: {buyerNode?.SelectSingleNode("ram:PostalTradeAddress/ram:LineOne", nsMgr)?.InnerText}");
            Console.WriteLine($"ZIP: {buyerNode?.SelectSingleNode("ram:PostalTradeAddress/ram:PostcodeCode", nsMgr)?.InnerText}");
            Console.WriteLine($"City: {buyerNode?.SelectSingleNode("ram:PostalTradeAddress/ram:CityName", nsMgr)?.InnerText}");
            Console.WriteLine($"Country: {buyerNode?.SelectSingleNode("ram:PostalTradeAddress/ram:CountryID", nsMgr)?.InnerText}");

            // Reference (Order)
            var buyerRefNode = xmlDoc.SelectSingleNode("//ram:BuyerReference", nsMgr);
            var orderRefNode = xmlDoc.SelectSingleNode("//ram:ContractReferencedDocument/ram:ID", nsMgr);
            if (buyerRefNode != null)
                Console.WriteLine($"Buyer Reference: {buyerRefNode.InnerText}");
            if (orderRefNode != null)
                Console.WriteLine($"Order Reference: {orderRefNode.InnerText}");

            // Delivery
            var deliveryDateNode = xmlDoc.SelectSingleNode("//ram:ActualDeliverySupplyChainEvent/ram:OccurrenceDateTime/udt:DateTimeString", nsMgr);
            if (deliveryDateNode != null)
            {
                Console.WriteLine("\n=== Delivery ===");
                Console.WriteLine($"Delivery Date: {deliveryDateNode.InnerText}");
            }

            // Payment
            Console.WriteLine("\n=== Payment / Settlement ===");
            var paymentRefNode = xmlDoc.SelectSingleNode("//ram:PaymentReference", nsMgr);
            var currencyNode = xmlDoc.SelectSingleNode("//ram:InvoiceCurrencyCode", nsMgr);
            var paymentMeansNode = xmlDoc.SelectSingleNode("//ram:SpecifiedTradeSettlementPaymentMeans", nsMgr);
            Console.WriteLine($"Payment Reference: {paymentRefNode?.InnerText}");
            Console.WriteLine($"Currency: {currencyNode?.InnerText}");
            if (paymentMeansNode != null)
            {
                Console.WriteLine($"Payment Type: {paymentMeansNode.SelectSingleNode("ram:TypeCode", nsMgr)?.InnerText}");
                Console.WriteLine($"Payment Info: {paymentMeansNode.SelectSingleNode("ram:Information", nsMgr)?.InnerText}");
            }

            // Totals
            Console.WriteLine("\n=== Monetary Summation ===");
            var lineTotalNode = xmlDoc.SelectSingleNode("//ram:SpecifiedTradeSettlementMonetarySummation/ram:LineTotalAmount", nsMgr);
            var taxTotalNode = xmlDoc.SelectSingleNode("//ram:SpecifiedTradeSettlementMonetarySummation/ram:TaxTotalAmount", nsMgr);
            var grandTotalNode = xmlDoc.SelectSingleNode("//ram:SpecifiedTradeSettlementMonetarySummation/ram:GrandTotalAmount", nsMgr);
            Console.WriteLine($"Line Total: {lineTotalNode?.InnerText} EUR");
            Console.WriteLine($"Tax Total: {taxTotalNode?.InnerText} EUR");
            Console.WriteLine($"Grand Total: {grandTotalNode?.InnerText} EUR");

            // Tax breakdown
            Console.WriteLine("\n=== Tax Details ===");
            var taxNodes = xmlDoc.SelectNodes("//ram:ApplicableTradeTax", nsMgr);
            if (taxNodes != null)
            {
                foreach (XmlNode tax in taxNodes)
                {
                    Console.WriteLine("- VAT Info:");
                    Console.WriteLine($"  Tax Amount: {tax.SelectSingleNode("ram:CalculatedAmount", nsMgr)?.InnerText}");
                    Console.WriteLine($"  Tax Type: {tax.SelectSingleNode("ram:TypeCode", nsMgr)?.InnerText}");
                    Console.WriteLine($"  Tax Basis: {tax.SelectSingleNode("ram:BasisAmount", nsMgr)?.InnerText}");
                    Console.WriteLine($"  Tax Rate: {tax.SelectSingleNode("ram:RateApplicablePercent", nsMgr)?.InnerText}%");
                }
            }

            // All positions
            Console.WriteLine("\n=== Line Items ===");
            var lineItems = xmlDoc.SelectNodes("//ram:IncludedSupplyChainTradeLineItem", nsMgr);
            if (lineItems != null)
            {
                foreach (XmlNode item in lineItems)
                {
                    var lineId = item.SelectSingleNode("ram:AssociatedDocumentLineDocument/ram:LineID", nsMgr);
                    var productName = item.SelectSingleNode("ram:SpecifiedTradeProduct/ram:Name", nsMgr);
                    var productDesc = item.SelectSingleNode("ram:SpecifiedTradeProduct/ram:Description", nsMgr);
                    var buyerAssignedId = item.SelectSingleNode("ram:SpecifiedTradeProduct/ram:BuyerAssignedID", nsMgr);
                    var quantity = item.SelectSingleNode("ram:SpecifiedLineTradeDelivery/ram:BilledQuantity", nsMgr);
                    var unitAttr = quantity?.Attributes?["unitCode"];
                    var unit = unitAttr != null ? unitAttr.Value : string.Empty;
                    var grossPrice = item.SelectSingleNode("ram:SpecifiedLineTradeAgreement/ram:GrossPriceProductTradePrice/ram:ChargeAmount", nsMgr);
                    var netPrice = item.SelectSingleNode("ram:SpecifiedLineTradeAgreement/ram:NetPriceProductTradePrice/ram:ChargeAmount", nsMgr);
                    var lineTotal = item.SelectSingleNode("ram:SpecifiedLineTradeSettlement/ram:SpecifiedTradeSettlementLineMonetarySummation/ram:LineTotalAmount", nsMgr);
                    var taxDetail = item.SelectSingleNode("ram:SpecifiedLineTradeSettlement/ram:ApplicableTradeTax", nsMgr);
                    var taxRate = taxDetail?.SelectSingleNode("ram:RateApplicablePercent", nsMgr);

                    Console.WriteLine($"\nLine {lineId?.InnerText}:");
                    Console.WriteLine($"  Product: {productName?.InnerText}");
                    Console.WriteLine($"  Description: {productDesc?.InnerText}");
                    Console.WriteLine($"  Product Number: {buyerAssignedId?.InnerText}");
                    Console.WriteLine($"  Quantity: {quantity?.InnerText} {unit}");
                    Console.WriteLine($"  Gross Price per Unit: {grossPrice?.InnerText} EUR");
                    Console.WriteLine($"  Net Price per Unit: {netPrice?.InnerText} EUR");
                    Console.WriteLine($"  Line Total: {lineTotal?.InnerText} EUR");
                    Console.WriteLine($"  VAT Rate: {taxRate?.InnerText}%");
                }
            }
        }
    }
}
