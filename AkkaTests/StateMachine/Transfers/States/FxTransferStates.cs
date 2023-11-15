using static Akka.Persistence.Fsm.PersistentFSM;

namespace AkkaTests.StateMachine.Transfers
{
    public interface IFxTransferState : IFsmState { }

    public class FxState
    {
        public class Init : IFxTransferState
        {
            public static Init Instance { get; } = new();
            private Init() { }
            public string Identifier => "Init";
        }

        public class Draft : IFxTransferState
        {
            public static Draft Instance { get; } = new();
            private Draft() { }
            public string Identifier => "Draft";
        }

        public class TransactionsCreated : IFxTransferState
        {
            public static TransactionsCreated Instance { get; } = new();
            private TransactionsCreated() { }
            public string Identifier => "TransactionsCreated";
        }


        public class TransferCreated : IFxTransferState
        {
            public static TransferCreated Instance { get; } = new();
            private TransferCreated() { }
            public string Identifier => "TransferCreated";
        }


        public class TransferExecuted : IFxTransferState
        {
            public static TransferExecuted Instance { get; } = new();
            private TransferExecuted() { }
            public string Identifier => "TransferExecuted";
        }

        public class Cancelled : IFxTransferState
        {
            public static Cancelled Instance { get; } = new();
            private Cancelled() { }
            public string Identifier => "Cancelled";
        }

        public class TransactionsPosted : IFxTransferState
        {
            public static TransactionsPosted Instance { get; } = new();
            private TransactionsPosted() { }
            public string Identifier => "TransactionsPosted";
        }
        public class Completed : IFxTransferState
        {
            public static Completed Instance { get; } = new();
            private Completed() { }
            public string Identifier => "Completed";
        }

        public class Error : IFxTransferState
        {
            public static Error Instance { get; } = new();
            private Error() { }
            public string Identifier => "Error";
        }




        public class Initializing : IFxTransferState
        {
            public static Initializing Instance { get; } = new();
            private Initializing() { }
            public string Identifier => "Initializing";
        }
        public class CreatingTransactions : IFxTransferState
        {
            public static CreatingTransactions Instance { get; } = new();
            private CreatingTransactions() { }
            public string Identifier => "CreatingTransactions";
        }


        public class CreatingTransfer : IFxTransferState
        {
            public static CreatingTransfer Instance { get; } = new();
            private CreatingTransfer() { }
            public string Identifier => "CreatingTransfer";
        }
        public class Executing : IFxTransferState
        {
            public static Executing Instance { get; } = new();
            private Executing() { }
            public string Identifier => "ExecutingTransferState";
        }

        public class Cancelling : IFxTransferState
        {
            public static Cancelling Instance { get; } = new();
            private Cancelling() { }
            public string Identifier => "CancellingTransferState";
        }


        public class PostingTransactions : IFxTransferState
        {
            public static PostingTransactions Instance { get; } = new();
            private PostingTransactions() { }
            public string Identifier => "PostingTransactionsState";
        }


        public class Completing : IFxTransferState
        {
            public static Completing Instance { get; } = new();
            private Completing() { }
            public string Identifier => "Completing";
        }
    }

}
