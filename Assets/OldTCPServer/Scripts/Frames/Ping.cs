namespace GameRoomServer
{
    public class Ping : Frame
    {
        public Ping(int id) : base(FrameType.Ping)
        {
            this.id = id;
        }

        public Ping(string[] dataArray) : base(FrameType.Ping, dataArray)
        {

        }
    }
}
