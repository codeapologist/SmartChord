using System.Collections.Generic;
using System.Linq;
using SmartChord.Parser;
using SmartChord.Parser.Models;
using SmartChord.Parser.Models.Elements;

namespace SmartChord.Transpose
{
    public class SongAnalyzer
    {
        public static readonly List<string> AmbiguousChords = new List<string>() { "A", "Am", "Ab", "Go", "Do" };

        public Note DiscoverKeyOfSong(Song song)
        {
            var stats = GetStatistics(song);

            var values = Extensions.GetValues<Note>();
            int highestScore = 0;
            var result = Note.Unknown;
            foreach (var key in values)
            {
                var scoreCard = new KeyScoreCard(key);
                var score = scoreCard.Score(stats);
                if (score > highestScore)
                {
                    highestScore = score;
                    result = key;
                }
            }

            return result;
        }

        private void MarkBassNotes(Song song)
        {
            var bassNotes = (from line in song.Lines
                             let slashElements = line.Elements.OfType<NonAlphaElement>().Where(x => x.IsSlash)
                             from element in line.Elements.OfType<ChordElement>()
                             where slashElements.Contains(element.PreviousElement)
                             select element);

            foreach (var bassNote in bassNotes)
            {
                bassNote.IsBassNote = true;
            }
        }


        private List<SongStat> GetStatistics(Song song)
        {
            MarkBassNotes(song);

            return (from line in song.Lines
                    from element in line.Elements.OfType<ChordElement>()
                    where !element.IsBassNote
                    group element by element.Chord.BaseChord
                into g
                    select new SongStat
                    {
                        RootNote = g.First().Chord.RootNote,
                        Tone = g.First().Chord.Tone,
                        Count = g.Count()
                    }).ToList();

        }


    }
}
