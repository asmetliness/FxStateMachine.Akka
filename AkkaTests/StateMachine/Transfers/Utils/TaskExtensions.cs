using Akka.Actor;
using Mediator;

namespace AkkaTests.StateMachine.Transfers.Utils
{
    public static class TaskExtensions
    {


        public static Task PipeToCompleted<TRequest, TResponse>(this ValueTask<TResponse> task, TRequest started, IActorRef self, IActorRef sender)
            where TRequest : ICommand<TResponse>
        {
            return task.PipeTo(self, sender, (result) =>
            {
                var localStarted = started;
                return new CommandCompleted<TRequest, TResponse>()
                {
                    Command = started,
                    Result = result
                };
            });
        }
    }


}
