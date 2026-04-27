using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RestaurantSimulator.Models;

namespace RestaurantSimulator.Services;

public class RestaurantDataService
{
    public List<Recipe> Recipes { get; private set; } = new();
    public List<Station> Stations { get; private set; } = new();
    public List<Ingredient> Ingredients { get; private set; } = new();

    public RestaurantDataService()
    {
        LoadRestaurantData();
    }

    public void LoadRestaurantData()
    {
        string filepath = "Assets/recipes.json";

        if(!File.Exists(filepath))
        {
            throw new FileNotFoundException($"File not found: {filepath}");
        }
        
        string jsonString = File.ReadAllText(filepath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        RestaurantData? restaurantData = JsonSerializer.Deserialize<RestaurantData>(jsonString, options);

        if (restaurantData != null)
        {
            Recipes = restaurantData.Recipes ?? new List<Recipe>();
            Stations = restaurantData.Stations ?? new List<Station>();
            Ingredients = restaurantData.Ingredients ?? new List<Ingredient>();
        }
    }
}