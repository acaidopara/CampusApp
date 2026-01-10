using System.Globalization;
using Blazor.WebAssembly.DynamicCulture.Extensions;
using Blazor.WebAssembly.DynamicCulture.Loader;
using BlazorPanzoom;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Rise.Client;
using Rise.Client.Identity;
using MudBlazor.Services;
using Rise.Client.MainComponents.Navigation;
using Rise.Client.Pages.Campus.Service;
using Rise.Client.Pages.CampusLife.Service;
using Rise.Shared.Departments;
using Rise.Shared.Resto;
using Rise.Shared.News;
using Rise.Shared.Events;
using Rise.Shared.Lessons;
using Rise.Shared.Support;
using Rise.Shared.Deadlines;
using Rise.Shared.Shortcuts;
using Rise.Client.Pages.Shortcuts.Service;
using Rise.Client.Pages.Timetable.Service;
using Rise.Client.Pages.Deadlines.Service;
using Rise.Client.Pages.Contact.Service;
using Rise.Client.Pages.Events.Service;
using Rise.Client.Pages.Resto.Service;
using Rise.Client.Pages.Settings.Service;
using Rise.Client.Pages.Notification.Service;
using Rise.Shared.CampusLife.Jobs;
using Rise.Shared.CampusLife.StudentClub;
using Rise.Shared.CampusLife.StudentDeals;
using Rise.Shared.Infrastructure;
using Rise.Shared.Student;
using NewsService = Rise.Client.Pages.News.Service.NewsService;
using Rise.Shared.Notifications;
using Rise.Client.Services;
using Rise.Shared.Absences;
using Rise.Client.Api;
using Rise.Client.Api.Interceptors;
using Microsoft.JSInterop;

try
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);

    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.BrowserConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}")
        .CreateLogger();

    Log.Information("Starting web application");
    builder.Services.AddTransient<ApiExceptionHandler>();
    builder.Services.AddTransient<CookieHandler>();

    builder.Services.AddHttpClient("API", client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001");
    })
    .AddHttpMessageHandler<CookieHandler>()
    .AddHttpMessageHandler<ApiExceptionHandler>();

    builder.Services.AddScoped<ApiClient>(sp =>
    {
        var factory = sp.GetRequiredService<IHttpClientFactory>();
        var cache = sp.GetRequiredService<ILocalStorageService>();
        var nav = sp.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
        var httpClient = factory.CreateClient("API");
        return new ApiClient(httpClient, nav, cache);
    });

    // Set up authorization
    builder.Services.AddAuthorizationCore();
    builder.Services.AddLocalStorageServices();

    // add custom services
    builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
    builder.Services.AddScoped<NavigationService>();
    builder.Services.AddSingleton<NetworkState>();
    builder.Services.AddScoped<OfflineService>(sp =>
    {
        var cache = sp.GetRequiredService<ILocalStorageService>();
        var factory = sp.GetRequiredService<IHttpClientFactory>();
        var httpClient = factory.CreateClient("API");
        return new OfflineService(cache,httpClient);
    });

    builder.Services.AddScoped<ServiceFactory>();
    builder.Services.AddScoped<TransportProvider>();

    // Register the account management interface
    builder.Services.AddScoped(sp => (IAccountManager)sp.GetRequiredService<AuthenticationStateProvider>());

    // Configure named HttpClient for auth interactions
    builder.Services.AddHttpClient("SecureApi", opt => opt.BaseAddress = new Uri(builder.Configuration["BackendUrl"] ?? "https://localhost:5001"))
        .AddHttpMessageHandler<CookieHandler>();

    builder.Services.AddScoped<IDepartmentService, DepartmentService>();
    builder.Services.AddScoped<IRestoService, RestoService>();
    builder.Services.AddScoped<IShortcutService, ShortcutService>();
    builder.Services.AddScoped<IInfrastructureService, InfrastructureService>();
    builder.Services.AddScoped<ILessonService, LessonService>();
    builder.Services.AddScoped<IEventService, EventService>();
    builder.Services.AddScoped<INewsService, NewsService>();
    builder.Services.AddScoped<IStudentService, StudentService>();
    builder.Services.AddScoped<IDeadlineService, DeadlineService>();
    builder.Services.AddScoped<IStudentClubService, StudentClubService>();
    builder.Services.AddScoped<IJobService, JobService>();
    builder.Services.AddScoped<IStudentDealService, StudentDealService>();
    builder.Services.AddScoped<ISupportService, FakeSupportService>();
    builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.AddScoped<IAbsenceService, AbsenceService>();

    // Adding MudBlazor (component library)
    builder.Services.AddMudServices();

    builder.Services.AddBlazorPanzoomServices();


    // Add localization with dynamic culture support
    builder.Services.AddLocalization();
    builder.Services.AddLocalizationDynamic(options =>
    {
        options.SetDefaultCulture("nl");
        options.AddSupportedCultures("en");
        options.AddSupportedUICultures("en");
    });

    var host = builder.Build();
    var net = host.Services.GetRequiredService<NetworkState>();
    //await net.InitializeAsync();

    await host.LoadSatelliteCultureAssembliesCultureAsync([new CultureInfo("nl"), new CultureInfo("en")]);
    CultureInfo.CurrentCulture = new CultureInfo("nl");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An exception occurred while creating the WASM host");
    throw;
}