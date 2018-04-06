using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SensorHub
{
    internal class Program
    {
        private const int PeriodMin = 1;
        private const int PeriodMax = 1000;
        private const int IdMin = 1;
        private const int IdMax = int.MaxValue;

        private static void Main(string[] args)
        {
            string serverHost;
            int serverPort;
            int period;
            int firstSensorId;
            int lastSensorId;
            if (args.Length > 3)
            {
                serverHost = args[0];
                if (string.IsNullOrEmpty(serverHost))
                {
                    Console.WriteLine("Empty serverHost");
                    return;
                }

                if (!int.TryParse(args[1], out serverPort) || serverPort < 0 || serverPort > ushort.MaxValue)
                {
                    Console.WriteLine("Invalid serverPort");
                    return;
                }

                if (!int.TryParse(args[2], out period) || period < PeriodMin || period > PeriodMax)
                {
                    Console.WriteLine("Invalid period");
                    return;
                }

                if (!int.TryParse(args[3], out firstSensorId) || firstSensorId < IdMin)
                {
                    Console.WriteLine("Invalid firstSensorId");
                    return;
                }

                lastSensorId = firstSensorId;
                if (args.Length > 4)
                {
                    if (!int.TryParse(args[4], out lastSensorId) || lastSensorId < firstSensorId)
                    {
                        Console.WriteLine("Invalid lastSensorId");
                        return;
                    }
                }
            }
            else
            {
                Usage();
                return;
            }

            try
            {
                var ip = Dns.GetHostEntry(serverHost).AddressList.FirstOrDefault();
                if (ip == null)
                {
                    Console.WriteLine("serverHost not found");
                    return;
                }

                var endPoint = new IPEndPoint(ip, serverPort);

                var sensors = new List<SensorController>();
                for (var id = firstSensorId; id <= lastSensorId; id++)
                {
                    var sensorController = new SensorController(endPoint, period, id);
                    sensorController.Start();
                    sensors.Add(sensorController);
                }

                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
                sensors.ForEach(s => s.RequestStop());
                // after all foreground threads completes, process will be terminated
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Usage()
        {
            Console.WriteLine("Usage:\n" +
                              $"{nameof(SensorHub)} <serverHost> <serverPort> <period> <firstSensorId> [<lastSensorId>]\n\n" +
                              "    <serverHost>:<serverPort> - server address\n" +
                              $"    <period> - period of sending values, {PeriodMin}-{PeriodMax} msec\n" +
                              $"    <firstSensorId> - sensor id, {IdMin}-{IdMax}\n" +
                              $"    <lastSensorId> - end of sensor ids range, <firstSensorId> - {IdMax}\n");
        }
    }
}