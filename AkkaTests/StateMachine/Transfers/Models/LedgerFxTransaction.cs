namespace AkkaTests.StateMachine.Transfers.Models;

public class LedgerFxTransaction
{
    public Guid Id { get; set; }
    public Guid LedgerTransactionId { get; set; }

    public Guid TransferId { get; set; }
}

