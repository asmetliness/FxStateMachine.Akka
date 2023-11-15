namespace AkkaTests.StateMachine.Transfers.Models
{
    public class FxModels
    {
        public record Init(TransferData TransferData) : BaseFxTransfer(TransferData, FxState.Init.Instance)
        {
            public override IFxTransfer Registered(FxEvent.Initialized registered)
            {
                return new Draft(TransferData with
                {
                    RiskLevel = registered.RiskLevel,
                    SendingAmount = registered.SendingAmount,
                    SenderAccountNumber = registered.SenderAccountNumber,
                    SenderAccountName = registered.SenderAccountName,
                    SenderCurrency = registered.SenderCurrency,
                    SenderAccountId = registered.SenderAccountId,
                    ReceiverAccountNumber = registered.ReceiverAccountNumber,
                    ReceiverAccountName = registered.ReceiverAccountName,
                    ReceiverCurrency = registered.ReceiverCurrency,
                    ReceiverAccountId = registered.ReceiverAccountId,
                    BaasProvider = registered.BaasProvider
                });
            }
        }

        public record Draft(TransferData TransferData) : BaseFxTransfer(TransferData, FxState.Draft.Instance)
        {
            public override IFxTransfer AddTransactions(List<LedgerFxTransaction> transactions)
            {
                return new WithTransactions(TransferData with
                {
                    Transactions = transactions
                });
            }
        }

        public record WithTransactions(TransferData TransferData) : BaseFxTransfer(TransferData, FxState.TransactionsCreated.Instance)
        {
            public override IFxTransfer Created(string baasProvider)
            {
                return new CreatedTransfer(TransferData with
                {
                    BaasProvider = baasProvider
                });
            }
        }

        public record CreatedTransfer(TransferData TransferData) : BaseFxTransfer(TransferData, FxState.TransferCreated.Instance)
        {
            public override IFxTransfer Executed(decimal exchangeRate, decimal receiveAmount)
            {
                return new ExecutedTransfer(TransferData with
                {
                    ReceiveAmount = receiveAmount,
                    ExchangeRate = exchangeRate
                });
            }
        }

        public record ExecutedTransfer(TransferData TransferData) : BaseFxTransfer(TransferData, FxState.TransferExecuted.Instance)
        {
            public override IFxTransfer AddTransactions(List<LedgerFxTransaction> transactions)
            {
                var currentTransactions = TransferData.Transactions;

                return new PostedTransfer(TransferData with
                {
                    Transactions = transactions.Concat(currentTransactions).ToList()
                });
            }
        }


        public record PostedTransfer(TransferData TransferData) : BaseFxTransfer(TransferData, FxState.TransactionsPosted.Instance)
        {
            public override IFxTransfer Complete()
            {
                return new CompletedTransfer(TransferData);
            }
        }

        public record CancelledTransfer(TransferData TransferData) : BaseFxTransfer(TransferData, FxState.Cancelled.Instance);

        public record CompletedTransfer(TransferData TransferData) : BaseFxTransfer(TransferData, FxState.Completed.Instance);

        public record ErrorTransfer(TransferData TransferData) : BaseFxTransfer(TransferData, FxState.Error.Instance);


    }
}
