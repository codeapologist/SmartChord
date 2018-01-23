using System.Collections.Generic;
using SmartChord.Parser.Models.Elements;

namespace SmartChord.Parser.Models
{
    public class SongLine
    {
        public SongLine PreviousLine { get; set; }
        public SongLine NextLine { get; set; }
        public List<BaseElement> Elements { get; set; } = new List<BaseElement>();

    }
}