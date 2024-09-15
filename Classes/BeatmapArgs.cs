using System.Collections.Generic;

namespace osu_taiko_SV_Helper.Classes
{
    public class BeatmapArgs
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
}
