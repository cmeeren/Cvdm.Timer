# Cvdm.Timer
Schedules execution of a C# callback using a friendly `System.Threading.Timer` wrapper.

This library is easy to use with dependency injection:

* Inject `ITimerFactory` into your components, and use `ITimerFactory.Create()` to create an `ITimer`.
* In production use, register `TimerFactory` as an implementation of `ITimerFactory`.
* In your tests, mock `ITimerFactory` and set up `ITimerFactory.Create()` to return a mock `ITimer`. Raise the `ITimer.Elapsed` event manually as needed.
* **Note:** `ITimer` is disposable, and you should not inject it directly unless you know what you're doing. Use `ITimerFactory` instead.

Here's the API of the `ITimer` interface:

```c#
// Occurs when InitialDelay or Interval elapses
event Action Elapsed;

// Gets or sets the delay after which to first raise the Elapsed event. Non-positive values will
// make the event be immediately raised when the timer is started. If this is set on an active
// timer, call Start() to make the new value take effect.
TimeSpan Delay { get; set; }

// Gets or sets the interval at which to raise the Elapsed event. Non-positive values disables
// periodic elapsing. If this is set on an active timer, call Start() or Start(startTime) to
// make the new value take effect.
TimeSpan Interval { get; set; }

// Gets a value indicating whether the timer is active.
bool IsEnabled { get; }

// Starts raising the Elapsed event according to Interval and InitialDelay.
void Start();

// Uses startTime to set the InitialDelay property and immediately starts the timer.
void Start(DateTime startTime);

/// Stops raising the Elapsed event.
void Stop();
```

`ITimer` also implements `IDisposable` to dispose the wrapped `System.Threading.Timer`