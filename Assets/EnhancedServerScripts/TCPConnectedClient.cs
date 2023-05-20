using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;
using EnhancedDodoServer;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Represents a TCP socket of a client
    /// </summary>
    public class TCPConnectedClient
    {
        /// <summary>
        /// Buffer where the messages are stored
        /// </summary>
        readonly byte[] readBuffer = new byte[Consts.tcpBufferSize];
        /// <summary>
        /// Handles tcp connection and sending and receiving messages
        /// </summary>
        public TcpClient tcpClient { get; private set; }
        /// <summary>
        /// public event invoked when a message arrives
        /// </summary>
        public UnityAction<string> onRead;
        /// <summary>
        /// public constructor, creates a new connection
        /// </summary>
        /// <param name="_tcpClient"></param>
        public TCPConnectedClient(TcpClient _tcpClient)
        {
            tcpClient = _tcpClient;
        }
        /// <summary>
        /// Start reading messages
        /// </summary>
        public void BeginRead()
        {
            tcpClient.GetStream().BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
        }
        /// <summary>
        /// Begins a connection
        /// </summary>
        public void BeginConnect()
        {
            throw new System.NotImplementedException();
            //IPAddress serverIP = IPAddress.Parse(Consts.serverIP);
            //tcpClient.BeginConnect(serverIP, Consts.serverPort, OnConnectionComplete, null);
        }
        void OnConnectionComplete(IAsyncResult asyncResult)
        {
            tcpClient.EndConnect(asyncResult);

            BeginRead();

        }
        /// <summary>
        /// Closes a connection
        /// </summary>
        public void Disconnect()
        {
            tcpClient.GetStream().Close();
            tcpClient.Close();
        }
        void OnRead(IAsyncResult asyncResult)
        {
            int length = tcpClient.GetStream().EndRead(asyncResult);
            if (length <= 0)//Connection closed
            {
                if (TCPProtocolServer.instance != null)
                {
                    TCPProtocolServer.instance.OnDisconnect(this);
                    return;
                }
            }

            string message = System.Text.Encoding.UTF8.GetString(readBuffer, 0, length);
            if (onRead != null) onRead(message);
            tcpClient.GetStream().BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
        }
        /// <summary>
        /// Sends a message to the client
        /// </summary>
        /// <param name="message">The message</param>
        public void Send(string message)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            tcpClient.GetStream().Write(data, 0, data.Length);
        }
    }
}