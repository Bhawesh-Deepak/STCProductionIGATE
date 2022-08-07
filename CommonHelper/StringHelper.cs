using System;

namespace CommonHelper
{
    public static class StringHelper
    {
        public static T GetDefaultIfStringNull<T>(this string data)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrWhiteSpace(data))
            {
                return default(T);
            }
            else
            {
                return (T)Convert.ChangeType(data, typeof(T));
            }

        }
    }
}
