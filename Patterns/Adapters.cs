using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Patterns
{
    public class ExternalPerformanceReporter
    {
        public void CollectStatisticNonThreadSafe(
            string methodName,
            TimeSpan executionTime)
        {
        }
        public PerformanceReport GetPerformanceReport()
        {
            return new PerformanceReport();
        }
        public void CleanUpStatistic()
        {
        }

        public class PerformanceReport
        {
            public IDictionary<string, TimeSpan> AsPlainMap()
            {
                return new Dictionary<string, TimeSpan>();
            }
        }
    }

    public interface IThreadSafePerformanceReporter
    {
        Task CollectStatisticAsync(string methodName, TimeSpan executionTime);
        Task<IReadOnlyDictionary<string, TimeSpan>> GetCurrentStatisticAndRefresh();
    }

    public class PerformanceReporterAdapter : IThreadSafePerformanceReporter
    {
        private ReporterWithCountdownEvent _reporterWithCountdownEvent = new ReporterWithCountdownEvent();

        public Task CollectStatisticAsync(string methodName, TimeSpan executionTime)
        {
            _reporterWithCountdownEvent.CountdownEvent.AddCount();
            _reporterWithCountdownEvent.ExternalPerformanceReporter.CollectStatisticNonThreadSafe(methodName, executionTime);
            _reporterWithCountdownEvent.CountdownEvent.Signal();

            return Task.FromResult(true);
        }

        public Task<IReadOnlyDictionary<string, TimeSpan>> GetCurrentStatisticAndRefresh()
        {
            var newReporterWithCountdownEvent = new ReporterWithCountdownEvent();
            var snapshot = Interlocked.Exchange(ref _reporterWithCountdownEvent, newReporterWithCountdownEvent);
            snapshot.CountdownEvent.Signal();
            snapshot.CountdownEvent.Wait();
            var statisticDictionary = snapshot.ExternalPerformanceReporter.GetPerformanceReport().AsPlainMap();
            return Task.FromResult((IReadOnlyDictionary<string, TimeSpan>)new ReadOnlyDictionary<string, TimeSpan>(
                statisticDictionary));
        }

        
        private class ReporterWithCountdownEvent
        {
            public CountdownEvent CountdownEvent { get; } = new CountdownEvent(1);
            public ExternalPerformanceReporter ExternalPerformanceReporter { get; } = new ExternalPerformanceReporter();
        }
    }
}
