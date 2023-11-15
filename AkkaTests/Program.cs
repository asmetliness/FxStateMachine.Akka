
using AkkaTests;
using AkkaTests.StateMachine.Transfers;
using Bank.TransfersOrchestrator.Application.Commands.FxTransferCommands;
using FluentResults;
using Mediator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMediator(opt =>
{
});

builder.Services
    .AddSingleton<ActorSystemService>()
    .AddHostedService((services) => services.GetRequiredService<ActorSystemService>());

builder.Services.AddSingleton<FxTransferProxy>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

await app.StartAsync();

var proxy = app.Services.GetRequiredService<FxTransferProxy>();

var transferId = Guid.NewGuid();

var test = await proxy.SendCommand(new CreateFxTransferCommand()
{
    TransferId = transferId

}, transferId, CancellationToken.None);

var test2 = await proxy.SendCommand(new CreateFxTransferCommand
{
    TransferId = transferId
}, transferId, CancellationToken.None);

