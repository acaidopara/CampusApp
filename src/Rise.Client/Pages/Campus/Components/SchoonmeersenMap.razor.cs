using BlazorPanzoom;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Infrastructure;

namespace Rise.Client.Pages.Campus.Components;

public partial class SchoonmeersenMap
{
    [Parameter] public required CampusDto.Index Campus { get; set; }
    [Parameter] public ClassroomDto.Index? SelectedClassroom { get; set; }
    [Parameter] public BuildingDto.Index? SelectedBuilding2 { get; set; }

    private double? _dx;
    private double? _dy;

    private Panzoom? _panzoom;

    private readonly PanzoomOptions _panzoomOptions = new()
    {
        Contain = Contain.Outside,
        StartScale = 1.5
    };

    protected override void OnParametersSet()
    {
        string? buildingName = null;

        if (SelectedClassroom != null && SelectedClassroom.Building.CampusId == Campus.Id)
            buildingName = SelectedClassroom.Building.Name;
        else if (SelectedBuilding2 != null && SelectedBuilding2.CampusId == Campus.Id)
            buildingName = SelectedBuilding2.Name;

        if (buildingName == null)
        {
            _dx = 0;
            _dy = 0;
            return;
        }

        double targetX = 391.518;
        double targetY = 320.809;
        
        switch (buildingName.ToLowerInvariant())
        {
            case "gebouw a":
                targetX = 200;
                targetY = 150;
                break;
            case "gebouw b":
                targetX = -420;
                targetY = 30;
                break;
            case "gebouw c":
                targetX = -150;
                targetY = 0;
                break;
            case "gebouw d":
                targetX = -330;
                targetY = 10;
                break;
            case "gebouw e":
                targetX = -200;
                targetY = 100;
                break;
            case "gebouw t":
                targetX = -40;
                targetY = 440;
                break;
            case "gebouw p":
                targetX = -395;
                targetY = -280;
                break;
            case "sporthal":
                targetX = -245;
                targetY = -380;
                break;
        }
        _dx = targetX;
        _dy = targetY;
    }
}