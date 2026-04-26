using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace RestaurantSimulator.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _selectedTab;
    public ObservableCollection<ViewModelBase> Tabs { get; } = new();
    public string Greeting { get; } = "Welcome to Restaurant Simulator!";

    public MainWindowViewModel()
    {
        Tabs.Add(new OrdersViewModel());

        SelectedTab = Tabs[0];
    }
}
