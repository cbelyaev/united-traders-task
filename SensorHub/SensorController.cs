using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SensorHub
{
    internal class SensorController
    {
        private readonly IPEndPoint _endPoint;
        private readonly int _id;
        private readonly byte[] _buffer;
        private readonly int _period;
        private bool _stopPending;

        public SensorController(IPEndPoint endPoint, int period, int id)
        {
            _endPoint = endPoint ?? throw new ArgumentNullException(nameof(endPoint));
            _period = period; // TODO validate
            _id = id; // TODO validate
            _buffer = new byte[sizeof(int) + sizeof(int)];
            var idBytes = BitConverter.GetBytes(_id);
            Array.Copy(idBytes, _buffer, idBytes.Length);
        }

        public void Start()
        {
            _stopPending = false;
            new Thread(Sensor).Start();
        }

        private void Sensor()
        {
            try
            {
                var seed = _id;
                // different generators created with default seed at one time but in different threads 
                // produce the same values
                var r = new Random(seed);
                using (var client = new TcpClient())
                {
                    client.Connect(_endPoint);
                    var stream = client.GetStream();
                    while (!_stopPending)
                    {
                        var now = DateTime.UtcNow;
                        var value = r.Next(int.MinValue, int.MaxValue);
                        var valueBytes = BitConverter.GetBytes(value);
                        Array.Copy(valueBytes, 0, _buffer, sizeof(int), valueBytes.Length);
                        stream.Write(_buffer, 0, _buffer.Length);
                        
                        var elapsed = (DateTime.UtcNow - now).TotalMilliseconds;
                        var toSleep = (int)(_period - elapsed);
                        if (toSleep > 0)
                            Thread.Sleep(toSleep); // TODO use AutoResetEvent to stop sleeping
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sensor {_id} failed: {ex.Message}");
                // TODO add reconnect logic
            }
        }

        public void RequestStop()
        {
            _stopPending = true;
        }
    }
}