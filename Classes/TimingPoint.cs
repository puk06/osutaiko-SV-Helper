namespace osu_taiko_SV_Helper.Classes
{
    public class TimingPoint
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
}
