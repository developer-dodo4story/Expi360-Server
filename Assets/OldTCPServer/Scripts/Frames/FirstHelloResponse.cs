namespace GameRoomServer
{
    public class FirstHelloResponse : Frame
    {
        public string macAddress;

        public FirstHelloResponse(int id, string macAddress) : base(FrameType.FirstHelloResponse)
        {
            this.id = id;
            this.macAddress = macAddress;
        }

        public FirstHelloResponse(string[] dataArray) : base(FrameType.FirstHelloResponse, dataArray)
        {
            macAddress = dataArray[startOffset + 1];
        }

        public override string ToString()
        {
            string s = base.ToString() + ";" + macAddress;
            return s + ";";
        }
    }
}
