using Octokit;
using System.Diagnostics;

namespace osu_taiko_SV_Helper.Utils;

internal static class GithubUtils
{
    internal static async void GithubUpdateChecker(string currentVersion)
    {
        try
        {
            var latestRelease = await GetVersion(currentVersion);
            if (latestRelease == currentVersion) return;
            DialogResult result =
                MessageBox.Show($"最新バージョンがあります！\n\n現在: {currentVersion} \n更新後: {latestRelease}\n\nダウンロードしますか？",
                    "アップデートのお知らせ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result != DialogResult.Yes) return;

            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://github.com/puk06/osutaiko-SV-Helper/releases/latest"
            };
            Process.Start(processStartInfo);
        }
        catch (Exception exception)
        {
            MessageBox.Show("アップデートチェック中にエラーが発生しました" + exception.Message, "エラー", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static async Task<string> GetVersion(string currentVersion)
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
}
