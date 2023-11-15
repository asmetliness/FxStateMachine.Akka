using Akka.Actor;
using Akka.DependencyInjection;

namespace AkkaTests
{
    public class ActorSystemService : IHostedService, IActorSystem
    {
        private ActorSystem _actorSystem = null!;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public ActorSystemService(
            IServiceProvider serviceProvider,
            IHostApplicationLifetime applicationLifetime)
        {
            _serviceProvider = serviceProvider;
            _applicationLifetime = applicationLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            var bootstrap = BootstrapSetup.Create();

            var diSetup = DependencyResolverSetup.Create(_serviceProvider);

            var actorSystemSetup = bootstrap.And(diSetup);
            _actorSystem = ActorSystem.Create("transfer-orchestrator", actorSystemSetup);

            _actorSystem.WhenTerminated.ContinueWith(_ =>
            {
                _applicationLifetime.StopApplication();
            });

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await CoordinatedShutdown
                .Get(_actorSystem)
                .Run(CoordinatedShutdown.ClrExitReason.Instance);
        }

        public IActorRef CreateActor(Props props, string name)
        {
            return _actorSystem.ActorOf(props, name);
        }
    }
}
