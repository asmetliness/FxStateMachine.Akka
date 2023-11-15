

using AkkaTests.StateMachine.Transfers;
using AkkaTests.StateMachine.Transfers.Models;
using FluentResults;
using Mediator;


namespace Bank.TransfersOrchestrator.Application.Commands.FxTransferCommands
{
    public class FxTransactionsCreatedCommand: ICommand<Result<FxEvent.TransactionsCreated>>
    {
        public bool IsSuccess { get; set; }
        public Guid ParentId { get; set; }
        public List<Guid> Transactions { get; set; }
    }

    public class FxTransactionsCreatedCommandHandler : ICommandHandler<FxTransactionsCreatedCommand, Result<FxEvent.TransactionsCreated>>
    {


        public ValueTask<Result<FxEvent.TransactionsCreated>> Handle(FxTransactionsCreatedCommand command, CancellationToken cancellationToken)
        {
            return ValueTask.FromResult(Result.Ok(new FxEvent.TransactionsCreated(new List<LedgerFxTransaction>())));
        }
    }


}
