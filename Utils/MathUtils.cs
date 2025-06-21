using System;

namespace osu_taiko_SV_Helper.Utils
{
    internal static class MathUtils
    {

        internal static int IntParse(string str)
        {
            try
            {
                return Convert.ToInt32(str);
            } 
            catch
            {
                return 0;
            }
        }

        internal static double DoubleParse(string str)
        {
            try
            {
                return Convert.ToDouble(str);
            }
            catch
            {
                return 0;
            }
        }
    }
}
