namespace osu_taiko_SV_Helper.Models
{
    internal class BeatmapArgs
    {
        internal (int start, int end) Point { get; set; }
        internal (double start, double end) Sv { get; set; }
        internal (int start, int end) Volume { get; set; }
        internal bool IsKiaiMode { get; set; }
        internal int SvMode { get; set; }
        internal int Offset { get; set; }
        internal int Mode { get; set; }
        internal bool Offset16 { get; set; }
        internal bool Offset12 { get; set; }
        internal bool BpmCompatibility { get; set; }
        internal double BaseBpm { get; set; }
    }
}
