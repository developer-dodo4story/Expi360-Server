using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedDodoServer;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Interface of a client that needs to first find server ip
    /// </summary>
    public interface INetworkDiscoverer
    {
        void DiscoverNetwork();
        void Broadcast(string message);
    }
}