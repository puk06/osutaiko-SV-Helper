using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public static async void GithubUpdateChecker(string currentVersion)
        {
            try
            {
                var latestRelease = await GetVersion(currentVersion);
                if (latestRelease == currentVersion) return;
                DialogResult result =
                    MessageBox.Show($"最新バージョンがあります！\n\n現在: {currentVersion} \n更新後: {latestRelease}\n\nダウンロードしますか？",
                        "アップデートのお知らせ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result != DialogResult.Yes) return;

                if (!File.Exists("./Updater/Software Updater.exe"))
                {
                    MessageBox.Show("アップデーターが見つかりませんでした。手動でダウンロードしてください。", "エラー", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                string updaterPath = Path.GetFullPath("./Updater/Software Updater.exe");
                const string author = "puk06";
                const string repository = "osutaiko-SV-Helper";
                const string executableName = "osu!taiko SV Helper";
                ProcessStartInfo args = new ProcessStartInfo()
                {
                    FileName = $"\"{updaterPath}\"",
                    Arguments = $"\"{latestRelease}\" \"{author}\" \"{repository}\" \"{executableName}\"",
                    UseShellExecute = true
                };

                Process.Start(args);
            }
            catch (Exception exception)
            {
                MessageBox.Show("アップデートチェック中にエラーが発生しました" + exception.Message, "エラー", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public static async Task<string> GetVersion(string currentVersion)
        {
            try
            {
                var releaseType = currentVersion.Split('-')[1];
                var githubClient = new GitHubClient(new ProductHeaderValue("osutaiko-SV-Helper"));
                var tags = await githubClient.Repository.GetAllTags("puk06", "osutaiko-SV-Helper");
                string latestVersion = currentVersion;
                foreach (var tag in tags)
                {
                    if (releaseType == "Release")
                    {
                        if (tag.Name.Split('-')[1] != "Release") continue;
                        latestVersion = tag.Name;
                        break;
                    }

                    latestVersion = tag.Name;
                    break;
                }

                return latestVersion;
            }
            catch
            {
                throw new Exception("アップデートの取得に失敗しました");
            }
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
