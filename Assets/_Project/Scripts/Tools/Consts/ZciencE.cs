using System;

namespace _Project.Scripts.Tools.Consts
{
    public static class ZciencE
    {
        #region Special Relativity

        public static double GammaMod(double velocity, double speedOfLight = 3 * (10 ^ 8)) => (Math.Sqrt(1 - ((velocity * velocity) / (speedOfLight * speedOfLight))));

        public static double LengthContraction(
            double originalLength,
            double velocity,
            double speedOfLight = 3 * (10 ^ 8)) =>
            (originalLength * GammaMod(velocity, speedOfLight));

        public static double MassDilation(
            double originalMass,
            double velocity,
            double speedOfLight = 3 * (10 ^ 8)) =>
            (originalMass / GammaMod(velocity, speedOfLight));

        public static double TimeDilation(
            double originalTime,
            double velocity,
            double speedOfLight = 3 * (10 ^ 8)) =>
            (originalTime / GammaMod(velocity, speedOfLight));

        #endregion
    }
}