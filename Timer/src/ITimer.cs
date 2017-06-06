namespace Cvdm.Timer
{
    using System;

    /// <summary>Raises events after a certain time and/or at regular intervals.</summary>
    public interface ITimer : IDisposable
    {
        /// <summary>Occurs when <see cref="Delay" /> or <see cref="Interval" /> elapses.</summary>
        event Action Elapsed;

        /// <summary>
        ///     Gets or sets the delay after which to first raise the <see cref="Elapsed" /> event. Non-positive
        ///     values will make the event be immediately raised when the timer is started. If this is set on an active
        ///     timer, call <see cref="Start()" /> to make the new value take effect.
        /// </summary>
        TimeSpan Delay { get; set; }

        /// <summary>
        ///     Gets or sets the interval at which to raise the <see cref="Elapsed" /> event. Non-positive values
        ///     disables periodic elapsing. If this is set on an active timer, call <see cref="Start()" /> or
        ///     <see cref="Start(DateTime)" /> to make the new value take effect.
        /// </summary>
        TimeSpan Interval { get; set; }

        /// <summary>Gets a value indicating whether the timer is active.</summary>
        bool IsEnabled { get; }

        /// <summary>
        ///     Starts raising the <see cref="Elapsed" /> event according to <see cref="Interval" /> and
        ///     <see cref="Delay" />.
        /// </summary>
        void Start();

        /// <summary>
        ///     Uses <paramref name="startTime" /> to set the <see cref="Delay" /> property and immediately
        ///     starts the timer.
        /// </summary>
        /// <param name="startTime">The time at which the <see cref="Elapsed"/> event should first be raised.</param>
        void Start(DateTime startTime);

        /// <summary>Stops raising the <see cref="Elapsed"/> event.</summary>
        void Stop();
    }
}