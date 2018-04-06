using System;
using System.Net;
using System.Net.Sockets;

namespace MonitorApp
{
    internal class TcpServer: IDisposable
    {
        private readonly TcpListener _tcpListener;
        private bool _stopped;

        public TcpServer(IPEndPoint endPoint)
        {
            _tcpListener = new TcpListener(endPoint ?? throw new ArgumentNullException(nameof(endPoint)));
        }

        public void Dispose() => Stop();

        public void Start()
        {
            _stopped = false;
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(OnClientAccepted, null);
        }

        private void OnClientAccepted(IAsyncResult ar)
        {
            if (_stopped)
                return;
            try
            {
                // TODO fire client creating event
                var client = _tcpListener.EndAcceptTcpClient(ar);
                var clientContext = new ClientContext(client);
                clientContext.Message += ClientContextOnMessage;
                // TODO fire client created event
                clientContext.BeginRead();
            }
            finally
            {
                _tcpListener.BeginAcceptTcpClient(OnClientAccepted, null);
            }
        }

        private void ClientContextOnMessage(object sender, MessageArgs messageArgs)
        {
            PerfMonitor.ValuesPerSecondIncrement();
#if DEBUG
            Trace.WriteLine($"{messageArgs.SensorId}: {messageArgs.Value}");
#endif
            // sender used for ClientContext (mainly TcpClient) info
            Message?.Invoke(sender, messageArgs);
        }

        public void Stop()
        {
            _stopped = true;
            _tcpListener.Stop();
        }

        public event EventHandler<MessageArgs> Message;
    }
}