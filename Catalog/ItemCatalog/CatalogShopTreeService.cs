namespace MerchantsPlus.Generator;

public class CatalogShopTreeService{
    public List<Shop> CollectShopSubtree(Category category, Shop root)
    {
        var subtreeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { root.Name };
        var queue = new Queue<string>();
        queue.Enqueue(root.Name);

        while (queue.Count > 0)
        {
            var currentName = queue.Dequeue();
            var children = category.Shops
                .Where(shop => string.Equals(shop.ParentShopName, currentName, StringComparison.OrdinalIgnoreCase))
                .Select(shop => shop.Name)
                .ToList();

            foreach (var childName in children)
            {
                if (subtreeNames.Add(childName))
                    queue.Enqueue(childName);
            }
        }

        return category.Shops.Where(shop => subtreeNames.Contains(shop.Name)).ToList();
    }
}