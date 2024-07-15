using Application.Interfaces;
using Application.Interfaces.ApiClients;
using Application.Services;
using Infrastructure.ApiClients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IWorkerService, NavigationService>();
builder.Services.AddSingleton<IWorkerService, CombatService>();
builder.Services.AddSingleton<IWorkerService, DroneService>();
builder.Services.AddSingleton<IWorkerService, MonitoringService>();

builder.Services.AddSingleton<IBotService, BotService>();
builder.Services.AddSingleton<IBotStateService, BotStateService>();
builder.Services.AddSingleton<IExecutor, Executor>();
builder.Services.AddSingleton<IGameService, GameService>();

builder.Services.AddHttpClient<IGameApiClient, GameApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5020");
});
builder.Services.AddHttpClient<IDroneApiClient, DroneApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5020");
});
builder.Services.AddHttpClient<IOverviewApiClient, OverviewApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5020");
});
builder.Services.AddHttpClient<ISelectItemApiClient, SelectItemApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5020");
});
builder.Services.AddHttpClient<IInfoPanelApiClient, InfoPanelApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5020");
});
builder.Services.AddHttpClient<IHudInterfaceApiClient, HudInterfaceApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5020");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
