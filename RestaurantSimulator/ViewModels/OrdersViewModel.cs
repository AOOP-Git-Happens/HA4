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
    private readonly Random _random = new();
    public ObservableCollection<Order> PendingOrders { get; } = new();
    public ObservableCollection<Order> AcceptedOrders { get; } = new();
    public ObservableCollection<Order> RejectedOrders { get; } = new();
    
    [ObservableProperty]
    private Order? _selectedPendingOrder;
    
    public OrdersViewModel()
    {
        Header = "Orders";

        _dataService = new RestaurantDataService();

        Task.Run(StartOrderGenerationLoop);
    }

    private async Task StartOrderGenerationLoop()
    {
        while (true)
        {
            // Wait if 3 orders came in
            if (PendingOrders.Count >= 3)
            {
                await Task.Delay(1000); 
                continue; 
            }

            // 1 - 10 seconds
            int waitTime = _random.Next(1000, 10001); // Milliseconds
            await Task.Delay(waitTime);

            // Create the order
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

    [RelayCommand]
    public void AcceptOrder(Order? order)
    {
        var target = order ?? SelectedPendingOrder;

        if (target != null)
        {
            AcceptedOrders.Add(target);
            PendingOrders.Remove(target);
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
