namespace MerchantsPlus.Generator;

public sealed class MerchantOptionWiringComponent{
    public void WireShopOption(
        SelectableOption category,
        List<SelectableOption> children,
        SelectableOption shop,
        Func<bool> isSyncing,
        Action<bool> setSyncing,
        Action<SelectableOption, IEnumerable<SelectableOption>> updateCategoryState,
        Action persistChanges)
    {
        shop.PropertyChanged += (_, eventArgs) =>
        {
            if (eventArgs.PropertyName != nameof(SelectableOption.IsSelected) || isSyncing())
                return;

            setSyncing(true);
            try
            {
                updateCategoryState(category, children);
            }
            finally
            {
                setSyncing(false);
            }

            persistChanges();
        };
    }

    public void WireCategoryOption(
        SelectableOption category,
        List<SelectableOption> children,
        Func<bool> isSyncing,
        Action<bool> setSyncing,
        Action<SelectableOption, IEnumerable<SelectableOption>> updateCategoryState,
        Action persistChanges)
    {
        category.PropertyChanged += (_, eventArgs) =>
        {
            if (eventArgs.PropertyName != nameof(SelectableOption.IsSelected) || isSyncing() || !category.IsEnabled)
                return;

            setSyncing(true);
            try
            {
                var target = category.IsSelected == true;
                foreach (var child in children.Where(option => option.IsEnabled))
                    child.IsSelected = target;

                updateCategoryState(category, children);
            }
            finally
            {
                setSyncing(false);
            }

            persistChanges();
        };
    }
}
