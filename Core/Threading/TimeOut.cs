using System;
using System.Diagnostics;

namespace VisionNet.Core.Threading
{
    public class TimeOut: IDisposable
    {
        private Stopwatch _stopwatch = new Stopwatch();
        private TimeSpan _maxDelay = TimeSpan.Zero;

        public TimeOut(TimeSpan maxDelay)
        {
            _maxDelay = maxDelay;
        }

        public TimeOut(int maxDelayMs = 0)
        {
            _maxDelay = TimeSpan.FromMilliseconds(maxDelayMs);
        }

        public void Start()
        {
            _stopwatch.Restart();
        }

        public bool IsExceded => _maxDelay == TimeSpan.Zero ? false : _stopwatch.Elapsed >= _maxDelay;

        public void Dispose()
        {
            _stopwatch.Stop();
            _stopwatch = null;
        }

        public static TimeOut Infinite => new TimeOut();
    }
}
