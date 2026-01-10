
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Rise.Client.Pages.Deadlines.Service;
using Rise.Domain.Entities;
using Rise.Shared.Deadlines;
using Rise.Shared.Resto;
using Rise.Shared.Shortcuts;

namespace Rise.Client.Pages.Resto.Components;

/// <summary>
/// Partial class for the Menugroupwrapper Razor component.
/// </summary>
public partial class MenuGroupWrapper
{
    protected string translateFoodType(FoodCategory type) => type switch
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