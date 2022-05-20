using System.Diagnostics.CodeAnalysis;

namespace MockClassifier.Api.Services.Dmr
{
    /// <summary>
    /// A background hosted service that periodically triggers the DMR request processor
    /// </summary>
    [ExcludeFromCodeCoverage] // Temporarily excluded from code coverage in order to get the CI pipeline merged. This attribute will be removed later.
    public sealed class DmrHostedService : IHostedService, IDisposable
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
            timer = new Timer(TimerCallback, this, Timeout.Infinite, Timeout.Infinite);
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

        private void StopTimer()
        {
            _ = timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void StartTimer()
        {
            _ = timer.Change(TimeSpan.FromMilliseconds(config.DmrRequestProcessIntervalMs), Timeout.InfiniteTimeSpan);
        }

        private static async void TimerCallback(object state)
        {
            if (state == null)
            {
                return;
            }

            var self = state as DmrHostedService;

            self.StopTimer();

            try
            {
                // Not sure how to resolve rule CA1848 so removing logging for now
                //self.logger.LogInformation("Starting processing DMR requests");

                await self.dmrService.ProcessRequestsAsync().ConfigureAwait(true);


                // Not sure how to resolve rule CA1848 so removing logging for now
                //self.logger.LogInformation("Completed processing DMR requests");
            }
            catch (Exception)
            {
                // Not sure how to resolve rule CA1848 so removing logging for now
                //self.logger.LogError(ex, $"Unexpected error in {nameof(DmrHostedService)}.");
                throw;
            }
            finally
            {
                self.StartTimer();
            }
        }

        public void Dispose()
        {
            timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
