using System;

namespace MonitorApp
{
    public class MessageArgs: EventArgs
    {
        public MessageArgs(int sensorId, int value)
        {
            SensorId = sensorId;
            Value = value;
        }

        public int SensorId { get; }
        public int Value { get; }
    }
}