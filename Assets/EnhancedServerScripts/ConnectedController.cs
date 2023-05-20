using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using EnhancedDodoServer;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Represents a client held by the server in a list
    /// </summary>
    public class ConnectedController
    {
        /// <summary>
        /// Is this client able to send and receive TCP messages
        /// </summary>
        public bool IsTCP { get; set; }
        /// <summary>
        /// TCP socket of this client
        /// </summary>
        public TCPConnectedClient tcpConnectedClient;
        /// <summary>
        /// Is this client able to send and receive UDP messages
        /// </summary>
        public bool IsUDP { get; set; }
        /// <summary>
        /// UDP socket of this client
        /// </summary>
        public UDPConnectedClient udpConnectedClient;
        /// <summary>
        /// Holds information about player input, steers player's character on the server side
        /// </summary>
        public PlayerInput playerInput;
        /// <summary>
        /// Message to be sent to this client on the client side
        /// </summary>
        public string message;
        /// <summary>
        /// Public constructor, creates a new client with his ip and clientID
        /// </summary>
        /// <param name="_ip">IP of the client</param>
        /// <param name="_clientID">ID of the client</param>
        public ConnectedController(IPAddress _ip, int _clientID) : this(_clientID)
        {
            //IsTCP = false;
            //IsUDP = false;            
            //playerInput = new PlayerInput();                        
            //playerInput.clientData.clientID = _clientID;
            playerInput.clientData.ip = _ip;
        }
        public ConnectedController(int _clientID)
        {
            IsTCP = false;
            IsUDP = false;
            //ip = _ip;
            playerInput = new PlayerInput();
            playerInput.clientData.clientID = _clientID;
        }
        /// <summary>
        /// Adds a tcp socket to this client
        /// </summary>
        /// <param name="_tcpConnectedClient">TCP socket</param>
        public void AddTCP(TCPConnectedClient _tcpConnectedClient)
        {
            IsTCP = true;
            tcpConnectedClient = _tcpConnectedClient;
        }
        /// <summary>
        /// Adds a udp socket to this client
        /// </summary>
        /// <param name="_udpConnectedClient">UDP socket</param>
        public void AddUDP(UDPConnectedClient _udpConnectedClient)
        {
            IsUDP = true;
            udpConnectedClient = _udpConnectedClient;
        }
        /// <summary>
        /// Removes tcp socket
        /// </summary>
        public void RemoveTCP()
        {
            IsTCP = false;
            tcpConnectedClient = null;
        }
        /// <summary>
        /// Removes udp socket
        /// </summary>
        public void RemoveUDP()
        {
            IsUDP = false;
            udpConnectedClient = null;
        }
        /// <summary>
        /// Send a message to this client over TCP
        /// </summary>
        /// <param name="message">The message</param>
        public void SendOverTCP(string message)
        {
            //tcpServer.Send(message, tcpConnectedClient.tcpClient);
            if (IsTCP)
                tcpConnectedClient.Send(message);
            else
                Debug.LogError("Can't send over TCP");
        }
        /// <summary>
        /// Send a message to this client over UDP
        /// </summary>
        /// <param name="message">The message</param>
        public void SendOverUDP(string message)
        {    
            //udpServer.Send(message, ipEndPoint);
            if (IsUDP)
                udpConnectedClient.Send(message);
            else
                Debug.LogError("Can't send over UDP");
        }
    }
}