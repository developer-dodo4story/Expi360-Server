using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using EnhancedDodoServer;
using System;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Represents a UDP socket of a client
    /// </summary>
    public class UDPConnectedClient
    {
        public static string exceptionError = "UDPConnectedClient Error";

        /// <summary>
        /// Handles opening and closing connections and sending,receiving messages
        /// </summary>
        UdpClient udpClient;
        /// <summary>
        /// The IPEndPoint this client is connected to
        /// </summary>
        public IPEndPoint ipEndPoint { get; private set; }
        /// <summary>
        /// Public constructor, creates a new client with a specified IPEndPoint to connect to
        /// </summary>
        /// <param name="_ipEndPoint">IPEndPoint to connect to</param>
        public UDPConnectedClient(IPEndPoint _ipEndPoint)
        {
            ipEndPoint = _ipEndPoint;
            udpClient = new UdpClient();
        }

        byte[] data = null;
        /// <summary>
        /// Sends a message to this client
        /// </summary>
        /// <param name="message"></param>
        public void Send(string message)
        {
            try
            {
                data = System.Text.Encoding.UTF8.GetBytes(message);
                udpClient.Send(data, data.Length, ipEndPoint);
            }
            catch (Exception e)
            {
                exceptionError = e.Message;
            }
        }
    }
}