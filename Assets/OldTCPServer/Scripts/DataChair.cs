using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameRoomServer
{
    [Serializable]
    public class DataChair
    {
        private int id = -1;
        public string macAddres;
        public Vector3 position;
        public Vector3 rotation;
        public bool connected;
        public bool working;

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public DataChair(string macAddres, Vector3 position, Vector3 rotation, bool connected, bool working)
        {
            this.macAddres = macAddres;
            this.position = position;
            this.rotation = rotation;
            this.connected = connected;
            this.working = working;
        }

        /// <summary>
        /// Use only to error case, all fields are blank.
        /// </summary>
        public DataChair()
        {
            
        }

        public override string ToString()
        {
            return "MacAddres: " + macAddres + ", Position: " + position + ", Rotation: " + rotation + ", Connected: " + connected + ", Working: " + working;
        }
    }
}
