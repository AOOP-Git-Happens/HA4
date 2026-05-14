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
        
        _dataService = RestaurantDataService.Instance;
        
        Ingredients = new ObservableCollection<Ingredient>(_dataService.Ingredients);
    }
}