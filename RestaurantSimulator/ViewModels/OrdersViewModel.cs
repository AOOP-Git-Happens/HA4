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

/// <summary>
/// handling order flow in application
/// generates random incoming orders
/// moving orders between pending, accepted and rejected
/// triggering kitchen process when an order is accepted
/// </summary>

public partial class OrdersViewModel : ViewModelBase
{
    //service to load recipes from json
    private readonly RestaurantDataService _dataService;
    //reference to Stations, to send orders for processing
    private readonly StationsViewModel _stationsViewModel;
    //random generator for order timing and selection
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
        _dataService = new RestaurantDataService();

        //start background loop that generates incoming orders
        Task.Run(StartOrderGenerationLoop);
    }

    //continioulsy generates new orders at random intervals
    private async Task StartOrderGenerationLoop()
    {
        while (true)
        {
            // Wait if 3 orders came in
            int pendingCount = await Dispatcher.UIThread.InvokeAsync(() => PendingOrders.Count);

            if (pendingCount >= 3)
            {
                await Task.Delay(1000);
                continue;
            }

            // wait random time: 1 - 10 seconds
            int waitTime = _random.Next(1000, 10001); // Milliseconds
            await Task.Delay(waitTime);

            // Create new order from random recipe
            if (_dataService.Recipes.Any())
            {
                var recipe = _dataService.Recipes[_random.Next(_dataService.Recipes.Count)];
                var newOrder = new Order
                {
                    OrderId = $"N-{_random.Next(1000, 9999)}",
                    TakenAt = DateTime.Now,
                    SelectedRecipe = recipe
                };

                // Thread safety
                // Dispatcher is used to add to an ObservableCollection 
                // Going from background to main thread
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

    //accepts an order and sends it to kitchen processing
    [RelayCommand]
    public void AcceptOrder(Order? order)
    {
        var target = order ?? SelectedPendingOrder;

        if (target != null)
        {
            AcceptedOrders.Add(target);
            PendingOrders.Remove(target);

        //start processing asynchronously 
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
