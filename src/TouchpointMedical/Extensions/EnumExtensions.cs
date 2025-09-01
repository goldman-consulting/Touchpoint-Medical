using Newtonsoft.Json.Linq;

using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

using TouchpointMedical.Logging;

namespace TouchpointMedical
{
    public static class EnumExtensions
    {
        public static string ToDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var descriptionAttributeValue = 
                field?.GetCustomAttribute<DescriptionAttribute>();
            var enumMemberAttributeValue =
                field?.GetCustomAttribute<EnumMemberAttribute>();

            return descriptionAttributeValue?.Description ?? enumMemberAttributeValue?.Value ?? 
                value.ToString();
        }

        public static bool HasAnyFlags<TEnum>(this TEnum value, TEnum testFlags)
            where TEnum : struct, Enum
        {
            var v = Convert.ToUInt64(value);
            var t = Convert.ToUInt64(testFlags);

            return (v & t) != 0;
        }

        public static bool HasAllFlags<TEnum>(this TEnum value, TEnum testFlags)
            where TEnum : struct, Enum
        {
            var v = Convert.ToUInt64(value);
            var t = Convert.ToUInt64(testFlags);

            return (v & t) == t;
        }

    }
}
