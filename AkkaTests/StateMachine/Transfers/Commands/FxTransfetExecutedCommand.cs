using System;
using System.Collections.Generic;
using System.Linq;
using AkkaTests.StateMachine.Transfers;
using FluentResults;
using Mediator;


namespace Bank.TransfersOrchestrator.Application.Commands.FxTransferCommands
{
    public class FxTransferExecutedCommand: ICommand<Result<FxEvent.Executed>>
    {
        public Guid TransferId { get; set; }
        public decimal? ReceiveAmount { get; set; }
        public decimal? ExchangeRate { get; set; }
        public bool Passed { get; set; }
        public string? Error { get; set; }
    }

    public class FxTransferExecutedCommandHandler : ICommandHandler<FxTransferExecutedCommand, Result<FxEvent.Executed>>
    {
        public async ValueTask<Result<FxEvent.Executed>> Handle(FxTransferExecutedCommand command, CancellationToken cancellationToken)
        {
            return Result.Ok(new FxEvent.Executed(10.0m, 10.0m));
        }
    }
}
