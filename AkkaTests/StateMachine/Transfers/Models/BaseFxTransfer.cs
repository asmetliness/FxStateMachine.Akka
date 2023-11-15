namespace AkkaTests.StateMachine.Transfers.Models;

public abstract record BaseFxTransfer(TransferData TransferData, IFxTransferState CurrentState) : IFxTransfer
{
    public virtual IFxTransfer AddTransactions(List<LedgerFxTransaction> transactions)
        => this;
    public virtual IFxTransfer Cancel(string error)
        => this;
    public virtual IFxTransfer Complete()
            => this;
    public virtual IFxTransfer Created(string baasProvider)
            => this;
    public virtual IFxTransfer Error(string errorMessage)
        => this;
    public virtual IFxTransfer Executed(decimal exchangeRate, decimal receiveAmount)
        => this;
    public virtual IFxTransfer Registered(FxEvent.Initialized registered)
        => this;

}

