using System.Collections.Generic;
using System.Linq;
using SmartChord.Parser;
using SmartChord.Parser.Models;
using SmartChord.Parser.Models.Elements;

namespace SmartChord.Transpose
{
    public class SongAnalyzer
    {

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
                             let wordElements = line.Elements.OfType<WordElement>().Where(x => x.IsSlash)
                             from element in line.Elements.OfType<ChordElement>()
                             where wordElements.Contains(element.PreviousElement)
                             select element);

            foreach (var bassNote in bassNotes)
            {
                bassNote.IsBassNote = true;
            }
        }
        private readonly List<string> _ambiguousChords = new List<string>() { "A", "Am", "Ab", "Go", "Do" };

        private void ResolveAmbiguousElements(Song song)
        {
            var results = from line in song.Lines
                          from element in line.Elements.OfType<ChordElement>()
                          where _ambiguousChords.Contains(element.Chord.ToString())
                          select new { element, line };

            var resultsToResolve = from result in results
                                   let nextWordElement = result.element.NextElement?.NextElement as WordElement
                                   let previousWordElement = result.element.PreviousElement?.PreviousElement as WordElement
                                   where nextWordElement != null && nextWordElement.IsFirstCharacterAlphaNumeric ||
                                         previousWordElement != null && previousWordElement.IsLastCharacterAlphaNumeric
                                   select result;

            foreach (var result in resultsToResolve)
            {
                var newElementList = (from element in result.line.Elements
                                      select element == result.element ? new WordElement(element.GetText()) : element)
                                     .ToList();

                result.line.Elements = new List<BaseElement>(newElementList);
            }

        }

        private List<SongStat> GetStatistics(Song song)
        {
            ResolveAmbiguousElements(song);
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
