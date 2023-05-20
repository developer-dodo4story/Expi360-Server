using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using EnhancedDodoServer;
using System;

namespace GamePad
{
    /// <summary>
    /// High level class representing a server. In this class you can choose which functionalities (like TCP/UDP) of the Server class you want to use
    /// </summary>
    public class GamePadServer : Server, IServerUDP
    {
        /// <summary>
        /// Singleton, you should use this.
        /// </summary>
        public static GamePadServer Instance;
        /// <summary>
        /// Method that initializes UDP
        /// </summary>
        public void InitializeUDP()
        {
            base.InitUDP();
        }
        public PingCanvas pingCanvasL, pingCanvasR;

        private int GetClientID(IPAddress adress)
        {
            int id = -1;
            ip_id_Dictionary.TryGetValue(adress, out id);
            return id;
        }

        /// <summary>
        /// Overridden method invoked when a new UDP message arrives
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ipAddr">The ip where the message came from</param>
        public override void ReadUDP(string message, IPAddress ipAddr)
        {
            base.ReadUDP(message, ipAddr);
            PlayerInput playerInput = JsonUtility.FromJson<PlayerInput>(message);
            int id = GetClientID(ipAddr);

            if (id == -1) //Player has disconnected and now reconnecting. Send him his old id
            {
                //ResendID(ipAddr); // juz nie trzeba odsyłac id.
            }
            else
            {
                //measure ping. Works if GenerateID() returns id++. Else pings.Find(p => p.id == id).AddTime(Time.time);
                //Ping ping = pings[id];
                //ping.AddTime((float)(DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime()).TotalSeconds);
                //playerInput.ping = ping;
                PlayerInput prevPlayerInput = GetPlayerInput(id);
                if (prevPlayerInput != null)
                {
                    if (prevPlayerInput.counter2 < playerInput.counter2 || (prevPlayerInput.counter2 == playerInput.counter2 && prevPlayerInput.counter1 < playerInput.counter1))
                    {
                        SetPlayerInput(id, playerInput);
                    }
                }
                else
                {
                    SetPlayerInput(id, playerInput);
                }

                if (!playerKnowsID[id])
                {
                    IPAddress ip = playerInput.clientData.ip;
                    PlayerKnowsID(ip, id);
                    playerKnowsID[id] = true;
                }
            }
        }



        /// <summary>
        /// Invoked when a first udp message arrives. Read player name from it 
        /// </summary>
        /// <param name="cc">Client whose message has been received</param>
        /// <param name="msg">The message</param>
        protected override void FirstUDPMessageReceived(ConnectedController cc, string msg)
        {
            base.FirstUDPMessageReceived(cc, msg);
            PlayerInput playerInput = JsonUtility.FromJson<PlayerInput>(msg);
            cc.playerInput.clientData.name = playerInput.clientData.name;
            playerKnowsID.Add(false);
        }
        protected override void Awake()
        {
            base.Awake();
            if (Instance == null)
                Instance = (GamePadServer)FindObjectOfType(typeof(GamePadServer));
            else
                Destroy(gameObject);

            idAssignment = NetworkIDAssignmentMethod.OverUDP;

#if !UNITY_EDITOR
            printInput = false;
#endif
        }
        protected override void Start()
        {
            base.Start();
            Server.instance.currentInputType = Consts.InputType.PhonePad;
        }

        bool printPing = false;
        private string[] messageOptimization = new string[2];
        private object[] objectOptimization = new object[2];
        protected override void Update()
        {
            base.Update();

            messageOptimization[0] = Consts.Words.serverName;
            messageOptimization[1] = Consts.Words.game;
            objectOptimization[0] = server;
            objectOptimization[1] = game;

            udpServer.SetBroadcastMessage(Consts.CreateMessage(messageOptimization, objectOptimization));

            if (Input.GetKeyDown(KeyCode.O) && pingCanvasL != null && pingCanvasR != null)
            {
                printPing = !printPing;
                pingCanvasL.gameObject.SetActive(printPing);
                pingCanvasR.gameObject.SetActive(printPing);

            }
            if (printPing && pingCanvasL != null && pingCanvasR != null)
            {
                string[] texts = new string[connectedControllers.Count];
                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i] = "Player " + (i + 1).ToString() + " AvgPing: " + connectedControllers[i].playerInput.ping.avg.ToString();
                }
                pingCanvasL.SetPingTexts(texts);
                pingCanvasR.SetPingTexts(texts);
            }
        }
    }
}