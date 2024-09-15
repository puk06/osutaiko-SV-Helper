using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace osu_taiko_SV_Helper.Classes
{
    public class Helper
    {
        public static Bitmap GetBackgroundImage(string path)
        {
            try
            {
                Bitmap bmp = new Bitmap(path);
                const int resizeWidth = 392;
                int resizeHeight = (int)(bmp.Height * ((double)resizeWidth / bmp.Width));
                Bitmap resizeBmp = new Bitmap(resizeWidth, resizeHeight);
                Graphics graphics = Graphics.FromImage(resizeBmp);
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(bmp, 0, -((resizeHeight - 102) / 2), resizeWidth, resizeHeight);
                graphics.Dispose();

                const double darknessFactor = 0.5;
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
            catch
            {
                return GetBackgroundImage("./src/no background.png");
            }
        }

        public static string GetSongsFolderLocation(string osuDirectory)
        {
            string userName = Environment.UserName;
            string file = Path.Combine(osuDirectory, $"osu!.{userName}.cfg");

            foreach (string readLine in File.ReadLines(file))
            {
                if (!readLine.StartsWith("BeatmapDirectory")) continue;
                string path = readLine.Split('=')[1].Trim(' ');
                return path == "Songs" ? Path.Combine(osuDirectory, "Songs") : path;
            }

            return Path.Combine(osuDirectory, "Songs");
        }

        public static async void GithubUpdateChecker()
        {
            try
            {
                const string softwareReleasesLatest = "https://github.com/puk06/osutaiko-SV-Helper/releases/latest";
                if (!File.Exists("./src/version"))
                {
                    MessageBox.Show("versionファイルが存在しないのでアップデートチェックは無視されます。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                StreamReader currentVersion = new StreamReader("./src/version");
                string currentVersionString = await currentVersion.ReadToEndAsync();
                currentVersion.Close();
                if (currentVersionString.Contains("beta"))
                {
                    MessageBox.Show("betaバージョンをお使いのようです！もしバグや変な挙動を見つけたら報告お願いします！\n(定期的に更新されるので、Twitter(@Hoshino1_)を定期的に確認してください！！)", "Betaバージョン", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var githubClient = new GitHubClient(new Octokit.ProductHeaderValue("osutaiko-SV-Helper"));
                var latestRelease = await githubClient.Repository.Release.GetLatest("puk06", "osutaiko-SV-Helper");
                if (latestRelease.Name == currentVersionString) return;
                DialogResult result = MessageBox.Show($"最新バージョンがあります！\n\n現在: {currentVersionString} \n更新後: {latestRelease.Name}\n\nダウンロードページを開きますか？", "アップデートのお知らせ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes) Process.Start(softwareReleasesLatest);
            }
            catch
            {
                MessageBox.Show("アップデートチェック中にエラーが発生しました", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void ErrorLogger(Exception error)
        {
            string currentTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            File.WriteAllText("Error.log", $"[{currentTime}] " + error.Message + "\n" + error.StackTrace);
        }

        public static void DebugLogger(string message)
        {
            Debug.WriteLine("[" + DateTime.Now + "] " + message);
            Console.WriteLine("[" + DateTime.Now + "] " + message);
        }

        public static void AddValueToArray<T>(ref IEnumerable<T> array, T value)
        {
            array = array.Append(value).ToArray();
        }

        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static string FindLatestFile(IEnumerable<string> files)
        {
            DateTime latestDate = DateTime.MinValue;
            string latestFile = "";

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (!DateTime.TryParseExact(fileName, "yyyy_MM_dd_HH_mm_ss_fff", null,
                        System.Globalization.DateTimeStyles.None, out DateTime fileDate)) continue;
                if (fileDate <= latestDate) continue;
                latestDate = fileDate;
                latestFile = file;
            }

            return latestFile;
        }
    }
}
