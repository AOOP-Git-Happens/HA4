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
        var stationsViewModel = new StationsViewModel();
        var ordersViewModel = new OrdersViewModel(stationsViewModel);

        Tabs.Add(ordersViewModel);
        Tabs.Add(stationsViewModel);
        Tabs.Add(new IngredientsViewModel());

        SelectedTab = Tabs[0];
    }
}
