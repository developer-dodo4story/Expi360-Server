using System.Net;
using UnityEngine;
using EnhancedDodoServer;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Holds player input on both client and server side
    /// </summary>
    [System.Serializable]
    public class PlayerInput
    {
        public PlayerInput(string name) : this()
        {
            clientData.name = name;
        }
        /// <summary>
        /// Public constructor creates new PlayerInput, initializes some values
        /// </summary>
        public PlayerInput()
        {
            counter1 = 0;
            counter2 = 0;

            // testy
            //counter1 = int.MaxValue - 100;
            //counter2 = 0;
            //end

            clientData = new ClientData();
            //clientData.clientID = -1;            
            leftStick = new Stick();
            rightStick = new Stick();
            directionalPad = new Stick();
            ping = new Ping();
        }        
        /// <summary>
        /// Local class holding two values, for both axes of a pad stick
        /// </summary>
        [System.Serializable]
        public class Stick
        {
            /// <summary>
            /// Horizontal axis
            /// </summary>
            public float Horizontal;
            /// <summary>
            /// Vertical axis
            /// </summary>
            public float Vertical;
            public void Zero()
            {
                Horizontal = 0f;
                Vertical = 0f;
            }
        }
        /// <summary>
        /// Holds information about client's ping
        /// </summary>
        public Ping ping;
        /// <summary>
        /// Holds client specific data
        /// </summary>
        public ClientData clientData;
        /// <summary>
        /// Message counter, used to guarantee order of message delivery
        /// </summary>        
        public int counter1;        
        /// <summary>
        /// Message counter, used to guarantee order of message delivery
        /// </summary>        
        public int counter2;
        /// <summary>
        /// Left stick of the pad
        /// </summary>
        public Stick leftStick;
        /// <summary>
        /// Right stick of the pad
        /// </summary>
        public Stick rightStick;
        /// <summary>
        /// Directional pad of the stick
        /// </summary>
        public Stick directionalPad;        
        public bool aButton;
        public bool bButton;
        public bool xButton;
        public bool yButton;
        public bool start;
        public bool back;
        public bool guide;
        public bool turbo;//nie ma tego na XboxPadzie, ale jest na naszych
        public float leftTrigger;
        public float rightTrigger;
        public bool leftBumper;
        public bool rightBumper;
        public void Zero()
        {
            leftStick.Zero();
            rightStick.Zero();
            directionalPad.Zero();
            aButton = false;
            bButton = false;
            xButton = false;
            yButton = false;
            start = false;
            back = false;
            guide = false;
            turbo = false;
            leftTrigger = 0f;
            rightTrigger = 0f;
            leftBumper = false;
            rightBumper = false;
            counter1 = 0;
            counter2 = 0;
        }
    }
}