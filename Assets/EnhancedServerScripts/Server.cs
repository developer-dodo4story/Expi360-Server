using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using EnhancedDodoServer;
using UnityEngine.Events;
using System;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Enum describing how to handle clientID assignment by server
    /// </summary>
    public enum NetworkIDAssignmentMethod { None, OverTCP, OverUDP }
    /// <summary>
    /// Abstract class representing a server, has methods that allow to utilize both TCP and UDP
    /// </summary>
    public abstract class Server : Singleton<Server>
    {
        public static string errorMessage = "Server Errors";

        /// <summary>
        /// Name of the server as a string, passed later on to lower layers of the server
        /// </summary>
        protected string serverName;
        /// <summary>
        /// Name of the game, passed later on to lower layers of the server
        /// </summary>
        public Consts.Game game;
        /// <summary>
        /// Name of the server, parsed to string serverName
        /// </summary>
        [HideInInspector]
        public Consts.Server server;
        /// <summary>
        /// public list of clients, through this variable you can access every connected client, his input and clientData, also send messages
        /// </summary>
        public List<ConnectedController> connectedControllers { get; private set; }
        /// <summary>
        /// Is it possible to send and receive udp messages?
        /// </summary>
        [HideInInspector]
        public bool udpInitialized = false;
        protected UDPProtocolServer udpServer;
        /// <summary>
        /// Is it possible to send and receive tcp messages?
        /// </summary>
        [HideInInspector]
        public bool tcpInitialized = false;
        TCPProtocolServer tcpServer;
        /// <summary>
        /// Determines how to assign ID to a newly connected client
        /// </summary>
        public NetworkIDAssignmentMethod idAssignment { get; protected set; }
        /// <summary>
        /// Current client ID, assigned to each client
        /// </summary>
        private static int clientID = 0;
        /// <summary>
        /// Binds clients by ip with their asssigned id
        /// </summary>
        protected Dictionary<IPAddress, int> ip_id_Dictionary = new Dictionary<IPAddress, int>();
        /// <summary>
        /// list of clients that need to be sent a certain message every frame through udp (like their ID if idAssignment == OverUDP)
        /// </summary>
        List<int> controllersToSendEveryFrameUDP = new List<int>();
        /// <summary>
        /// list of bools indicating whether a certain client already knows his id and we should stop sending him it (useful when idAssignment == OverUDP)
        /// </summary>
        protected List<bool> playerKnowsID = new List<bool>();
        /// <summary>
        /// public event fired when a new client has connected to the server
        /// </summary>
        public UnityAction<ConnectedController> onNewClientConnected;
        /// <summary>
        /// public event fired when udp connection has been initialized
        /// </summary>
        public UnityAction onUDPInitialized;
        /// <summary>
        /// public event fired when tcp connection has been initialized
        /// </summary>
        public UnityAction onTCPInitialized;
        /// <summary>
        /// public event fired when a udp message is sent to all clients        
        /// </summary>
        //public UnityAction onUDPMessageSentToAllClients;
        //public UnityAction onTCPMessageSentToAllClients;
        protected List<int> disconnectionCounters = new List<int>();
        protected const int maxNFramesToDisconnect = 60;
        protected DodoFX dodoFX = new DodoFX();
        protected IPEndPoint fxControllerEndPoint;
        /// <summary>
        /// For debug purpose, print the input of each client
        /// </summary>
        public bool printInput = false;
        [HideInInspector]
        public Consts.InputType currentInputType;
        #region DodoFX
        //Test UI
        //public UnityEngine.UI.Button chairButton, bubblesButton, smokeButton, windButton, thunderButton;
        //public UnityEngine.UI.InputField chairNumber, chairDirection, chairSpeed, bubblesAngle, bubblesIntensity, bubblesHowLong, smokeAngle, smokeIntensity, smokeHowLong, windAngle, windIntensity, windHowLong, thunderAngle, thunderIntensity, thunderHowLong;
        public void SwitchInputType(Consts.InputType inputType)
        {
            if (inputType == currentInputType) return;


        }
        protected virtual void Start()
        {
            //if (chairButton != null)
            //{
            //    chairButton.onClick.AddListener(delegate
            //    {
            //        SetChairDirection(int.Parse(chairNumber.text), chairDirection.text[0], int.Parse(chairSpeed.text));
            //    });
            //}
            //if (bubblesButton != null)
            //{
            //    bubblesButton.onClick.AddListener(delegate
            //    {
            //        EnableFX(Consts.DodoFX.Bubbles, int.Parse(bubblesAngle.text), int.Parse(bubblesIntensity.text), float.Parse(bubblesHowLong.text));
            //    });
            //}
            //if (smokeButton != null)
            //{
            //    smokeButton.onClick.AddListener(delegate
            //    {
            //        EnableFX(Consts.DodoFX.Smoke, int.Parse(smokeAngle.text), int.Parse(smokeIntensity.text), float.Parse(smokeHowLong.text));
            //    });
            //}
            //if (windButton != null)
            //{
            //    windButton.onClick.AddListener(delegate
            //    {
            //        EnableFX(Consts.DodoFX.Wind, int.Parse(windAngle.text), int.Parse(windIntensity.text), float.Parse(windHowLong.text));
            //    });
            //}
            //if (thunderButton != null)
            //{
            //    thunderButton.onClick.AddListener(delegate
            //    {
            //        EnableFX(Consts.DodoFX.Thunder, int.Parse(thunderAngle.text), int.Parse(thunderIntensity.text), float.Parse(thunderHowLong.text));
            //    });
            //}



            InitUDP();
        }
        /// <summary>
        /// Sets the direction of movement and speed of a chair
        /// </summary>
        /// <param name="number">Number of the chair: 0-49</param>
        /// <param name="direction">'S' - Stop, 'L' - Left, 'R' - Right, must be uppercase</param>
        /// <param name="speed">0-99</param>
        public void SetChairDirection(int number, char direction, int speed)
        {
            dodoFX.SetChairDirection(number, direction, speed);
        }
        /// <summary>
        /// Enables a specified effect for certain time
        /// </summary>
        /// <param name="fxType">Type of effect</param>
        /// <param name="angle">Angle at which it is placed on the 360 screen</param>
        /// <param name="intensity">How strong to emit the effect, clamped to 0-255</param>
        /// <param name="howLong">Time after which the effect will be disabled, in seconds</param>
        public void EnableFX(Consts.DodoFX fxType, int angle, int intensity, float howLong)
        {
            if (howLong < 0f) { Debug.LogError("Negative time"); return; }
            intensity = Mathf.Clamp(intensity, 0, 255);
            StartCoroutine(FXCoroutine(fxType, angle, intensity, howLong));
        }
        /// <summary>
        /// Enables a specified effect
        /// </summary>
        /// <param name="fxType">Type of effect</param>
        /// <param name="number">Number of the FX device</param>
        /// <param name="intensity">How strong to emit the effect, clamped to 0-255</param>
        public void EnableFX(Consts.DodoFX fxType, int number, int intensity)
        {
            dodoFX.SetFXData(fxType, number, intensity, true);
        }
        /// <summary>
        /// Disables a specified effect
        /// </summary>
        /// <param name="fxType">Type of effect</param>
        /// <param name="number">Number of the FX device</param>
        /// <param name="intensity">How strong to emit the effect, clamped to 0-255</param>
        public void DisableFX(Consts.DodoFX fxType, int number)
        {
            dodoFX.SetFXData(fxType, number, 0, false);
        }
        IEnumerator FXCoroutine(Consts.DodoFX fxType, int angle, int intensity, float howLong)
        {
            FXEnableData[] fxEnableDatas = dodoFX.CalcFXEnableData(fxType, angle);
            foreach (FXEnableData fxEnableData in fxEnableDatas)
            {
                int newIntensity = Mathf.RoundToInt(fxEnableData.intensityMultiplier * (float)intensity);
                EnableFX(fxType, fxEnableData.index, newIntensity);
            }
            yield return new WaitForSeconds(howLong);
            foreach (FXEnableData fxEnableData in fxEnableDatas)
            {
                DisableFX(fxType, fxEnableData.index);
            }
        }

        byte[] data = null;
        string fx = null;
        protected void SendFXData(DodoFX _dodoFX)
        {
            try
            {
                fx = _dodoFX.CreateMessage();
                data = System.Text.Encoding.UTF8.GetBytes(fx);
                udpServer.Send(data, fxControllerEndPoint);
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }
        }
        #endregion
        protected List<Ping> pings = new List<Ping>();
        protected virtual void Update()
        {
            NotifyClients();
            HandleDisconnections();
            SendFXData(dodoFX);
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
            {
                Application.Quit();
            }
        }

        private void HandleDisconnections()
        {
            for (int i = 0; i < disconnectionCounters.Count; i++)
            {
                int counter = disconnectionCounters[i];
                counter = disconnectionCounters[i]++;
                if (counter >= maxNFramesToDisconnect)
                {
                    // Works only if clientID is incremented by 1 starting from 0. See GenerateClientID()
                    connectedControllers[i].playerInput.Zero();
                }
            }
        }
        protected PlayerInput GetPlayerInput(int clientID)
        {
            return connectedControllers[clientID].playerInput;
        }
        protected void SetPlayerInput(int clientID, PlayerInput playerInput)
        {
            connectedControllers[clientID].playerInput = playerInput;
            disconnectionCounters[clientID] = 0;
            if (printInput)
            {
                print("PlayerName: " + playerInput.clientData.name + ", MaxPing: " + playerInput.ping.max + ", MinPing: " + playerInput.ping.min + ",AvgPing: " + playerInput.ping.avg + ", ClientID: " + clientID + ", " + playerInput.leftStick.Horizontal + ", " + playerInput.leftStick.Vertical + ", " + playerInput.rightStick.Horizontal + ", " + playerInput.rightStick.Vertical + ", " + playerInput.directionalPad.Horizontal + ", " + playerInput.directionalPad.Vertical + ", " + playerInput.start + ", " + playerInput.back + ", " + playerInput.guide + ", " + playerInput.leftTrigger + ", " + playerInput.rightTrigger + ", " + playerInput.leftBumper + ", " + playerInput.rightBumper + ", " + playerInput.aButton + ", " + playerInput.bButton + ", " + playerInput.xButton + ", " + playerInput.yButton);
            }
        }
        private void NotifyClients()
        {
            if (controllersToSendEveryFrameUDP.Count != 0)
            {
                foreach (int i in controllersToSendEveryFrameUDP)
                {
                    if (connectedControllers.Count > i && connectedControllers.Count > i)
                    {
                        connectedControllers[i].SendOverUDP(connectedControllers[i].message);
                    }
                }
                /*
                 * Dawid Bobyla
                 * Poniżej optymalizacja pamieci, ale nie jestem pewien czy 100% odpowiada orginalnej konstrukcji
                for (int i = 0; i < connectedControllers.Count; i++)
                {
                    connectedControllers[i].SendOverUDP(connectedControllers[i].message);
                }
                */
            }
        }
        protected override void Awake()
        {
            // Singleton
            base.Awake();

            connectedControllers = new List<ConnectedController>();
            //serverName = System.Enum.GetName(typeof(Consts.Server), server);
            fxControllerEndPoint = new IPEndPoint(Consts.fxControllerIP, Consts.fxControllerPort);
        }
        protected virtual void InitTCP()
        {
            if (TCPProtocolServer.instance == null)
            {
                Debug.LogError("Missing component: TCPProtocolServer");
                return;
            }
            TCPProtocolServer.instance.Connect();
            tcpInitialized = true;
            tcpServer = TCPProtocolServer.instance;
            if (onTCPInitialized != null)
                onTCPInitialized();
        }
        protected virtual void InitUDP()
        {
            udpServer = UDPProtocolServer.instance;
            if (udpServer == null)
            {
                Debug.LogError("Missing component: UDPProtocolServer");
                return;
            }
            udpServer.Connect();
            if (idAssignment == NetworkIDAssignmentMethod.OverUDP)
            {
                //UDPProtocolServer.instance.BroadcastServer(serverName, game);
                udpServer.BroadcastServer(Consts.CreateMessage(new string[] { Consts.Words.serverName, Consts.Words.game }, new object[] { server, game }));
            }
            udpInitialized = true;


            if (onUDPInitialized != null)
                onUDPInitialized();
        }
        /// <summary>
        /// Invoked when a new UDP message arrives
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ip">IP where the message came from</param>
        public virtual void ReadUDP(string message, IPAddress ip)
        {

        }
        /// <summary>
        /// Invoked when a new TCP message arrives
        /// </summary>
        /// <param name="message">The message</param>        
        public virtual void ReadTCP(string message)
        {

        }
        ConnectedController CheckIfExists(IPAddress ipAddress)
        {
            foreach (ConnectedController cc in connectedControllers)
            {
                if (cc.IsTCP)
                {
                    var tcpIP = ((IPEndPoint)(cc.tcpConnectedClient.tcpClient.Client.RemoteEndPoint)).Address;

                    if (ipAddress.Equals(tcpIP))
                    {
                        return cc;
                    }
                }
                if (cc.IsUDP)
                {
                    var udpIP = cc.udpConnectedClient.ipEndPoint.Address;
                    if (ipAddress.Equals(udpIP))
                    {
                        return cc;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Adds a controller and generates an ID for it
        /// </summary>
        /// <returns></returns>
        public ConnectedController AddController()
        {
            int id = GenerateClientID();
            ConnectedController cc = new ConnectedController(id);
            AddController(cc);
            return cc;
        }
        void AddController(ConnectedController cc)
        {
            connectedControllers.Add(cc);
            int disconnectionCounter = 0;
            disconnectionCounters.Add(disconnectionCounter);
        }

        int GenerateClientID()
        {
            return clientID++;
        }

        ConnectedController AddController(UDPConnectedClient udpConnectedClient)
        {
            var udpIP = udpConnectedClient.ipEndPoint.Address;
            int id = GenerateClientID();
            pings.Add(new Ping(id));
            ConnectedController cc = CheckIfExists(udpIP);
            if (cc == null)
            {
                cc = new ConnectedController(udpIP, id);
                if (!ip_id_Dictionary.ContainsKey(udpIP))
                {
                    //ip_id_Dictionary.Add(udpIP, -1); // id will be set once the client knows it
                    ip_id_Dictionary.Add(udpIP, id); // id will be set once the client knows it
                }
                cc.AddUDP(udpConnectedClient);
                AddController(cc);
            }
            else if (cc != null && !cc.IsUDP)
            {// There exists a ConnectedController with a TCP(yes,TCP) connection from the same source
                cc.AddUDP(udpConnectedClient);
            }
            else { Debug.LogError("Such UDP connection already exists"); }

            return cc;
        }
        /// <summary>
        /// Adds a newly connected client as a ConnectedController
        /// </summary>
        /// <param name="udpConnectedClient">Client's UDPConnectedClient representing his UDP socket</param>
        /// <param name="firstUDPMessage">First udp message gotten from this client</param>
        public void AddController(UDPConnectedClient udpConnectedClient, string firstUDPMessage)
        {
            ConnectedController cc = AddController(udpConnectedClient);
            FirstUDPMessageReceived(cc, firstUDPMessage);

            //Send id to client over udp // nie musimy wysyłać id.
            //if (idAssignment == NetworkIDAssignmentMethod.OverUDP)
            //{
            //    SendIDOverUDP(cc);
            //}

            //Invoke event when a new client connects
            if(onNewClientConnected != null)
            {
                onNewClientConnected.Invoke(cc);
            }
        }
        /// <summary>
        /// In case when a client has disconnected during the game, resend him his old id instead of creating a new one
        /// </summary>
        /// <param name="ip"></param>
        public void ResendID(IPAddress ip)
        {
            int id;
            if (!ip_id_Dictionary.TryGetValue(ip, out id))
            {
                Debug.LogError("No such id");
                return;
            }
            SendIDOverUDP(connectedControllers[id]);
            // when reconnected reset counters (counter1 and counter2)
            connectedControllers[id].playerInput.counter1 = 0;
            connectedControllers[id].playerInput.counter2 = 0;
        }
        void SendIDOverUDP(ConnectedController cc)
        {
            int id = cc.playerInput.clientData.clientID;
            string message = Consts.CreateMessage(Consts.Words.id, id);//Consts.Words.id + Consts.Words.wordSeparator + cc.playerInput.clientData.clientID + Consts.Words.sequenceSeparator;
            cc.message = message;
            if (!controllersToSendEveryFrameUDP.Contains(id))
            {
                playerKnowsID[id] = false;
                controllersToSendEveryFrameUDP.Add(cc.playerInput.clientData.clientID);
            }
        }
        void SendIDOverTCP(ConnectedController cc)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Invoke when a certain client is aware of his assigned id. Stop sending it to him
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        protected void PlayerKnowsID(IPAddress ip, int id)
        {
            controllersToSendEveryFrameUDP.Remove(id);
        }
        /// <summary>
        /// override in parent class to extract serverID and send it to client
        /// </summary>
        /// <param name="cc">The client held by the server</param>
        /// <param name="msg">First udp message </param>
        protected virtual void FirstUDPMessageReceived(ConnectedController cc, string msg)
        {
        }
        //protected virtual void FirstTCPMessageReceived(ConnectedController cc, string msg)
        //{
        //}
        /// <summary>
        /// Adds a newly connected client as a ConnectedController
        /// </summary>
        /// <param name="tcpConnectedClient">Client's TCPConnectedClient representing his TCP socket</param>        
        public void AddController(TCPConnectedClient tcpConnectedClient)
        {
            var tcpIP = ((IPEndPoint)(tcpConnectedClient.tcpClient.Client.RemoteEndPoint)).Address;
            int id = GenerateClientID();
            ConnectedController cc = CheckIfExists(tcpIP);
            if (cc == null)
            {
                cc = new ConnectedController(tcpIP, id);
                cc.AddTCP(tcpConnectedClient);
                AddController(cc);
            }
            else if (cc != null && !cc.IsTCP)
            {// There exists a ConnectedController with a UDP(yes) connection from the same source
                cc.AddTCP(tcpConnectedClient);
            }
            else { Debug.LogError("Such TCP connection already exists"); return; }

            if (idAssignment == NetworkIDAssignmentMethod.OverTCP)
            {
                // not implemented
                SendIDOverTCP(cc);
            }
        }
        /// <summary>
        /// Removes client from the list based on his TCP socket
        /// </summary>
        /// <param name="_tcpConnectedClient"></param>
        public void RemoveController(TCPConnectedClient _tcpConnectedClient)
        {
            var cc = connectedControllers.Find(x => x.tcpConnectedClient == _tcpConnectedClient);
            cc.RemoveTCP();
            if (cc.IsUDP == false && cc.IsTCP == false)
                connectedControllers.Remove(cc);
        }
        /// <summary>
        /// Removes client from the list based on his UDP socket
        /// </summary>
        /// <param name="_udpConnectedClient"></param>
        public void RemoveController(UDPConnectedClient _udpConnectedClient)
        {
            var cc = connectedControllers.Find(x => x.udpConnectedClient == _udpConnectedClient);
            cc.RemoveUDP();
            if (cc.IsUDP == false && cc.IsTCP == false)
                connectedControllers.Remove(cc);
        }

    }
}