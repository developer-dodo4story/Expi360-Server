using System;
using System.Globalization;

namespace GameRoomServer
{
    [Serializable]
    public class ChairInput
    {
        #region Axis Controls
        public float leftStickXAxis;
        public float leftStickYAxis;
        public float rightStickXAxis;
        public float rightStickYAxis;
        public float leftTrigger;
        public float rightTrigger;
        #endregion

        #region Buttons
        public bool AButton;
        public bool BButton;
        public bool XButton;
        public bool YButton;
        public bool leftBumper;
        public bool rightBumper;
        public bool backButton;
        public bool startButton;
        public bool leftStickClick;
        public bool rightStickClick;
        public bool dPadUp;
        public bool dPadDown;
        public bool dPadLeft;
        public bool dPadRight;
        #endregion

        public ChairInput(
            float leftStickXAxis,
            float leftStickYAxis,
            float rightStickXAxis,
            float rightStickYAxis,
            float leftTrigger,
            float rightTrigger,
            bool aButton,
            bool bButton,
            bool xButton,
            bool yButton,
            bool leftBumper,
            bool rightBumper,
            bool backButton,
            bool startButton,
            bool leftStickClick,
            bool rightStickClick,
            bool dPadUp,
            bool dPadDown,
            bool dPadLeft,
            bool dPadRight)
        {
            this.leftStickXAxis = leftStickXAxis;
            this.leftStickYAxis = leftStickYAxis;
            this.rightStickXAxis = rightStickXAxis;
            this.rightStickYAxis = rightStickYAxis;
            this.leftTrigger = leftTrigger;
            this.rightTrigger = rightTrigger;
            AButton = aButton;
            BButton = bButton;
            XButton = xButton;
            YButton = yButton;
            this.leftBumper = leftBumper;
            this.rightBumper = rightBumper;
            this.backButton = backButton;
            this.startButton = startButton;
            this.leftStickClick = leftStickClick;
            this.rightStickClick = rightStickClick;
            this.dPadUp = dPadUp;
            this.dPadDown = dPadDown;
            this.dPadLeft = dPadLeft;
            this.dPadRight = dPadRight;
        }

        public void ResetInput()
        {
            leftStickXAxis = 0f;
            leftStickYAxis = 0f;
            rightStickXAxis = 0f;
            rightStickYAxis = 0f;
            leftTrigger = 0f;
            rightTrigger = 0f;

            AButton = false;
            BButton = false;
            XButton = false;
            YButton = false;
            leftBumper = false;
            rightBumper = false;
            backButton = false;
            startButton = false;
            leftStickClick = false;
            rightStickClick = false;
            dPadUp = false;
            dPadDown = false;
            dPadLeft = false;
            dPadRight = false;
        }

        public ChairInput(string[] arr)
        {
            int index = 0;

            float.TryParse(arr[index++], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out leftStickXAxis);
            float.TryParse(arr[index++], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out leftStickYAxis);
            float.TryParse(arr[index++], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out rightStickXAxis);
            float.TryParse(arr[index++], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out rightStickYAxis);
            float.TryParse(arr[index++], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out leftTrigger);
            float.TryParse(arr[index++], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out rightTrigger);

            AButton = ParseBool(arr[index++]);
            BButton = ParseBool(arr[index++]);
            XButton = ParseBool(arr[index++]);
            YButton = ParseBool(arr[index++]);
            leftBumper = ParseBool(arr[index++]);
            rightBumper = ParseBool(arr[index++]);
            backButton = ParseBool(arr[index++]);
            startButton = ParseBool(arr[index++]);
            leftStickClick = ParseBool(arr[index++]);
            rightStickClick = ParseBool(arr[index++]);
            dPadUp = ParseBool(arr[index++]);
            dPadDown = ParseBool(arr[index++]);
            dPadLeft = ParseBool(arr[index++]);
            dPadRight = ParseBool(arr[index++]);
        }

        public override string ToString()
        {
            string s = "";

            s += this.leftStickXAxis + ";";
            s += this.leftStickYAxis + ";";
            s += this.rightStickXAxis + ";";
            s += this.rightStickYAxis + ";";
            s += this.leftTrigger + ";";
            s += this.rightTrigger + ";";

            s += SerializeBool(AButton) + ";";
            s += SerializeBool(BButton) + ";";
            s += SerializeBool(XButton) + ";";
            s += SerializeBool(YButton) + ";";
            s += SerializeBool(leftBumper) + ";";
            s += SerializeBool(rightBumper) + ";";
            s += SerializeBool(backButton) + ";";
            s += SerializeBool(startButton) + ";";
            s += SerializeBool(leftStickClick) + ";";
            s += SerializeBool(rightStickClick) + ";";
            s += SerializeBool(dPadUp) + ";";
            s += SerializeBool(dPadDown) + ";";
            s += SerializeBool(dPadLeft) + ";";
            s += SerializeBool(dPadRight);

            return s;
        }

        private bool ParseBool(string s)
        {
            return s == "1" ? true : false;
        }

        private string SerializeBool(bool b)
        {
            return b ? "1" : "0";
        }
    }
}
