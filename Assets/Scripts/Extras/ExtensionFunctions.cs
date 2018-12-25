using System;

namespace UNetUI.Extras
{
    public static class ExtensionFunctions
    {
        public static float Map(float from, float fromMin, float fromMax, float toMin, float toMax, bool round3Dec = true)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return (float)Math.Round(to, 3);
        }
    }
}