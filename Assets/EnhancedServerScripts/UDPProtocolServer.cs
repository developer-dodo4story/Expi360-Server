using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.Net.Sockets;
using EnhancedDodoServer;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Class implementing the lowest level transport layer of a UDP server
    /// </summary>
    public class UDPProtocolServer : Singleton<UDPProtocolServer>
    {
        public string errorMessage = "UDPProtocolServer Errors";

        /// <summary>
        /// Used to connect, send and receive data
        /// </summary>
        UdpClient udpClient;
        /// <summary>
        /// List of IPEndPoints from which server has gotten a message
        /// </summary>
        List<IPEndPoint> udpClients = new List<IPEndPoint>();
        /// <summary>
        /// Name of the server to be broadcast with UDP
        /// </summary>
        string serverName;
        /// <summary>
        /// Name of the game server is playing, broadcast with UDP
        /// </summary>
        Consts.Game game;
        /// <summary>
        /// message consisting of serverName and game, broadcast        
        /// </summary>
        string invitationMessage;
        /// <summary>
        /// Should the server be broadcasting invitationMessage?        
        /// </summary>
        bool broadcastServerNameAndIP = false;
        /// <summary>
        /// IPEndPoint for broadcasting
        /// </summary>
        IPEndPoint broadcastEndPoint;
        /// <summary>
        /// Initialize broadcasting of the server
        /// </summary>
        /// <param name="_serverName">Name of the server</param>
        /// <param name="_game">Name of the game</param>
        public void BroadcastServer(string message)
        {
            //serverName = _serverName;
            //game = _game;
            invitationMessage = message;//Consts.CreateMessage(new string[] { Consts.Words.serverName, Consts.Words.game },new object[] { serverName, game });

            broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, Consts.clientPort);
            udpClient.EnableBroadcast = true;

            broadcastServerNameAndIP = true;
        }
        /// <summary>
        /// Setter for a message broadcast to new clients
        /// </summary>
        /// <param name="message"></param>
        public void SetBroadcastMessage(string message)
        {
            invitationMessage = message;
        }
        /// <summary>
        /// Creates new socket for UDP on a specified port and start to listen for messages
        /// </summary>
        public void Connect()
        {
            udpClient = new UdpClient(Consts.serverPort);
            udpClient.BeginReceive(OnClientConnect, null);
        }
        /// <summary>
        /// Closes the socket
        /// </summary>
        public void Disconnect()
        {
            udpClient.Close();
        }
        void OnClientConnect(IAsyncResult asyncResult)
        {
            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.EndReceive(asyncResult, ref ipEndPoint);
                string message = System.Text.Encoding.UTF8.GetString(data);
                HandleMessage(message, ipEndPoint);
            }
            catch (SocketException e)
            {
                errorMessage = e.Message;
            }
            udpClient.BeginReceive(OnClientConnect, null);
        }

        byte[] data = null;
        private void Update()
        {
            if (broadcastServerNameAndIP)
            {
                try
                {
                    data = System.Text.Encoding.UTF8.GetBytes(invitationMessage);
                    udpClient.Send(data, data.Length, broadcastEndPoint);
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                }
            }
            //if (networkDiscoverers.Count != 0)
            //{
            //    //print("ND" + networkDiscoverers.Count);
            //    foreach (IPEndPoint ipEndPoint in networkDiscoverers)
            //    {
            //        byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            //        udpClient.Send(data, data.Length, ipEndPoint);
            //    }
            //}
        }
        private void HandleMessage(string message, IPEndPoint ipEndPoint)
        {
            HandleClient(message, ipEndPoint);
        }
        /// <summary>
        /// Sends data to certain endpoint
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ipEndPoint"></param>
        public void Send(byte[] data, IPEndPoint ipEndPoint)
        {
            try
            {
                udpClient.Send(data, data.Length, ipEndPoint);
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }
        }

        private void HandleClient(string message, IPEndPoint ipEndPoint)
        {
            bool clientAlreadyConnected = CheckIfAlreadyConnected(ipEndPoint);

            if (!clientAlreadyConnected)
            {
                //StopResponding(ipEndPoint);

                //Czekaj na właściwego klienta w kolejnosci.
                //PlayerInput playerInput = JsonUtility.FromJson<PlayerInput>(message);
                //int id = playerInput.clientData.clientID;

                //if (id == udpClients.Count)
                //{
                udpClients.Add(ipEndPoint);
                UDPConnectedClient udpConnectedClient = new UDPConnectedClient(ipEndPoint);
                Server.instance.AddController(udpConnectedClient, message);
                //}
            }
            else
            {
                Server.instance.ReadUDP(message, ipEndPoint.Address);
            }
        }

        bool CheckIfAlreadyConnected(IPEndPoint _ipEndPoint)
        {
            //This doesnt work
            //return udpClients.Contains(_ipEndPoint);
            foreach (IPEndPoint iep in udpClients)
            {
                if (iep.Address.Equals(_ipEndPoint.Address))
                {
                    return true;
                }
            }
            return false;
        }
    }
}