﻿using System;
using System.Collections.Generic;
using System.Net;

namespace GTA5_Private_Public_Lobby.Services
{
    public interface IConnectionService : IDisposable
    {
        IEnumerable<IPAddress> GetActiveConnections(TimeSpan maxLastSeen);

        void Start(int udpPort);

        void Stop();
    }
}
