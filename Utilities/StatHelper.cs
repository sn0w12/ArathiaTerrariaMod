using System;

namespace Arathia.Utilities
{
    public static class StatHelper
    {
        /// <summary>
        /// Converts a given stat value to a format of 1.x, where x is derived from the stat value.
        /// </summary>
        /// <param name="statValue">The stat value to be converted.</param>
        /// <returns>A float in the format of 1.x, where x is derived from the stat value.</returns>
        public static float ConvertToOneXFormat(float statValue)
        {
            int numberOfDigits = statValue.ToString().Length;
            float factor = (float)Math.Pow(10, numberOfDigits);
            return 1 + (statValue / factor);
        }
    }
}