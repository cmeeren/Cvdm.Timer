namespace Cvdm.Timer
{
    /// <inheritdoc />
    public class TimerFactory : ITimerFactory
    {
        /// <inheritdoc />
        public ITimer Create()
        {
            return new DefaultTimer();
        }
    }
}