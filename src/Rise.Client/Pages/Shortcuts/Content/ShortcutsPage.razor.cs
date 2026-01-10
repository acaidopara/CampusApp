using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Rise.Shared.Common;
using Rise.Shared.Shortcuts;

namespace Rise.Client.Pages.Shortcuts.Content
{
    public class SnelkoppelingenBase : ComponentBase
    {
        [Inject] public required IShortcutService ShortcutService { get; set; }
        [Inject] public required AuthenticationStateProvider AuthProvider { get; set; }
        [Inject] public required NavigationManager NavManager { get; set; }
        [Inject] public required ISnackbar Snackbar { get; set; }
        [Inject] public required IStringLocalizer<Resources.Pages.Shortcuts.Shortcuts> Loc { get; set; }

        protected IEnumerable<IGrouping<ShortcutType, ShortcutDto.Index>> QuicklinkSections { get; private set; } = new List<IGrouping<ShortcutType, ShortcutDto.Index>>();
        protected IEnumerable<ShortcutDto.Index> Quicklinks { get; private set; } = new List<ShortcutDto.Index>();

        protected bool IsManageMode { get; private set; }
        private int? UserId { get; set; }

        private List<int> ToAdd { get; } = [];
        private List<int> ToRemove { get; } = [];

        private record ShortcutColourPair(int ShortcutId, string Colour);

        private List<ShortcutColourPair> ShortcutColourList { get; } = [];

        private List<ShortcutDto.Index> OriginalQuicklinks { get; set; } = [];

        protected const string DefaultColour = "var(--secondary-color)";

        protected bool _isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

            if (query["guest"] == "true")
            {
                Snackbar.Add(Loc["LoginRequired"].Value, Severity.Warning);
            }

            var authState = await AuthProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var userIdString = user.FindFirst("Id")?.Value;

            if (int.TryParse(userIdString, out var parsedId))
                UserId = parsedId;
            
            var fetchTasks = new List<Task>
            {
                FetchShortcutsAsync(),
                LoadAllShortcutsAsync(false)
            };

            await Task.WhenAll(fetchTasks);

            _isLoading = false;
            StateHasChanged();
        }

        private async Task FetchShortcutsAsync()
        {
            if (UserId.HasValue)
            {
                var request = new ShortcutRequest.GetForUser() { UserId = UserId.Value };
                var result = await ShortcutService.GetUserShortcutsAsync(request);
                Quicklinks = result.IsSuccess ? result.Value.Shortcuts : new List<ShortcutDto.Index>();
            }
            else
            {
                var defaults = await ShortcutService.GetDefaultShortcuts(new QueryRequest.SkipTake());
                if (defaults.IsSuccess)
                    Quicklinks = defaults.Value.Shortcuts;
            }
        }

        private async Task LoadAllShortcutsAsync(bool needsFiltering)
        {
            var all = await ShortcutService.GetIndexAsync(new QueryRequest.SkipTake());
            if (!all.IsSuccess) return;

            var shortcuts = all.Value.Shortcuts;

            if (needsFiltering && Quicklinks.Any())
            {
                var existingIds = Quicklinks.Select(q => q.Id).ToHashSet();
                shortcuts = shortcuts.Where(s => !existingIds.Contains(s.Id)).ToList();
            }

            QuicklinkSections = shortcuts.GroupBy(x => x.ShortcutType);
        }


        protected async Task ToggleManageMode()
        {
            if (!IsManageMode)
            {
                OriginalQuicklinks = Quicklinks
                    .Select(q => new ShortcutDto.Index
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Icon = q.Icon,
                        Label = q.Label,
                        LinkUrl = q.LinkUrl,
                        ShortcutType = q.ShortcutType,
                        Colour = q.Colour
                    })
                    .ToList();
            }
            else
            {
                Quicklinks = OriginalQuicklinks
                    .Select(q => new ShortcutDto.Index
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Icon = q.Icon,
                        Label = q.Label,
                        LinkUrl = q.LinkUrl,
                        ShortcutType = q.ShortcutType,
                        Colour = q.Colour
                    })
                    .ToList();

                ToAdd.Clear();
                ToRemove.Clear();
                ShortcutColourList.Clear();
                Snackbar.Add(Loc["WijzigingenGeannuleerd"].Value, Severity.Info);
            }

            IsManageMode = !IsManageMode;
            StateHasChanged();
            await LoadAllShortcutsAsync(IsManageMode);
        }

        protected async Task AddShortcut(int? id)
        {
            if (!id.HasValue || id.Value == 0) return;

            if (!UserId.HasValue)
            {
                var allShortcuts = QuicklinkSections.SelectMany(x => x).ToList();
                var shortcutToNavigate = allShortcuts.FirstOrDefault(s => s.Id == id.Value);
                if (shortcutToNavigate?.LinkUrl != null)
                    NavManager.NavigateTo(shortcutToNavigate.LinkUrl);
                return;
            }

            if (!IsManageMode)
            {
                var allShortcuts = QuicklinkSections.SelectMany(x => x).ToList();
                var shortcutToNavigate = allShortcuts.FirstOrDefault(s => s.Id == id.Value);
                if (shortcutToNavigate?.LinkUrl != null)
                {
                    NavManager.NavigateTo(shortcutToNavigate.LinkUrl);
                }
                return;
            }

            var allShortcutsInEditMode = QuicklinkSections.SelectMany(x => x).ToList();
            var shortcutToAdd = allShortcutsInEditMode.FirstOrDefault(s => s.Id == id.Value);
            if (shortcutToAdd is null) return;

            if (Quicklinks.All(q => q.Id != id.Value))
            {
                Quicklinks = Quicklinks.Append(shortcutToAdd).ToList();
                ToAdd.Add(id.Value);
                ToRemove.Remove(id.Value);
                await LoadAllShortcutsAsync(true);
            }
        }


        protected async Task RemoveShortcut(int id)
        {
            if (!UserId.HasValue) return;

            Quicklinks = Quicklinks.Where(q => q.Id != id).ToList();
            ToRemove.Add(id);
            ToAdd.Remove(id);

            await LoadAllShortcutsAsync(true);
        }

        protected void HandleMoveLeft(int index)
        {
            if (index <= 0)
            {
                Snackbar.Add(Loc["NietVerderNaarLinks"].Value, Severity.Warning);
                return;
            }

            var list = Quicklinks.ToList();
            (list[index - 1], list[index]) = (list[index], list[index - 1]);
            Quicklinks = list;
        }

        protected void HandleMoveRight(int index)
        {
            var list = Quicklinks.ToList();
            if (index >= list.Count - 1)
            {
                Snackbar.Add(Loc["NietVerderNaarRechts"].Value, Severity.Warning);
                return;
            }

            (list[index], list[index + 1]) = (list[index + 1], list[index]);
            Quicklinks = list;
        }

        protected async Task SaveShortcuts()
        {
            bool showSuccesMessage = true;
            if (!UserId.HasValue) return;

            bool orderChanged = !Quicklinks.Select(q => q.Id)
                .SequenceEqual(OriginalQuicklinks.Select(q => q.Id));

            if (!ToAdd.Any() && !ToRemove.Any() && !orderChanged && !ShortcutColourList.Any())
            {
                Snackbar.Add(Loc["GeenWijzigingen"].Value, Severity.Info);
                await FetchShortcutsAsync();
                await LoadAllShortcutsAsync(false);
                return;
            }

            foreach (var id in ToRemove.Distinct())
            {
                var res = await ShortcutService.RemoveShortcutFromUserAsync(new ShortcutRequest.RemoveFromUser { ShortcutId = id, UserId = UserId.Value });
                if (res.Errors.Contains("Request not send"))
                {
                    Snackbar.Add(Loc["OfflineMessage"].Value, Severity.Error);
                    showSuccesMessage = false;
                }
            }


            foreach (var id in ToAdd.Distinct())
            {
                var res = await ShortcutService.AddShortcutToUserAsync(new ShortcutRequest.AddToUser { ShortcutId = id, UserId = UserId.Value });
                if (res.Errors.Contains("Request not send"))
                {
                    Snackbar.Add(Loc["OfflineMessage"].Value, Severity.Error);
                    showSuccesMessage = false;
                }
            }


            if (orderChanged)
            {
                var res = await UpdateShortcutOrderAsync();
                if (!res)
                {
                    showSuccesMessage = false;
                }
            }

            foreach (var colourPair in ShortcutColourList)
            {
                var res = await HandleColourChanged(colourPair.ShortcutId, colourPair.Colour);
                if (!res)
                {
                    showSuccesMessage = false;
                }
            }

            ToAdd.Clear();
            ToRemove.Clear();
            OriginalQuicklinks.Clear();
            IsManageMode = false;
            ShortcutColourList.Clear();

            if (showSuccesMessage) Snackbar.Add(Loc["SnelkoppelingenOpgeslagen"].Value, Severity.Success);

            await FetchShortcutsAsync();
            await LoadAllShortcutsAsync(false);
        }


        private async Task<bool> UpdateShortcutOrderAsync()
        {
            if (!UserId.HasValue) return true;

            var orderedIds = Quicklinks.Select(q => q.Id).ToList();

            if (!orderedIds.Any())
                return true;

            var request = new ShortcutRequest.ChangeOrder
            {
                UserId = UserId.Value,
                OrderedShortcutIds = orderedIds
            };

            var response = await ShortcutService.UpdateShortcutOrderForUserAsync(request);
            if (!response.IsSuccess && response.Errors.Contains("Request not send"))
            {
                Snackbar.Add(Loc["OfflineMessage"].Value, Severity.Error);
                return false;
            }
            else if (!response.IsSuccess)
            {
                Snackbar.Add(Loc["FoutBijOpslaanVolgorde"].Value, Severity.Error);
                return false;
            }
            return true;
        }

        protected string GetShortcutTypeName(ShortcutType type) =>
            type switch
            {
                ShortcutType.CommunicationAndSoftware => Loc["CommunicatieEnSoftware"].Value,
                ShortcutType.SchedulesAndCalendars => Loc["RoostersEnKalenders"].Value,
                ShortcutType.Studyservices => Loc["Studiediensten"].Value,
                ShortcutType.SupportAndIt => Loc["OndersteuningEnIt"].Value,
                ShortcutType.StudentLifeAndWellbeing => Loc["StudentenlevenEnWelzijn"].Value,
                ShortcutType.AlertsAndAbsence => Loc["MeldingenEnAfwezigheid"].Value,
                _ => type.ToString()
            };

        private async Task<bool> HandleColourChanged(int shortcutId, string colour)
        {
            if (!UserId.HasValue) return true;

            var request = new ShortcutRequest.UpdateColour
            {
                UserId = UserId.Value,
                ShortcutId = shortcutId,
                Colour = colour
            };

            var response = await ShortcutService.UpdateUserShortcutColourAsync(request);
            if (!response.IsSuccess && response.Errors.Contains("Request not send"))
            {
                Snackbar.Add(Loc["OfflineMessage"].Value, Severity.Error);
                return false;
            }
            else if (!response.IsSuccess)
            {
                Snackbar.Add("Fout bij opslaan kleur.", Severity.Error);
                return false;
            }

            var list = Quicklinks.ToList();
            var idx = list.FindIndex(q => q.Id == shortcutId);
            if (idx >= 0)
            {
                list[idx].Colour = colour;
                Quicklinks = list;
            }
            StateHasChanged();
            return true;
        }

        protected void ColorList(int shortcutId, string colour)
        {
            if (shortcutId <= 0 || string.IsNullOrWhiteSpace(colour))
                return;

            var existingIndex = ShortcutColourList.FindIndex(c => c.ShortcutId == shortcutId);
            if (existingIndex >= 0)
            {
                ShortcutColourList[existingIndex] = ShortcutColourList[existingIndex] with { Colour = colour };
            }
            else
            {
                ShortcutColourList.Add(new ShortcutColourPair(shortcutId, colour));
            }

            var list = Quicklinks.ToList();
            var idx = list.FindIndex(q => q.Id == shortcutId);
            if (idx >= 0)
            {
                list[idx].Colour = colour;
                Quicklinks = list;
            }

            StateHasChanged();
        }
    }
}