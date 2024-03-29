﻿using System;
using System.Threading;
using Durty.OBS.Watcher.Contracts;
using OBS.WebSocket.NET;

namespace Durty.OBS.Watcher
{
    public class ObsManager
    {
        private readonly string _serverIp;
        private readonly int _port;
        private readonly string _password;
        private readonly ILogger _logger;
        private bool _connectionEstablished;

        public ObsWebSocket Obs { get; }
        public bool AutoReconnect { get; set; } = true;
        public bool IsConnected => Obs.IsConnected;

        public ObsManager(
            ILogger logger,
            string serverIp, 
            int port, 
            string password = "")
        {
            _serverIp = serverIp;
            _port = port;
            _password = password;
            _logger = logger;

            Obs = new ObsWebSocket()
            {
                Timeout = TimeSpan.FromMinutes(1)
            };
            Obs.Disconnected += OnDisconnected;
            Obs.OBSExit += OnObsExit;
        }

        private void OnObsExit(object sender, EventArgs e)
        {
            _logger.Write(LogLevel.Warn, "OBS exited.");
            TryReconnect();
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            _logger.Write(LogLevel.Warn, "Disconnected from OBS WebSocket");
            TryReconnect();
        }

        public bool Connect()
        {
            Obs.Connect($"ws://{_serverIp}:{_port}", _password);
            _connectionEstablished = true; //TODO: Temporary fix until Obs.Connect immediately returns connected state
            return _connectionEstablished;
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
                _logger.Write(LogLevel.Info, "Trying to reconnect.");
                Reconnect();
            }
        }
    }
}
