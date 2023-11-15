using Akka.Persistence;
using Akka.Persistence.Fsm;
using AkkaTests.StateMachine.Transfers.Models;
using Bank.TransfersOrchestrator.Application.Commands.FxTransferCommands;
using FluentResults;
using Mediator;
using System.Collections.Immutable;
using static Akka.Persistence.Fsm.PersistentFSM;

namespace AkkaTests.StateMachine.Transfers.Fsm
{

    public class FxTransferActorV2 : PersistentFSM<IFxTransferState, IFxTransfer, IFxTransferEvents>
    {
        private readonly Guid _transferId;
        private readonly IMediator _mediator;
        public override string PersistenceId => _transferId.ToString();



        public FxTransferActorV2(Guid transferId, IMediator mediator)
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
                    var result = GetResult(() =>
                    {
                        return _mediator.Send(started);
                    });

                    if (result.IsSuccess)
                    {
                        return GoTo(FxState.Draft.Instance)
                            .Applying(result.Value)
                            .Replying(result)
                            .AndThen(state => SaveSnapshot(state));
                    }
                    return Stay().Replying(result);
                }
                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {state.StateName}"));
            });


            When(FxState.Draft.Instance, (command, state) =>
            {
                if (command.FsmEvent is FxTransactionsCreatedCommand fxTransactionsCreated)
                {
                    var result = GetResult(() =>
                    {
                        return _mediator.Send(fxTransactionsCreated);
                    });
                    if (result.IsFailed)
                    {
                        return Stay().Replying(result);
                    }
                    if (fxTransactionsCreated.IsSuccess)
                    {
                        return GoTo(FxState.Error.Instance)
                            .Applying(new FxEvent.Error("Unable to create fx transfer"))
                            .Replying(result)
                            .AndThen(state => SaveSnapshot(state));
                    }
                    return GoTo(FxState.TransactionsCreated.Instance)
                        .Applying(result.Value)
                        .Replying(result)
                        .AndThen(state => SaveSnapshot(state));
                }

                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {state.StateName}"));
            });

            When(FxState.TransactionsCreated.Instance, (command, state) =>
            {
                if (command.FsmEvent is FxTransferCreatedCommand fxTransferCreated)
                {
                    var result = GetResult(() =>
                    {
                        return _mediator.Send(fxTransferCreated);
                    });
                    if (result.IsFailed)
                    {
                        return Stay().Replying(result);
                    }
                    if (fxTransferCreated.Started)
                    {
                        return GoTo(FxState.TransferCreated.Instance)
                            .Applying(result.Value)
                            .Replying(result)
                            .AndThen(state => SaveSnapshot(state));
                    }
                    return GoTo(FxState.Cancelled.Instance)
                        .Applying(new FxEvent.Error(fxTransferCreated.Error))
                        .Replying(result)
                        .AndThen(state => SaveSnapshot(state));
                }

                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {state.StateName}"));
            });

            When(FxState.TransferCreated.Instance, (command, state) =>
            {
                if (command.FsmEvent is FxTransferExecutedCommand fxTransferExecuted)
                {
                    var result = GetResult(() =>
                    {
                        return _mediator.Send(fxTransferExecuted);
                    });
                    if (result.IsFailed)
                    {
                        return Stay().Replying(result);
                    }
                    if (fxTransferExecuted.Passed)
                    {
                        return GoTo(FxState.TransferExecuted.Instance)
                            .Applying(result.Value)
                            .Replying(result)
                            .AndThen(state => SaveSnapshot(state));
                    }

                    return GoTo(FxState.Cancelled.Instance)
                        .Applying(new FxEvent.Cancelled(fxTransferExecuted.Error))
                        .Replying(result)
                        .AndThen(state => SaveSnapshot(state));
                }
                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {state.StateName}"));
            });

            When(FxState.TransferExecuted.Instance, (command, state) =>
            {
                if (command.FsmEvent is FxTransactionsCreatedCommand fxTransactionsCreated)
                {
                    var result = GetResult(() =>
                    {
                        return _mediator.Send(fxTransactionsCreated);
                    });

                    if (!result.IsSuccess)
                    {
                        return Stay().Replying(result);
                    }
                    if (!fxTransactionsCreated.IsSuccess)
                    {
                        return GoTo(FxState.Error.Instance)
                            .Applying(new FxEvent.Error("Unable to create fx transfer"))
                            .Replying(result)
                            .AndThen(state => SaveSnapshot(state));
                    }
                    return GoTo(FxState.TransactionsPosted.Instance)
                        .Applying(result.Value)
                        .Replying(result)
                        .AndThen(state => SaveSnapshot(state));
                }
                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {state.StateName}"));
            });


            When(FxState.TransactionsPosted.Instance, (command, state) =>
            {
                if (command.FsmEvent is FxTransactionsPostedCommand postedCommand)
                {
                    var result = GetResult(() =>
                    {
                        return _mediator.Send(postedCommand);
                    });
                    if (result.IsFailed)
                    {
                        return Stay().Replying(result);
                    }

                    return GoTo(FxState.Completed.Instance)
                        .Applying(result.Value)
                        .Replying(result)
                        .AndThen(state => SaveSnapshot(state));
                }

                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {state.StateName}"));
            });

            When(FxState.Completed.Instance, (command, state) =>
            {
                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {state.StateName}"));
            });

            When(FxState.Error.Instance, (command, state) =>
            {
                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {state.StateName}"));
            });

            When(FxState.Cancelled.Instance, (command, state) =>
            {
                return Stay().Replying(Result.Fail($"Invalid command for current transfer state: {state.StateName}"));
            });
        }

        private Result<TResponse> GetResult<TResponse>(Func<ValueTask<Result<TResponse>>> func)
        {
            var task = func().AsTask();
            try
            {
                task.Wait();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.ToString());
            }

            return task.Result;
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
