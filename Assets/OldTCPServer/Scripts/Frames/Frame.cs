using System.Net.NetworkInformation;

namespace GameRoomServer {
    public abstract class Frame
    {
        public enum FrameType { FirstHello = 0, FirstHelloResponse = 1, Init = 2, CommandFrame = 3, DataFrame = 4, StatusFrame = 5, Disconnected = 6, Ping = 7, Speed = 8, PadVib = 9 }

        public FrameType frameType;
        public int startOffset = 1;
        public int id;

        private static PhysicalAddress macAddres = null;

        public Frame(FrameType type)
        {
            frameType = type;
        }

        public Frame(FrameType type, string[] dataArray)
        {
            frameType = type;
            int.TryParse(dataArray[1], out id);
        }

        public override string ToString()
        {
            return "@" + ((int)frameType).ToString() + ";" + id;
        }

        /// <summary>
        /// Gets the MAC address of the current PC.
        /// </summary>
        /// <returns></returns>
        public PhysicalAddress GetMacAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddres = nic.GetPhysicalAddress();
                    return nic.GetPhysicalAddress();
                }
            }
            if (macAddres == null)
            {
                byte[] errorMac = System.Text.Encoding.UTF8.GetBytes("ErrorMAC:" + GetHashCode());
                macAddres = new PhysicalAddress(errorMac);
            }
            return macAddres;
        }
    }
}
