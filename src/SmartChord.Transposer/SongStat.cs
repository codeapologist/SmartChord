using SmartChord.Parser.Models;

namespace SmartChord.Transpose
{
    public class SongStat
    {
        public Note RootNote { get; set; }
        public Tone Tone { get; set; }
        public int Count { get; set; }
    }
}