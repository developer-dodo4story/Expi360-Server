namespace GameRoomServer
{
    public class DisconnectedFrame : Frame
    {
        public DisconnectedFrame(int id) : base(FrameType.Disconnected)
        {
            this.id = id;
        }

        public DisconnectedFrame(string[] dataArray) : base(FrameType.Disconnected, dataArray)
        {

        }

        public override string ToString()
        {
            string s = base.ToString();
            return s + ";";
        }
    }
}
