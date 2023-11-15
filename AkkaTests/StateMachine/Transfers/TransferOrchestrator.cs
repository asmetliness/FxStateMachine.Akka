using Akka.Actor;
using Akka.Persistence;
using AkkaTests.StateMachine.Transfers.Fsm;
using Bank.TransfersOrchestrator.Application.Commands.FxTransferCommands;
using Mediator;

namespace AkkaTests.StateMachine.Transfers
{
    public class TransferOrchestrator : ReceivePersistentActor
    {
        public override string PersistenceId => nameof(TransferOrchestrator);
        private readonly IMediator _mediator;
        internal static Props Props(IMediator mediator)
        {
            return Akka.Actor.Props.Create(() => new TransferOrchestrator(mediator));
        }

        public TransferOrchestrator(IMediator mediator) 
        {
            _mediator = mediator;

            Command<GetTransfer>((command) =>
            {
                var transferActor = Context.Child(command.TransferId.ToString());
                if(transferActor is Nobody)
                {
                    transferActor = Context.ActorOf(FxTransferActorV3.Props(command.TransferId, _mediator), command.TransferId.ToString());
                }

                Sender.Tell(transferActor);
            });

        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            //configure it later
            return new OneForOneStrategy(
                10,
                5,
                Decider.From(ex =>
                {

                    return Directive.Resume;
                }));
        }
    }

    public record GetTransfer(Guid TransferId);
}
