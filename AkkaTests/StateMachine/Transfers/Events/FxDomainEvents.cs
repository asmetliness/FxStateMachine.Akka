using AkkaTests.StateMachine.Transfers.Models;

namespace AkkaTests.StateMachine.Transfers
{
    public interface IFxTransferEvents
    {
    }

    public class FxEvent
    {
        public record Initialized(
             Guid TransferId,
             int? RiskLevel,
             decimal SendingAmount,
             string SenderAccountNumber,
             string SenderAccountName,
             string SenderCurrency,
             string ReceiverAccountNumber,
             string ReceiverAccountName,
             string ReceiverCurrency,
             string BaasProvider,
             Guid SenderAccountId,
             Guid ReceiverAccountId) : IFxTransferEvents;

        public record TransactionsCreated(List<LedgerFxTransaction> Transactions) : IFxTransferEvents;

        public record TransferCreated(string BaasProvider) : IFxTransferEvents;

        public record Executed(decimal ExchangeRate, decimal ReceiveAmount) : IFxTransferEvents;

        public record AdditionalTransactionsCreated(List<LedgerFxTransaction> Transactions) : IFxTransferEvents;

        public record Completed : IFxTransferEvents;
        public record Cancelled(string ErrorMessage) : IFxTransferEvents;
        public record Error(string ErrorMessage) : IFxTransferEvents;
    }
 }
