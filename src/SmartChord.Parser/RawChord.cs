namespace SmartChord.Parser
{
    public class RawChord
    {
        public string BaseNote { get; set; }
        public string Pitch { get; set; }
        public string Tone { get; set; }
        public string Added1 { get; set; }
        public string Sus { get; set; }
        public string SusPitch { get; set; }
        public string Added2 { get; set; }

        public override string ToString()
        {
            return string.Concat(BaseNote, Pitch, Tone, Added1, Sus, SusPitch, Added2);
        }
    }
}
