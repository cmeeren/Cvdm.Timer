namespace Cvdm.Timer
{
    /// <inheritdoc />
    public class TimerFactory : ITimerFactory
    {
        /// <inheritdoc />
        public DefaultTimer Create()
        {
            return new DefaultTimer();
        }
    }
}