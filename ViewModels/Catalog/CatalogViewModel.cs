namespace MerchantsPlus.Generator;

public class CatalogViewModel : ConditionEditorViewModel
{
    public static CatalogViewModel FromCatalog(IReadOnlyList<Category> categories, IReadOnlyList<string> unorganized)
    {
        var vm = new CatalogViewModel();
        vm.LoadSettings();
        vm.Load(categories, unorganized);
        vm.LoadMerchants();
        vm.InitializeConditionOptions();
        vm.LoadMerchantAssignments();
        vm.RefreshMerchantWorkspace();
        vm.InitializeUnorganizedBlacklistEditor();
        vm.RefreshOutputFiles();
        return vm;
    }
}
