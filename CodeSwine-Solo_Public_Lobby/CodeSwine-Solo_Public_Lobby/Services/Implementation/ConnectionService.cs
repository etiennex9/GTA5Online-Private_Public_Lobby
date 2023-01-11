﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using PacketDotNet;
using SharpPcap;

namespace CodeSwine_Solo_Public_Lobby.Services.Implementation
{
    public class ConnectionService : IConnectionService
    {
        private readonly ConcurrentDictionary<IPAddress, DateTimeOffset> _lastSeen = new();
        private IEnumerable<ICaptureDevice> _captureDevices = null;

        public IEnumerable<IPAddress> GetActiveConnections(TimeSpan maxLastSeen)
        {
            var now = DateTimeOffset.UtcNow;

            return _lastSeen
                .Where(kvp => kvp.Value + maxLastSeen >= now)
                .Select(kvp => kvp.Key);
        }

        public void Start(int udpPort)
        {
            if (_captureDevices is not null)
            {
                throw new Exception("Already initialized");
            }

            _lastSeen.Clear();
            _captureDevices = CaptureDeviceList.New();

            foreach (var device in _captureDevices)
            {
                device.OnPacketArrival += CaptureDevice_OnPacketArrival;
                device.Open(DeviceMode.Promiscuous, 1000);
                device.Filter = "udp port " + udpPort;
                device.StartCapture();
            }
        }

        public void Stop()
        {
            if (_captureDevices is not null)
            {
                foreach (var device in _captureDevices)
                {
                    device.OnPacketArrival -= CaptureDevice_OnPacketArrival;
                }

                foreach (var device in _captureDevices)
                {
                    device.StopCapture();
                    device.Close();
                }

                _captureDevices = null;
            }
        }

        private void CaptureDevice_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            if (packet is EthernetPacket ethernetPacket &&
                ethernetPacket.HasPayloadPacket &&
                ethernetPacket.Type is EthernetType.IPv4 or EthernetType.IPv6)
            {
                var ipPacket = ethernetPacket.PayloadPacket as IPPacket;

                _lastSeen[ipPacket.SourceAddress] = DateTimeOffset.UtcNow;
                _lastSeen[ipPacket.DestinationAddress] = DateTimeOffset.UtcNow;
            }
        }

        #region IDisposable

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Stop();

                    _lastSeen.Clear();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
