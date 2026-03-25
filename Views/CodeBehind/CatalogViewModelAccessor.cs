namespace MerchantsPlus.Generator;

public sealed class CatalogViewModelAccessor
{
    private readonly Func<object?> _dataContextAccessor;

    public CatalogViewModelAccessor(Func<object?> dataContextAccessor)
    {
        _dataContextAccessor = dataContextAccessor;
    }

    public CatalogViewModel? Current => _dataContextAccessor() as CatalogViewModel;

    public void Execute(Action<CatalogViewModel> action)
    {
        if (Current is { } viewModel)
            action(viewModel);
    }
}