using System.Collections.Generic;
using System.Linq;

namespace SmartChord.Parser.Models
{
    public class Song
    {
        public List<SongLine> Lines { get; } = new List<SongLine>();

        public override string ToString()
        {
            var lines = new List<string>();

            foreach (var songLine in Lines)
            {
                var elementList = songLine.Elements
                    .Select(x => x.GetText())
                    .ToList();

                lines.Add(string.Join(null, elementList));
            }

            return string.Join("\r\n", lines);
        }
    }
}