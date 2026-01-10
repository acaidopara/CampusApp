using Microsoft.Extensions.DependencyInjection;
using Rise.Persistence;
using Rise.Services.Absences;
using Rise.Services.CampusLife;
using Rise.Services.Deadlines;
using Rise.Services.Departments;
using Rise.Services.Lessons;
using Rise.Services.Events;
using Rise.Services.Infrastructure;
using Rise.Services.News;
using Rise.Services.Resto;
using Rise.Services.Shortcuts;
using Rise.Services.Student;
using Rise.Shared.CampusLife.Jobs;
using Rise.Shared.CampusLife.StudentClub;
using Rise.Shared.CampusLife.StudentDeals;
using Rise.Shared.Deadlines;
using Rise.Services.Notifications;
using Rise.Services.PushNotifications;
using Rise.Shared.Absences;
using Rise.Shared.Departments;
using Rise.Shared.Lessons;
using Rise.Shared.Events;
using Rise.Shared.Infrastructure;
using Rise.Shared.News;
using Rise.Shared.Resto;
using Rise.Shared.Shortcuts;
using Rise.Shared.Student;
using Rise.Shared.Notifications;
using Rise.Shared.PushNotifications;

namespace Rise.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDepartmentService, DepartmentService>();        
        services.AddScoped<ILessonService, LessonService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<IRestoService, RestoService>();    
        services.AddScoped<IDeadlineService, DeadlineService>(); 
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IRestoService, RestoService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IShortcutService, ShortcutService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IInfrastructureService, InfrastructureService>();
        services.AddScoped<IStudentClubService, StudentClubService>();
        services.AddScoped<IStudentDealService, StudentDealService>();
        services.AddScoped<IJobService, JobService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAbsenceService, AbsenceService>();
        services.AddSingleton<IPushNotificationService, PushNotificationService>();
        services.AddTransient<DbSeeder>();
        
        // Add other application services here.
        return services;
    }
}