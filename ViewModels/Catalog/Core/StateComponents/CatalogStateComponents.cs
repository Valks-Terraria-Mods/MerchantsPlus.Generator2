namespace MerchantsPlus.Generator;

public sealed class CatalogStateComponents{
    private readonly CatalogCollectionsComponent _collections;
    private readonly CatalogEditorInputsComponent _editorInputs;
    private readonly CatalogSelectionComponent _selection;
    private readonly CatalogSettingsComponent _settings;
    private readonly CatalogOutputComponent _output;
    private readonly CatalogMetricsComponent _metrics;

    public CatalogStateComponents()
        : this(new CatalogCollectionsComponent(), new CatalogEditorInputsComponent(), new CatalogSelectionComponent(), new CatalogSettingsComponent(), new CatalogOutputComponent(), new CatalogMetricsComponent())
    {
    }

    public CatalogStateComponents(
        CatalogCollectionsComponent collections,
        CatalogEditorInputsComponent editorInputs,
        CatalogSelectionComponent selection,
        CatalogSettingsComponent settings,
        CatalogOutputComponent output,
        CatalogMetricsComponent metrics)
    {
        _collections = collections;
        _editorInputs = editorInputs;
        _selection = selection;
        _settings = settings;
        _output = output;
        _metrics = metrics;
    }

    public CatalogCollectionsComponent Collections => _collections;
    public CatalogEditorInputsComponent EditorInputs => _editorInputs;
    public CatalogSelectionComponent Selection => _selection;
    public CatalogSettingsComponent Settings => _settings;
    public CatalogOutputComponent Output => _output;
    public CatalogMetricsComponent Metrics => _metrics;
}
