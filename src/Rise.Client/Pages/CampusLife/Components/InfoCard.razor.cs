using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Events;

namespace Rise.Client.Pages.CampusLife.Components;

public partial class InfoCard
{
    [Parameter] public string? Date { get; set; }
    [Parameter] public string? Money { get; set; }
    [Parameter] public object? Location { get; set; }

    private bool HasMoney
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Money))
                return false;

            var match = Regex.Match(Money, @"[0-9]+([.,][0-9]{1,2})?");
            if (!match.Success)
                return false;

            var cleanValue = match.Value;

            return decimal.TryParse(cleanValue, NumberStyles.Any, CultureInfo.CurrentCulture, out var amount)
                   && amount != 0;
        }
    }

    private string GetLocationText()
    {
        return Location switch
        {
            null => "",
            string s => s,
            AddressDto.Index a => string.Join(", ",
                                      new[] { a.AddressLine1, a.AddressLine2 }.Where(s =>
                                          !string.IsNullOrWhiteSpace(s)))
                                  + $", {a.PostalCode} {a.City}",
            _ => Location.ToString() ?? ""
        };
    }
}