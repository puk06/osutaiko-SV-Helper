using System.Collections.Generic;
using System.Linq;

namespace osu_taiko_SV_Helper.Utils
{
    internal static class ArrayUtils
    {
        internal static void AddValueToArray<T>(ref IEnumerable<T> array, T value)
        {
            array = array.Append(value).ToArray();
        }
    }
}
