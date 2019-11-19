using System;
using System.Threading;
using OBSWebsocketDotNet;

namespace Durty.OBS.Watcher
{
    public class ObsManager
    {
        private readonly string _serverIp;
        private readonly int _port;
        private readonly string _password;
        private bool _connectionEstablished;

        public OBSWebsocket Obs { get; }
        public bool AutoReconnect { get; set; } = true;
        public bool IsConnected => Obs.IsConnected;

        public ObsManager(string serverIp, int port, string password = "")
        {
            _serverIp = serverIp;
            _port = port;
            _password = password;

            Obs = new OBSWebsocket()
            {
                WSTimeout = TimeSpan.FromMinutes(1)
            };
            Obs.Disconnected += OnDisconnected;
            Obs.OBSExit += OnObsExit;
        }

        private void OnObsExit(object sender, EventArgs e)
        {
            Console.WriteLine("OBS exited.");
            TryReconnect();
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected from OBS WebSocket");
            TryReconnect();
        }

        public bool Connect()
        {
            Obs.Connect($"ws://{_serverIp}:{_port}", _password);
            _connectionEstablished = Obs.IsConnected;
            return Obs.IsConnected;
        }

        public void Disconnect()
        {
            Obs.Disconnect();
            _connectionEstablished = false;
        }

        public bool Reconnect(int retryDelyMilliseconds = 3000, int retryCount = 5)
        {
            if (Obs.IsConnected)
            {
                Obs.Disconnect();
            }

            for (int i = 0; i < retryCount; i++)
            {
                Thread.Sleep(retryDelyMilliseconds);
                bool success = Connect();
                if (success)
                {
                    return true;
                }
            }

            return false;
        }

        private void TryReconnect()
        {
            if (AutoReconnect && _connectionEstablished)
            {
                Console.WriteLine("Trying to reconnect.");
                Reconnect();
            }
        }
    }
}
