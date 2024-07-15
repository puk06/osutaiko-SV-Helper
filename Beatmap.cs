using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace osu_taiko_SV_Helper
{
    internal class Beatmap
    {
        private string _beatmapPath;
        private BeatmapArgs _args;
        private string _rawbeatmap;
        private List<TimingPoint> _timingPoints;
        private List<HitObject> _hitObjects;
        private string[] _beatmap;
        private List<Bpm> _bpmList;

        private const string TimingpointsSection = "[TimingPoints]";
        private const string HitobjectsSection = "[HitObjects]";

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
            if (!Directory.Exists(backupFolderPath)) Directory.CreateDirectory(backupFolderPath);
            string backupPath = Path.GetFileNameWithoutExtension(_beatmapPath);
            if (!Directory.Exists(Path.Combine(backupFolderPath, backupPath)))
                Directory.CreateDirectory(Path.Combine(backupFolderPath, backupPath));
            string fileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff") + ".osu";
            File.Copy(_beatmapPath, Path.Combine(backupFolderPath, backupPath, fileName), true);
            return Task.CompletedTask;
        }

        public Task Parse()
        {
            _beatmap = _rawbeatmap
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Where(line => line != "")
                .ToArray();

            int timingPointsIndex = Array.IndexOf(_beatmap, TimingpointsSection);
            int hitObjectsIndex = Array.IndexOf(_beatmap, HitobjectsSection);

            _timingPoints = _beatmap
                .Skip(timingPointsIndex + 1)
                .Take(hitObjectsIndex - timingPointsIndex - 1)
                .Where(line => line != "")
                .Select(line =>
                {
                    string[] objects = line.Split(',');
                    return new TimingPoint
                    {
                        Time = int.Parse(objects[0]),
                        BeatLength = double.Parse(objects[1]),
                        Meter = int.Parse(objects[2]),
                        SampleSet = int.Parse(objects[3]),
                        SampleIndex = int.Parse(objects[4]),
                        Volume = int.Parse(objects[5]),
                        Uninherited = int.Parse(objects[6]),
                        Effects = int.Parse(objects[7])
                    };
                })
                .ToList();

            _hitObjects = _beatmap
                .Skip(hitObjectsIndex + 1)
                .Where(line => line != "")
                .Select(line =>
                {
                    string[] objects = line.Split(',');
                    return new HitObject
                    {
                        Time = int.Parse(objects[2])
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
            switch (_args.SvMode)
            {
                case 0:
                {
                    var hitObjectsTime = _hitObjects.Where(element => element.Time >= _args.Point[0]).TakeWhile(element => element.Time <= _args.Point[1]).Select(element => element.Time).ToList();
                    if (hitObjectsTime.Count < 2) throw new Exception("HitObjects count is less than 2.");
                    hitObjectsTime = new List<int> { hitObjectsTime[0], hitObjectsTime[hitObjectsTime.Count - 1] };

                    double baseBpm = _args.BaseBpm;
                    double startBpm = _bpmList.Last(bpm => bpm.Time <= hitObjectsTime[0]).Value;
                    double commonSvRatio = (_args.Sv[1] - _args.Sv[0]) / (hitObjectsTime[1] - hitObjectsTime[0]);
                    double commonVolumeRatio = (double)(_args.Volume[1] - _args.Volume[0]) / (hitObjectsTime[1] - hitObjectsTime[0]);

                    foreach (var element in _hitObjects.Where(element => element.Time >= _args.Point[0]).TakeWhile(element => element.Time <= _args.Point[1]))
                    {
                        double currentBpm = _bpmList.Last(bpm => bpm.Time <= element.Time).Value;
                        if (_args.Offset16) _args.Offset = (int)Math.Round(60000 / currentBpm / 16);
                        if (_args.Offset12) _args.Offset = (int)Math.Round(60000 / currentBpm / 12);
                        if (_timingPoints.Any(timingPoint => timingPoint.Time == element.Time - _args.Offset && timingPoint.Uninherited == 0))
                        {
                            switch (_args.Mode)
                            {
                                case 0:
                                    _timingPoints.RemoveAll(timingPoint => timingPoint.Time == element.Time - _args.Offset && timingPoint.Uninherited == 0);
                                    break;
                                case 2:
                                    continue;
                                case 3:
                                    var prevTimingPoint = _timingPoints.Find(timingPoint => timingPoint.Time == element.Time - _args.Offset && timingPoint.Uninherited == 0);
                                    prevTimingPoint.BeatLength /= _args.Sv[0] + (commonSvRatio * (element.Time - hitObjectsTime[0]));
                                    continue;
                            }
                        }

                        TimingPoint currentTimingPoint = new TimingPoint
                        {
                            Time = element.Time - _args.Offset,
                            BeatLength = -100 / (_args.Sv[0] + (commonSvRatio * (element.Time - hitObjectsTime[0]))),
                            Meter = 4,
                            SampleSet = 1,
                            SampleIndex = 0,
                            Volume = (int)Math.Round(_args.Volume[0] + (commonVolumeRatio * (element.Time - hitObjectsTime[0]))),
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
                    var hitObjectsTime = _hitObjects.Where(element => element.Time >= _args.Point[0]).TakeWhile(element => element.Time <= _args.Point[1]).Select(element => element.Time).ToList();
                    if (hitObjectsTime.Count < 2) throw new Exception("HitObjects count is less than 2.");
                    hitObjectsTime = new List<int> { hitObjectsTime[0], hitObjectsTime[hitObjectsTime.Count - 1] };

                    double baseBpm = _args.BaseBpm;
                    double startBpm = _bpmList.Last(bpm => bpm.Time <= hitObjectsTime[0]).Value;
                    double commonVolumeRatio = (double)(_args.Volume[1] - _args.Volume[0]) / (hitObjectsTime[1] - hitObjectsTime[0]);

                    foreach (var element in _hitObjects.Where(element => element.Time >= _args.Point[0]).TakeWhile(element => element.Time <= _args.Point[1]))
                    {
                        double currentBpm = _bpmList.Last(bpm => bpm.Time <= element.Time).Value;
                        if (_args.Offset16) _args.Offset = (int)Math.Round(60000 / currentBpm / 16);
                        if (_args.Offset12) _args.Offset = (int)Math.Round(60000 / currentBpm / 12);
                        if (_timingPoints.Any(timingPoint => timingPoint.Time == element.Time - _args.Offset && timingPoint.Uninherited == 0))
                        {
                            switch (_args.Mode)
                            {
                                case 0:
                                    _timingPoints.RemoveAll(timingPoint => timingPoint.Time == element.Time - _args.Offset && timingPoint.Uninherited == 0);
                                    break;
                                case 2:
                                    continue;
                                case 3:
                                    var prevTimingPoint = _timingPoints.Find(timingPoint => timingPoint.Time == element.Time - _args.Offset && timingPoint.Uninherited == 0);
                                    prevTimingPoint.BeatLength /= 100 / ((((100 / _args.Sv[1]) - (100 / _args.Sv[0])) / (hitObjectsTime[1] - hitObjectsTime[0]) * (element.Time - hitObjectsTime[0])) + (100 / _args.Sv[0]));
                                    continue;
                            }
                        }

                        TimingPoint currentTimingPoint = new TimingPoint
                        {
                            Time = element.Time - _args.Offset,
                            BeatLength = -100 / (100 / ((((100 / _args.Sv[1]) - (100 / _args.Sv[0])) / (hitObjectsTime[1] - hitObjectsTime[0]) * (element.Time - hitObjectsTime[0])) + (100 / _args.Sv[0]))),
                            Meter = 4,
                            SampleSet = 1,
                            SampleIndex = 0,
                            Volume = (int)Math.Round(_args.Volume[0] + (commonVolumeRatio * (element.Time - hitObjectsTime[0]))),
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
                    double baseBpm = _args.BaseBpm;
                    double startBpm = _bpmList.Last(bpm => bpm.Time <= _args.Point[0]).Value;
                    double interval = 60000 / startBpm / 16;
                    double commonSvRatio = (_args.Sv[1] - _args.Sv[0]) / (_args.Point[1] - _args.Point[0]);
                    double commonVolumeRatio = (double)(_args.Volume[1] - _args.Volume[0]) / (_args.Point[1] - _args.Point[0]);

                    for (var i = (double)_args.Point[0]; i <= _args.Point[1]; i += interval)
                    {
                        if (i > _args.Point[1]) break;
                        double currentBpm = _bpmList.Last(bpm => bpm.Time <= i).Value;
                        if (_args.Offset16) _args.Offset = (int)Math.Round(60000 / currentBpm / 16);
                        if (_args.Offset12) _args.Offset = (int)Math.Round(60000 / currentBpm / 12);

                        interval = 60000 / currentBpm / 16;

                        if (_timingPoints.Any(timingPoint => timingPoint.Time == (int)Math.Round(i - _args.Offset) && timingPoint.Uninherited == 0))
                        {
                            switch (_args.Mode)
                            {
                                case 0:
                                    _timingPoints.RemoveAll(timingPoint => timingPoint.Time == (int)Math.Round(i - _args.Offset) && timingPoint.Uninherited == 0);
                                    break;
                                case 2:
                                    continue;
                                case 3:
                                    var prevTimingPoint = _timingPoints.Find(timingPoint => timingPoint.Time == (int)Math.Round(i - _args.Offset) && timingPoint.Uninherited == 0);
                                    prevTimingPoint.BeatLength /= _args.Sv[0] + (commonSvRatio * (i - _args.Point[0]));
                                    continue;
                            }
                        }

                        TimingPoint currentTimingPoint = new TimingPoint
                        {
                            Time = (int)Math.Round(i - _args.Offset),
                            BeatLength = -100 / (_args.Sv[0] + (commonSvRatio * (i - _args.Point[0]))),
                            Meter = 4,
                            SampleSet = 1,
                            SampleIndex = 0,
                            Volume = (int)Math.Round(_args.Volume[0] + (commonVolumeRatio * (i - _args.Point[0]))),
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
                return b.Time - a.Time;
            });
            return Task.CompletedTask;
        }

        public Task Output()
        {
            var beatmapList = new List<string>(_beatmap);

            int timingPointsIndex = Array.IndexOf(_beatmap, TimingpointsSection);
            int hitObjectsIndex = Array.IndexOf(_beatmap, HitobjectsSection);

            beatmapList.RemoveRange(timingPointsIndex + 1, hitObjectsIndex - timingPointsIndex - 1);

            foreach (var timingPoint in _timingPoints)
            {
                beatmapList.Insert(timingPointsIndex + 1, $"{timingPoint.Time},{timingPoint.BeatLength},{timingPoint.Meter},{timingPoint.SampleSet},{timingPoint.SampleIndex},{timingPoint.Volume},{timingPoint.Uninherited},{timingPoint.Effects}");
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

    internal class TimingPoint
    {
        public int Time { get; set; }
        public double BeatLength { get; set; }
        public int Meter { get; set; }
        public int SampleSet { get; set; }
        public int SampleIndex { get; set; }
        public int Volume { get; set; }
        public int Uninherited { get; set; }
        public int Effects { get; set; }
    }

    internal class BeatmapArgs
    {
        public List<int> Point { get; set; }
        public List<double> Sv { get; set; }
        public List<int> Volume { get; set; }
        public bool IsKiaiMode { get; set; }
        public int SvMode { get; set; }
        public int Offset { get; set; }
        public int Mode { get; set; }
        public bool Offset16 { get; set; }
        public bool Offset12 { get; set; }
        public bool BpmCompatibility { get; set; }
        public double BaseBpm { get; set; }
    }

    internal class HitObject
    {
        public int Time { get; set; }
    }

    internal class Bpm
    {
        public int Time { get; set; }
        public double Value { get; set; }
    }
}
