using Microsoft.AspNetCore.Components;
using Rise.Shared.Common;
using Rise.Shared.News;

namespace Rise.Client.Pages.News.Content;

public partial class NewsArticlePage
{
    [Parameter] public int NewsId { get; set; }
    private NewsDto.DetailExtended? _article;

    protected override async Task OnInitializedAsync()
    {
        var result = await NewsService.GetNewsById(new GetByIdRequest.GetById
        {
            Id = NewsId
        });
        Console.WriteLine(result);
        _article = result.IsSuccess ? result.Value.News : null;
    }
}