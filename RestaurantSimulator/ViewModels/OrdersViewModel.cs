using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using RestaurantSimulator.Models;
using RestaurantSimulator.Services;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RestaurantSimulator.ViewModels;

public partial class OrdersViewModel : ViewModelBase
{
    private readonly RestaurantDataService _dataService;
    private readonly StationsViewModel _stationsViewModel;
    private readonly Random _random = new();
    public ObservableCollection<Order> PendingOrders { get; } = new();
    public ObservableCollection<Order> AcceptedOrders { get; } = new();
    public ObservableCollection<Order> RejectedOrders { get; } = new();

    [ObservableProperty]
    private Order? _selectedPendingOrder;

    public OrdersViewModel(StationsViewModel stationsViewModel)
    {
        Header = "Orders";

        _stationsViewModel = stationsViewModel;
        _dataService = RestaurantDataService.Instance;

        Task.Run(StartOrderGenerationLoop);
    }

    private async Task StartOrderGenerationLoop()
    {
        while (true)
        {
            int pendingCount = await Dispatcher.UIThread.InvokeAsync(() => PendingOrders.Count);

            if (pendingCount >= 3)
            {
                await Task.Delay(1000);
                continue;
            }

            int waitTime = _random.Next(1000, 10001); 
            await Task.Delay(waitTime);

            if (_dataService.Recipes.Any())
            {
                var recipe = _dataService.Recipes[_random.Next(_dataService.Recipes.Count)];
                var newOrder = new Order
                {
                    OrderId = $"N-{_random.Next(1000, 9999)}",
                    TakenAt = DateTime.Now,
                    SelectedRecipe = recipe
                };

                Dispatcher.UIThread.Post(() =>
                {
                    if (PendingOrders.Count < 3)
                    {
                        PendingOrders.Add(newOrder);
                    }
                });
            }
        }
    }

    [RelayCommand]
    public void AcceptOrder(Order? order)
    {
        var target = order ?? SelectedPendingOrder;

        if (target != null)
        {
            AcceptedOrders.Add(target);
            PendingOrders.Remove(target);

            _ = Task.Run(async () =>
            {
                try
                {
                    await _stationsViewModel.ProcessOrderAsync(target);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Order processing error: {ex.Message}");
                }
            });
        }
    }

    [RelayCommand]
    public void RejectOrder(Order order)
    {
        var target = order ?? SelectedPendingOrder;

        if (target != null)
        {
            RejectedOrders.Add(target);
            PendingOrders.Remove(target);
        }
    }
}