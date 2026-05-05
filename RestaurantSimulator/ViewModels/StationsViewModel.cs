using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using RestaurantSimulator.Models;
using RestaurantSimulator.Services;

namespace RestaurantSimulator.ViewModels;

/// <summary>
/// handling kitchen stations and processing orders
/// creates station instances from json definitions
/// assigns recipe steps to correct stations
/// simulates step execution with async tasks
/// updates ui progress in real time
/// </summary>

public partial class StationsViewModel : ViewModelBase
{
    private readonly RestaurantDataService _dataService;

    //list of active kitchen station (bound to UI)
    public ObservableCollection<KitchenStation> Stations { get; } = new();

    public StationsViewModel()
    {
        Header = "Stations";

        _dataService = new RestaurantDataService();

        //initialise stations based on json definitions
        LoadStations();
    }

    //creates station instances based on type and default count
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

    //main method that processes an entire order step-by-step
    public async Task ProcessOrderAsync(Order order)
    {
        if (order.SelectedRecipe == null)
            return;

        int totalSteps = order.SelectedRecipe.Steps.Count;
        int completedSteps = 0;

        foreach (var step in order.SelectedRecipe.Steps)
        {
            //update order status in UI
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                order.CurrentStep = step.Step;
                order.OverallProgress = completedSteps * 100 / totalSteps;
            });

            //wait until correct stations is avaliable 
            KitchenStation station = await WaitForFreeStationAsync(step.StationType);
            await RunStepOnStationAsync(station, order, step);

            completedSteps++;

            //update overall progress
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                order.OverallProgress = completedSteps * 100 / totalSteps;
            });
        }

        //make order as completed
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            order.IsCompleted = true;
            order.CurrentStep = "Completed";
            order.OverallProgress = 100;
        });
    }

    //wait until a free station of the required type is avaliable
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

    //move active station to top of UI list
    private void MoveBusyStationToTop(KitchenStation station)
    {
        int oldIndex = Stations.IndexOf(station);

        if (oldIndex > 0)
        {
            Stations.Move(oldIndex, 0);
        }
    }

    //simulate execution of single recipe step
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

        //simulated time based on json duration
        int totalMilliseconds = step.Duration * 1000; //process time

        for (int progress = 0; progress <= 100; progress += 10)
        {
            await Task.Delay(totalMilliseconds / 10);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                station.Progress = progress;
            });
        }

        //reset station after finishing step
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            station.IsBusy = false;
            station.CurrentOrderId = "Idle";
            station.CurrentStepName = "Waiting";
            station.Progress = 0;
        });
    }
}