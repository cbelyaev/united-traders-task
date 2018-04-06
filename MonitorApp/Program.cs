using System;
using System.Net;
using System.Windows.Forms;

namespace MonitorApp
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var port = 20000;
            if (args.Length > 0 && int.TryParse(args[0], out var p) && p > 1)
                port = p;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var server = new TcpServer(new IPEndPoint(IPAddress.Any, port)))
            {
                var form = new MainForm();
                server.Message += (o, a) => form.OnMessage(o, a);
                server.Start();
                Application.Run(form);
            }
        }
    }
}