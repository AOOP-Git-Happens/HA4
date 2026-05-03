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
        string filepath = "Assets/Recipes.json";

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

    public void SaveRestaurantData()
    {
        string filepath = "Assets/Recipes.json";
        
        // Rebuild the main data object
        var restaurantData = new RestaurantData
        {
            Recipes = this.Recipes,
            Stations = this.Stations,
            Ingredients = this.Ingredients
        };

        // Use camelCase to match your original JSON formatting
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        };

        string jsonString = JsonSerializer.Serialize(restaurantData, options);
        File.WriteAllText(filepath, jsonString);
    }
}