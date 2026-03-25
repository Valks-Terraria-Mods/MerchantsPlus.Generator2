namespace MerchantsPlus.Generator;

public sealed class MerchantWorkspaceEditorBuildResult{
    private readonly IReadOnlyList<MerchantShopBadgeRow> _ownedShopBadges;
    private readonly IReadOnlyList<MerchantShopBadgeRow> _availableShopBadges;
    private readonly MerchantShopBadgeRow? _selectedAddShop;

    public MerchantWorkspaceEditorBuildResult(
        IReadOnlyList<MerchantShopBadgeRow> ownedShopBadges,
        IReadOnlyList<MerchantShopBadgeRow> availableShopBadges,
        MerchantShopBadgeRow? selectedAddShop)
    {
        _ownedShopBadges = ownedShopBadges;
        _availableShopBadges = availableShopBadges;
        _selectedAddShop = selectedAddShop;
    }

    public IReadOnlyList<MerchantShopBadgeRow> OwnedShopBadges => _ownedShopBadges;
    public IReadOnlyList<MerchantShopBadgeRow> AvailableShopBadges => _availableShopBadges;
    public MerchantShopBadgeRow? SelectedAddShop => _selectedAddShop;
}
