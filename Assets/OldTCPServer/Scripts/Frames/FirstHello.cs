namespace GameRoomServer
{
    public class FirstHello : Frame
    {
        public FirstHello(int id) : base(FrameType.FirstHello)
        {
            this.id = id;
        }

        public FirstHello(string[] dataArray) : base(FrameType.FirstHello, dataArray)
        {

        }

        public override string ToString()
        {
            string s = base.ToString();
            return s + ";";
        }
    }
}
