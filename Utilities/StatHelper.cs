using System;

namespace Arathia.Utilities
{
    public static class StatHelper
    {
        public static float ConvertToOneXFormat(float statValue)
        {
            int numberOfDigits = statValue.ToString().Length;
            float factor = (float)Math.Pow(10, numberOfDigits);
            return 1 + (statValue / factor);
        }
    }
}