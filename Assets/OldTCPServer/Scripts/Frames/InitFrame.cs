using UnityEngine;

namespace GameRoomServer
{
    public class InitFrame : Frame
    {
        public float positionX;
        public float positionY;
        public float positionZ;
        public float targetRotation;
        //public AudioFile[] audioFiles;

        public InitFrame(Vector3 position, float targetRotation) : base(FrameType.Init)
        {
            positionX = position.x;
            positionY = position.y;
            positionZ = position.z;
            this.targetRotation = targetRotation;
        }

        public InitFrame(string[] dataArray) : base(FrameType.Init, dataArray)
        {
            float.TryParse(dataArray[startOffset + 1], out positionX);
            float.TryParse(dataArray[startOffset + 2], out positionY);
            float.TryParse(dataArray[startOffset + 3], out positionZ);
            float.TryParse(dataArray[startOffset + 4], out targetRotation);
        }

        public override string ToString()
        {
            string s = base.ToString() + ";" + positionX.ToString("0.0000") + ";" + positionY.ToString("0.0000") + ";" + positionZ.ToString("0.0000") + ";" + targetRotation.ToString("0.0000");
            return s + ";";
        }
    }
}
