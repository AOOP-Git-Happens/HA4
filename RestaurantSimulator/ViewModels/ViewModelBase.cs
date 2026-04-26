using CommunityToolkit.Mvvm.ComponentModel;

namespace RestaurantSimulator.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private string _header = string.Empty;
}
