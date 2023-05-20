using EnhancedDodoServer;
using System.Text;
using UnityEngine;
namespace EnhancedDodoServer
{
    /// <summary>
    /// Stores information about fx
    /// </summary>
    public struct FXData
    {
        public int intensity;
        public bool enabled;
        public FXData(int _intesity, bool _enabled)
        {
            intensity = _intesity;
            enabled = _enabled;
        }
    }
    /// <summary>
    /// Stores information about direction and speed of a chair movement
    /// </summary>
    public struct ChairDirection
    {
        public char direction;
        public int speed;
        public ChairDirection(char _direction, int _speed)
        {
            direction = _direction;
            speed = _speed;
        }
    }
    /// <summary>
    /// Stores information about fx to be enabled
    /// </summary>
    public struct FXEnableData
    {
        public int index;
        public float intensityMultiplier;
    }
    /// <summary>
    /// Handles all the FX
    /// </summary>
    public class DodoFX
    {
        FXData[] bubbles;
        FXData[] smoke;
        FXData[] wind;
        FXData[] thunder;
        ChairDirection[] chairDirections;
        public delegate FXEnableData[] CalculationMethod(int angle, int firstAngle, int numberOfElements);
        public CalculationMethod IndexCalculationMethod;
        /// <summary>
        /// Inits and resets all vars
        /// </summary>
        public DodoFX()
        {
            bubbles = new FXData[6];
            for (int i = 0; i < bubbles.Length; i++) bubbles[i] = new FXData(0, false);
            smoke = new FXData[6];
            for (int i = 0; i < smoke.Length; i++) smoke[i] = new FXData(0, false);
            wind = new FXData[3];
            for (int i = 0; i < wind.Length; i++) wind[i] = new FXData(0, false);
            thunder = new FXData[1];
            for (int i = 0; i < thunder.Length; i++) thunder[i] = new FXData(0, false);
            chairDirections = new ChairDirection[50];
            for (int i = 0; i < chairDirections.Length; i++) chairDirections[i] = new ChairDirection('S', 0);

            IndexCalculationMethod = new CalculationMethod(CalcIndexFromAngle_Lerp);
        }
        void SetFXData(FXData[] table, int number, int intensity, bool enabled)
        {
            if (number < 0 || number >= table.Length) { Debug.LogError("Wrong number"); return; }
            intensity = Mathf.Clamp(intensity, 0, 255);

            table[number].intensity = intensity;
            table[number].enabled = enabled;
        }
        /// <summary>
        /// Sets chair direction
        /// </summary>
        /// <param name="number"></param>
        /// <param name="direction"></param>
        /// <param name="speed"></param>
        public void SetChairDirection(int number, char direction, int speed)
        {
            if (number < 0 || number >= chairDirections.Length) { Debug.LogError("Wrong number"); return; }
            if (direction != 'S' && direction != 'L' && direction != 'R') { Debug.LogError("Wrong direction"); return; }
            speed = Mathf.Clamp(speed, 0, 99);

            chairDirections[number].direction = direction;
            chairDirections[number].speed = speed;
        }

        FXData[] table = null;
        /// <summary>
        /// Sets data for a specified type of an FX
        /// </summary>
        /// <param name="fxType"></param>
        /// <param name="number"></param>
        /// <param name="intensity"></param>
        /// <param name="enabled"></param>
        public void SetFXData(Consts.DodoFX fxType, int number, int intensity, bool enabled)
        {
            table = GetTable(fxType);
            SetFXData(table, number, intensity, enabled);
        }
        FXData[] GetTable(Consts.DodoFX fxType)
        {
            //FXData[] table;
            switch (fxType)
            {
                case Consts.DodoFX.Bubbles:
                    table = bubbles;
                    break;
                case Consts.DodoFX.Smoke:
                    table = smoke;
                    break;
                case Consts.DodoFX.Thunder:
                    table = thunder;
                    break;
                case Consts.DodoFX.Wind:
                    table = wind;
                    break;
                default:
                    table = null;
                    break;
            }
            return table;
        }
        /// <summary>
        /// Calculates the data depending on the angle of gaze
        /// </summary>
        /// <param name="fxType"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public FXEnableData[] CalcFXEnableData(Consts.DodoFX fxType, int angle)
        {
            while (angle > 360) angle -= 360;
            while (angle < 0) angle += 360;
            FXData[] table = GetTable(fxType);
            int firstAngle = 0;


            switch (fxType)
            {
                case Consts.DodoFX.Bubbles:
                    firstAngle = 30;
                    break;
                case Consts.DodoFX.Smoke:
                    firstAngle = 30;
                    break;
                case Consts.DodoFX.Thunder:
                    firstAngle = 0;
                    break;
                case Consts.DodoFX.Wind:
                    firstAngle = 60;
                    break;
            }

            return IndexCalculationMethod(angle, firstAngle, table.Length);
        }
        /// <summary>
        /// Calculates an index of an FX based on gaze angle. If angle is inbetween FXs it will lerp smoothly between them
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="firstAngle"></param>
        /// <param name="numberOfElements"></param>
        /// <returns></returns>
        public FXEnableData[] CalcIndexFromAngle_Lerp(int angle, int firstAngle, int numberOfElements)
        {
            FXEnableData[] fxEnableData;
            if (numberOfElements == 1)
            {
                fxEnableData = new FXEnableData[1];
                fxEnableData[0].index = 0;
                fxEnableData[0].intensityMultiplier = 1;
                return fxEnableData;
            }
            else
            {
                float angleSeparation = 360.0f / (float)numberOfElements;
                float importantValue = (float)(angle - firstAngle) / angleSeparation;
                //int index = Mathf.FloorToInt((float)(angle - firstAngle) / angleSeparation);
                int index = Mathf.FloorToInt(importantValue);
                Debug.Log("IV: " + importantValue);
                float percentage = 1.0f - (importantValue - Mathf.Floor(importantValue));
                if (index < 0) index += numberOfElements;
                //float leftAngle = (float)index * angleSeparation;
                //float rightAngle = (float)(index + 1) * angleSeparation;
                //float percentage = (angle - leftAngle) / angleSeparation;            
                fxEnableData = new FXEnableData[2];
                fxEnableData[0].index = index;
                fxEnableData[0].intensityMultiplier = percentage;
                fxEnableData[1].index = index + 1;
                if (fxEnableData[1].index >= numberOfElements) fxEnableData[1].index -= numberOfElements;
                fxEnableData[1].intensityMultiplier = 1.0f - percentage;
                return fxEnableData;
            }
        }
        /// <summary>
        /// Calculates an index of an FX based on gaze angle. If angle is inbetween FXs it will find the closest one
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="firstAngle"></param>
        /// <param name="numberOfElements"></param>
        /// <returns></returns>
        public FXEnableData[] CalcIndexFromAngle_Closest(int angle, int firstAngle, int numberOfElements)
        {
            float angleSeparation = 360.0f / (float)numberOfElements;
            int index = Mathf.RoundToInt((float)(angle - firstAngle) / angleSeparation);
            FXEnableData[] fxEnableData = new FXEnableData[1];
            fxEnableData[0].index = index;
            fxEnableData[0].intensityMultiplier = 1f;
            return fxEnableData;
        }

        private string zeros = null;
        string CreateMessage(int intensity)
        {
            sb1 = new StringBuilder();
            zeros = "";
            if (intensity >= 100)
            {
                zeros = "";
            }
            else if (intensity >= 10)
            {
                zeros = "0";
            }
            else
            {
                zeros = "00";
            }
            return sb1.AppendFormat("{0}{1}{2}{3}", "{", zeros, intensity.ToString(), "}").ToString();
            //return "{" + zeros + intensity.ToString() + "}";
        }

        private static StringBuilder sb1 = new StringBuilder();
        private static StringBuilder sb2 = new StringBuilder();

        private string message = null;
        /// <summary>
        /// Creates a message to be sent
        /// </summary>
        /// <returns></returns>
        public string CreateMessage()
        {
            message = "{Sta}{Ide}{Deg}{Rsv}";
            sb2.Length = 0;
            sb2.Append("{Sta}{Ide}{Deg}{Rsv}");

            for (int i = 0; i < bubbles.Length; i++)
            {
                sb2.Append(CreateMessage(bubbles[i].intensity));
                sb2.Append(CreateMessage(smoke[i].intensity));

                //message += CreateMessage(bubbles[i].intensity);
                //message += CreateMessage(smoke[i].intensity);
            }
            for (int i = 0; i < wind.Length; i++)
            {
                sb2.Append(CreateMessage(wind[i].intensity));

                //message += CreateMessage(wind[i].intensity);
            }
            for (int i = 0; i < thunder.Length; i++)
            {
                sb2.Append(CreateMessage(thunder[i].intensity));

                //message += CreateMessage(thunder[i].intensity);
            }
            for (int i = 0; i < chairDirections.Length; i++)
            {
                //message += CreateMessage(chairDirections[i]); // to juz Krzysiek zakomentowal
                //string zeros = "";
                if (chairDirections[i].speed < 10) zeros = "0";
                //message += "{" + chairDirections[i].direction.ToString() + zeros + chairDirections[i].speed.ToString() + "}";
                sb2.Append("{");
                sb2.Append(chairDirections[i].direction.ToString());
                sb2.Append(zeros);
                sb2.Append(chairDirections[i].speed.ToString());
                sb2.Append("}");
            }
            return sb2.ToString();
            //return message;
        }

    }
}