using System;

namespace _Project.Scripts.Tools.Extensions
{
    public static class MathExtensions
    {
        #region Constants

        public const double GOLDENRATIO = 1.6180339887498948482;

        public const double PLASTICNUMBER = 1.32471795724474602596;

        public const double TAU = 6.283185307179586;

        public const double SPEEDOFLIGHT = 299792458;

        public const double PLANCKLENGTH = 1.61622938 * (10 * -35);

        public const double PLANCKTIME = 5.3911613 * (10 ^ -44);

        /// <summary>
        /// Note that this is in Kelvin
        /// </summary>
        public const double PLANCKTEMPERATURE = 1.41680833 * (10 ^ 32);

        public const double PLANCKMASS = 4.341 * (10 ^ -9);

        public const double GRAVITATIONALCONSTANT = 6.6740831 * (10 ^ -11);

        public const double PLANCKSCONSTANT = 6.62 * (10 ^ -34);

        /// <summary>
        /// One light year in metres
        /// </summary>
        public const double LIGHTYEAR = 9460730472580800;

        #endregion

        #region Add Towards

        #region Overflow

        public static float AddTowardsOverFlow(
            float value1, float value2, float target) =>
            value1 + value2 * Math.Sign(target - value1);

        #endregion

        #region Obstructed

        public static float AddTowardsObstructed(
            float value1, float value2, float target)
        {
            float v = value1 + value2 * Math.Sign(target - value1);

            return
                value1 < target //If we were firstly underneath the target
                    //Underneath
                    ? v > target //v is larger than the target
                        ? target //Clamp to target
                        : v //Else return v
                    //Not firstly underneath
                    : value1 > target //If firstly above
                        ? v < target //If smaller than target
                            ? target //Clamp to target
                            : v //Else return v
                        //Not above and not below, must be exactly equal to target
                        : target;
        }

        public static int AddTowardsObstructed(
            int value1, int value2, int target)
        {
            int v = value1 + value2 * Math.Sign(target - value1);

            return
                value1 < target //If we were firstly underneath the target
                    //Underneath
                    ? v > target //v is larger than the target
                        ? target //Clamp to target
                        : v //Else return v
                    //Not firstly underneath
                    : value1 > target //If firstly above
                        ? v < target //If smaller than target
                            ? target //Clamp to target
                            : v //Else return v
                        //Not above and not below, must be exactly equal to target
                        : target;
        }

        public static double AddTowardsObstructed(
            double value1, double value2, double target)
        {
            double v = value1 + value2 * Math.Sign(target - value1);

            return
                value1 < target //If we were firstly underneath the target
                    //Underneath
                    ? v > target //v is larger than the target
                        ? target //Clamp to target
                        : v //Else return v
                    //Not firstly underneath
                    : value1 > target //If firstly above
                        ? v < target //If smaller than target
                            ? target //Clamp to target
                            : v //Else return v
                        //Not above and not below, must be exactly equal to target
                        : target;
        }

        #endregion

        #region Rebound

        public static float AddTowardsRebound(
            float value1, float value2, float target)
        {
            float v = value1 + value2 * Math.Sign(target - value1);

            float offcut = Math.Abs(v - value1);

            return
                value1 < target //If we were firstly underneath the target
                    //Underneath
                    ? v > target //v is larger than the target
                        ? target - offcut //Rebound from target
                        : v //Else return v
                    //Not firstly underneath
                    : value1 > target //If firstly above
                        ? v < target //If smaller than target
                            ? target + offcut //Rebound from target
                            : v //Else return v
                        //Not above and not below, must be exactly equal to target
                        : target;
        }

        public static int AddTowardsRebound(
            int value1, int value2, int target)
        {
            int v = value1 + value2 * Math.Sign(target - value1);

            int offcut = Math.Abs(v - value1);

            return
                value1 < target //If we were firstly underneath the target
                    //Underneath
                    ? v > target //v is larger than the target
                        ? target - offcut //Rebound from target
                        : v //Else return v
                    //Not firstly underneath
                    : value1 > target //If firstly above
                        ? v < target //If smaller than target
                            ? target + offcut //Rebound from target
                            : v //Else return v
                        //Not above and not below, must be exactly equal to target
                        : target;
        }

        public static double AddTowardsRebound(
            double value1, double value2, double target)
        {
            double v = value1 + value2 * Math.Sign(target - value1);

            double offcut = Math.Abs(v - value1);

            return
                value1 < target //If we were firstly underneath the target
                    //Underneath
                    ? v > target //v is larger than the target
                        ? target - offcut //Rebound from target
                        : v //Else return v
                    //Not firstly underneath
                    : value1 > target //If firstly above
                        ? v < target //If smaller than target
                            ? target + offcut //Rebound from target
                            : v //Else return v
                        //Not above and not below, must be exactly equal to target
                        : target;
        }

        #endregion

        #endregion

        #region ClampWrap

        //Useful for clamping angles
        ////{TODO} Could probably just redo this using modulo
        //public static float ClampWrap(float value, float min, float max)
        //{
        //	if (value < min)
        //		return value + (max * ((value / max) + 1));
        //	if (value > max)
        //		return value - (max * (value / max));
        //	return value;
        //}

        public static float ClampWrap(float value, float min, float max)
        {
            value = (value - min) % (max - min) + min;

            if (value < min)
                value += max;

            return value;
        }

        public static double ClampWrap(double value, double min, double max)
        {
            value = (value - min) % (max - min) + min;

            if (value < min)
                value += max;

            return value;
        }

        public static int ClampWrap(int value, int min, int max)
        {
            value = (value - min) % (max - min) + min;

            if (value < min)
                value += max;

            return value;
        }

        #endregion

        #region ReMap

        public static float ReMap(float value, float inMin, float inMax, float outMin, float outMax) =>
            (value - inMin) / (outMin - inMin) * (outMax - inMax) + inMax;

        public static double ReMap(double value, double inMin, double inMax, double outMin, double outMax) =>
            (value - inMin) / (outMin - inMin) * (outMax - inMax) + inMax;

        public static float ReMapClamped(float value, float inMin, float inMax, float outMin, float outMax,
            bool clampIn = true, bool clampOut = true) =>
            ReMap(
                value.Clamp(clampIn ? inMin : float.NegativeInfinity,
                    clampIn ? inMax : float.PositiveInfinity
                ),
                inMin, inMax,
                outMin, outMax
            ).Clamp(clampOut ? outMin : float.NegativeInfinity, clampOut ? outMax : float.PositiveInfinity
            );

        public static double ReMapClamped(double value, double inMin, double inMax, double outMin, double outMax,
            bool clampIn = true, bool clampOut = true) =>
            ReMap(value.Clamp(clampIn ? inMin : double.NegativeInfinity, clampIn ? inMax : double.PositiveInfinity),
                inMin, inMax, outMin, outMax
            ).Clamp(clampOut ? outMin : double.NegativeInfinity, clampOut ? outMax : double.PositiveInfinity);

        #endregion

        #region Derivatives Of Position

        public static double PosFromDerivatives(double velocity, double acceleration, double jerk, double jounce,
            double time) =>
            velocity * time + acceleration * time * time / 2 + jerk * time * time * time / 6 +
            jounce * time * time * time * time / 24;

        #endregion

        #region Rounding

        #region Double

        /// <summary>
        /// Rounds to the nearest multiple of 'n'.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="n">N.</param>
        public static double RoundToN(double val, double n)
            => n * Math.Round(val / n);

        /// <summary>
        /// Rounds up to the nearest multiple of 'n'.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="n">N.</param>
        public static double CeilToN(double val, double n)
            => n * Math.Ceiling(val / n);

        /// <summary>
        /// Rounds down to the nearest multiple of 'n'.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="n">N.</param>
        public static double FloorToN(double val, double n)
            => n * Math.Floor(val / n);

        #endregion

        #region Float

        public static float Round(float val)
            => (float)Math.Round(val);

        public static float Floor(float val)
            => (float)Math.Floor(val);

        public static float Ceil(float val)
            => (float)Math.Ceiling(val);


        /// <summary>
        /// Rounds to the nearest multiple of 'n'.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="n">N.</param>
        public static float RoundToN(float val, float n)
            => n * Round(val / n);

        /// <summary>
        /// Rounds up to the nearest multiple of 'n'.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="n">N.</param>
        public static float CeilToN(float val, float n)
            => n * Ceil(val / n);

        /// <summary>
        /// Rounds down to the nearest multiple of 'n'.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="n">N.</param>
        public static float FloorToN(float val, float n)
            => n * Floor(val / n);

        #endregion

        #endregion

        #region Lerps

        #region Double

        /// <summary>
        /// Lerp
        /// </summary>
        public static double Lerp(double a, double b, double t)
            => (b - a) * t + a;


        /// <summary>
        /// Quadratic Lerp
        /// </summary>
        public static double QuadLerp(double a, double b, double t)
            => (b - a) * t * t + a;

        /// <summary>
        /// A Sine Lerp
        /// </summary>
        public static double SineLerp(double a, double b, double t)
            => (b - a) * Math.Sin(t * Math.PI / 2) + a;

        //Finds the 't' used to lerp between two numbers
        public static double UnLerp(double a, double b, double lerped) =>
            //Lerp function: lerped = ((b-a) * t) + a
            //Solve for t
            //t = (lerped - a) / (b - a)
            (lerped - a) / (b - a);

        public static double UnQuadLerp(double a, double b, double lerped) =>
            //Lerp function: lerped = ((b-a) * t * t) + a
            //Solve for t
            //t = Sqrt((lerped - a) / (b - a))
            Math.Sqrt((lerped - a) / (b - a));

        #endregion

        #region Float

        /// <summary>
        /// Lerp
        /// </summary>
        public static float Lerp(float a, float b, float t) => (b - a) * t + a;


        /// <summary>
        /// Quadratic Lerp
        /// </summary>
        public static float QuadLerp(float a, float b, float t) => (b - a) * t * t + a;

        /// <summary>
        /// A Sine Lerp
        /// </summary>
        public static float SineLerp(float a, float b, float t) =>
            (b - a) * (float)Math.Sin(t * Math.PI / 2) + a;

        //Finds the 't' used to lerp between two numbers
        public static float UnLerp(float a, float b, float lerped) =>
            //Lerp function: lerped = ((b-a) * t) + a
            //Solve for t
            //t = (lerped - a) / (b - a)
            (lerped - a) / (b - a);

        public static float UnQuadLerp(float a, float b, float lerped) =>
            //Lerp function: lerped = ((b-a) * t * t) + a
            //Solve for t
            //t = Sqrt((lerped - a) / (b - a))
            (float)Math.Sqrt((lerped - a) / (double)(b - a));

        #endregion

        #endregion

        #region Quick Convert

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float ToDeg(this float v) => v * 180 / (float)Math.PI;

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double ToDeg(this double v) => v * 180 / Math.PI;

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float ToRad(this float v) => v * (float)Math.PI / 180;

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double ToRad(this double v) => v * Math.PI / 180;

        #endregion
    }
}