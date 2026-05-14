using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using RestaurantSimulator.Models;
using RestaurantSimulator.Services;

namespace RestaurantSimulator.ViewModels;

public partial class StationsViewModel : ViewModelBase
{
    private readonly RestaurantDataService _dataService;

    public ObservableCollection<KitchenStation> Stations { get; } = new();

    public StationsViewModel()
    {
        Header = "Stations";

        _dataService = RestaurantDataService.Instance;

        LoadStations();
    }

    private void LoadStations()
    {
        foreach (var stationDefinition in _dataService.Stations)
        {
            for (int i = 1; i <= stationDefinition.DefaultCount; i++)
            {
                Stations.Add(new KitchenStation
                {
                    StationId = $"{stationDefinition.Type}-{i}",
                    Type = stationDefinition.Type,
                    IsBusy = false,
                    CurrentOrderId = "Idle",
                    CurrentStepName = "Waiting",
                    Progress = 0
                });
            }
        }
    }

    public async Task ProcessOrderAsync(Order order)
    {
        if (order.SelectedRecipe == null)
            return;

        foreach (var requiredIngredient in order.SelectedRecipe.RequiredIngredients)
        {
            var stockIngredient = _dataService.Ingredients.FirstOrDefault(i => i.Name == requiredIngredient.Name);
            
            if (stockIngredient != null)
            {
                await Dispatcher.UIThread.InvokeAsync(() => 
                {
                    stockIngredient.InitialStock -= requiredIngredient.Quantity;
                });
            }
        }

        int totalSteps = order.SelectedRecipe.Steps.Count;
        int completedSteps = 0;

        foreach (var step in order.SelectedRecipe.Steps)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                order.CurrentStep = step.Step;
                order.OverallProgress = completedSteps * 100 / totalSteps;
            });

            KitchenStation station = await WaitForFreeStationAsync(step.StationType);
            await RunStepOnStationAsync(station, order, step);

            completedSteps++;

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                order.OverallProgress = completedSteps * 100 / totalSteps;
            });
        }

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            order.IsCompleted = true;
            order.CurrentStep = "Completed";
            order.OverallProgress = 100;
        });
    }

    private async Task<KitchenStation> WaitForFreeStationAsync(string stationType)
    {
        while (true)
        {
            KitchenStation? station = null;

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                station = Stations.FirstOrDefault(s =>
                    s.Type == stationType && !s.IsBusy);

                if (station != null)
                {
                    station.IsBusy = true;
                }
            });

            if (station != null)
                return station;

            await Task.Delay(500);
        }
    }

    private void MoveBusyStationToTop(KitchenStation station)
    {
        int oldIndex = Stations.IndexOf(station);

        if (oldIndex > 0)
        {
            Stations.Move(oldIndex, 0);
        }
    }

    private async Task RunStepOnStationAsync(
        KitchenStation station,
        Order order,
        RecipeStep step)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            MoveBusyStationToTop(station);

            station.CurrentOrderId = order.OrderId;
            station.CurrentStepName = step.Step;
            station.Progress = 0;
        });

        int totalMilliseconds = step.Duration * 1000; 

        for (int progress = 0; progress <= 100; progress += 10)
        {
            await Task.Delay(totalMilliseconds / 10);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                station.Progress = progress;
            });
        }

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            station.IsBusy = false;
            station.CurrentOrderId = "Idle";
            station.CurrentStepName = "Waiting";
            station.Progress = 0;
        });
    }
}