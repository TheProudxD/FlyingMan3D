using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Tools.Converter
{
    public static class TypeParser
    {
        private const char IN_CELL_SEPORATOR = ';';

        private static readonly Dictionary<string, Color> Colors = new()
        {
            { "white", Color.white },
            { "black", Color.black },
            { "yellow", Color.yellow },
            { "red", Color.red },
            { "green", Color.green },
            { "blue", Color.blue },
        };

        public static Color ParseColor(string color)
        {
            color = color.Trim();
            return Colors.GetValueOrDefault(color, default(Color));
        }

        public static Vector3 ParseVector3(string s)
        {
            string[] vectorComponents = s.Split(IN_CELL_SEPORATOR);

            if (vectorComponents.Length < 3)
            {
                Debug.LogError("Can't parse Vector3. Wrong text format");
                return default(Vector3);
            }

            float x = ParseFloat(vectorComponents[0]);
            float y = ParseFloat(vectorComponents[1]);
            float z = ParseFloat(vectorComponents[2]);
            return new Vector3(x, y, z);
        }

        public static int ParseInt(string s)
        {
            if (!int.TryParse(s, System.Globalization.NumberStyles.Integer,
                    System.Globalization.CultureInfo.GetCultureInfo("en-US"), out int result))
            {
                Debug.LogError("Can't parse int, wrong text: " + s);
            }

            return result;
        }

        public static float ParseFloat(string s)
        {
            if (!float.TryParse(s, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.GetCultureInfo("en-US"), out float result))
            {
                Debug.LogError("Can't pars float,wrong text: " + s);
            }

            return result;
        }
    }
}