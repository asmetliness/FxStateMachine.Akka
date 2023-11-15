using Akka.Actor;
using FluentResults;
using Mediator;

namespace AkkaTests.StateMachine.Transfers
{
    public class FxTransferProxy
    {
        private readonly ActorSystemService _actorSystem;
        private readonly IActorRef _transferOrchestrator;
        private readonly IMediator _mediator;


        public FxTransferProxy(ActorSystemService actorSystem, IMediator mediator)
        {
            _actorSystem = actorSystem;
            _mediator = mediator;

            _transferOrchestrator = actorSystem.CreateActor(
                TransferOrchestrator.Props(_mediator), nameof(TransferOrchestrator));

        }


        public async Task<Result<TResponse>> SendCommand<TResponse>(
            ICommand<Result<TResponse>> request, 
            Guid transferId,
            CancellationToken token)
        {

            var transferActor = await _transferOrchestrator
                .Ask<IActorRef>(new GetTransfer(transferId), token);

            var result = await transferActor.Ask(request, token);
            if(result is Result errorResult)
            {
                return Result.Fail<TResponse>(errorResult.Errors);
            }
            if(result is Result<TResponse> typedResult)
            {
                return typedResult;
            }

            throw new InvalidMessageException($"Unknown type {result.GetType().Name}");
        }



    }
}
