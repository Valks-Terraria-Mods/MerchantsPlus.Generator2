namespace MerchantsPlus.Generator;

public sealed class GlobalFlagsActionHandler
{
    private readonly CatalogViewModelAccessor _viewModelAccessor;

    public GlobalFlagsActionHandler(CatalogViewModelAccessor viewModelAccessor)
    {
        _viewModelAccessor = viewModelAccessor;
    }

    public void AddWhitelistRule() => _viewModelAccessor.Execute(vm => vm.AddGlobalWhitelistRule());

    public void RemoveWhitelistRule(object? sender)
    {
        if (KeywordRuleRowResolver.TryResolve(sender, out var row))
            _viewModelAccessor.Execute(vm => vm.RemoveGlobalWhitelistRule(row));
    }

    public void AddBlacklistRule() => _viewModelAccessor.Execute(vm => vm.AddGlobalBlacklistRule());

    public void RemoveBlacklistRule(object? sender)
    {
        if (KeywordRuleRowResolver.TryResolve(sender, out var row))
            _viewModelAccessor.Execute(vm => vm.RemoveGlobalBlacklistRule(row));
    }

    public void Apply() => _viewModelAccessor.Execute(vm => vm.ApplyGlobalFlags());

    public void ResetAllPrices() => _viewModelAccessor.Execute(vm => vm.ResetAllPrices());

    public void ResetAllConditions() => _viewModelAccessor.Execute(vm => vm.ResetAllConditionOverrides());

    public void SaveUnassignedDefaultPrice() => _viewModelAccessor.Execute(vm => vm.SaveUnassignedDefaultPrice());

    public void ResetUnassignedDefaultPrice() => _viewModelAccessor.Execute(vm => vm.ResetUnassignedDefaultPrice());
}