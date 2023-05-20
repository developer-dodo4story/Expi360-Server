using System.Globalization;

namespace GameRoomServer
{
    public class PadVibration : Frame
    {
        public int Gain;
        public int Time; // miliseconds

        public PadVibration(int gain, int time) : base(FrameType.PadVib)
        {
            this.Gain = gain;
            this.Time = time;
        }

        public PadVibration(string[] dataArray) : base(FrameType.PadVib, dataArray)
        {
            int.TryParse(dataArray[startOffset + 1], out Gain);
            int.TryParse(dataArray[startOffset + 2], out Time);
        }

        public override string ToString()
        {
            string s = base.ToString() + ";" + Gain + ";" + Time;
            return s + ";";
        }
    }
}
