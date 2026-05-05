using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RestaurantSimulator.Models;

public partial class Order : ObservableObject
{
    public string OrderId { get; set; } = string.Empty;
    public DateTime TakenAt { get; set; }
    public Recipe? SelectedRecipe { get; set; }

    [ObservableProperty]
    private bool isCompleted = false;
    [ObservableProperty]
    private string _currentStep = "Waiting";
    [ObservableProperty]
    private int _overallProgress = 0;
}