using System.Collections.Generic;

namespace RestaurantSimulator.Models;

public class Order
{
    public int Id { get; set; }
    public string Description { get; set; }
}

public class Station
{
    public string Name { get; set; }
}

public class Ingredient
{
    public string Name { get; set; }
    public int Quantity { get; set; }
}

public class RestaurantData
{
    public List<Order> Orders { get; set; } = new();
    public List<Station> Stations { get; set; } = new();
    public List<Ingredient> Ingredients { get; set; } = new();
}