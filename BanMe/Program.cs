using BanMe.Components;
using BanMe.Data;
using BanMe.Jobs;
using BanMe.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDbContext<BanMeContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IChampGameStatsService, ChampGameStatsService>();

builder.Services.AddScoped<IBanMeInfoService, BanMeInfoService>();

builder.Services.AddQuartz(options =>
{
    options.UseMicrosoftDependencyInjectionJobFactory();


    var jobKey = JobKey.Create(nameof(PatchUpdateBackgroundJob));
    
    options
        .AddJob<PatchUpdateBackgroundJob>(jobKey)
        .AddTrigger(trigger =>
            trigger
                .ForJob(jobKey)
                .WithSimpleSchedule(schedule =>
                schedule.WithIntervalInSeconds(10)
                .RepeatForever())
                );
});

builder.Services.AddQuartzHostedService();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
    //await SeedData.InitPlayerDb(services);
    //await SeedData.UpdateBanMeInfoPatch(services);
	//await SeedData.InitChampGameStatsDb(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BanMe.Client._Imports).Assembly);

app.Run();
