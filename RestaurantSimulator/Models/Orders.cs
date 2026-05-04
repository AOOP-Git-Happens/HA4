using System;

namespace RestaurantSimulator.Models;

public class Order
{
    public string OrderId { get; set; } = string.Empty;
    public DateTime TakenAt { get; set; }
    public Recipe? SelectedRecipe { get; set; }
    public bool IsCompleted { get; set; } = false;
    public bool IsRejected { get; set; } = false;
}