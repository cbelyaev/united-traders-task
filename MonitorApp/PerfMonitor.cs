using System.Diagnostics;

namespace MonitorApp
{
    internal static class PerfMonitor
    {
        private const string CounterCategoryName = "UnitedTradersTask.MonitorApp";
        private const string ValuesPerSecondCounterName = "# points received / sec";
        private static readonly PerformanceCounter ValuesPerSecondPerformanceCounter;
        private static readonly bool Enabled;

        static PerfMonitor()
        {
            try
            {
                if (!PerformanceCounterCategory.Exists(CounterCategoryName))
                {
                    CreateCategory();
                }
                else if (GetCounters().Length != 1)
                {
                    PerformanceCounterCategory.Delete(CounterCategoryName);
                    CreateCategory();
                }

                ValuesPerSecondPerformanceCounter = new PerformanceCounter
                {
                    CategoryName = CounterCategoryName,
                    CounterName = ValuesPerSecondCounterName,
                    MachineName = ".",
                    ReadOnly = false,
                    RawValue = 0
                };
                Enabled = true;
            }
            catch
            {
                // TODO log
                Enabled = false;
            }
        }

        private static void CreateCategory()
        {
            PerformanceCounterCategory.Create(CounterCategoryName,
                $"Category {CounterCategoryName}",
                PerformanceCounterCategoryType.SingleInstance,
                new CounterCreationDataCollection(new[]
                {
                    new CounterCreationData
                    {
                        CounterName = ValuesPerSecondCounterName,
                        CounterHelp = "Total number of values received per second",
                        CounterType = PerformanceCounterType.RateOfCountsPerSecond32
                    }
                }));
        }

        private static PerformanceCounter[] GetCounters()
        {
            return new PerformanceCounterCategory(CounterCategoryName).GetCounters();
        }

        public static void ValuesPerSecondIncrement()
        {
            if (!Enabled)
                return;
            try
            {
                ValuesPerSecondPerformanceCounter?.Increment();
            }
            catch
            {
                // TODO log
            }
        }

        public static float GetValuesPerSecond()
        {
            if (!Enabled)
                return float.NaN;
            try
            {
                return ValuesPerSecondPerformanceCounter?.NextValue() ?? float.NaN;
            }
            catch
            {
                // TODO log
                return float.NaN;
            }
        }
    }
}