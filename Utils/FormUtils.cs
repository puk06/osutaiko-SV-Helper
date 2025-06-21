using osu_taiko_SV_Helper.Models;

namespace osu_taiko_SV_Helper.Utils;

internal static class FormUtils
{
    internal static void SetLabelText(Control label, string text)
    {
        const int maxLabelWidth = 380;
        label.MaximumSize = new Size(maxLabelWidth, int.MaxValue);

        if (TextRenderer.MeasureText(text, label.Font).Width > maxLabelWidth)
        {
            while (TextRenderer.MeasureText(text + "...", label.Font).Width > maxLabelWidth)
            {
                text = text.Substring(0, text.Length - 1);
            }
            text += "...";
        }

        label.Text = text;
    }

    internal static void PlaySound(SystemSounds systemSounds)
    {
        switch (systemSounds)
        {
            case SystemSounds.Success:
                System.Media.SystemSounds.Asterisk.Play();
                break;
            case SystemSounds.Error:
                System.Media.SystemSounds.Hand.Play();
                break;
            default:
                throw new NotImplementedException();
        }
    }
}
