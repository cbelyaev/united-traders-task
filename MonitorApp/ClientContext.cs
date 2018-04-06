using System;
using System.Net.Sockets;

namespace MonitorApp
{
    internal sealed class ClientContext: IDisposable
    {
        private const int MessageLength = sizeof(int) + sizeof(int);
        private readonly byte[] _buffer = new byte[MessageLength];
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;

        public ClientContext(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
        }

        public void Dispose()
        {
            // TODO fire client closing event
            _client.Close();
            _stream.Close();
            // TODO fire client closed event
        }

        public IAsyncResult BeginRead() => _stream.BeginRead(_buffer, 0, _buffer.Length, OnClientRead, null);

        private void OnClientRead(IAsyncResult ar)
        {
            try
            {
                var read = _stream.EndRead(ar);
                if (read == 0)
                {
                    Dispose();
                    return;
                }

                var sensorId = BitConverter.ToInt32(_buffer, 0);
                var value = BitConverter.ToInt32(_buffer, sizeof(int));
                Message?.Invoke(this, new MessageArgs(sensorId, value));
                // if message handler fails with exception, client context will be disposed

                BeginRead();
            }
            catch
            {
                Dispose();
            }
        }

        public event EventHandler<MessageArgs> Message;
    }
}