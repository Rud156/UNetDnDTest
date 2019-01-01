using System;
using UnityEngine;

namespace UNetUI.Extras
{
    public static class ExtensionFunctions
    {
        public static float Map(float from, float fromMin, float fromMax, float toMin, float toMax,
            bool round5Dec = true)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return (float) Math.Round(to, 5);
        }

        public static Color ConvertAndClampColor(float r = 0, float g = 0, float b = 0, float a = 0)
        {
            return new Color(Mathf.Clamp(r, 0, 255) / 255, Mathf.Clamp(g, 0, 255) / 255, Mathf.Clamp(b, 0, 255) / 255,
                Mathf.Clamp(a, 0, 255) / 255);
        }

        public static float To360Angle(float angle)
        {
            while (angle < 0.0f)
                angle += 360.0f;
            while (angle >= 360.0f)
                angle -= 360.0f;

            return angle;
        }

        public static string FormatWithCommas(int score) => $"{score:n0}";
    }
}