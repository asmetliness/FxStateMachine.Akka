using Akka.Actor;

namespace AkkaTests
{
    public interface IActorSystem
    {
        IActorRef CreateActor(Props props, string name);
    }
}
