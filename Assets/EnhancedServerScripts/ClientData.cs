using System.Net;
using EnhancedDodoServer;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Class holding client specific data
    /// </summary>
    [System.Serializable]
    public class ClientData
    {
        public ClientData()
        {
            clientID = -1;
            name = "Player";
            ip = null;
        }
        /// <summary>
        /// ID of the client on the server
        /// </summary>
        public int clientID;
        /// <summary>
        /// Name of the client 
        /// </summary>
        public string name;
        /// <summary>
        /// Client's ip
        /// </summary>
        public IPAddress ip;        
    }

}