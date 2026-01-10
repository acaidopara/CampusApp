using Rise.Shared.Common;
using Rise.Shared.Departments;

namespace Rise.Client.Pages.Contact.Content;

public partial class ContactPage
{
    private IEnumerable<DepartmentDto.Index> _department = [];
    private IEnumerable<DepartmentDto.Index> _management = [];
    private IEnumerable<DepartmentDto.Index> _other = [];
    private bool _isLoading = true;

    private async Task FetchDepartment()
    {
        try
        {
            var request = new QueryRequest.SkipTake { Skip = 0 };
            var result = await DepartmentService.GetIndexAsync(request);

            var allDepartments = result.IsSuccess
                ? result.Value.Departments
                : [];

            var departments = allDepartments as DepartmentDto.Index[] ?? allDepartments.ToArray();
            _department = departments.Where(d => d.DepartmentType == "Department");
            _management = departments.Where(d => d.DepartmentType == "Management");
            _other = departments.Where(d => d.DepartmentType == "Other");
        }
        catch
        {
            _department = [];
            _management = [];
            _other = [];
        }
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await FetchDepartment();
        }
        catch
        {
            Console.WriteLine(Loc["ErrorLoadingDepartments"].Value);
        }

        _isLoading = false;
    }
    
    private Task OnDepartmentSelected(DepartmentDto.Index dept)
    {
        Nav.NavigateTo($"/contact/{dept.Id}");
        return Task.CompletedTask;
    }

}