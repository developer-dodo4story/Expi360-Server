using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using EnhancedDodoServer;
using System;
using System.Text;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Holds all constant values and utility methods   
    /// </summary>
    public class Consts
    {
        /// <summary>
        /// Enum representing a game
        /// </summary>
        public enum Game { BellyRumble, PotionsMaster, CityUnderAttack, Empatki, FieldsOfGlory, Submarine, Empty, Gamepad}
        /// <summary>
        /// Enum representing a server
        /// </summary>
        public enum Server { Room1, Room2, Room3, Room4, Room5 }
        /// <summary>
        /// Enum representing type of an effect
        /// </summary>
        public enum DodoFX { Bubbles, Smoke, Wind, Thunder}
        /// <summary>
        /// PhonePad - build na telefony
        /// XboxPad - build na xboxPady
        /// </summary>
        public enum InputType { PhonePad, XboxPad}
        /// <summary>
        /// Local class holding words sent in messages
        /// </summary>
        public static class Words
        {
            /// <summary>
            /// char separating consequent sentences
            /// </summary>
            public const char sequenceSeparator = '|';
            /// <summary>
            /// char separating consequent words
            /// </summary>
            public const char wordSeparator = ':';
            /// <summary>
            /// string indicating an ID
            /// </summary>
            public const string id = "ID";
            /// <summary>
            /// string indicating an IP request from a client
            /// </summary>
            public const string serverIPRequest = "ServerIPRequest";
            /// <summary>
            /// string indicating a server response to the ip request
            /// </summary>
            public const string serverIPResponse = "ServerIPResponse";
            /// <summary>
            /// string indicating a client name
            /// </summary>
            public const string clientName = "ClientName";
            /// <summary>
            /// string indicating a server name
            /// </summary>
            public const string serverName = "ServerName";
            /// <summary>
            /// string indicating a game name
            /// </summary>
            public const string game = "Game";
        }
        /// <summary>
        /// The port on which the server listens
        /// </summary>
        public const int serverPort = 56789;
        /// <summary>
        /// The port on which clients listen
        /// </summary>
        public const int clientPort = 5789;
        /// <summary>
        /// Byte size of a tcp buffer
        /// </summary>
        public const int tcpBufferSize = 5000;
        public static IPAddress fxControllerIP = IPAddress.Parse("127.0.0.1");
        public const int fxControllerPort = 11003;
        /// <summary>
        /// Creates a string message to be sent
        /// </summary>
        /// <param name="name">Name of the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns></returns>
        public static string CreateMessage(string name, object value)
        {
            sb1.Length = 0; // czysci stringbuilder
            //sb1 = new StringBuilder();
            sb1.AppendFormat("{0}{1}{2}{3}", name, Words.wordSeparator, value.ToString(), Words.sequenceSeparator);
            return sb1.ToString();
            //return name + Words.wordSeparator + value.ToString() + Words.sequenceSeparator;
        }

        // Stringbuilder do optymalizacji stringow
        private static StringBuilder sb1 = new StringBuilder();
        private static StringBuilder sb2 = new StringBuilder();

        /// <summary>
        /// Creates a string message from multiple variables
        /// </summary>
        /// <param name="names">Table of names</param>
        /// <param name="values">Table of values</param>
        /// <returns></returns>
        public static string CreateMessage(string[] names, object[] values)
        {
            //string message = "";
            sb2.Length = 0;
            //sb2 = new StringBuilder();
            for (int i=0; i<names.Length; i++)
            {
                //message += CreateMessage(names[i], values[i]);
                sb2.AppendFormat("{0}", CreateMessage(names[i], values[i]));
            }
            return sb2.ToString();
            //return message;
        }       
        /// <summary>
        /// Splits a message to sequences containing a name and a value of variable
        /// </summary>
        /// <param name="message">Received message</param>
        /// <returns>Table of sequences</returns>
        public static string[] SequenceSplit(string message)
        {
            return message.Split(Consts.Words.sequenceSeparator);
        }
        /// <summary>
        /// Splits a sequence to words 
        /// </summary>
        /// <param name="sequence">A sequence received by SequenceSplit</param>
        /// <returns>A string word</returns>
        public static string[] WordSplit(string sequence)
        {
            return sequence.Split(Consts.Words.wordSeparator);
        }
    }
}