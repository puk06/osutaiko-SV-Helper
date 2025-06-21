using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels;
using OsuParsers.Decoders;
using osu_taiko_SV_Helper.Models;
using osu_taiko_SV_Helper.Utils;
using osu_taiko_SV_Helper.BeatmapProcessing;

namespace osu_taiko_SV_Helper.Forms
{
    public partial class Main : Form
    {
        private const string CurrentVersion = "v1.0.2-Release";
        private readonly StructuredOsuMemoryReader _sreader = new StructuredOsuMemoryReader();
        private readonly OsuBaseAddresses _baseAddresses = new OsuBaseAddresses();
        private readonly MemoryData _memoryData = new MemoryData();
        private bool _working;
        private bool _isDirectoryLoaded;
        private int _currentTime;
        private string _osuDirectory;
        private string _songsPath;
        private string _currentBeatmapPath;
        private bool _memoryError;
        private bool _firstLoad = true;
        private bool _readBeatmapError;
        private string _preBackgroundPath;
        private readonly Dictionary<string, string> _configDictionary = new Dictionary<string, string>();

        public Main()
        {
            InitializeComponent();

            CheckConfig();

            Thread getMemoryDataThread = new Thread(UpdateMemoryData){ IsBackground = true };
            getMemoryDataThread.Start();

            Loop();
        }

        private void CheckConfig()
        {
            if (!File.Exists("Config.cfg"))
            {
                SV_MODE_COMBOBOX.SelectedIndex = 0;
                MODE_COMBOBOX.SelectedIndex = 0;
                GithubUtils.GithubUpdateChecker(CurrentVersion);
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

                    LogUtils.ShowErrorMessage("Config.cfgのDEFAULT_SV_MODEの値が不正であったため、初期値の0が適用されます。0、1、2のどれかを入力してください。");
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
                    LogUtils.ShowErrorMessage("Config.cfgのDEFAULT_MODEの値が不正であったため、初期値の0が適用されます。0、1、2、3のどれかを入力してください。");
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
            if (_configDictionary.TryGetValue("UPDATE_CHECK", out string test5) && test5 == "true")
            {
                GithubUtils.GithubUpdateChecker(CurrentVersion);
            }
        }

        private async void Loop()
        {
            while (true)
            {
                await Task.Delay(15);

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
                                control == VERSION_LABEL
                            )
                            {
                                continue;
                            }

                            control.Enabled = false;
                        }

                        WORK_STATUS_TEXT.Text = _readBeatmapError ? "Beatmap Error Occurred" : "Memory Error Occurred";
                        WORK_STATUS_TEXT.ForeColor = Color.Red;

                        continue;
                    }

                    if (_firstLoad) continue;

                    foreach (Control control in Controls)
                    {
                        if ((OFFSET_CHECKBOX16.Checked || OFFSET_CHECKBOX12.Checked) && control == OFFSET_TEXTBOX)
                        {
                            OFFSET_TEXTBOX.Enabled = false;
                            continue;
                        }

                        control.Enabled = true;
                    }

                    if (!_working)
                    {
                        WORK_STATUS_TEXT.Text = "Ready! Waiting for your operation...";
                        WORK_STATUS_TEXT.ForeColor = Color.LimeGreen;
                    }

                    void SetLabelText(Control label, string text)
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

                    SetLabelText(TITLE_LABEL, _memoryData.Title);
                    SetLabelText(ARTIST_LABEL, _memoryData.Artist);
                    SetLabelText(VERSION_LABEL, _memoryData.Version);
                    VERSION_LABEL.Location = new Point(390 - VERSION_LABEL.Width, 2);

                    if (_preBackgroundPath == _memoryData.BackgroundPath) continue;

                    if (Background_Picture_Box.Image != null)
                    {
                        Background_Picture_Box.Image.Dispose();
                        Background_Picture_Box.Image = null;
                    }

                    Background_Picture_Box.Image = FileUtils.GetBackgroundImage(_memoryData.BackgroundPath);
                    _preBackgroundPath = _memoryData.BackgroundPath;
                }
                catch (Exception error)
                {
                    LogUtils.DebugLogger(error.Message);
                    foreach (Control control in Controls)
                    {
                        if (control == WORK_STATUS_TEXT ||
                            control == WORK_STATUS_LABEL ||
                            control == Background_Picture_Box ||
                            control == TITLE_LABEL ||
                            control == ARTIST_LABEL ||
                            control == VERSION_LABEL) continue;
                        control.Enabled = false;
                    }

                    WORK_STATUS_TEXT.Text = "Error Occurred";
                    WORK_STATUS_TEXT.ForeColor = Color.Red;
                }
            }
        }

        private void UpdateMemoryData()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(15);

                    if (Process.GetProcessesByName("osu!").Length == 0) throw new Exception("osu! is not running.");
                    if (!_isDirectoryLoaded)
                    {
                        Process osuProcess = Process.GetProcessesByName("osu!")[0];
                        string tempOsuDirectory = Path.GetDirectoryName(osuProcess.MainModule.FileName);
                        LogUtils.DebugLogger($"osu! directory: {tempOsuDirectory}");
                        if (string.IsNullOrEmpty(tempOsuDirectory) || !Directory.Exists(tempOsuDirectory))
                            throw new Exception("osu! directory not found.");

                        _osuDirectory = tempOsuDirectory;
                        _songsPath = FileUtils.GetSongsFolderLocation(_osuDirectory);
                        _isDirectoryLoaded = true;
                        LogUtils.DebugLogger($"Songs folder: {_songsPath}");
                        LogUtils.DebugLogger("Directory Data initialized.");

                        _songsPath = FileUtils.GetSongsFolderLocation(_osuDirectory);
                        _isDirectoryLoaded = true;
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

                    if (osuBeatmapPath == _songsPath || _memoryData.BeatmapPath == osuBeatmapPath)
                    {
                        _memoryError = false;
                        continue;
                    }

                    _memoryData.BeatmapPath = osuBeatmapPath;
                    _currentBeatmapPath = osuBeatmapPath;
                    LogUtils.DebugLogger("Map change detected.");
                    LogUtils.DebugLogger($"Current beatmap path: {osuBeatmapPath}");

                    if (!File.Exists(osuBeatmapPath))
                    {
                        // Fix the beatmap path(idk why)
                        LogUtils.DebugLogger("Beatmap file not found. Trying to fix the path... (Attempting 1)");
                        osuBeatmapPath = Path.Combine(_songsPath ?? "", _baseAddresses.Beatmap.FolderName?.Trim() ?? "", _baseAddresses.Beatmap.OsuFileName ?? "");
                        LogUtils.DebugLogger($"Current beatmap path: {osuBeatmapPath}");

                        if (!File.Exists(osuBeatmapPath))
                        {
                            LogUtils.DebugLogger("Beatmap file not found. Trying to fix the path again... (Attempting 2)");
                            osuBeatmapPath = Path.Combine(_songsPath ?? "", _baseAddresses.Beatmap.FolderName ?? "", _baseAddresses.Beatmap.OsuFileName?.Trim() ?? "");
                            LogUtils.DebugLogger($"Current beatmap path: {osuBeatmapPath}");
                        }

                        if (!File.Exists(osuBeatmapPath))
                        {
                            LogUtils.DebugLogger("Beatmap file not found. Trying to fix the path again... (Attempting 3)");
                            osuBeatmapPath = Path.Combine(_songsPath ?? "", _baseAddresses.Beatmap.FolderName?.Trim() ?? "", _baseAddresses.Beatmap.OsuFileName.Trim() ?? "");
                            LogUtils.DebugLogger($"Current beatmap path: {osuBeatmapPath}");
                        }

                        if (File.Exists(osuBeatmapPath)) LogUtils.DebugLogger("Beatmap file found.");
                    }

                    if (!File.Exists(osuBeatmapPath))
                    {
                        _memoryError = true;
                        _readBeatmapError = true;
                        _currentBeatmapPath = osuBeatmapPath;
                        continue;
                    }

                    _currentBeatmapPath = osuBeatmapPath;

                    _readBeatmapError = false;

                    OsuParsers.Beatmaps.Beatmap beatmapData = BeatmapDecoder.Decode(osuBeatmapPath);
                    _memoryData.Title = beatmapData.MetadataSection.Title;
                    _memoryData.Artist = beatmapData.MetadataSection.Artist;
                    _memoryData.Version = beatmapData.MetadataSection.Version;

                    string backgroundPath = Path.Combine(_songsPath ?? "", _baseAddresses.Beatmap.FolderName ?? "",
                        beatmapData.EventsSection.BackgroundImage ?? "");
                    if (!File.Exists(backgroundPath)) backgroundPath = "./src/no background.png";
                    _memoryData.BackgroundPath = backgroundPath;

                    _memoryError = false;
                    if (_firstLoad) _firstLoad = false;
                }
                catch (Exception error)
                {
                    LogUtils.DebugLogger(error.Message);
                    _memoryError = true;
                }
            }
        }

        private async void MAKE_BUTTON_Click(object sender, EventArgs e)
        {
            MAKE_BUTTON.Enabled = false;
            UNDO_BUTTON.Enabled = false;

            var reasons = ValueChecker();
            if (reasons.Any())
            {
                LogUtils.ShowErrorMessage("Failed to apply SV. The reasons are as follows.\n" + string.Join("\n", reasons));
                return;
            }

            try
            {
                BeatmapArgs args = new BeatmapArgs
                {
                    Point = (MathUtils.IntParse(SV_STARTTIME_TEXTBOX.Text), MathUtils.IntParse(SV_ENDTIME_TEXTBOX.Text)),
                    Sv = (MathUtils.DoubleParse(SV_START_TEXTBOX.Text), MathUtils.DoubleParse(SV_END_TEXTBOX.Text)),
                    Volume = (MathUtils.IntParse(VOLUME_START_TEXTBOX.Text), MathUtils.IntParse(VOLUME_END_TEXTBOX.Text)),
                    IsKiaiMode = KIAI_MODE_CHECKBOX.Checked,
                    SvMode = SV_MODE_COMBOBOX.SelectedIndex,
                    Offset = MathUtils.IntParse(OFFSET_TEXTBOX.Text),
                    Mode = MODE_COMBOBOX.SelectedIndex,
                    Offset16 = OFFSET_CHECKBOX16.Checked,
                    Offset12 = OFFSET_CHECKBOX12.Checked,
                    BpmCompatibility = BPM_COMP_CHECKBOX.Checked,
                    BaseBpm = string.IsNullOrEmpty(BASE_BPM_TEXTBOX.Text) || !USE_CUSTOM_BPM_CHECKBOX.Checked || BASE_BPM_TEXTBOX.Text == "0" ? 0 : MathUtils.DoubleParse(BASE_BPM_TEXTBOX.Text)
                };

                _working = true;
                Stopwatch sw = new Stopwatch();
                sw.Start();

                WORK_STATUS_TEXT.Text = "Making...";
                WORK_STATUS_TEXT.ForeColor = Color.LimeGreen;

                BeatmapProcessor beatmapProcessor = new BeatmapProcessor();
                WORK_STATUS_TEXT.Text = "Preparing...";
                await beatmapProcessor.BeatmapParser(_currentBeatmapPath, args);

                WORK_STATUS_TEXT.Text = "Backing up Beatmaps...";
                await beatmapProcessor.Backup();

                WORK_STATUS_TEXT.Text = "Parsing Beatmaps...";
                await beatmapProcessor.Parse();

                WORK_STATUS_TEXT.Text = "Making Beatmaps...";
                await beatmapProcessor.Make();
                await beatmapProcessor.Output();

                sw.Stop();

                WORK_STATUS_TEXT.Text = $"SV Applied! ({sw.ElapsedMilliseconds}ms)";
                System.Media.SystemSounds.Asterisk.Play();
                await Task.Delay(3000);
                _working = false;
            }
            catch (Exception error)
            {
                LogUtils.DebugLogger(error.Message);
                LogUtils.ShowErrorMessage(error.Message);
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

        private IEnumerable<string> ValueChecker()
        {
            IEnumerable<string> reasons = Array.Empty<string>();
            const string TimeFormat = "^\\d+:\\d+:\\d+.+$";

            if (Regex.IsMatch(SV_STARTTIME_TEXTBOX.Text, TimeFormat))
            {
                string[] time = Regex.Replace(SV_STARTTIME_TEXTBOX.Text, "[^0-9:]", "").Split(':');
                if (!int.TryParse(time[0], out _) || !int.TryParse(time[1], out _) || !int.TryParse(time[2], out _))
                {
                    ArrayUtils.AddValueToArray(ref reasons, "❌️ 開始時間の入力形式が間違っています。");
                }

                int minute = MathUtils.IntParse(time[0]);
                int second = MathUtils.IntParse(time[1]);
                int millisecond = MathUtils.IntParse(time[2]);
                int totalMillisecond = (minute * 60000) + (second * 1000) + millisecond;
                SV_STARTTIME_TEXTBOX.Text = totalMillisecond.ToString();
            }

            if (Regex.IsMatch(SV_ENDTIME_TEXTBOX.Text, TimeFormat))
            {
                string[] time = Regex.Replace(SV_ENDTIME_TEXTBOX.Text, "[^0-9:]", "").Split(':');
                if (!int.TryParse(time[0], out _) || !int.TryParse(time[1], out _) || !int.TryParse(time[2], out _))
                {
                    ArrayUtils.AddValueToArray(ref reasons, "❌️ 終了時間の入力形式が間違っています。");
                }

                int minute = MathUtils.IntParse(time[0]);
                int second = MathUtils.IntParse(time[1]);
                int millisecond = MathUtils.IntParse(time[2]);
                int totalMillisecond = (minute * 60000) + (second * 1000) + millisecond;
                SV_ENDTIME_TEXTBOX.Text = totalMillisecond.ToString();
            }

            if (string.IsNullOrEmpty(SV_STARTTIME_TEXTBOX.Text) || string.IsNullOrEmpty(SV_ENDTIME_TEXTBOX.Text))
            {
                ArrayUtils.AddValueToArray(ref reasons, "❌️ 開始時間と終了時間を入力してください。");
            } 
            else
            {
                if (!int.TryParse(SV_STARTTIME_TEXTBOX.Text, out _) || !int.TryParse(SV_ENDTIME_TEXTBOX.Text, out _))
                {
                    ArrayUtils.AddValueToArray(ref reasons, "❌️ 開始時間か終了時間の入力形式が間違っています。");
                }

                if (MathUtils.IntParse(SV_ENDTIME_TEXTBOX.Text) <= MathUtils.IntParse(SV_STARTTIME_TEXTBOX.Text))
                {
                    ArrayUtils.AddValueToArray(ref reasons, "❌️ 終了時間を開始時間と同じ、もしくはより早くすることはできません。");
                }

                if (MathUtils.IntParse(SV_STARTTIME_TEXTBOX.Text) < 0 || MathUtils.IntParse(SV_ENDTIME_TEXTBOX.Text) < 0)
                {
                    ArrayUtils.AddValueToArray(ref reasons, "❌️ 時間に負の値を入力することはできません。");
                }

                if (SV_STARTTIME_TEXTBOX.Text == "0" && SV_ENDTIME_TEXTBOX.Text == "0")
                {
                    ArrayUtils.AddValueToArray(ref reasons, "❌️ 開始時間と終了時間に同時に0を入力することはできません。");
                }
            }

            if (string.IsNullOrEmpty(SV_START_TEXTBOX.Text) || string.IsNullOrEmpty(SV_END_TEXTBOX.Text))
            {
                ArrayUtils.AddValueToArray(ref reasons, "❌️ 開始SVと終了SVを入力してください。");
            } 
            else
            {
                if (!double.TryParse(SV_START_TEXTBOX.Text, out _) || !double.TryParse(SV_END_TEXTBOX.Text, out _))
                {
                    ArrayUtils.AddValueToArray(ref reasons, "❌️ 開始SVか終了SVの入力形式が間違っています。");
                }

                if (SV_START_TEXTBOX.Text == "0" || SV_END_TEXTBOX.Text == "0")
                {
                    ArrayUtils.AddValueToArray(ref reasons, "❌️ 開始SVと終了SVに0を入力することはできません。");
                }

                if (MathUtils.DoubleParse(SV_START_TEXTBOX.Text) < 0 || MathUtils.DoubleParse(SV_END_TEXTBOX.Text) < 0)
                {
                    ArrayUtils.AddValueToArray(ref reasons, "❌️ SVに負の値を入力することはできません。");
                }
            }

            if (string.IsNullOrEmpty(VOLUME_START_TEXTBOX.Text) || string.IsNullOrEmpty(VOLUME_END_TEXTBOX.Text))
            {
                ArrayUtils.AddValueToArray(ref reasons, "❌️ 開始ボリュームと終了ボリュームを入力してください。");
            }
            else
            {
                if (!int.TryParse(VOLUME_START_TEXTBOX.Text, out _) || !int.TryParse(VOLUME_END_TEXTBOX.Text, out _))
                {
                    ArrayUtils.AddValueToArray(ref reasons, "❌️ 開始ボリュームか終了ボリュームの入力形式が間違っています。");
                }

                if (MathUtils.IntParse(VOLUME_START_TEXTBOX.Text) > 100) VOLUME_START_TEXTBOX.Text = "100";
                if (MathUtils.IntParse(VOLUME_END_TEXTBOX.Text) > 100) VOLUME_END_TEXTBOX.Text = "100";
                if (MathUtils.IntParse(VOLUME_START_TEXTBOX.Text) < 0) VOLUME_START_TEXTBOX.Text = "1";
                if (MathUtils.IntParse(VOLUME_END_TEXTBOX.Text) < 0) VOLUME_END_TEXTBOX.Text = "1";
            }

            if (int.TryParse(OFFSET_TEXTBOX.Text, out int offset))
            {
                if (offset < 0) OFFSET_TEXTBOX.Text = (-offset).ToString();
            }
            else
            {
                OFFSET_TEXTBOX.Text = "0";
                ArrayUtils.AddValueToArray(ref reasons, "❌️ オフセットの入力形式が間違っています。");
            }

            if (!USE_CUSTOM_BPM_CHECKBOX.Checked) return reasons;

            if (!double.TryParse(BASE_BPM_TEXTBOX.Text, out _))
            {
                BASE_BPM_TEXTBOX.Text = "";
            }
            else if (MathUtils.DoubleParse(BASE_BPM_TEXTBOX.Text) <= 0)
            {
                BASE_BPM_TEXTBOX.Text = "";
                ArrayUtils.AddValueToArray(ref reasons, "❌️ ベースBPMに0以下の値を入力することはできません。");
            }

            return reasons;
        }

        private void SET_START_TIME_BUTTON_Click(object sender, EventArgs e)
            => SV_STARTTIME_TEXTBOX.Text = _currentTime.ToString();

        private void SET_END_TIME_BUTTON_Click(object sender, EventArgs e) 
            => SV_ENDTIME_TEXTBOX.Text = _currentTime.ToString();

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
                    string latestBackup = FileUtils.FindLatestFile(files);
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
                    LogUtils.DebugLogger(error.Message);
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

        private void ARROW_LABEL1_Click(object sender, EventArgs e)
        {
            (SV_STARTTIME_TEXTBOX.Text, SV_ENDTIME_TEXTBOX.Text) = (SV_ENDTIME_TEXTBOX.Text, SV_STARTTIME_TEXTBOX.Text);
        }

        private void ARROW_LABEL2_Click(object sender, EventArgs e)
        {
            (SV_START_TEXTBOX.Text, SV_END_TEXTBOX.Text) = (SV_END_TEXTBOX.Text, SV_START_TEXTBOX.Text);
        }

        private void ARROW_LABEL3_Click(object sender, EventArgs e)
        {
            (VOLUME_START_TEXTBOX.Text, VOLUME_END_TEXTBOX.Text) = (VOLUME_END_TEXTBOX.Text, VOLUME_START_TEXTBOX.Text);
        }
    }
}
