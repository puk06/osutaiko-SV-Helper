namespace osu_taiko_SV_Helper.Utils;

internal static class FileUtils
{
    public static Bitmap GetBackgroundImage(string path)
    {
        const int resizeWidth = 392;
        const double darknessFactor = 0.5;
        const string EmptyBackgroundImagePath = "./src/EmptyBackground.png";

        if (!File.Exists(path))
        {
            path = EmptyBackgroundImagePath;
        }

        Bitmap bmp = new Bitmap(path);
        int resizeHeight = (int)(bmp.Height * ((double)resizeWidth / bmp.Width));

        Bitmap resizeBmp = new Bitmap(resizeWidth, resizeHeight);

        Graphics graphics = Graphics.FromImage(resizeBmp);
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        graphics.DrawImage(bmp, 0, -((resizeHeight - 102) / 2), resizeWidth, resizeHeight);
        graphics.Dispose();

        for (int y = 0; y < resizeHeight; y++)
        {
            for (int x = 0; x < resizeWidth; x++)
            {
                Color pixel = resizeBmp.GetPixel(x, y);
                int r = (int)(pixel.R * darknessFactor);
                int g = (int)(pixel.G * darknessFactor);
                int b = (int)(pixel.B * darknessFactor);
                Color darkPixel = Color.FromArgb(r, g, b);
                resizeBmp.SetPixel(x, y, darkPixel);
            }
        }

        return resizeBmp;
    }

    public static string GetSongsFolderLocation(string osuDirectory)
    {
        string userName = Environment.UserName;
        string file = Path.Combine(osuDirectory, $"osu!.{userName}.cfg");
        if (!File.Exists(file)) return Path.Combine(osuDirectory, "Songs");

        foreach (string readLine in File.ReadLines(file))
        {
            if (!readLine.StartsWith("BeatmapDirectory")) continue;
            string path = readLine.Split('=')[1].Trim(' ');
            return path == "Songs" ? Path.Combine(osuDirectory, "Songs") : path;
        }

        return Path.Combine(osuDirectory, "Songs");
    }

    public static string FindLatestFile(IEnumerable<string> files)
    {
        DateTime latestDate = DateTime.MinValue;
        string latestFile = "";

        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);

            if (!DateTime.TryParseExact(fileName, "yyyy_MM_dd_HH_mm_ss_fff", null, System.Globalization.DateTimeStyles.None, out DateTime fileDate)) continue;
            if (fileDate <= latestDate) continue;

            latestDate = fileDate;
            latestFile = file;
        }

        return latestFile;
    }
}
