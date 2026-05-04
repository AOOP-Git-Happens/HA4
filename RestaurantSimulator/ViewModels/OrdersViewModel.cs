using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using RestaurantSimulator.Models;
using RestaurantSimulator.Services;

namespace RestaurantSimulator.ViewModels;

public partial class OrdersViewModel : ViewModelBase
{
    private readonly RestaurantDataService _dataService;
    private readonly Random _random = new();
    public ObservableCollection<Recipe> Recipes { get; set; }
    public ObservableCollection<Order> CreatedOrders { get; } = new();
    
    public OrdersViewModel()
    {
        Header = "Orders";
        // Load the data using your service
        _dataService = new RestaurantDataService();
    }

    [RelayCommand]
    public void AddRandomOrder()
    {
        // 1. Ensure we actually have recipes loaded from the JSON
        if (_dataService.Recipes == null || !_dataService.Recipes.Any())
            return;

        // 2. Pick a random recipe
        var randomRecipe = _dataService.Recipes[_random.Next(_dataService.Recipes.Count)];

        // 3. Create the new Order object
        var newOrder = new Order
        {
            OrderId = $"ORD-{_random.Next(1000, 9999)}",
            TakenAt = DateTime.Now,
            SelectedRecipe = randomRecipe
        };

        // 4. Add it to the collection (this updates the UI automatically)
        CreatedOrders.Add(newOrder);
    }
}

