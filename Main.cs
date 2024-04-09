using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels;
using OsuParsers.Decoders;
using Microsoft.Win32;

namespace osu_taiko_SV_Helper
{
    public partial class SvHelper : Form
    {
        private readonly BeatmapData _preData = new BeatmapData();
        private readonly StructuredOsuMemoryReader _sreader = new StructuredOsuMemoryReader();
        private readonly OsuBaseAddresses _baseAddresses = new OsuBaseAddresses();
        private readonly MemoryData _memoryData = new MemoryData();
        private bool _working;
        private bool _isDbLoaded;
        private int _currentTime;
        private string _osuDirectory;
        private string _songsPath;
        private string _currentBeatmapPath;
        private bool _memoryError;
        private bool _firstLoad = true;
        private bool _readBeatmapError;
        private readonly Dictionary<string, string> _configDictionary = new Dictionary<string, string>();

        public SvHelper()
        {
            InitializeComponent();
            CheckConfig();
            Thread getMemoryDataThread = new Thread(UpdateMemoryData) { IsBackground = true };
            getMemoryDataThread.Start();
            UpdateLoop();
        }

        private void CheckConfig()
        {
            if (!File.Exists("Config.cfg"))
            {
                SV_MODE_COMBOBOX.SelectedIndex = 0;
                MODE_COMBOBOX.SelectedIndex = 0;
                return;
            }

            string[] lines = File.ReadAllLines("Config.cfg");
            foreach (string line in lines)
            {
                string[] parts = line.Split('=');

                if (parts.Length != 2) continue;
                string name = parts[0].Trim();
                string value = parts[1].Trim();
                _configDictionary[name] = value;
            }

            var defaultsvmodeTest = _configDictionary.TryGetValue("DEFAULT_SV_MODE", out string defaultsvmodestring);
            if (defaultsvmodeTest)
            {
                var defaultModeResult = int.TryParse(defaultsvmodestring, out int defaultsvmode);
                if (!defaultModeResult || !(defaultsvmode == 0 || defaultsvmode == 1 || defaultsvmode == 2))
                {
                    MessageBox.Show("Config.cfgのDEFAULT_SV_MODEの値が不正であったため、初期値の0が適用されます。0、1、2のどれかを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    SV_MODE_COMBOBOX.SelectedIndex = defaultsvmode;
                }
            }

            var defaultmodeTest = _configDictionary.TryGetValue("DEFAULT_MODE", out string defaultmodestring);
            if (defaultmodeTest)
            {
                var defaultModeResult = int.TryParse(defaultmodestring, out int defaultmode);
                if (!defaultModeResult || !(defaultmode == 0 || defaultmode == 1 || defaultmode == 2 || defaultmode == 3))
                {
                    MessageBox.Show("Config.cfgのDEFAULT_MODEの値が不正であったため、初期値の0が適用されます。0、1、2、3のどれかを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MODE_COMBOBOX.SelectedIndex = defaultmode;
                }
            }

            KIAI_MODE_CHECKBOX.Checked = _configDictionary.TryGetValue("KIAI_MODE", out string test) && test == "true";
            OFFSET_CHECKBOX12.Checked = _configDictionary.TryGetValue("OFFSET_12", out string test2) && test2 == "true";
            OFFSET_CHECKBOX16.Checked = _configDictionary.TryGetValue("OFFSET_16", out string test3) && test3 == "true";
            BPM_COMP_CHECKBOX.Checked = _configDictionary.TryGetValue("BPM_COMPATIBILITY", out string test4) && test4 == "true";
        }

        private async void UpdateLoop()
        {
            while (true)
            {
                await Task.Delay(1);
                await Loop();
            }
        }

        private Task Loop()
        {
            try
            {
                if (_memoryError)
                {
                    foreach (Control control in Controls)
                    {
                        if (control == WORK_STATUS_TEXT ||
                            control == WORK_STATUS_LABEL ||
                            control == Background_Picture_Box ||
                            control == TITLE_LABEL ||
                            control == ARTIST_LABEL ||
                            control == VERSION_LABEL) continue;
                        control.Enabled = false;
                        WORK_STATUS_TEXT.Text = _readBeatmapError ? "Beatmap Error Occurred" : "Memory Error Occurred";
                        WORK_STATUS_TEXT.ForeColor = Color.Red;
                    }
                    return Task.CompletedTask;
                }

                if (_firstLoad) return Task.CompletedTask;
                foreach (Control control in Controls) control.Enabled = true;

                if (!_working)
                {
                    WORK_STATUS_TEXT.Text = "Ready! Waiting for your operation...";
                    WORK_STATUS_TEXT.ForeColor = Color.LimeGreen;
                }

                ARTIST_LABEL.Text = _memoryData.Artist;
                VERSION_LABEL.Text = _memoryData.Version;

                int versionWidth = VERSION_LABEL.Width;
                VERSION_LABEL.Location = new Point(390 - versionWidth, 2);

                string title = _memoryData.Title;
                const int maxLabelWidth = 380;
                TITLE_LABEL.MaximumSize = new Size(maxLabelWidth, int.MaxValue);
                if (TextRenderer.MeasureText(title, TITLE_LABEL.Font).Width > maxLabelWidth)
                {
                    while (TextRenderer.MeasureText(title + "...", TITLE_LABEL.Font).Width > maxLabelWidth)
                    {
                        title = title.Substring(0, title.Length - 1);
                    }

                    title += "...";
                }
                TITLE_LABEL.Text = title;

                if (_preData.BeatmapStr == _memoryData.BeatmapStr) return Task.CompletedTask;
                Background_Picture_Box.Image = GetBackgroundImage(_memoryData.BackgroundPath);
                _preData.BeatmapStr = _memoryData.BeatmapStr;

                return Task.CompletedTask;
            }
            catch (Exception error)
            {
                ErrorLogger(error);
                foreach (Control control in Controls)
                {
                    if (control == WORK_STATUS_TEXT ||
                        control == WORK_STATUS_LABEL ||
                        control == Background_Picture_Box ||
                        control == TITLE_LABEL ||
                        control == ARTIST_LABEL ||
                        control == VERSION_LABEL) continue;
                    control.Enabled = false;
                    WORK_STATUS_TEXT.Text = "Error Occurred";
                    WORK_STATUS_TEXT.ForeColor = Color.Red;
                }
                return Task.CompletedTask;
            }
        }

        private static Bitmap GetBackgroundImage(string path)
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

                const double darknessFactor = 0.7;
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

        private void UpdateMemoryData()
        {
            while (true)
            {
                try
                {
                    if (!_isDbLoaded)
                    {
                        if (Process.GetProcessesByName("osu!").Length > 0)
                        {
                            Process osuProcess = Process.GetProcessesByName("osu!")[0];
                            _osuDirectory = Path.GetDirectoryName(osuProcess.MainModule.FileName);
                        }
                        else
                        {
                            using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey("osu\\DefaultIcon"))
                            {
                                if (registryKey != null)
                                {
                                    string str = registryKey.GetValue(null).ToString().Remove(0, 1);
                                    _osuDirectory = str.Remove(str.Length - 11);
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(_osuDirectory) || !Directory.Exists(_osuDirectory))
                        {
                            _memoryError = true;
                            continue;
                        }

                        _songsPath = GetSongsFolderLocation(_osuDirectory);
                        _isDbLoaded = true;
                    }

                    if (!_sreader.CanRead)
                    {
                        _memoryError = true;
                        continue;
                    }

                    _sreader.TryRead(_baseAddresses.Beatmap);
                    _sreader.TryRead(_baseAddresses.GeneralData);

                    _currentTime = _baseAddresses.GeneralData.AudioTime;

                    string osuBeatmapPath = Path.Combine(_songsPath ?? "", _baseAddresses.Beatmap.FolderName ?? "",
                        _baseAddresses.Beatmap.OsuFileName ?? "");
                    if (!File.Exists(osuBeatmapPath))
                    {
                        _memoryError = true;
                        _readBeatmapError = true;
                        continue;
                    }
                    _readBeatmapError = false;

                    _currentBeatmapPath = osuBeatmapPath;
                    if (_memoryData.BeatmapStr == _baseAddresses.Beatmap.MapString) continue;
                    OsuParsers.Beatmaps.Beatmap beatmapData = BeatmapDecoder.Decode(osuBeatmapPath);
                    _memoryData.Title = beatmapData.MetadataSection.Title;
                    _memoryData.Artist = beatmapData.MetadataSection.Artist;
                    _memoryData.Version = beatmapData.MetadataSection.Version;
                    _memoryData.BeatmapStr = _baseAddresses.Beatmap.MapString;

                    string backgroundPath = Path.Combine(_songsPath ?? "", _baseAddresses.Beatmap.FolderName ?? "",
                        beatmapData.EventsSection.BackgroundImage ?? "");
                    if (!File.Exists(backgroundPath)) backgroundPath = "./src/no background.png";
                    _memoryData.BackgroundPath = backgroundPath;
                    _memoryError = false;
                    if (_firstLoad) _firstLoad = false;
                }
                catch (Exception error)
                {
                    ErrorLogger(error);
                    _memoryError = true;
                }
            }
        }

        private static string GetSongsFolderLocation(string osuDirectory)
        {
            foreach (string file in Directory.GetFiles(osuDirectory))
            {
                if (!Regex.IsMatch(file, "^osu!\\.+\\.cfg$")) continue;
                foreach (string readLine in File.ReadLines(file))
                {
                    if (!readLine.StartsWith("BeatmapDirectory")) continue;
                    return Path.Combine(osuDirectory, readLine.Split('=')[1].Trim(' '));
                }
            }
            return Path.Combine(osuDirectory, "Songs");
        }

        private async void MAKE_BUTTON_Click(object sender, EventArgs e)
        {
            MAKE_BUTTON.Enabled = false;
            UNDO_BUTTON.Enabled = false;

            if (Regex.IsMatch(SV_STARTTIME_TEXTBOX.Text, "^\\d+:\\d+:\\d+.+$"))
            {
                string[] time = SV_STARTTIME_TEXTBOX.Text
                    .Replace(" ", "")
                    .Replace("-", "")
                    .Split(':');
                int minute = int.Parse(time[0]);
                int second = int.Parse(time[1]);
                int millisecond = int.Parse(time[2]);
                int totalMillisecond = minute * 60000 + second * 1000 + millisecond;
                SV_STARTTIME_TEXTBOX.Text = totalMillisecond.ToString();
            }

            if (Regex.IsMatch(SV_ENDTIME_TEXTBOX.Text, "^\\d+:\\d+:\\d+.+$"))
            {
                string[] time = SV_ENDTIME_TEXTBOX.Text
                    .Replace(" ", "")
                    .Replace("-", "")
                    .Split(':');
                int minute = int.Parse(time[0]);
                int second = int.Parse(time[1]);
                int millisecond = int.Parse(time[2]);
                int totalMillisecond = minute * 60000 + second * 1000 + millisecond;
                SV_ENDTIME_TEXTBOX.Text = totalMillisecond.ToString();
            }

            bool result = ValueChecker();
            if (!result)
            {
                MAKE_BUTTON.Enabled = true;
                UNDO_BUTTON.Enabled = true;
                return;
            }

            BeatmapArgs args = new BeatmapArgs
            {
                Point = new List<int> { int.Parse(SV_STARTTIME_TEXTBOX.Text), int.Parse(SV_ENDTIME_TEXTBOX.Text) },
                Sv = new List<double> { double.Parse(SV_START_TEXTBOX.Text), double.Parse(SV_END_TEXTBOX.Text) },
                Volume = new List<int> { int.Parse(VOLUME_START_TEXTBOX.Text), int.Parse(VOLUME_END_TEXTBOX.Text) },
                IsKiaiMode = KIAI_MODE_CHECKBOX.Checked,
                SvMode = SV_MODE_COMBOBOX.SelectedIndex,
                Offset = int.Parse(OFFSET_TEXTBOX.Text),
                Mode = MODE_COMBOBOX.SelectedIndex,
                Offset16 = OFFSET_CHECKBOX16.Checked,
                Offset12 = OFFSET_CHECKBOX12.Checked,
                BpmCompatibility = BPM_COMP_CHECKBOX.Checked,
                BaseBpm = string.IsNullOrEmpty(BASE_BPM_TEXTBOX.Text) || !USE_CUSTOM_BPM_CHECKBOX.Checked || BASE_BPM_TEXTBOX.Text == "0" ? 0 : double.Parse(BASE_BPM_TEXTBOX.Text)
            };
            
            try
            {
                _working = true;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                WORK_STATUS_TEXT.Text = "Making...";
                WORK_STATUS_TEXT.ForeColor = Color.LimeGreen;
                Beatmap beatmap = new Beatmap();
                WORK_STATUS_TEXT.Text = "Preparing...";
                await beatmap.BeatmapParser(_currentBeatmapPath, args);
                WORK_STATUS_TEXT.Text = "Backing up Beatmaps...";
                await beatmap.Backup();
                WORK_STATUS_TEXT.Text = "Parsing Beatmaps...";
                await beatmap.Parse();
                WORK_STATUS_TEXT.Text = "Making Beatmaps...";
                await beatmap.Make();
                await beatmap.Output();
                sw.Stop();
                WORK_STATUS_TEXT.Text = $"SV Applied! ({sw.ElapsedMilliseconds}ms)";
                System.Media.SystemSounds.Asterisk.Play();
                await Task.Delay(3000);
                _working = false;
            }
            catch (Exception error)
            {
                ErrorLogger(error);
                WORK_STATUS_TEXT.Text = "Error Occurred";
                WORK_STATUS_TEXT.ForeColor = Color.Red;
                System.Media.SystemSounds.Hand.Play();
                await Task.Delay(3000);
                _working = false;
            }
            finally
            {
                MAKE_BUTTON.Enabled = true;
                UNDO_BUTTON.Enabled = true;
            }
        }

        private static void ErrorLogger(Exception error)
        {
            string currentTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            File.WriteAllText("error.log", $"[{currentTime}] " + error.Message + "\n" + error.StackTrace);
        }

        private bool ValueChecker()
        {
            if (string.IsNullOrEmpty(SV_STARTTIME_TEXTBOX.Text) || string.IsNullOrEmpty(SV_ENDTIME_TEXTBOX.Text))
            {
                MessageBox.Show("開始時間と終了時間を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!int.TryParse(SV_STARTTIME_TEXTBOX.Text, out _) || !int.TryParse(SV_ENDTIME_TEXTBOX.Text, out _))
            {
                MessageBox.Show("開始時間か終了時間の入力形式が間違っています。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            if (int.Parse(SV_ENDTIME_TEXTBOX.Text) <= int.Parse(SV_STARTTIME_TEXTBOX.Text))
            {
                MessageBox.Show("終了時間を開始時間と同じ、もしくはより早くすることはできません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            if (int.Parse(SV_STARTTIME_TEXTBOX.Text) < 0 || int.Parse(SV_ENDTIME_TEXTBOX.Text) < 0)
            {
                MessageBox.Show("時間に負の値を入力することはできません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (SV_STARTTIME_TEXTBOX.Text == "0" && SV_ENDTIME_TEXTBOX.Text == "0")
            {
                MessageBox.Show("開始時間と終了時間に同時に0を入力することはできません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            if (string.IsNullOrEmpty(SV_START_TEXTBOX.Text) || string.IsNullOrEmpty(SV_END_TEXTBOX.Text))
            {
                MessageBox.Show("開始SVと終了SVを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            if (!double.TryParse(SV_START_TEXTBOX.Text, out _) || !double.TryParse(SV_END_TEXTBOX.Text, out _))
            {
                MessageBox.Show("開始SVか終了SVの入力形式が間違っています。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            if (SV_START_TEXTBOX.Text == "0" || SV_END_TEXTBOX.Text == "0")
            {
                MessageBox.Show("開始SVと終了SVに0を入力することはできません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (double.Parse(SV_START_TEXTBOX.Text) < 0 || double.Parse(SV_END_TEXTBOX.Text) < 0)
            {
                MessageBox.Show("SVに負の値を入力することはできません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            
            
            if (string.IsNullOrEmpty(VOLUME_START_TEXTBOX.Text) || string.IsNullOrEmpty(VOLUME_END_TEXTBOX.Text))
            {
                MessageBox.Show("開始ボリュームと終了ボリュームを入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            if (!int.TryParse(VOLUME_START_TEXTBOX.Text, out _) || !int.TryParse(VOLUME_END_TEXTBOX.Text, out _))
            {
                MessageBox.Show("開始ボリュームか終了ボリュームの入力形式が間違っています。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (int.Parse(VOLUME_START_TEXTBOX.Text) > 100) VOLUME_START_TEXTBOX.Text = "100";
            if (int.Parse(VOLUME_END_TEXTBOX.Text) > 100) VOLUME_END_TEXTBOX.Text = "100";
            if (int.Parse(VOLUME_START_TEXTBOX.Text) < 0) VOLUME_START_TEXTBOX.Text = "1";
            if (int.Parse(VOLUME_END_TEXTBOX.Text) < 0) VOLUME_END_TEXTBOX.Text = "1";
            
            if (int.TryParse(OFFSET_TEXTBOX.Text, out int offset))
            {
                if (offset < 0) OFFSET_TEXTBOX.Text = (-offset).ToString();
            }
            else
            {
                MessageBox.Show("オフセットの入力形式が間違っています。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!USE_CUSTOM_BPM_CHECKBOX.Checked) return true;
            if (!double.TryParse(BASE_BPM_TEXTBOX.Text, out _))
            {
                BASE_BPM_TEXTBOX.Text = "";
            }
            else if (double.Parse(BASE_BPM_TEXTBOX.Text) <= 0)
            {
                MessageBox.Show("ベースBPMに0以下の値を入力することはできません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void SET_START_TIME_BUTTON_Click(object sender, EventArgs e) => SV_STARTTIME_TEXTBOX.Text = _currentTime.ToString();

        private void SET_END_TIME_BUTTON_Click(object sender, EventArgs e) => SV_ENDTIME_TEXTBOX.Text = _currentTime.ToString();

        private void RESET_BUTTON_Click(object sender, EventArgs e)
        {
            SV_STARTTIME_TEXTBOX.Text = "";
            SV_ENDTIME_TEXTBOX.Text = "";
            SV_START_TEXTBOX.Text = "1";
            SV_END_TEXTBOX.Text = "1";
            VOLUME_START_TEXTBOX.Text = "100";
            VOLUME_END_TEXTBOX.Text = "100";
            OFFSET_TEXTBOX.Text = "0";
            KIAI_MODE_CHECKBOX.Checked = false;
            MODE_COMBOBOX.SelectedIndex = 0;
            SV_MODE_COMBOBOX.SelectedIndex = 0;
        }

        private async void UNDO_BUTTON_Click(object sender, EventArgs e)
        {
            MAKE_BUTTON.Enabled = false;
            UNDO_BUTTON.Enabled = false;
            string backupPath = Path.Combine("Backups", Path.GetFileNameWithoutExtension(_currentBeatmapPath));
            if (Directory.Exists(backupPath))
            {
                try
                {
                    _working = true;
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    WORK_STATUS_TEXT.Text = "Restoring...";
                    string[] files = Directory.GetFiles(backupPath);
                    string latestBackup = FindLatestFile(files);
                    if (string.IsNullOrEmpty(latestBackup))
                    {
                        sw.Stop();
                        WORK_STATUS_TEXT.Text = $"Backup not found! ({sw.ElapsedMilliseconds}ms)";
                        WORK_STATUS_TEXT.ForeColor = Color.Red;
                        System.Media.SystemSounds.Hand.Play();
                    }
                    else
                    {
                        File.Copy(latestBackup, _currentBeatmapPath, true);
                        File.Delete(latestBackup);
                        if (Directory.GetFiles(backupPath).Length == 0) Directory.Delete(backupPath);
                        sw.Stop();
                        WORK_STATUS_TEXT.Text = $"Restored! ({sw.ElapsedMilliseconds}ms)";
                        System.Media.SystemSounds.Asterisk.Play();
                    }
                    await Task.Delay(3000);
                    _working = false;
                }
                catch (Exception error)
                {
                    ErrorLogger(error);
                    WORK_STATUS_TEXT.Text = "Error Occurred";
                    WORK_STATUS_TEXT.ForeColor = Color.Red;
                    System.Media.SystemSounds.Hand.Play();
                    await Task.Delay(3000);
                    _working = false;
                }
                finally
                {
                    MAKE_BUTTON.Enabled = true;
                    UNDO_BUTTON.Enabled = true;
                }
            } 
            else
            {
                _working = true;
                WORK_STATUS_TEXT.Text = "Backup not found!";
                WORK_STATUS_TEXT.ForeColor = Color.Red;
                System.Media.SystemSounds.Hand.Play();
                await Task.Delay(3000);
                _working = false;
            }
        }

        private static string FindLatestFile(IEnumerable<string> files)
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

        private void OFFSET_CHECKBOX16_CheckedChanged(object sender, EventArgs e)
        {
            if (OFFSET_CHECKBOX16.Checked && OFFSET_CHECKBOX12.Checked) OFFSET_CHECKBOX12.Checked = false;
        }

        private void OFFSET_CHECKBOX12_CheckedChanged(object sender, EventArgs e)
        {
            if (OFFSET_CHECKBOX16.Checked && OFFSET_CHECKBOX12.Checked) OFFSET_CHECKBOX16.Checked = false;
        }

        private void BPM_COMP_CHECKBOX_CheckedChanged(object sender, EventArgs e)
        {
            if (!BPM_COMP_CHECKBOX.Checked) USE_CUSTOM_BPM_CHECKBOX.Checked = false;
            USE_CUSTOM_BPM_CHECKBOX.Visible = BPM_COMP_CHECKBOX.Checked;
            USE_CUSTOM_BPM_CHECKBOX.Visible = BPM_COMP_CHECKBOX.Checked;
        }

        private void USE_CUSTOM_BPM_CHECKBOX_CheckedChanged(object sender, EventArgs e)
        {
            BASE_BPM_LABEL.Visible = USE_CUSTOM_BPM_CHECKBOX.Checked;
            BASE_BPM_TEXTBOX.Visible = USE_CUSTOM_BPM_CHECKBOX.Checked;
            BASE_BPM_LABEL.Enabled = USE_CUSTOM_BPM_CHECKBOX.Checked;
            BASE_BPM_TEXTBOX.Enabled = USE_CUSTOM_BPM_CHECKBOX.Checked;
        }
    }

    public class BeatmapData
    {
        public string BeatmapStr { get; set; }
    }

    public class MemoryData
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Version { get; set; }
        public string BackgroundPath { get; set;}
        public string BeatmapStr { get; set; }
    }
}
