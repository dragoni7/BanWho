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

builder.Services.AddScoped<IChampGameStatsService, ChampGameStatsService>();

builder.Services.AddScoped<IBanMeInfoService, BanMeInfoService>();

builder.Services.AddSingleton<ILeagueDataCrawler, LeagueDataCrawler>();

builder.Services.AddInfrastructure();
builder.Services.AddMediatRType<PatchUpdatedHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
    //await SeedData.InitPlayerDb(services);
    //await SeedData.UpdateBanMeInfoPatch(services);
	await SeedData.InitChampGameStatsDb(services);
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
