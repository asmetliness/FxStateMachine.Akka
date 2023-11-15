

using AkkaTests.StateMachine.Transfers;
using FluentResults;
using Mediator;

namespace Bank.TransfersOrchestrator.Application.Commands.FxTransferCommands
{
    public class FxTransferCreatedCommand: ICommand<Result<FxEvent.TransferCreated>>
    {
        public Guid TransferId { get; set; }
        public string BaasProviderTransferId { get; set; }
        public bool Started { get; set; }
        public string? Error { get; set; }
    }

    public class FxTransferCreatedCommandHandler : ICommandHandler<FxTransferCreatedCommand, Result<FxEvent.TransferCreated>>
    {
        public async ValueTask<Result<FxEvent.TransferCreated>> Handle(FxTransferCreatedCommand command, CancellationToken cancellationToken)
        {
            return Result.Ok(new FxEvent.TransferCreated(""));
        }
    }
}
