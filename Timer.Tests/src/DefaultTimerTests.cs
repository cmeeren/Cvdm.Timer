namespace Cvdm.Timer.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Xunit;

    public sealed class DefaultTimerTests
    {
        private readonly AutoResetEvent timerStopped;
        private readonly List<DateTime> raisedAt;
        private DateTime startTime;
        private int elapsedCount;

        private readonly DefaultTimer sut;

        public DefaultTimerTests()
        {
            this.timerStopped = new AutoResetEvent(false);
            this.raisedAt = new List<DateTime>();
            this.elapsedCount = 0;
            this.sut = new DefaultTimer();
        }

        [Fact]
        public void IsInstantiatedWithCorrectDefaults()
        {
            // Assert
            this.sut.Delay.Should().Be(TimeSpan.Zero);
            this.sut.Interval.Should().Be(TimeSpan.Zero);
            this.sut.IsEnabled.Should().BeFalse();
        }

        [Fact]
        public void IsNotRunning_WhenInstantiated()
        {
            // Arrange
            var eventWasRaised = false;

            // Act
            this.sut.Elapsed += () => eventWasRaised = true;

            // Assert
            eventWasRaised.Should().BeFalse();
        }

        [Theory]
        [InlineData(10)]
        [InlineData(200)]
        [InlineData(500)]
        public void RaisesEventAfterInitialDelay(int delayMs)
        {
            // Arrange
            this.sut.Delay = delayMs.Milliseconds();
            this.sut.Elapsed += () => this.RecordElapsedAndStopAfter(1);

            // Act
            this.RunTimerUntilStopped();

            // Assert
            this.raisedAt[0].Should().BeCloseTo(delayMs.Milliseconds().After(this.startTime), 40);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-1000)]
        [InlineData(-10000)]
        public void RaisesEventImmediately_WhenInitialDelayIsZeroOrNegative(int delayMs)
        {
            // Arrange
            this.sut.Delay = delayMs.Milliseconds();
            this.sut.Elapsed += () => this.RecordElapsedAndStopAfter(1);

            // Act
            this.RunTimerUntilStopped();

            // Assert
            this.raisedAt[0].Should().BeCloseTo(this.startTime, 50);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(200)]
        [InlineData(500)]
        public void RaisesEventRegularlyByInterval(int intervalMs)
        {
            // Arrange
            this.sut.Interval = intervalMs.Milliseconds();
            this.sut.Elapsed += () => this.RecordElapsedAndStopAfter(3);

            // Act
            this.RunTimerUntilStopped();

            // Assert
            this.raisedAt[1].Should().BeCloseTo(intervalMs.Milliseconds().After(this.raisedAt[0]), 40);
            this.raisedAt[2].Should().BeCloseTo(intervalMs.Milliseconds().After(this.raisedAt[1]), 40);
        }

        [Fact]
        public void IsEnabled_IsTrueAfterTimerStarted()
        {
            // Arrange
            this.sut.Delay = 100.Milliseconds();

            // Act
            this.sut.Start();

            // Assert
            this.sut.IsEnabled.Should().BeTrue();
        }

        [Fact]
        public void IsEnabled_IsFalseAfterTimerIsStoppedManually()
        {
            // Arrange
            this.sut.Delay = 100.Milliseconds();
            this.sut.Elapsed += () => this.RecordElapsedAndStopAfter(1);

            // Act
            this.sut.Start();
            this.sut.Stop();

            // Assert
            this.sut.IsEnabled.Should().BeFalse();
        }

        [Fact]
        public void IsEnabled_IsFalseAfterTimerStopsAutomatically()
        {
            // Arrange
            this.sut.Delay = 10.Milliseconds();
            this.sut.Interval = -42.Milliseconds();

            // Act
            this.sut.Start();
            this.sut.IsEnabled.Should().BeTrue(); // Sanity check
            Task.Delay(50).Wait();

            // Assert
            this.sut.IsEnabled.Should().BeFalse();
        }

        [Fact]
        public void StartByStartTime_SetsDelayAndStartsTimer()
        {
            // Arrange
            this.sut.Elapsed += () => this.RecordElapsedAndStopAfter(1);
            DateTime scheduledStartTime = DateTime.Now.AddMilliseconds(200);

            // Act
            this.sut.Start(scheduledStartTime);
            this.timerStopped.WaitOne(1000);

            // Assert
            this.raisedAt[0].Should().BeCloseTo(scheduledStartTime, 50);
        }

        [Fact]
        public void IgnoresDelayUpdatesWhenRunning()
        {
            // Arrange
            this.sut.Delay = 100.Milliseconds();
            this.sut.Elapsed += () => this.RecordElapsedAndStopAfter(1);

            this.startTime = DateTime.Now;
            this.sut.Start();

            // Act
            this.sut.Delay = 1.Seconds();
            this.timerStopped.WaitOne(1000);

            // Assert
            this.raisedAt[0].Should().BeCloseTo(100.Milliseconds().After(this.startTime), 40);
        }

        [Fact]
        public void UpdatesDelayAfterRestart()
        {
            // Arrange
            this.sut.Delay = 100.Milliseconds();
            this.sut.Elapsed += () => this.RecordElapsedAndStopAfter(1);

            this.sut.Start();

            // Act
            this.sut.Delay = 1.Seconds();
            this.RunTimerUntilStopped();

            // Assert
            this.raisedAt[0].Should().BeCloseTo(1.Seconds().After(this.startTime), 40);
        }

        [Fact]
        public void IgnoresIntervalUpdatesWhenRunning()
        {
            // Arrange
            this.sut.Interval = 50.Milliseconds();
            this.sut.Elapsed += () => this.RecordElapsedAndStopAfter(5);

            this.startTime = DateTime.Now;
            this.sut.Start();

            // Act
            this.sut.IsEnabled.Should().BeTrue(); // Sanity check
            this.sut.Interval = 150.Milliseconds();
            this.timerStopped.WaitOne(1000);

            // Assert
            this.raisedAt[4].Should().BeCloseTo(50.Milliseconds().After(this.raisedAt[3]), 40);
        }

        [Fact]
        public void UpdatesIntervalAfterRestart()
        {
            // Arrange
            this.sut.Interval = 50.Milliseconds();
            this.sut.Elapsed += () => this.RecordElapsedAndStopAfter(5);

            this.sut.Start();

            // Act
            this.sut.Interval = 150.Milliseconds();
            this.RunTimerUntilStopped();

            // Assert
            this.raisedAt[4].Should().BeCloseTo(150.Milliseconds().After(this.raisedAt[3]), 40);
        }

        private void RecordElapsedAndStopAfter(int stopAfter)
        {
            this.raisedAt.Add(DateTime.Now);
            if (++this.elapsedCount < stopAfter) return;

            this.sut.Stop();
            this.timerStopped.Set();
        }

        private void RunTimerUntilStopped(int timeoutMs = 2000)
        {
            this.startTime = DateTime.Now;
            this.sut.Start();
            this.timerStopped.WaitOne(timeoutMs);
        }
    }
}