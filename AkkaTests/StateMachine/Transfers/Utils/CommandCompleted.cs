using Mediator;

namespace AkkaTests.StateMachine.Transfers.Utils
{
    public class CommandCompleted<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
        public TRequest Command { get; set; }
        public TResponse Result { get; set; }
    }


}
