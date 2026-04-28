using System.Collections.Generic;

namespace RestaurantSimulator.Models;

public class RestaurantData
{
    public List<Station> Stations { get; set; } = new();
    public List<Ingredient> Ingredients { get; set; } = new();
    public List<Recipe> Recipes { get; set; } = new();
}

public class Station
{
    public string Type { get; set; } = string.Empty;
    public int DefaultCount { get; set; } = 0;
}

public class Ingredient
{
    public string Name { get; set; } = string.Empty;
    public double InitialStock { get; set; } = 0;
    public string Unit { get; set; } = string.Empty;
    public double Cost { get; set; } = 0;
}

public class Recipe
{
    public string Name { get; set;} = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public double SalePrice { get; set; } 
    public List<RequiredIngredient> RequiredIngredients { get; set; } = new();
    public List<RecipeStep> Steps{ get; set; } = new();
}

public class RequiredIngredient
{

    public string Name { get; set; } = string.Empty;
    public double Quantity { get; set; } 
}

public class RecipeStep
{
    public string Step { get; set; } = string.Empty;
    public int Duration { get; set; } = 0;
    public string StationType { get; set; } = string.Empty;
}