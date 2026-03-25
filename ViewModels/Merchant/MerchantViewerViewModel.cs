namespace MerchantsPlus.Generator;

public abstract class MerchantViewerViewModel : MerchantBaseViewModel
{
    protected void RefreshMerchantViewer()
    {
        SaveMerchantAssignmentsAndOutput();
        VisibleMerchantAssignments.Clear();
        if (string.IsNullOrWhiteSpace(SelectedMerchantName))
            return;

        var assignment = BuildEffectiveAssignment(GetOrCreateAssignment(SelectedMerchantName));
        var categories = assignment.Categories.OrderBy(v => v, StringComparer.OrdinalIgnoreCase).ToArray();
        var shops = assignment.Shops.OrderBy(v => v, StringComparer.OrdinalIgnoreCase).ToArray();

        if (categories.Length == 0 && shops.Length == 0)
        {
            VisibleMerchantAssignments.Add(new MerchantTreeNode("No categories or shops assigned."));
            return;
        }

        foreach (var category in categories)
        {
            var node = new MerchantTreeNode(category, categoryName: category);
            foreach (var shop in shops.Where(v => v.StartsWith($"{category} / ", StringComparison.OrdinalIgnoreCase)))
            {
                var shopName = shop[(shop.IndexOf(" / ", StringComparison.Ordinal) + 3)..];
                node.Children.Add(new MerchantTreeNode(shopName, categoryName: category, shopName: shopName));
            }

            VisibleMerchantAssignments.Add(node);
        }
    }
}
