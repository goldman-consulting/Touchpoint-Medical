using Newtonsoft.Json;

using System.Net.Http.Headers;
using System.Text;

namespace TouchpointMedical
{
    public static class StringExtensions
    {
        public static string Mask(this string value, 
            char maskCharacter = '*', string? prefix = null,
            string? suffix = null)
        {
            return $"{prefix ?? string.Empty}{new string(maskCharacter, value.Length)}{suffix ?? string.Empty}";
        }

        public static string MaskSSN(this string value,
            char maskCharacter = '*')
        {
            var maskedSSN1 = new string(maskCharacter, 3);
            var maskedSSN2 = new string(maskCharacter, 2);
            var last4 = value.Length > 4 ? value[^4..] : value.PadLeft(4-value.Length);

            return $"{maskedSSN1}-{maskedSSN2}-{last4}";
        }

        public static bool HasValue(this string? value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static string AsString(this List<string> items)
        {
            var sb = new StringBuilder();

            foreach (var item in items)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }

                sb.Append(item);
            }

            return sb.ToString();
        }
    }
}
