using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace GameRoomServer
{
    // Client class for server app.
    public class TCPConnectedChair
    {
        public long ping = -1;
        private List<long> pings = new List<long>();
        private Ping pingData = new Ping(0);
        private readonly bool isPing = true;
        public int disconnectedCounter = 0;

        #region Data
        public bool connected = false;
        public bool statusReady = false;
        public bool macAddresReady = false;
        public string macAddress;
        public int id;
        public string ipaddr;

        public string chairStatus; // TODO: Need an enum for that.
        public float chairSpeed;
        public ChairInput chairInput;

        /// <summary>
        /// For Clients, the connection to the server.
        /// For Servers, the connection to a client.
        /// </summary>
        public readonly TcpClient connection;

        private Stopwatch stopwatch = new Stopwatch();

        readonly TaskExecutorScript taskExecutor;

        readonly byte[] readBuffer = new byte[5000];

        NetworkStream stream
        {
            get
            {
                return connection.GetStream();
            }
        }

        #endregion

        #region Init
        public TCPConnectedChair(TcpClient tcpClient, TaskExecutorScript taskExecutor)
        {
            if (tcpClient == null)
            {
                return;
            }

            this.connected = true;
            this.taskExecutor = taskExecutor;
            this.connection = tcpClient;
            this.connection.NoDelay = true; // Disable Nagle's cache algorithm
            statusReady = false;
            stopwatch.Start();

            stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
        }

        internal void Close()
        {
            connected = false;

            if (connection != null)
            {
                connection.Close();
            }
        }
        #endregion

        #region PublicAPI
        /// <summary>
        /// Sets chair's rotation speed.
        /// </summary>
        /// <param name="rotSpeed">Value between (-4095, 4095)</param>
        public void SetRotationSpeed(int rotSpeed)
        {
            if (!connected)
            {
                return;
            }

            //DataFrame dFrame = new DataFrame(chairStatus, rotSpeed, chairInput);
            SpeedFrame speedFrame = new SpeedFrame(rotSpeed);
            Send(speedFrame.ToString());
        }

        /// <summary>
        /// Sets pad vibration.
        /// </summary>
        /// <param name="gain">Int between (0,100)</param>
        /// <param name="time">Time in miliseconds as int</param>
        public void SetPadVibration(int gain, int time)
        {
            if (!connected)
            {
                return;
            }

            PadVibration padVibFrame = new PadVibration(gain, time);
            Send(padVibFrame.ToString());
        }
        #endregion

        void OnRead(IAsyncResult ar)
        {
            disconnectedCounter = 0;
            connected = true;
            int length = stream.EndRead(ar);

            if (length <= 0)
            { // Connection closed
                TCPServer.instance.OnDisconnect(this);
                return;
            }

            string newMessage = System.Text.Encoding.UTF8.GetString(readBuffer, 0, length);

            HandleMessage(newMessage);

            stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);

        }

        private readonly char[] splitChars = { '@' };
        private void HandleMessage(string newMessage)
        {
            string[] dataBufferArray = newMessage.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            int bufferLength = dataBufferArray.Length;

            //taskExecutor.ScheduleTask(new Task(delegate
            //{
            //    UnityEngine.Debug.Log("Task splited: " + newMessage + " Ping: " + ping);
            //}));

            for (int i = 0; i < bufferLength; i++)
            {
                string msg = dataBufferArray[i];
                ParseFrameServer(msg);
            }
        }

        private void ParseFrameServer(string frameData)
        {
            string[] dataArray = frameData.Split(';');
            int enumValue = -1;
            bool isValid = int.TryParse(dataArray[0], out enumValue);
            if (isValid == true)
            {
                //Frame.FrameType frameType = (Frame.FrameType)Enum.Parse(typeof(Frame.FrameType), dataAray[0]);
                Frame.FrameType frameType = (Frame.FrameType)enumValue;
                switch (frameType)
                {
                    case Frame.FrameType.FirstHello:
                        // server nie odbiera tego komunikatu
                        // tylko go wysyła
                        break;
                    case Frame.FrameType.FirstHelloResponse:
                        HandleFirstHelloResponseServer(dataArray);
                        break;
                    case Frame.FrameType.Init:
                        // server nie odbiera tego komunikatu
                        // tylko go wysyła
                        break;
                    case Frame.FrameType.CommandFrame:
                        // server nie odbiera tego komunikatu
                        // tylko go wysyła
                        break;
                    case Frame.FrameType.DataFrame:
                        HandleDataFrameServer(dataArray);
                        HandlePingServer();
                        break;
                    case Frame.FrameType.Disconnected:
                        HandleDisconnectedServer(dataArray);
                        break;
                    case Frame.FrameType.Ping:
                        HandlePingServer();
                        break;
                }
            }
        }

        private void HandlePing(long ping)
        {
            if (pings.Count > 100)
            {
                pings.Clear();
                pings.Add(ping);
                this.ping = ping;
            }
            else
            {
                pings.Add(ping);
                long sum = 0;
                for (int i = 0; i < pings.Count; i++)
                {
                    sum += pings[i];
                }
                this.ping = sum / pings.Count;
            }
        }

        #region ServerHandlers
        private void HandlePingServer()
        {
            if (isPing)
            {
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                    HandlePing(stopwatch.ElapsedMilliseconds);
                }

                var aa = (IPEndPoint)(connection.Client.RemoteEndPoint);
                ipaddr = aa.Address.ToString();
                stopwatch.Reset();
                stopwatch.Start();
                pingData.id = id;
                //Send(pingData.ToString());
            }
        }

        private void HandleFirstHelloResponseServer(string[] dataArray)
        {
            FirstHelloResponse firstHelloResponse = new FirstHelloResponse(dataArray);
            // indentyfikacja
            int id = firstHelloResponse.id;
            string macAddress = firstHelloResponse.macAddress;
            this.macAddress = macAddress;
            this.id = id;
            macAddresReady = true;

            //Server wysyła pierwszego pinga
            //if (isPing)
            //{
            //    pingData.id = id;
            //    Send(pingData.ToString());
            //}

            // thread Unity
            taskExecutor.ScheduleTask(new Task(delegate
            {
                UnityEngine.Debug.Log("Przypisuję " + id + " " + macAddress);
                DataChair chair = TCPServer.instance.DataChairs.GetChairByMacAddres(macAddress);
                chair.Id = id;
                InitFrame initFrame = new InitFrame(chair.position, chair.rotation.y);
                TCPServer.instance.Send(id, initFrame.ToString());
            }));
        }

        private void HandleDataFrameServer(string[] dataArray)
        {
            DataFrame data = new DataFrame(dataArray);
            chairStatus = data.status;
            chairSpeed = data.speed;
            chairInput = data.input;
        }

        private void HandleDisconnectedServer(string[] dataArray)
        {
            DisconnectedFrame data = new DisconnectedFrame(dataArray);
            taskExecutor.ScheduleTask(new Task(delegate
            {
                UnityEngine.Debug.Log("Disconected id: " + data.id);
                //TCPServer.instance.clientList.Remove(this);
                TCPServer.instance.OnDisconnect(this); // Imo above line can be replace with this one. Correct me if I'm wrong.
            }));
        }
        #endregion

        #region API
        internal void Send(string message)
        {
            if (connected)
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
                try
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                catch (IOException e)
                {
                    chairInput.ResetInput();
                    connected = false;
                }
                catch (SocketException e)
                {
                    connected = false;
                }
            }
        }

        internal void SendDataFrame(DataFrame data)
        {
            if (connected) // TODO: There are problems with 'connected'.
            {
                data.id = id;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data.ToString());
                try
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                catch (IOException e)
                {
                    connected = false;
                }
                catch (SocketException e)
                {
                    connected = false;
                }
            }
        }

        internal void SendDataFrame(CommandRotationFrame data)
        {
            // from serwer
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data.ToString());
            try
            {
                stream.Write(buffer, 0, buffer.Length);
            }
            catch (IOException e)
            {
                connected = false;
            }
            catch (SocketException e)
            {
                connected = false;
            }
        }

        internal void SendDataFrame(DisconnectedFrame data)
        {
            if (connected)
            {
                data.id = id;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data.ToString());
                try
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                catch (IOException e)
                {
                    connected = false;
                }
                catch (SocketException e)
                {
                    connected = false;
                }
            }
        }

        internal void SendDataFrame(Frame data)
        {
            if (connected)
            {
                data.id = id;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data.ToString());
                try
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                catch (IOException e)
                {
                    connected = false;
                }
                catch (SocketException e)
                {
                    connected = false;
                }
            }
        }
        #endregion
    }
}
