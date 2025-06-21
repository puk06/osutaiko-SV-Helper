namespace osu_taiko_SV_Helper.Models
{
    internal class TimingPoint
    {
        internal double Time { get; set; }
        internal double BeatLength { get; set; }
        internal int Meter { get; set; }
        internal int SampleSet { get; set; }
        internal int SampleIndex { get; set; }
        internal int Volume { get; set; }
        internal int Uninherited { get; set; }
        internal int Effects { get; set; }

        internal string GetString()
        {
            return $"{Time},{BeatLength},{Meter},{SampleSet},{SampleIndex},{Volume},{Uninherited},{Effects}";
        }
    }
}
