using Mediator;
using System.Transactions;

namespace AkkaTests.StateMachine.Transfers.Models;

public interface IFxTransfer
{
    IFxTransferState CurrentState { get; }
    TransferData TransferData { get; }
    IFxTransfer Registered(FxEvent.Initialized transferRegistered);
    IFxTransfer AddTransactions(List<LedgerFxTransaction> transactions);
    IFxTransfer Created(string baasProvider);
    IFxTransfer Executed(decimal exchangeRate, decimal receiveAmount);
    IFxTransfer Complete();
    IFxTransfer Cancel(string error);
    IFxTransfer Error(string errorMessage);
}

