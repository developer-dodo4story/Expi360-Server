using System;
using UnityEngine;

namespace GameRoomServer
{
    public class DataFrame : Frame
    {
        public string status;
        public int speed;
        public ChairInput input;

        public DataFrame(string[] dataArray) : base(FrameType.DataFrame, dataArray)
        {
            int index = 1;

            status = dataArray[startOffset + index++]; // We skip the Frame Type and connection id from input string, so we begin with chair status.
            int.TryParse(dataArray[startOffset + index++], out speed);
            int inputLength = dataArray.Length - 1 - ++index;
            string[] inputArray = new string[inputLength];
            Array.Copy(dataArray, index, inputArray, 0, inputLength);
            input = new ChairInput(inputArray);
        }

        public DataFrame(string status, int speed, ChairInput input) : base(FrameType.DataFrame)
        {
            this.status = status;
            this.speed = speed;
            this.input = input;
        }

        public override string ToString()
        {
            return base.ToString() + ';' +
                status + ';' +
                speed + ';' +
                input.ToString() + ';';
        }
    }
}
