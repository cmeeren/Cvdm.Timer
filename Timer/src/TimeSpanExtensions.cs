namespace Cvdm.Timer
{
    using System;

    /// <summary>Provides <see cref="TimeSpan" /> extension methods.</summary>
    internal static class TimeSpanExtensions
    {
        /// <summary>Converts negative time spans to <see cref="TimeSpan.Zero" />.</summary>
        /// <param name="timeSpan">The <see cref="TimeSpan" /> to convert.</param>
        /// <returns>The input <paramref name="timeSpan" /> if it is positive; otherwise, <see cref="TimeSpan.Zero" />.</returns>
        public static TimeSpan ZeroIfNegative(this TimeSpan timeSpan)
        {
            return timeSpan < TimeSpan.Zero ? TimeSpan.Zero : timeSpan;
        }
    }
}