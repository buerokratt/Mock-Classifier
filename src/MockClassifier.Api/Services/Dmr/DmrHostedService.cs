namespace MockClassifier.Api.Services.Dmr
{
    /// <summary>
    /// A background hosted service that periodically triggers the DMR request processor
    /// </summary>
    public class DmrHostedService : IHostedService
    {
        private readonly IDmrService dmrService;
        private readonly DmrServiceSettings config;
        private readonly ILogger<DmrHostedService> logger;
        private readonly Timer timer;

        public DmrHostedService(IDmrService dmrService, DmrServiceSettings config, ILogger<DmrHostedService> logger)
        {
            this.dmrService = dmrService;
            this.config = config;
            this.logger = logger;
            this.timer = new Timer(TimerCallback, this, Timeout.Infinite, Timeout.Infinite);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            StartTimer();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            StopTimer();

            return Task.CompletedTask;
        }

        private void StopTimer() => timer.Change(Timeout.Infinite, Timeout.Infinite);
        private void StartTimer() => timer.Change(TimeSpan.FromMilliseconds(config.DmrRequestProcessIntervalMs), Timeout.InfiniteTimeSpan);

        private static async void TimerCallback(object state)
        {
            if (state == null)
                return;

            var self = state as DmrHostedService;

            self.StopTimer();

            try
            {
                self.logger.LogInformation("Starting processing DMR requests");

                await self.dmrService.ProcessRequestsAsync();

                self.logger.LogInformation("Completed processing DMR requests");
            }
            catch (Exception ex)
            {
                self.logger.LogError(ex, $"Unexpected error in {nameof(DmrHostedService)}.");
            }
            finally
            {
                self.StartTimer();
            }
        }
    }
}
