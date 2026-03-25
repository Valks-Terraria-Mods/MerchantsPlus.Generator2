using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MerchantsPlus.Generator;

public sealed class AssignmentTreeNode : INotifyPropertyChanged
{
    private bool _isExpanded;

    public AssignmentTreeNode(SelectableOption option, bool isCategory, string? displayLabel = null, string? secondaryLabel = null)
    {
        Option = option;
        IsCategory = isCategory;
        DisplayLabel = string.IsNullOrWhiteSpace(displayLabel) ? option.Label : displayLabel;
        SecondaryLabel = secondaryLabel ?? string.Empty;
    }

    public SelectableOption Option { get; }
    public bool IsCategory { get; }
    public string DisplayLabel { get; }
    public string SecondaryLabel { get; }
    public bool HasSecondaryLabel => !string.IsNullOrWhiteSpace(SecondaryLabel);
    public ObservableCollection<AssignmentTreeNode> Children { get; } = [];

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value)
                return;

            _isExpanded = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
