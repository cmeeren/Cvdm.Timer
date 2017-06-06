namespace Cvdm.Timer
{
    using System;
    using System.Threading;

    /// <inheritdoc />
    public class DefaultTimer : ITimer
    {
        private readonly Timer timer;

        /// <summary>Initializes an instance of the <see cref="DefaultTimer" /> class.</summary>
        public DefaultTimer()
        {
            this.timer = new Timer(this.Elapse, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <inheritdoc />
        public event Action Elapsed;

        /// <inheritdoc />
        public TimeSpan Delay { get; set; }

        /// <inheritdoc />
        public TimeSpan Interval { get; set; }

        /// <inheritdoc />
        public bool IsEnabled { get; private set; }

        /// <inheritdoc />
        public void Start()
        {
            this.timer.Change(this.Delay.ZeroIfNegative(), this.Interval.ZeroIfNegative());
            this.IsEnabled = true;
        }

        /// <inheritdoc />
        public void Start(DateTime startTime)
        {
            this.Delay = startTime - DateTime.Now;
            this.Start();
        }

        /// <inheritdoc />
        public void Stop()
        {
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);
            this.IsEnabled = false;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.timer.Dispose();
        }

        private void Elapse(object _)
        {
            this.Elapsed?.Invoke();
            if (this.Interval <= TimeSpan.Zero) this.IsEnabled = false;
        }
    }
}