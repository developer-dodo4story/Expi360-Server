namespace GameRoomServer
{
    public class SpeedFrame : Frame
    {
        public int speed;

        public SpeedFrame(int sp) : base(FrameType.Speed)
        {
            this.speed = sp;
        }

        public SpeedFrame(string[] dataArray) : base(FrameType.Speed, dataArray)
        {
            int.TryParse(dataArray[startOffset + 1], out speed);
        }

        public override string ToString()
        {
            string s = base.ToString() + ";" + speed;
            return s + ";";
        }
    }
}
