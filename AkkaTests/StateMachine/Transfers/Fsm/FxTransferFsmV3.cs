using Akka.Actor;
using Akka.Persistence;
using Akka.Persistence.Fsm;
using AkkaTests.StateMachine.Transfers.Models;
using AkkaTests.StateMachine.Transfers.Utils;
using Bank.TransfersOrchestrator.Application.Commands.FxTransferCommands;
using FluentResults;
using Mediator;


namespace AkkaTests.StateMachine.Transfers.Fsm
{
    //
    public class FxTransferActorV3 : PersistentFSM<IFxTransferState, IFxTransfer, IFxTransferEvents>
    {
        private readonly Guid _transferId;
        private readonly IMediator _mediator;
        public override string PersistenceId => _transferId.ToString();


        public static Props Props(Guid transferId, IMediator mediator)
        {
            return Akka.Actor.Props.Create(() => new FxTransferActorV3(transferId, mediator));
        }

        public FxTransferActorV3(Guid transferId, IMediator mediator)
        {
            _transferId = transferId;
            _mediator = mediator;

            StartWith(FxState.Init.Instance, new FxModels.Init(new TransferData()
            {
                Id = _transferId,
            }));

            When(FxState.Init.Instance, (command, state) =>
            {
                if (command.FsmEvent is CreateFxTransferCommand started)
                {
                    var sender = Sender;

                    _ = _mediator.Send(started)
                        .PipeToCompleted(started, Self, sender);

                    return GoTo(FxState.Initializing.Instance);
                }

                return Stay()
                    .Replying(Result.Fail($"Invalid command for current transfer state: {nameof(command.StateData.CurrentState.Identifier)}"));
            });

            When(FxState.Initializing.Instance, (command, state) =>
            {
                if (command.FsmEvent is CommandCompleted<CreateFxTransferCommand, Result<FxEvent.Initialized>> completed)
                {
                    if (completed.Result.IsFailed)
                    {
                        return Stay().Replying(completed.Result);
                    }
                    return GoTo(FxState.Draft.Instance)
                        .Applying(completed.Result.Value)
                        .Replying(completed.Result)
                        .AndThen(state => SaveSnapshot(state));
                }
                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {nameof(command.StateData.CurrentState.Identifier)}"));
            });

            When(FxState.Draft.Instance, (command, state) =>
            {
                if (command.FsmEvent is FxTransactionsCreatedCommand fxTransactionsCreated)
                {
                    var sender = Sender;
                    //handle command, save to database
                    _ = _mediator.Send(fxTransactionsCreated)
                        .PipeToCompleted(fxTransactionsCreated, Self, sender);

                    return GoTo(FxState.CreatingTransactions.Instance);
                }

                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {nameof(command.StateData.CurrentState.Identifier)}"));
            });

            When(FxState.CreatingTransactions.Instance, (command, state) =>
            {
                if (command.FsmEvent is CommandCompleted<FxTransactionsCreatedCommand, Result<FxEvent.TransactionsCreated>> completed)
                {
                    if (!completed.Result.IsSuccess)
                    {
                        return Stay().Replying(completed.Result);
                    }
                    if (!completed.Command.IsSuccess)
                    {
                        return GoTo(FxState.Error.Instance)
                            .Applying(new FxEvent.Error("Unable to create fx transfer"))
                            .Replying(completed.Result)
                            .AndThen(state => SaveSnapshot(state));
                    }
                    return GoTo(FxState.TransactionsCreated.Instance)
                        .Applying(completed.Result.Value)
                        .Replying(completed.Result)
                        .AndThen(state => SaveSnapshot(state));
                }

                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {nameof(command.StateData.CurrentState.Identifier)}"));
            });

            When(FxState.TransactionsCreated.Instance, (command, state) =>
            {

                if (command.FsmEvent is FxTransferCreatedCommand fxTransferCreated)
                {
                    var sender = Sender;
                    _ = _mediator.Send(fxTransferCreated)
                        .PipeToCompleted(fxTransferCreated, Self, sender);

                    return GoTo(FxState.CreatingTransfer.Instance);
                }

                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {nameof(command.StateData.CurrentState.Identifier)}"));
            });

            When(FxState.CreatingTransfer.Instance, (command, state) =>
            {
                if (command.FsmEvent is CommandCompleted<FxTransferCreatedCommand, Result<FxEvent.TransferCreated>> completed)
                {
                    if (completed.Result.IsFailed)
                    {
                        return Stay().Replying(completed.Result);
                    }
                    if (completed.Command.Started)
                    {
                        return GoTo(FxState.TransferCreated.Instance)
                            .Applying(completed.Result.Value)
                            .Replying(completed.Result)
                            .AndThen(state => SaveSnapshot(state));
                    }
                    return GoTo(FxState.Cancelled.Instance)
                        .Applying(new FxEvent.Cancelled(completed.Command.Error))
                        .Replying(completed.Result)
                        .AndThen(state => SaveSnapshot(state));
                }
                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {nameof(command.StateData.CurrentState.Identifier)}"));
            });

            When(FxState.TransferCreated.Instance, (command, state) =>
            {
                if (command.FsmEvent is FxTransferExecutedCommand fxTransferExecuted)
                {
                    var sender = Sender;
                    _ = _mediator.Send(fxTransferExecuted)
                        .PipeToCompleted(fxTransferExecuted, Self, sender);

                    return GoTo(FxState.Executing.Instance);
                }

                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {nameof(command.StateData.CurrentState.Identifier)}"));
            });

            When(FxState.Executing.Instance, (command, state) =>
            {
                if (command.FsmEvent is CommandCompleted<FxTransferExecutedCommand, Result<FxEvent.Executed>> completed)
                {
                    if (completed.Result.IsFailed)
                    {
                        return Stay().Replying(completed.Result);
                    }
                    if (completed.Command.Passed)
                    {
                        return GoTo(FxState.TransferExecuted.Instance)
                            .Applying(completed.Result.Value)
                            .Replying(completed.Result)
                            .AndThen(state => SaveSnapshot(state));
                    }

                    return GoTo(FxState.Cancelled.Instance)
                        .Applying(new FxEvent.Cancelled(completed.Command.Error))
                        .Replying(completed.Result)
                        .AndThen(state => SaveSnapshot(state));
                }

                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {nameof(command.StateData.CurrentState.Identifier)}"));
            });

            When(FxState.TransferExecuted.Instance, (command, state) =>
            {

                if (command.FsmEvent is FxTransactionsCreatedCommand fxTransactionsCreated)
                {
                    var sender = Sender;
                    _ = _mediator.Send(fxTransactionsCreated)
                        .PipeToCompleted(fxTransactionsCreated, Self, sender);

                    return GoTo(FxState.PostingTransactions.Instance);
                }

                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {nameof(command.StateData.CurrentState.Identifier)}"));

            });

            When(FxState.PostingTransactions.Instance, (command, state) =>
            {
                if (command.FsmEvent is CommandCompleted<FxTransactionsCreatedCommand, Result<FxEvent.TransactionsCreated>> completed)
                {
                    if (!completed.Result.IsSuccess)
                    {
                        return Stay().Replying(completed.Result);
                    }
                    if (!completed.Command.IsSuccess)
                    {
                        return GoTo(FxState.Error.Instance)
                            .Applying(new FxEvent.Error("Unable to create fx transfer"))
                            .Replying(completed.Result)
                            .AndThen(state => SaveSnapshot(state));
                    }
                    return GoTo(FxState.TransactionsPosted.Instance)
                        .Applying(completed.Result.Value)
                        .Replying(completed.Result)
                        .AndThen(state => SaveSnapshot(state));
                }

                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {nameof(command.StateData.CurrentState.Identifier)}"));
            });


            When(FxState.TransactionsPosted.Instance, (command, state) =>
            {
                if (command.FsmEvent is FxTransactionsPostedCommand postedCommand)
                {
                    var sender = Sender;
                    _ = _mediator.Send(postedCommand)
                        .PipeToCompleted(postedCommand, Self, sender);

                    return GoTo(FxState.Completing.Instance);
                }
                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {nameof(command.StateData.CurrentState.Identifier)}"));
            });

            When(FxState.Completing.Instance, (command, state) =>
            {
                if (command.FsmEvent is CommandCompleted<FxTransactionsPostedCommand, Result<FxEvent.Completed>> completed)
                {
                    if (completed.Result.IsFailed)
                    {
                        return Stay().Replying(completed.Result);
                    }

                    return GoTo(FxState.Completed.Instance)
                        .Applying(completed.Result.Value)
                        .Replying(completed.Result)
                        .AndThen(state => SaveSnapshot(state));
                }

                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {nameof(command.StateData.CurrentState.Identifier)}"));
            });


            When(FxState.Completed.Instance, (command, state) =>
            {
                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {nameof(command.StateData.CurrentState.Identifier)}"));
            });

            When(FxState.Error.Instance, (command, state) =>
            {
                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {nameof(command.StateData.CurrentState.Identifier)}"));
            });

            When(FxState.Cancelled.Instance, (command, state) =>
            {
                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {nameof(command.StateData.CurrentState.Identifier)}"));
            });
        }

        protected override IFxTransfer ApplyEvent(IFxTransferEvents newEvent, IFxTransfer currentState)
        {
            return newEvent switch
            {
                FxEvent.Initialized registered => currentState.Registered(registered),

                FxEvent.TransactionsCreated transactionsCreated => currentState.AddTransactions(transactionsCreated.Transactions),

                FxEvent.TransferCreated transferCreated => currentState.Created(transferCreated.BaasProvider),

                FxEvent.Executed transferExecuted => currentState.Executed(transferExecuted.ExchangeRate, transferExecuted.ReceiveAmount),

                FxEvent.Completed => currentState.Complete(),

                FxEvent.Cancelled cancelled => currentState.Cancel(cancelled.ErrorMessage),

                FxEvent.Error error => currentState.Error(error.ErrorMessage),

                _ => currentState,
            };
        }
    }


}
