using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using EnhancedDodoServer;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Class implementing the lowest level transport layer of a TCP server
    /// </summary>
    public class TCPProtocolServer : Singleton<TCPProtocolServer>
    {
        /// <summary>
        /// Handles establishing and closing connections
        /// </summary>
        TcpListener listener;
        /// <summary>
        /// Creates new TCP socket and starts accepting incoming connections
        /// </summary>
        public void Connect()
        {
            listener = new TcpListener(IPAddress.Any, Consts.serverPort);
            listener.Start();
            listener.BeginAcceptTcpClient(OnClientConnect, null);
        }        
        void OnClientConnect(IAsyncResult asyncResult)
        {
            TcpClient tcpClient = listener.EndAcceptTcpClient(asyncResult);
            TCPConnectedClient tcpConnectedClient = new TCPConnectedClient(tcpClient);
            tcpConnectedClient.BeginRead();
            tcpConnectedClient.onRead += OnRead;
            Server.instance.AddController(tcpConnectedClient);
            listener.BeginAcceptTcpClient(OnClientConnect, null);
        }
        /// <summary>
        /// Invoked when an empty message has been received from a connected client - disconnect
        /// </summary>
        /// <param name="client">The client to be disconnected</param>
        public void OnDisconnect(TCPConnectedClient client)
        {
            Server.instance.RemoveController(client);
            //remove from list
        }
        void OnRead(string message)
        {
            //do sth with msg        
            Server.instance.ReadTCP(message);
        }
    }
}