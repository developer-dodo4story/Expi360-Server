using System.Globalization;

namespace GameRoomServer
{
    public class CommandRotationFrame : Frame
    {
        public float targetRotation;

        public CommandRotationFrame(float actualRotation) : base(FrameType.CommandFrame)
        {
            this.targetRotation = actualRotation;
        }

        public CommandRotationFrame(string[] dataArray) : base(FrameType.CommandFrame, dataArray)
        {
            float.TryParse(dataArray[startOffset + 1], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out targetRotation);
        }

        public override string ToString()
        {
            string s = base.ToString() + ";" + targetRotation;
            return s + ";";
        }
    }
}
