using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.Infrastructure;

namespace Rise.Client.Pages.Campus.Components;

public partial class FacilitySection
{
    [Parameter] public required FacilityDto.Index Facilities { get; set; }
    private List<Facility> _list = [];

    protected override void OnParametersSet()
    {
        _list =
        [
            new Facility("Bibliotheek", Icons.Material.Filled.MenuBook, Facilities.Library),
            new Facility("RITA Helpdesk", Icons.Material.Filled.Computer, Facilities.RitaHelpdesk),
            new Facility("Revolte Lokaal", Icons.Material.Filled.Groups, Facilities.RevolteRoom),
            new Facility("Parking", Icons.Material.Filled.LocalParking, Facilities.ParkingLot),
            new Facility("Fietsenstalling", Icons.Material.Filled.PedalBike, Facilities.BikeStorage),
            new Facility("Student Shop", Icons.Material.Filled.Store, Facilities.StudentShop),
            new Facility("Restaurant", Icons.Material.Filled.Restaurant, Facilities.Restaurant),
            new Facility("Cafetaria", Icons.Material.Filled.Coffee, Facilities.Cafeteria),
            new Facility("Sporthal", Icons.Material.Filled.SportsBasketball, Facilities.SportsHall),
            new Facility("STUVO", Icons.Material.Filled.School, Facilities.Stuvo),
            new Facility("Lockers", Icons.Material.Filled.Lock, Facilities.Lockers)
        ];
    }
}