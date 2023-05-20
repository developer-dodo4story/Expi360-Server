using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

namespace GameRoomServer
{
    [RequireComponent(typeof(TaskExecutorScript))]
    public class TCPServer : MonoBehaviour
    {
        #region Data
        public static TCPServer instance;
        public TaskExecutorScript taskExecutor;

        private DataChairs dataChairs;

        public DataChairs DataChairs
        {
            get
            {
                return dataChairs;
            }
        }

        public List<TCPConnectedChair> clientList = new List<TCPConnectedChair>();
        public List<TCPConnectedChair> newClientList = new List<TCPConnectedChair>();
        Dictionary<int, TCPConnectedChair> clientDictionary = new Dictionary<int, TCPConnectedChair>();
        [SerializeField]
        private int framesNeededForClientLoss = 10;

        TcpListener listener;
        #endregion
        public bool debug = false;
        private bool isUsingServer;
        private Canvas canvas;
        [SerializeField] private GameObject debugPrefab;
        private Text debugWindow;
        [SerializeField] private int debugColumns = 3;

        #region Unity Events
        private void Awake()
        {
            if (TCPServer.instance == null)
            {
                instance = this;
            } else if (TCPServer.instance != this)
            {
                Destroy(this.gameObject);

                return;
            }

            Application.runInBackground = true;
        }

        private void Start()
        {
            StartTCP();
            Cursor.visible = false;
            DontDestroyOnLoad(this.gameObject);

            if (debug)
            {
                InitDebug();
            }
        }

        public void StartTCP()
        {
            LoadDataChairs();
            listener = new TcpListener(IPAddress.Any, Globals.PORT);
            listener.Start();
            listener.BeginAcceptTcpClient(OnServerConnect, null);
            isUsingServer = true;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.I))
            {
                SwitchInputType();
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
            {
                Application.Quit();
            }

            if (!isUsingServer)
            {
                HandleLocalInput();
            }
        }

        private void LateUpdate()
        {
            if (isUsingServer)
            {
                HandleServer();
            }
        }

        private void SwitchInputType()
        {
            isUsingServer = !isUsingServer;

            if (!isUsingServer)
            {
                foreach (TCPConnectedChair chair in clientList)
                {
                    chair.Close();
                }

                if (listener != null)
                {
                    listener.Stop();
                }

                clientList = new List<TCPConnectedChair>();
                string[] joystickNames = Input.GetJoystickNames();
                Debug.Log("JOYSTICKS: " + joystickNames.Length);


                for (int i = 0; i < joystickNames.Length; i++)
                {
                    if (joystickNames[i] != "")
                    {
                        clientList.Add(new TCPConnectedChair(null, taskExecutor));
                    }
                }
            }
            else
            {
                StartTCP();
            }
        }

        private void HandleServer()
        {
            HandleConnections();

            foreach (TCPConnectedChair chair in clientList)
            {
                if (chair.disconnectedCounter > framesNeededForClientLoss)
                {
                    Debug.Log("Client lost " + chair.id); // TODO: Add disconnected event.
                    chair.chairInput.ResetInput();
                    chair.connected = false;
                }

                chair.disconnectedCounter++;
            }

            HandleDebug();
        }

        private void HandleLocalInput()
        {
            int index = 1;
            float horizontal = 0;
            float vertical = 0;

            foreach (TCPConnectedChair chair in clientList)
            {
                try
                {
                    horizontal = Input.GetAxis("Horizontal" + index.ToString());
                    vertical = -1f * Input.GetAxis("Vertical" + index.ToString());
                }
                catch
                {
                }

                chair.chairInput = new ChairInput(
                    horizontal,
                    vertical,
                    0,
                    0,
                    0,
                    0,
                    Input.GetKey("joystick " + index.ToString() + " button 0"),
                    Input.GetKey("joystick " + index.ToString() + " button 1"),
                    Input.GetKey("joystick " + index.ToString() + " button 2"),
                    Input.GetKey("joystick " + index.ToString() + " button 3"),
                    false,
                    false,
                    false,
                    Input.GetKey("joystick " + index.ToString() + " button 7"),
                    false,
                    false,
                    false,
                    false,
                    false,
                    false
                );
                index++;
            }
        }

        private void HandleConnections()
        {
            if (TCPServer.instance.newClientList.Count > 0 && TCPServer.instance.newClientList[0].macAddresReady)
            {
                string macAddress = TCPServer.instance.newClientList[0].macAddress;
                //Debug.Log(macAddress + "   " + TCPServer.instance.clientList.Count);
                for (int i = 0; i < TCPServer.instance.clientList.Count; i++)
                {
                    //Debug.Log("clientlist mac " + TCPServer.instance.clientList[i].macAddress);
                    if (TCPServer.instance.clientList[i].macAddress == macAddress)
                    {
                        Debug.Log("Mac taki sam, usun");
                        TCPServer.instance.clientList.RemoveAt(i);
                        //TCPServer.instance.clientList.Add(TCPServer.instance.newClientList[0]);
                        TCPServer.instance.clientList.Insert(i, TCPServer.instance.newClientList[0]);
                        TCPServer.instance.newClientList.RemoveAt(0);

                        return;
                    }
                }
                //Debug.Log("Nie odnaloeziono macc, dodaj nowy");
                TCPServer.instance.clientList.Add(TCPServer.instance.newClientList[0]);
                TCPServer.instance.newClientList.RemoveAt(0);
                //Debug.Log("Ile klientow: " + TCPServer.instance.clientList.Count);
            }
        }

        private void LoadDataChairs()
        {
            SerializeHelper.LoadFromJSON("ChairsData1", SerializeHelper.pathToProject + "/Data/Chairs", ref dataChairs);
        }

        protected void OnApplicationQuit()
        {
            if (listener != null)
            {
                listener.Stop();
            }

            for (int i = 0; i < clientList.Count; i++)
            {
                clientList[i].Close();
            }
        }
        #endregion

        #region Async Events
        void OnServerConnect(IAsyncResult ar)
        {
            TcpClient tcpClient = listener.EndAcceptTcpClient(ar);
            TCPConnectedChair client = new TCPConnectedChair(tcpClient, taskExecutor);
            //clientList.Add(client);
            newClientList.Add(client);
            IntroduceYourself(client);
            listener.BeginAcceptTcpClient(OnServerConnect, null);
        }
        #endregion

        #region API
        public void OnDisconnect(TCPConnectedChair client)
        {
            // TODO: Add disconnected event.
            clientList.Remove(client);
        }

        private void IntroduceYourself(TCPConnectedChair client)
        {
            int idHash = client.GetHashCode();

            if (idHash == 0)
            {
                idHash = 9898989;
            }

            client.id = idHash;
            clientDictionary.Add(idHash, client);
            Frame firstHello = new FirstHello(idHash);
            string firstHelloMessage = firstHello.ToString();
            client.Send(firstHelloMessage);
        }

        internal void Send(string message)
        {
            BroadcastChatMessage(message);
        }

        public void SendBroadcast(CommandRotationFrame data)
        {
            if (clientList.Count > 0)
            {
                int count = instance.clientList.Count;

                for (int i = 0; i < count; i++)
                {
                    instance.clientList[i].SendDataFrame(data);
                }
            }
        }

        public void Send(int id, string message)
        {
            TCPConnectedChair client = null;
            bool exist = clientDictionary.TryGetValue(id, out client);

            if (exist)
            {
                client.Send(message);
            }
            else
            {
                Debug.Log("Brak clienta o podanym id: " + id);
            }
        }

        internal static void BroadcastChatMessage(string message)
        {
            for (int i = 0; i < instance.clientList.Count; i++)
            {
                TCPConnectedChair client = instance.clientList[i];
                client.Send(message);
            }
        }
        #endregion

        private void HandleDebug()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                debug = !debug;

                if (canvas == null && debug)
                {
                    InitDebug();
                }

                if (!debug && canvas != null)
                {
                    debugWindow.text = "";
                }
            }

            if (debug)
            {
                PrintDebug();
            }
        }

        private void InitDebug()
        {
            canvas = GameObject.FindObjectOfType<Canvas>();

            if (canvas == null)
            {
                return;
            }

            GameObject debugObject = Instantiate(debugPrefab, canvas.transform);
            debugWindow = debugObject.GetComponent<Text>();
        }

        private void PrintDebug()
        {
            if (canvas == null)
            {
                return;
            }

            debugWindow.text = clientList.Count + " clients connected.\n\n";
            int counter = 0;

            foreach(TCPConnectedChair client in clientList)
            {
                debugWindow.text += "Client IP: " + (client.ipaddr != null ? client.ipaddr : "N/A");
                debugWindow.text += "  Ping: " + client.ping + 
                    "  LeftStickXAxis: " + (client.chairInput != null ? client.chairInput.leftStickXAxis.ToString() : "N/A") + " ### ";

                counter++;

                if (counter % debugColumns == 0)
                {
                    debugWindow.text += "\n\n";
                }
            }
        }
    }
}
