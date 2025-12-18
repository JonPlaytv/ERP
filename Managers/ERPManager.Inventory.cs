using System;
using System.Collections.Generic;
using System.Linq;
using ERP_Fix.Models;

namespace ERP_Fix
{
    public partial class ERPManager
    {
        // Inventory Query
        public int GetArticleCount() => articles.Count;
        public List<Article> GetAllArticles() => articles;
        public int GetArticleTypeCount() => articleTypes.Count;
        public List<ArticleType> GetAllArticleTypes() => articleTypes;
        public int GetStorageSlotCount() => storageSlots.Count;
        public List<StorageSlot> GetAllStorageSlots() => storageSlots;

        // Inventory Finders
        public Article? FindArticle(int id) => articles.FirstOrDefault(a => a.Id == id);
        public Article? FindArticleByScannerId(long scannerId) => articles.FirstOrDefault(a => a.ScannerId == scannerId);
        public ArticleType? FindArticleType(int id) => articleTypes.FirstOrDefault(t => t.Id == id);
        public StorageSlot? FindStorageSlot(ArticleSimilar article) => storageSlots.FirstOrDefault(slot => slot.Fill.Contains(article));
        private StorageSlot? FindStorageSlotById(int id) => storageSlots.FirstOrDefault(t => t.Id == id);
        public List<Article> GetArticlesByType(ArticleType type) => articles.Where(article => article.Type == type).ToList();

        // Warehouse Logic
        public long GenerateScannerId()
        {
            Random rnd = new();
            long newId;
            do
            {
                newId = rnd.NextInt64(100000000000000, 999999999999999);
            }
            while (ScannerIds.Contains(newId));

            ScannerIds.Add(newId);
            return newId;
        }

        public ArticleType NewArticleType(string name)
        {
            ArticleType generated = new ArticleType(lastArticleTypeId + 1, name);
            articleTypes.Add(generated);
            lastArticleTypeId += 1;
            return generated;
        }

        public void DeleteArticleType(int id) => DeleteArticleType(FindArticleType(id));

        public void DeleteArticleType(ArticleType? articleType)
        {
            if (articleType == null)
            {
                Console.WriteLine("[ERROR] Article type not found.");
                return;
            }

            bool referenced = articles.Any(a => a.Type.Id == articleType.Id)
                            || orders.Any(o => o.Articles.Any(i => i.Type.Id == articleType.Id))
                            || prices.Any(p => p.PriceList.Keys.Any(t => t.Id == articleType.Id));
            if (referenced)
            {
                Console.WriteLine($"[ERROR] Cannot delete ArticleType {articleType.Id} because it is referenced by Articles, Orders, or Prices.");
                return;
            }

            var toRemove = articleTypes.FirstOrDefault(t => t.Id == articleType.Id) ?? articleType;
            if (articleTypes.Remove(toRemove))
            {
                Console.WriteLine($"[INFO] Article type with ID {toRemove.Id} has been deleted.");
            }
            else
            {
                Console.WriteLine($"[ERROR] Article type with ID {articleType.Id} not found.");
            }
        }

        public StorageSlot NewStorageSlot()
        {
            StorageSlot generated = new StorageSlot(lastSlotId + 1, new List<Article>());
            storageSlots.Add(generated);
            lastSlotId += 1;
            return generated;
        }

        public void DeleteStorageSlot(int id) => DeleteStorageSlot(storageSlots.FirstOrDefault(s => s.Id == id));

        public void DeleteStorageSlot(StorageSlot? slot)
        {
            if (slot == null)
            {
                Console.WriteLine("[ERROR] Storage slot not found.");
                return;
            }
            if (storageSlots.Remove(slot))
            {
                Console.WriteLine($"[INFO] Storage slot with ID {slot.Id} has been deleted.");
            }
            else
            {
                Console.WriteLine($"[ERROR] Storage slot with ID {slot.Id} not found.");
            }
        }

        public Article NewArticle(int typeId, int stock, bool toList = true)
        {
            ArticleType? articleType = FindArticleType(typeId);
            if (articleType == null)
            {
                throw new ArgumentException($"Article type with ID {typeId} does not exist.");
            }

            long scannerId = GenerateScannerId();
            Article generated = new Article(lastStockId + 1, articleType, stock, scannerId);
            if (toList) articles.Add(generated);
            lastStockId += 1;

            return generated;
        }

        public void DeleteArticle(int id) => DeleteArticle(FindArticle(id));

        public void DeleteArticle(Article? article)
        {
            if (article == null)
            {
                Console.WriteLine("[ERROR] Article not found.");
                return;
            }

            foreach (var slot in storageSlots)
            {
                while (slot.Fill.Remove(article)) { }
            }

            if (articles.Remove(article))
            {
                Console.WriteLine($"[INFO] Article with ID {article.Id} has been deleted.");
            }
            else
            {
                Console.WriteLine($"[ERROR] Article with ID {article.Id} not found.");
            }
        }

        public void RestockArticle(int id, int amount)
        {
            Article? article = FindArticle(id);
            if (article != null) article.Stock += amount;
            else Console.WriteLine($"[ERROR] Article with ID {id} not found.");
        }

        public void WithdrawArticle(int id, int amount)
        {
            Article? article = FindArticle(id);
            if (article != null)
            {
                if (article.Stock >= amount) article.Stock -= amount;
                else Console.WriteLine($"[ERROR] Not enough stock to withdraw! Current: {article.Stock}, Requested: {amount}");
            }
            else Console.WriteLine($"[ERROR] Article with ID {id} not found.");
        }

        public void SortArticle(int id, int slotId)
        {
            Article? article = FindArticle(id);
            if (article != null)
            {
                StorageSlot? slot = FindStorageSlotById(slotId);
                if (slot != null) slot.Fill.Add(article);
            }
            else Console.WriteLine($"[ERROR] Article with ID {id} not found.");
        }

        public void DisplayInventory()
        {
            Console.ForegroundColor = SECTION_INDICATOR_COLOR;
            Console.WriteLine("======= Inventory =======");
            Console.ResetColor();
            foreach (Article item in articles)
            {
                StorageSlot? slot = FindStorageSlot(item);
                string slot_id = (slot == null) ? "unsorted" : slot.Id.ToString();
                Console.WriteLine($"ArticleType-ID: {item.Type.Id}, Name: {item.Type.Name}, Article-ID: {item.Id}, Stock: {item.Stock}, In Slot {slot_id}");
            }
            Console.WriteLine("=========================");
        }

        public void ListStorageSlots()
        {
            Console.ForegroundColor = SECTION_INDICATOR_COLOR;
            Console.WriteLine("===== Storage Slots =====");
            Console.ResetColor();
            foreach (StorageSlot slot in storageSlots)
            {
                Console.WriteLine($"ID: {slot.Id}");
            }
            Console.WriteLine("=========================");
        }
    }
}
