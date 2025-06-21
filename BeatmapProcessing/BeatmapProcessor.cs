using osu_taiko_SV_Helper.Models;
using osu_taiko_SV_Helper.Utils;
using System.Text.RegularExpressions;

namespace osu_taiko_SV_Helper.BeatmapProcessing;

internal class BeatmapProcessor
{
    private string _beatmapPath = string.Empty;
    private BeatmapArgs _args = new BeatmapArgs();
    private string _rawbeatmap = string.Empty;
    private List<TimingPoint> _timingPoints = new List<TimingPoint>();
    private List<HitObject> _hitObjects = new List<HitObject>();
    private string[] _beatmap = Array.Empty<string>();
    private List<Bpm> _bpmList = new List<Bpm>();

    private const string TimingpointsSection = "[TimingPoints]";
    private const string ColoursSection = "[Colours]";
    private const string HitobjectsSection = "[HitObjects]";
    private readonly static string NewLine = Environment.NewLine;

    public Task BeatmapParser(string beatmapPath, BeatmapArgs args)
    {
        if (!File.Exists(beatmapPath)) throw new FileNotFoundException("Beatmap file not found.");

        _beatmapPath = beatmapPath;
        _args = args;
        _rawbeatmap = File.ReadAllText(_beatmapPath);
        _timingPoints = new List<TimingPoint>();
        _hitObjects = new List<HitObject>();
        _bpmList = new List<Bpm>();

        return Task.CompletedTask;
    }

    public Task Backup()
    {
        const string backupFolderPath = "./Backups/";
        if (!Directory.Exists(backupFolderPath))
        {
            Directory.CreateDirectory(backupFolderPath);
        }

        string backupPath = Path.GetFileNameWithoutExtension(_beatmapPath);
        if (!Directory.Exists(Path.Combine(backupFolderPath, backupPath)))
        {
            Directory.CreateDirectory(Path.Combine(backupFolderPath, backupPath));
        }

        string fileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff") + ".osu";
        File.Copy(_beatmapPath, Path.Combine(backupFolderPath, backupPath, fileName), true);

        return Task.CompletedTask;
    }

    public Task Parse()
    {
        _beatmap = _rawbeatmap
            .Split(NewLine, StringSplitOptions.None)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        int timingPointsIndex = Array.IndexOf(_beatmap, TimingpointsSection);
        int coloursIndex = Array.IndexOf(_beatmap, ColoursSection);
        int hitObjectsIndex = Array.IndexOf(_beatmap, HitobjectsSection);

        _timingPoints = _beatmap
            .Skip(timingPointsIndex + 1)
            .Take(coloursIndex == -1 ? hitObjectsIndex - timingPointsIndex - 1 : coloursIndex - timingPointsIndex - 1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                string[] objects = line.Split(',');
                if (objects.Length != 8)
                {
                    throw new Exception("Invalid Timingpoint Format: " + line);
                }

                return new TimingPoint
                {
                    Time = MathUtils.DoubleParse(objects[0]),
                    BeatLength = MathUtils.DoubleParse(objects[1]),
                    Meter = MathUtils.IntParse(objects[2]),
                    SampleSet = MathUtils.IntParse(objects[3]),
                    SampleIndex = MathUtils.IntParse(objects[4]),
                    Volume = MathUtils.IntParse(objects[5]),
                    Uninherited = MathUtils.IntParse(objects[6]),
                    Effects = MathUtils.IntParse(objects[7])
                };
            })
            .ToList();

        _hitObjects = _beatmap
            .Skip(hitObjectsIndex + 1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                string[] objects = line.Split(',');
                return new HitObject
                {
                    Time = MathUtils.IntParse(objects[2])
                };
            })
            .ToList();

        foreach (var timing in _timingPoints.Where(timing => timing.Uninherited == 1))
        {
            _bpmList.Add(new Bpm
            {
                Time = timing.Time,
                Value = 60000 / timing.BeatLength
            });
        }

        return Task.CompletedTask;
    }

    public Task Make()
    {
        List<int> hitObjectsTime = _hitObjects
            .Where(element => element.Time >= _args.Point.start)
            .TakeWhile(element => element.Time <= _args.Point.end)
            .Select(element => element.Time)
            .ToList();

        if (hitObjectsTime.Count < 2) throw new Exception("HitObjects count is less than 2.");

        int startTime = hitObjectsTime.First();
        int endTime = hitObjectsTime.Last();

        int pointStart = _args.Point.start;
        int pointEnd = _args.Point.end;

        double svStart = _args.Sv.start;
        double svEnd = _args.Sv.end;

        int volumeStart = _args.Volume.start;
        int volumeEnd = _args.Volume.end;

        double baseBpm = _args.BaseBpm;

        List<HitObject> allHitObjects = _hitObjects
            .Where(element => element.Time >= _args.Point.start)
            .TakeWhile(element => element.Time <= _args.Point.end)
            .ToList();

        switch (_args.SvMode)
        {
            case 0:
                {
                    Bpm? bpmAtStart = _bpmList.LastOrDefault(bpm => bpm.Time <= startTime);
                    double startBpm = bpmAtStart == null ? 0 : bpmAtStart.Value;
                    double commonSvRatio = (svEnd - svStart) / (endTime - startTime);
                    double commonVolumeRatio = (double)(volumeEnd - volumeStart) / (endTime - startTime);

                    foreach (var hitObject in allHitObjects)
                    {
                        Bpm? bpmAtCurrent = _bpmList.LastOrDefault(bpm => bpm.Time <= hitObject.Time);
                        double currentBpm = bpmAtCurrent == null ? 0 : bpmAtCurrent.Value;

                        if (_args.Offset16) _args.Offset = (int)Math.Round(60000 / currentBpm / 16);
                        if (_args.Offset12) _args.Offset = (int)Math.Round(60000 / currentBpm / 12);

                        if (_timingPoints.Any(timingPoint => timingPoint.Time == hitObject.Time - _args.Offset && timingPoint.Uninherited == 0))
                        {
                            switch (_args.Mode)
                            {
                                case 0:
                                    _timingPoints.RemoveAll
                                    (
                                        timingPoint =>
                                            timingPoint.Time == hitObject.Time - _args.Offset &&
                                            timingPoint.Uninherited == 0
                                    );
                                    break;
                                case 2:
                                    continue;
                                case 3:
                                    var prevTimingPoint = _timingPoints.Find
                                    (
                                        timingPoint =>
                                            timingPoint.Time == hitObject.Time - _args.Offset &&
                                            timingPoint.Uninherited == 0
                                    );

                                    if (prevTimingPoint != null)
                                    {
                                        prevTimingPoint.BeatLength /= svStart + (commonSvRatio * (hitObject.Time - startTime));
                                    }
                                    continue;
                            }
                        }

                        TimingPoint currentTimingPoint = new TimingPoint
                        {
                            Time = hitObject.Time - _args.Offset,
                            BeatLength = -100 / (svStart + (commonSvRatio * (hitObject.Time - startTime))),
                            Meter = 4,
                            SampleSet = 1,
                            SampleIndex = 0,
                            Volume = (int)Math.Round(volumeStart + (commonVolumeRatio * (hitObject.Time - startTime))),
                            Uninherited = 0,
                            Effects = _args.IsKiaiMode ? 1 : 0
                        };

                        if (_args.BpmCompatibility && (int)Math.Round(startBpm) != (int)Math.Round(currentBpm))
                        {
                            currentTimingPoint.BeatLength *= baseBpm == 0 ? startBpm / currentBpm : baseBpm / currentBpm;
                        }

                        _timingPoints.Add(currentTimingPoint);
                    }

                    break;
                }

            case 1:
                {
                    Bpm? bpmAtStart = _bpmList.LastOrDefault(bpm => bpm.Time <= startTime);
                    double startBpm = bpmAtStart == null ? 0 : bpmAtStart.Value;
                    double commonVolumeRatio = (double)(volumeEnd - volumeStart) / (endTime - startTime);

                    foreach (var hitObject in allHitObjects)
                    {
                        Bpm? bpmAtCurrent = _bpmList.LastOrDefault(bpm => bpm.Time <= hitObject.Time);
                        double currentBpm = bpmAtCurrent == null ? 0 : bpmAtCurrent.Value;

                        if (_args.Offset16) _args.Offset = (int)Math.Round(60000 / currentBpm / 16);
                        if (_args.Offset12) _args.Offset = (int)Math.Round(60000 / currentBpm / 12);

                        if (_timingPoints.Any(timingPoint => timingPoint.Time == hitObject.Time - _args.Offset && timingPoint.Uninherited == 0))
                        {
                            switch (_args.Mode)
                            {
                                case 0:
                                    _timingPoints.RemoveAll
                                    (
                                        timingPoint =>
                                            timingPoint.Time == hitObject.Time - _args.Offset &&
                                            timingPoint.Uninherited == 0
                                    );
                                    break;
                                case 2:
                                    continue;
                                case 3:
                                    var prevTimingPoint = _timingPoints.Find
                                    (
                                        timingPoint =>
                                            timingPoint.Time == hitObject.Time - _args.Offset &&
                                            timingPoint.Uninherited == 0
                                    );

                                    double currentTime = hitObject.Time;

                                    double svDifference = (100.0 / svEnd) - (100.0 / svStart);
                                    double timeSpan = endTime - startTime;
                                    double timeProgress = currentTime - startTime;

                                    double interpolatedSv = (svDifference / timeSpan) * timeProgress + (100.0 / svStart);

                                    if (prevTimingPoint != null)
                                    {
                                        prevTimingPoint.BeatLength /= (100.0 / interpolatedSv);
                                    }
                                    continue;
                            }
                        }

                        double beatLength = -100 / (100 / ((((100 / svEnd) - (100 / svStart)) / (endTime - startTime) * (hitObject.Time - startTime)) + (100 / svStart)));

                        TimingPoint currentTimingPoint = new TimingPoint
                        {
                            Time = hitObject.Time - _args.Offset,
                            BeatLength = beatLength,
                            Meter = 4,
                            SampleSet = 1,
                            SampleIndex = 0,
                            Volume = (int)Math.Round(volumeStart + (commonVolumeRatio * (hitObject.Time - startTime))),
                            Uninherited = 0,
                            Effects = _args.IsKiaiMode ? 1 : 0
                        };

                        if (_args.BpmCompatibility && (int)Math.Round(startBpm) != (int)Math.Round(currentBpm))
                        {
                            currentTimingPoint.BeatLength *= baseBpm == 0 ? startBpm / currentBpm : baseBpm / currentBpm;
                        }

                        _timingPoints.Add(currentTimingPoint);
                    }

                    break;
                }

            case 2:
                {
                    Bpm? bpmAtStart = _bpmList.LastOrDefault(bpm => bpm.Time <= pointStart);
                    double startBpm = bpmAtStart == null ? 0 : bpmAtStart.Value;
                    double interval = 60000 / startBpm / 16;
                    double commonSvRatio = (svEnd - svStart) / (pointStart - pointStart);
                    double commonVolumeRatio = (double)(volumeEnd - volumeStart) / (pointStart - pointStart);

                    for (double i = pointStart; i <= pointStart; i += interval)
                    {
                        if (i > pointStart) break;

                        Bpm? bpmAtCurrent = _bpmList.LastOrDefault(bpm => bpm.Time <= i);
                        double currentBpm = bpmAtCurrent == null ? 0 : bpmAtCurrent.Value;

                        if (_args.Offset16) _args.Offset = (int)Math.Round(60000 / currentBpm / 16);
                        if (_args.Offset12) _args.Offset = (int)Math.Round(60000 / currentBpm / 12);

                        interval = 60000 / currentBpm / 16;

                        if (_timingPoints.Any(timingPoint => timingPoint.Time == (int)Math.Round(i - _args.Offset) && timingPoint.Uninherited == 0))
                        {
                            switch (_args.Mode)
                            {
                                case 0:
                                    _timingPoints.RemoveAll(timingPoint =>
                                        timingPoint.Time == (int)Math.Round(i - _args.Offset) &&
                                        timingPoint.Uninherited == 0);
                                    break;
                                case 2:
                                    continue;
                                case 3:
                                    var prevTimingPoint = _timingPoints.Find(timingPoint =>
                                        timingPoint.Time == (int)Math.Round(i - _args.Offset) &&
                                        timingPoint.Uninherited == 0);

                                    if (prevTimingPoint != null)
                                    {
                                        prevTimingPoint.BeatLength /= svStart + (commonSvRatio * (i - pointStart));
                                    }
                                    continue;
                            }
                        }

                        TimingPoint currentTimingPoint = new TimingPoint
                        {
                            Time = (int)Math.Round(i - _args.Offset),
                            BeatLength = -100 / (svStart + (commonSvRatio * (i - pointStart))),
                            Meter = 4,
                            SampleSet = 1,
                            SampleIndex = 0,
                            Volume = (int)Math.Round(volumeStart + (commonVolumeRatio * (i - pointStart))),
                            Uninherited = 0,
                            Effects = _args.IsKiaiMode ? 1 : 0
                        };

                        if (_args.BpmCompatibility && (int)Math.Round(startBpm) != (int)Math.Round(currentBpm))
                        {
                            currentTimingPoint.BeatLength *= baseBpm == 0 ? startBpm / currentBpm : baseBpm / currentBpm;
                        }

                        _timingPoints.Add(currentTimingPoint);
                    }

                    break;
                }

            default:
                throw new Exception("Invalid SV mode.");
        }

        _timingPoints.Sort((a, b) =>
        {
            if (a.Time == b.Time) return a.Uninherited - b.Uninherited;
            return a.Time.CompareTo(b.Time);
        });

        return Task.CompletedTask;
    }

    public Task Output()
    {
        var beatmapList = new List<string>(_beatmap);

        int timingPointsIndex = Array.IndexOf(_beatmap, TimingpointsSection);
        int coloursIndex = Array.IndexOf(_beatmap, ColoursSection);
        int hitObjectsIndex = Array.IndexOf(_beatmap, HitobjectsSection);

        beatmapList.RemoveRange(timingPointsIndex + 1, coloursIndex == -1 ? hitObjectsIndex - timingPointsIndex - 1 : coloursIndex - timingPointsIndex - 1);
        foreach (var timingPoint in _timingPoints)
        {
            beatmapList.Insert(timingPointsIndex + 1, timingPoint.GetString());
        }

        for (int i = 0; i < beatmapList.Count; i++)
        {
            if (!Regex.IsMatch(beatmapList[i], @"^\[.+\]$")) continue;
            beatmapList.Insert(i, "");
            i++;
        }

        _beatmap = beatmapList.ToArray();
        File.WriteAllLines(_beatmapPath, _beatmap);
        return Task.CompletedTask;
    }
}
