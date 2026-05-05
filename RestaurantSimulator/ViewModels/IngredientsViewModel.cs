using System.Collections.ObjectModel;
using RestaurantSimulator.Models;
using RestaurantSimulator.Services;

namespace RestaurantSimulator.ViewModels;

public partial class IngredientsViewModel : ViewModelBase
{
    public ObservableCollection<Ingredient> Ingredients { get; set; }
    
    private readonly RestaurantDataService _dataService;

    public IngredientsViewModel()
    {
        Header = "Ingredients";
        
        // Load the data using your service
        _dataService = new RestaurantDataService();
        
        // Expose the loaded ingredients to the UI
        Ingredients = new ObservableCollection<Ingredient>(_dataService.Ingredients);
    }
}