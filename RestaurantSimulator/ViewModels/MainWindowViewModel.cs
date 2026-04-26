using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace RestaurantSimulator.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _selectedTab;
    public ObservableCollection<ViewModelBase> Tabs { get; } = new();

    public MainWindowViewModel()
    {
        Tabs.Add(new OrdersViewModel());
        Tabs.Add(new StationsViewModel());
        Tabs.Add(new IngredientsViewModel());

        SelectedTab = Tabs[0];
    }
}
