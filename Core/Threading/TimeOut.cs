using System;
using System.Diagnostics;

namespace VisionNet.Core.Threading
{
    /// <summary>
    /// Provides a stopwatch-based timeout helper that compares the elapsed time against a configured maximum delay.
    /// </summary>
    public class TimeOut: IDisposable
    {
        private Stopwatch _stopwatch = new Stopwatch();
        private TimeSpan _maxDelay = TimeSpan.Zero;

        /// <summary>
        /// Initializes a new instance with the specified maximum delay measured as a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="maxDelay">The maximum allowable elapsed time. Use <see cref="TimeSpan.Zero"/> for a non-expiring timeout.</param>
        public TimeOut(TimeSpan maxDelay)
        {
            _maxDelay = maxDelay;
        }

        /// <summary>
        /// Initializes a new instance with the specified maximum delay expressed in milliseconds.
        /// </summary>
        /// <param name="maxDelayMs">The maximum allowable elapsed time in milliseconds. Specify <c>0</c> for a non-expiring timeout; negative values cause the timeout to be considered exceeded immediately.</param>
        public TimeOut(int maxDelayMs = 0)
        {
            _maxDelay = TimeSpan.FromMilliseconds(maxDelayMs);
        }

        /// <summary>
        /// Starts or restarts the timeout measurement from zero elapsed time.
        /// </summary>
        public void Start()
        {
            _stopwatch.Restart();
        }

        /// <summary>
        /// Gets a value indicating whether the elapsed time has reached or exceeded the configured maximum delay.
        /// </summary>
        /// <value><see langword="true"/> when the timeout is active and the elapsed time is greater than or equal to the maximum delay; otherwise, <see langword="false"/>. A timeout created with a zero maximum delay never expires.</value>
        public bool IsExceded => _maxDelay == TimeSpan.Zero ? false : _stopwatch.Elapsed >= _maxDelay;

        /// <summary>
        /// Stops the underlying stopwatch and releases associated resources.
        /// </summary>
        public void Dispose()
        {
            _stopwatch.Stop();
            _stopwatch = null;
        }

        /// <summary>
        /// Gets a timeout instance that never expires.
        /// </summary>
        public static TimeOut Infinite => new TimeOut();
    }
}
