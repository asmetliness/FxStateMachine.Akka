

using AkkaTests.StateMachine.Transfers;
using FluentResults;
using Mediator;

namespace Bank.TransfersOrchestrator.Application.Commands.FxTransferCommands
{
    public class FxTransactionsPostedCommand: ICommand<Result<FxEvent.Completed>>
    {
        public Guid[] TransactionIds { get; set; }
        public bool IsSuccess { get; set; }
        public string? Details { get; set; }
    }

    public class FxTransactionsPostedCommandHandler : ICommandHandler<FxTransactionsPostedCommand, Result<FxEvent.Completed>>
    {
        public async ValueTask<Result<FxEvent.Completed>> Handle(FxTransactionsPostedCommand command, CancellationToken cancellationToken)
        {
            return Result.Ok();
        }
    }
}
