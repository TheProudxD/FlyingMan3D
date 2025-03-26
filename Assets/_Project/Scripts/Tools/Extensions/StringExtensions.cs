using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace _Project.Scripts.Tools.Extensions
{
    public static class StringExtensions
    {
        public static string ToString<T>(this T value) => value.ToString();

        public static string ToLowerString<T>(this T value) => ToString(value).ToLower();

        public static string ToUpperString<T>(this T value) => ToString(value).ToUpper();

        public static TEnum FromString<TEnum>(this string text, TEnum defaultValue, bool caseInsensitive = true)
        {
            if (string.IsNullOrEmpty(text)) return defaultValue;
            if (!Enum.IsDefined(typeof(TEnum), text)) return defaultValue;

            return (TEnum)Enum.Parse(typeof(TEnum), text, caseInsensitive);
        }

        public static float ParseFloat(this string value, float @default) =>
            !float.TryParse(value, out float f) ? @default : f;

        public static int ParseInt(this string value, int @default = 0) =>
            (int)ParseFloat(value, @default);

        public static bool TryBool(this string self) => bool.TryParse(self, out bool res) && res;

        public static float TrySingle(this string self) => float.TryParse(self, out float res) ? res : 0f;

        /// <summary>
        /// Returns 0-255
        /// </summary>
        public static int Hex_to_Dec(this string hex) => Convert.ToInt32(hex, 16);

        /// <summary>
        /// Returns a float between 0->1
        /// </summary>
        public static float Hex_to_Dec01(string hex) => hex.Hex_to_Dec() / 255f;

        /// <summary>
        /// Converts a string to its MD5 hash representation.
        /// </summary>
        /// <param name="input">The string to be hashed.</param>
        /// <param name="encoding">The encoding to be used for converting the string to bytes.</param>
        /// <returns>The MD5 hash representation of the input string.</returns>
        public static string ToMD5(this string input, Encoding encoding)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            using var md5 = MD5.Create();
            byte[] hashedBytes = md5.ComputeHash(encoding.GetBytes(input));
            // Get the hashed string.  
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Converts a string to its SHA1 hash value using the specified encoding.
        /// </summary>
        /// <param name="input">The string to be hashed.</param>
        /// <param name="encoding">The encoding to be used for converting the string to bytes.</param>
        /// <returns>The SHA1 hash value of the input string.</returns>
        public static string ToSHA1(this string input, Encoding encoding)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            using var sha1 = SHA1.Create();
            byte[] hashedBytes = sha1.ComputeHash(encoding.GetBytes(input));
            // Get the hashed string.  
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Converts a string to its SHA256 hash value.
        /// </summary>
        /// <param name="input">The string to be hashed.</param>
        /// <param name="encoding">The encoding to be used for converting the string to bytes.</param>
        /// <returns>The SHA256 hash value of the input string.</returns>
        public static string ToSHA256(this string input, Encoding encoding)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            using var sha256 = SHA256.Create();
            byte[] hashedBytes = sha256.ComputeHash(encoding.GetBytes(input));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Converts a string to its SHA384 hash value using the specified encoding.
        /// </summary>
        /// <param name="input">The string to be hashed.</param>
        /// <param name="encoding">The encoding to be used for converting the string to bytes.</param>
        /// <returns>The SHA384 hash value of the input string.</returns>
        public static string ToSHA384(this string input, Encoding encoding)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            using var sha384 = SHA384.Create();
            byte[] hashedBytes = sha384.ComputeHash(encoding.GetBytes(input));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Converts a string to its SHA512 hash representation.
        /// </summary>
        /// <param name="input">The string to be hashed.</param>
        /// <param name="encoding">The encoding to be used for converting the string to bytes.</param>
        /// <returns>The SHA512 hash representation of the input string.</returns>
        public static string ToSHA512(this string input, Encoding encoding)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            using var sha512 = SHA512.Create();
            byte[] hashedBytes = sha512.ComputeHash(encoding.GetBytes(input));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Converts a string to its MD5 hash value.
        /// </summary>
        /// <param name="input">The input string to be hashed.</param>
        /// <returns>The MD5 hash value of the input string.</returns>
        public static string ToMD5(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            using var md5 = MD5.Create();
            byte[] hashedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            // Get the hashed string.  
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Converts a string to its SHA1 hash value.
        /// </summary>
        /// <param name="input">The input string to be hashed.</param>
        /// <returns>The SHA1 hash value of the input string.</returns>
        public static string ToSHA1(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            using var sha1 = SHA1.Create();
            byte[] hashedBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            // Get the hashed string.  
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Converts a string to its SHA256 hash value.
        /// </summary>
        /// <param name="input">The string to be hashed.</param>
        /// <returns>The SHA256 hash value of the input string.</returns>
        public static string ToSHA256(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            using var sha256 = SHA256.Create();
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Converts a string to its SHA384 hash value.
        /// </summary>
        /// <param name="input">The input string to be hashed.</param>
        /// <returns>The SHA384 hash value of the input string.</returns>
        public static string ToSHA384(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            using var sha384 = SHA384.Create();
            byte[] hashedBytes = sha384.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Converts a string to its SHA512 hash value.
        /// </summary>
        /// <param name="input">The string to be hashed.</param>
        /// <returns>The SHA512 hash value of the input string.</returns>
        public static string ToSHA512(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            using var sha512 = SHA512.Create();
            byte[] hashedBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Converts a base64 encoded string to a byte array.
        /// </summary>
        /// <param name="input">The base64 encoded string to convert.</param>
        /// <returns>The byte array representation of the base64 encoded string.</returns>
        public static byte[] FromBase64ToByteArray(this string input) =>
            string.IsNullOrEmpty(input) ? Array.Empty<byte>() : Convert.FromBase64String(input);


        /// <summary>
        /// Converts a base64 encoded string to a byte array using the specified encoding.
        /// </summary>
        /// <param name="input">The base64 encoded string to convert.</param>
        /// <param name="encoding">The encoding to use for the conversion.</param>
        /// <returns>A byte array representing the decoded base64 string.</returns>
        public static byte[] FromBase64ToByteArray(this string input, Encoding encoding) =>
            string.IsNullOrEmpty(input) ? Array.Empty<byte>() : Convert.FromBase64String(input);

        /// <summary>
        /// Converts a base64 encoded string to its original UTF8 string representation.
        /// </summary>
        /// <param name="input">The base64 encoded string to convert.</param>
        /// <returns>The original UTF8 string representation of the base64 encoded string.</returns>
        public static string FromBase64(this string input) => string.IsNullOrEmpty(input)
            ? ""
            : Encoding.UTF8.GetString(Convert.FromBase64String(input));


        /*
        /// <summary>
        /// Converts a base64 encoded string to a Bitmap object.
        /// </summary>
        /// <param name="input">The base64 encoded string.</param>
        /// <returns>The Bitmap object.</returns>

        public static Bitmap FromBase64ToBitmap(this string input)
        {
            var byteImage = FromBase64ToByteArray(input);
            if (byteImage == null)
                return null;
            var ms = new System.IO.MemoryStream(byteImage);
            var bitmap = new Bitmap(ms);
            return bitmap;
        }
        */
        /// <summary>
        /// Converts a string to its Base64 representation using the specified encoding.
        /// </summary>
        /// <param name="input">The string to be converted.</param>
        /// <param name="encoding">The encoding to be used for converting the string to bytes.</param>
        /// <returns>The Base64 representation of the input string.</returns>
        public static string ToBase64(this string input, Encoding encoding) => string.IsNullOrEmpty(input)
            ? ""
            : Convert.ToBase64String(encoding.GetBytes(input));

        /// <summary>
        /// Converts a base64 encoded string to its original form using the specified encoding.
        /// </summary>
        /// <param name="input">The base64 encoded string to convert.</param>
        /// <param name="encoding">The encoding to use for the conversion.</param>
        /// <returns>The original string represented by the base64 encoded input.</returns>
        public static string FromBase64(this string input, Encoding encoding) => string.IsNullOrEmpty(input)
            ? ""
            : encoding.GetString(Convert.FromBase64String(input));

        /// <summary>Checks if a string is Null or white space</summary>
        public static bool IsNullOrWhiteSpace(this string val) => string.IsNullOrWhiteSpace(val);

        /// <summary>Checks if a string is Null or empty</summary>
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        /// <summary>Checks if a string contains null, empty or white space.</summary>
        public static bool IsBlank(this string val) => val.IsNullOrWhiteSpace() || val.IsNullOrEmpty();

        /// <summary>Checks if a string is null and returns an empty string if it is.</summary>
        public static string OrEmpty(this string val) => val ?? string.Empty;

        public static string Shorten(this string val, int maxLength)
        {
            if (val.IsBlank()) return val;

            return val.Length <= maxLength ? val : val.Substring(0, maxLength);
        }

        /// <summary>Slices a string from the start index to the end index.</summary>
        /// <result>The sliced string.</result>
        public static string Slice(this string val, int startIndex, int endIndex)
        {
            if (val.IsBlank())
            {
                throw new ArgumentNullException(nameof(val), "Value cannot be null or empty.");
            }

            if (startIndex < 0 || startIndex > val.Length - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            // If the end index is negative, it will be counted from the end of the string.
            endIndex = endIndex < 0 ? val.Length + endIndex : endIndex;

            if (endIndex < 0 || endIndex < startIndex || endIndex > val.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(endIndex));
            }

            return val.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// Converts the input string to an alphanumeric string, optionally allowing periods.
        /// </summary>
        /// <param name="input">The input string to be converted.</param>
        /// <param name="allowPeriods">A boolean flag indicating whether periods should be allowed in the output string.</param>
        /// <returns>
        /// A new string containing only alphanumeric characters, underscores, and optionally periods.
        /// If the input string is null or empty, an empty string is returned.
        /// </returns>
        public static string ConvertToAlphanumeric(this string input, bool allowPeriods = false)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            List<char> filteredChars = new List<char>();
            int lastValidIndex = -1;

            // Iterate over the input string, filtering and determining valid start/end indices
            foreach (char character in input
                         .Where(character => char
                             .IsLetterOrDigit(character) || character == '_' || (allowPeriods && character == '.'))
                         .Where(character =>
                             filteredChars.Count != 0 || (!char.IsDigit(character) && character != '.')))
            {
                filteredChars.Add(character);
                lastValidIndex = filteredChars.Count - 1; // Update lastValidIndex for valid characters
            }

            // Remove trailing periods
            while (lastValidIndex >= 0 && filteredChars[lastValidIndex] == '.')
            {
                lastValidIndex--;
            }

            // Return the filtered string
            return lastValidIndex >= 0
                ? new string(filteredChars.ToArray(), 0, lastValidIndex + 1)
                : string.Empty;
        }

        /// <summary>
        /// Converts a string to its Base64 representation.
        /// </summary>
        /// <param name="input">The input string to be converted.</param>
        /// <returns>The Base64 representation of the input string.</returns>
        public static string ToBase64(this string input) => string.IsNullOrEmpty(input)
            ? ""
            : Convert.ToBase64String(Encoding.UTF8.GetBytes(input));

        // Rich text formatting, for Unity UI elements that support rich text.
        public static string RichColor(this string text, string color) => $"<color={color}>{text}</color>";

        public static string RichSize(this string text, int size) => $"<size={size}>{text}</size>";

        public static string RichBold(this string text) => $"<b>{text}</b>";

        public static string RichItalic(this string text) => $"<i>{text}</i>";

        public static string RichUnderline(this string text) => $"<u>{text}</u>";

        public static string RichStrikethrough(this string text) => $"<s>{text}</s>";

        public static string RichFont(this string text, string font) => $"<font={font}>{text}</font>";

        public static string RichAlign(this string text, string align) => $"<align={align}>{text}</align>";

        public static string RichGradient(this string text, string color1, string color2) =>
            $"<gradient={color1},{color2}>{text}</gradient>";

        public static string RichRotation(this string text, float angle) => $"<rotate={angle}>{text}</rotate>";

        public static string RichSpace(this string text, float space) => $"<space={space}>{text}</space>";
    }
}