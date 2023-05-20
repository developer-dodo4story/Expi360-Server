using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameRoomServer
{
    [Serializable]
    public class DataChairs
    {
        public List<DataChair> chairs = new List<DataChair>();

        public DataChair GetChairByMacAddres(string macAddres)
        {
            foreach (DataChair chair in chairs)
            {
                if (chair.macAddres.Equals(macAddres))
                {
                    return chair;
                }
            }
            // Error nie znaleziono obiektu na liscie
            return new DataChair();
        }
    }
}
