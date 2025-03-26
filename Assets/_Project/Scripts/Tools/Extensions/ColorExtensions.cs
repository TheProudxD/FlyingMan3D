using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Tools.Extensions
{
    public static class ColorExtensions
    {
        public static Color RandomColor => new(Random.value, Random.value, Random.value);

        public static string ToHex(this Color color) => $"#{ColorUtility.ToHtmlStringRGBA(color)}";

        public static Color SetA(this Color c, float a)
        {
            c.a = a;
            return c;
        }

        public static Color FromHex(this string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }

            throw new ArgumentException("Invalid hex string", nameof(hex));
        }

        public static Color Add(this Color thisColor, Color otherColor)
            => (thisColor + otherColor).Clamp01();

        public static Color Subtract(this Color thisColor, Color otherColor)
            => (thisColor - otherColor).Clamp01();

        public static Color Blend(this Color color1, Color color2, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);

            return new Color(
                color1.r * (1 - ratio) + color2.r * ratio,
                color1.g * (1 - ratio) + color2.g * ratio,
                color1.b * (1 - ratio) + color2.b * ratio,
                color1.a * (1 - ratio) + color2.a * ratio
            );
        }

        public static Color Invert(this Color color)
            => new(1 - color.r, 1 - color.g, 1 - color.b, color.a);
        
        private static Color Clamp01(this Color color)
        {
            return new Color
            {
                r = Mathf.Clamp01(color.r),
                g = Mathf.Clamp01(color.g),
                b = Mathf.Clamp01(color.b),
                a = Mathf.Clamp01(color.a)
            };
        }

        // Sets out values to Hex String 'FF'
        public static void GetHexFromColor(this Color color, out string red, out string green, out string blue,
            out string alpha)
        {
            red = color.r.Dec01_to_Hex();
            green = color.g.Dec01_to_Hex();
            blue = color.b.Dec01_to_Hex();
            alpha = color.a.Dec01_to_Hex();
        }
    }
}