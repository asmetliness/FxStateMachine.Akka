namespace AkkaTests.StateMachine.Transfers.Models;

public record TransferData
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public int? RiskLevel { get; set; }
    public decimal SendingAmount { get; set; }
    public Guid SenderAccountId { get; set; }
    public string SenderAccountNumber { get; set; } = null!;
    public string SenderAccountName { get; set; } = null!;
    public string SenderCurrency { get; set; } = null!;
    public string? BaasProvider { get; set; }
    public Guid ReceiverAccountId { get; set; }
    public string ReceiverAccountNumber { get; set; } = null!;
    public string ReceiverAccountName { get; set; } = null!;
    public string ReceiverCurrency { get; set; } = null!;
    public decimal? ReceiveAmount { get; set; }
    public decimal? ExchangeRate { get; set; }
    public decimal? FeeAmount { get; set; }
    public decimal? ExpensesAmount { get; set; }
    public bool ExecutedInProvider { get; set; } = false;

    public string? Reason { get; set; }
    public string? BaasProviderTransactionId { get; set; }
    public ICollection<LedgerFxTransaction> Transactions { get; set; } = new List<LedgerFxTransaction>();
}

