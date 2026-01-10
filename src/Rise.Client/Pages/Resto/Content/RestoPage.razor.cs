using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rise.Shared.Common;
using Rise.Shared.Infrastructure;
using Rise.Shared.Resto;

namespace Rise.Client.Pages.Resto.Content
{
    public partial class RestoPage
    {
        [Parameter] public int? campusId { get; set; }
        [Parameter] public int? buildingId { get; set; }
        [Inject] public required IRestoService RestoService { get; set; }
        [Inject] public required IInfrastructureService InfrastructureService { get; set; }
        private List<(int id, string campusName)> _campuses = [];
        private readonly List<string> _weekOrder = ["Ma", "Di", "Wo", "Do", "Vr"];
        private readonly string _abbrev = DateTime.Now.ToString("ddd", new CultureInfo("nl-NL"));
        public required ILookup<string, RestoDto> RestoMenusStructured; // key is campusNaam, comment aub niet weer verwijderen.
        private string? _geselecteerdeCampus;
        private string? _preferedCampus;
        private bool _isVegan;
        private bool _isVeggie;
        private bool _isRestosLoading;
        private bool _isLoading = true;
        private static DateTime MondayDate => DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);

        protected override async void OnParametersSet()
        {
            if (campusId == null || buildingId == null)
            {
                LoadGeneralRestoView();
            }
            else
            {
                await LoadCampusRestoView(campusId.Value, buildingId.Value);
            }
        }

        void LoadGeneralRestoView()
        {

        }

        async Task LoadCampusRestoView(int campusId, int buildingId)
        {
            var req = new BuildingRequest.GetById
            {
                BuildingId = buildingId,
                CampusId = campusId
            };
            var buildings = await InfrastructureService.GetBuildingById(req);

            var searchReq = new SearchRequest.SkipTake
            {
                Skip = 0,
                Take = 50,
                OrderBy = "Id",
                SearchTerm = buildings.Value.Building.Name
            };
            var restosOfBuilding = mapToLookup((await RestoService.GetIndexAsync(searchReq)).Value.Restos.ToList());
            await FetchRestos(campusId);
            var campus = _campuses.Where(c => c.id.Equals(campusId)).First();
            _geselecteerdeCampus = campus.campusName;
            if (restosOfBuilding[campus.campusName].Count() == 1)
            {
                _expandedRestoId = restosOfBuilding[campus.campusName].First().Id;
            }
            StateHasChanged();
        }

        private List<RestoDto> GetRestosFromCampus(string campusName)
        {
            return RestoMenusStructured[campusName].ToList();
        }

        protected override async Task OnInitializedAsync()
        {
            var request = new QueryRequest.SkipTake
            {
                Skip = 0,
                Take = 50,
                OrderBy = "Id",
            };

            var result = await InfrastructureService.GetCampusesIndexAsync(request);
            _campuses = result.IsSuccess ? result.Value.Campuses.Where(c => c.HasResto).Select(c => (c.Id, c.Name)).ToList() : new List<(int Id, string Name)>();

            await LoadCurrentCampus();
            _isLoading = false;
        }
        private async Task FetchRestos(int campusId)
        {
            _isRestosLoading = true;
            var searchRequest = new GetByIdRequest.GetById
            {
                Id = campusId,
            };
            RestoMenusStructured = mapToLookup((await InfrastructureService.GetRestosFromCampus(searchRequest)).Value.Restos.ToList());
            _isRestosLoading = false;
            StateHasChanged();
        }

        private ILookup<string, RestoDto> mapToLookup(List<RestoDto> restos) // om duplicate code te vermijden.
        {
            return restos.Select(resto =>
                {
                    var todayIndex = _weekOrder.IndexOf(_abbrev);

                    resto.Menu.Items = resto.Menu.Items
                        .Where(kvp => _weekOrder.IndexOf(kvp.Key) >= todayIndex)
                        .OrderBy(kvp => _weekOrder.IndexOf(kvp.Key))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    return resto;
                })
            .ToLookup(r => r.CampusName);
        }

        private async Task LoadCurrentCampus()
        {
            try
            {
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                if (user.Identity?.IsAuthenticated == true)
                {
                    var campusClaim = user.FindFirst("PreferedCampus");
                    if (campusClaim != null && !string.IsNullOrEmpty(campusClaim.Value))
                    {
                        _preferedCampus = "Campus " + campusClaim.Value;
                    }
                    else
                    {
                        _preferedCampus = "Campus Schoonmeersen";
                    }
                    if (_campuses.Count == 0)
                    {
                        throw new InvalidOperationException("Offline");
                    }
                    var campus = _campuses.Where(c => c.campusName.Equals(_preferedCampus)).First();
                    await FetchRestos(campus.id);
                    if (RestoMenusStructured[campus.campusName].Count() == 1)
                    {
                        _expandedRestoId = RestoMenusStructured[campus.campusName].First().Id;
                    }
                    _geselecteerdeCampus = campus.campusName;
                }
            }
            catch(InvalidOperationException)
            {
                Snackbar.Add($"Error loading campus preference: {Loc["OfflineMessage"]}", Severity.Error);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading campus preference: {ex.Message}", Severity.Error);
            }
        }
    }

}