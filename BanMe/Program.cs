using BanMe.Components;
using BanMe.Data;
using BanMe.Services;
using Microsoft.EntityFrameworkCore;
using BanMeInfrastructure;
using Quartz;
using BanMe.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDbContext<BanMeDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IRiotApiInstance, RiotApiInstance>();

builder.Services.AddScoped<IChampGameStatsService, ChampGameStatsService>();

builder.Services.AddScoped<IBanMeInfoService, BanMeInfoService>();

builder.Services.AddScoped<ILeagueDataCrawler, LeagueDataCrawler>();

builder.Services.AddScoped<IDbSeeder, DbSeeder>();

builder.Services.AddInfrastructure();
builder.Services.AddMediatRType<PatchUpdatedHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDbSeeder>();

    //await seeder.SeedChampGameStatsAsync();
    //await seeder.SeedPlayerPuuidsAsync();
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
