using System.Diagnostics;

namespace osu_taiko_SV_Helper.Utils;

internal static class LogUtils
{
    internal static void DebugLogger(string message)
    {
        Debug.WriteLine("[" + DateTime.Now + "] " + message);
        Console.WriteLine("[" + DateTime.Now + "] " + message);
    }

    internal static void ShowErrorMessage(string message)
    {
        MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
