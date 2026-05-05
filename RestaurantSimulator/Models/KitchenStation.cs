using CommunityToolkit.Mvvm.ComponentModel;

namespace RestaurantSimulator.Models;

public partial class KitchenStation : ObservableObject
{
    [ObservableProperty]
    private string _stationId = string.Empty;

    [ObservableProperty]
    private string _type = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _currentOrderId = "Idle";

    [ObservableProperty]
    private string _currentStepName = "Waiting";

    [ObservableProperty]
    private int _progress;
}