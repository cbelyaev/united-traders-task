namespace MonitorApp
{
    internal class SensorData
    {
        public int Value { get; private set; }
        public int Diff { get; private set; }

        public SensorData(int value)
        {
            Value = value;
            Diff = 0;
        }

        public SensorData Update(int newValue)
        {
            Diff = Value - newValue;
            Value = newValue;
            return this;
        }
    }
}