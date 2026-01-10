using Microsoft.AspNetCore.Components;
using Rise.Domain.Entities;
using Rise.Shared.Resto;

namespace Rise.Client.Pages.Resto.Components;

public partial class CampusGroup
{
    [Parameter] public required RestoDto Value { get; set; }
    [Parameter] public bool ShowVegan { get; set; }

    [Parameter] public bool ShowVeggie { get; set; }

    [Parameter] public bool IsExpanded { get; set; }

    [Parameter] public EventCallback<int?> OnExpandRequested { get; set; }
    private bool IsCompleting { get; set; } = false;
    private bool IsLoading { get; set; } = false;
    private async Task ToggleExpand()
    {
        if (OnExpandRequested.HasDelegate)
        {
            if (IsExpanded)
            {
                await OnExpandRequested.InvokeAsync(null);
            }
            else
            {
                await OnExpandRequested.InvokeAsync(Value.Id);
            }
        }
        else
        {
            IsExpanded = !IsExpanded;
        }
    }

    private string TranslateFoodType(FoodCategory type) => type switch
    {   
        FoodCategory.Soep => Loc["Soep"].Value,
        FoodCategory.WarmeMaaltijd => Loc["WarmeMaaltijd"].Value,
        FoodCategory.Dessert => Loc["Dessert"].Value,
        FoodCategory.Zetmeel => Loc["Zetmeel"].Value,
        FoodCategory.Wekelijks => Loc["Weekly"].Value,
        FoodCategory.Broodjes => Loc["Broodjes"].Value,
        FoodCategory.Onbekend => Loc["Onbekend"].Value,
        FoodCategory.Groenten => Loc["Groenten"].Value,
        FoodCategory.Snacks => Loc["Snacks"].Value,

        _ => type.ToString()
    };
}