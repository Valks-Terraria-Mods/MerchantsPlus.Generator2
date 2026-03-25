using System.ComponentModel;

namespace MerchantsPlus.Generator;

public sealed class SelectableOption : INotifyPropertyChanged
{
    private bool? _isSelected;
    private bool _isEnabled;

    public SelectableOption(string key, string label, bool? isSelected = false, bool isEnabled = true)
    {
        Key = key;
        Label = label;
        _isSelected = isSelected;
        _isEnabled = isEnabled;
    }

    public string Key { get; }
    public string Label { get; }

    public bool? IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
                return;

            _isSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled == value)
                return;

            _isEnabled = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
