
using AkkaTests.StateMachine.Transfers;
using FluentResults;

using Mediator;

namespace Bank.TransfersOrchestrator.Application.Commands.FxTransferCommands;

public class CreateFxTransferCommand : ICommand<Result<FxEvent.Initialized>>
{
    public Guid TransferId { get; set; }
    public Guid ClientId { get; set; }
    public decimal Amount { get; set; }
    public Guid SenderAccountId { get; set; }
    public Guid ReceiverAccountId { get; set; }
}

public class CreateFxTransferCommandHandler : ICommandHandler<CreateFxTransferCommand, Result<FxEvent.Initialized>>
{
    public async ValueTask<Result<FxEvent.Initialized>> Handle(CreateFxTransferCommand request, CancellationToken cancellationToken)
    {
        return Result.Ok(new FxEvent.Initialized(request.TransferId, 0, 0.0m, "sender number", "sender name",
            "sender curr", "receiver number", "receiver name", "receiver currency", "provider", Guid.NewGuid(), Guid.NewGuid()));
    }
}