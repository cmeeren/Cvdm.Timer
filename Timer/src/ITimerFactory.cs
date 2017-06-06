namespace Cvdm.Timer
{
    /// <summary>Creates timers.</summary>
    public interface ITimerFactory
    {
        /// <summary>Creates a timer.</summary>
        /// <returns>A timer that can be used to schedule execution of code.</returns>
        ITimer Create();
    }
}