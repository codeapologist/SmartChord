using System;
using System.Linq;
using SmartChord.Parser;
using SmartChord.Parser.Models;
using SmartChord.Parser.Models.Elements;

namespace SmartChord.Transpose
{
    public class Transposer
    {
        private const int NumberOfNotes = 12;
        private readonly SongParser _parser;

        public Transposer() : this(new SongParser())
        { }

        public Transposer(SongParser parser)
        {
            _parser = parser;
        }

        public string ChangeKey(string chordsheet, string destinationKey)
        {
            var song = _parser.ParseSong(chordsheet);

            var analyzer = new SongAnalyzer();
            var key = analyzer.DiscoverKeyOfSong(song);
            if (key == Note.Unknown)
            {
                throw new InvalidOperationException("Chordsheet does not contain any valid chords.");
            }
            var originalKey = key.GetDisplayName();

            return ChangeKey(song, destinationKey, originalKey);
        }

        public string ChangeKey(string chordsheet, string destinationKey, string originalKey)
        {
            var song = _parser.ParseSong(chordsheet);
            return ChangeKey(song, destinationKey, originalKey);

        }

        public string ChangeKey(Song song, string destinationKey, string originalKey)
        {
            var noteDifference = destinationKey.ToNote() - originalKey.ToNote();

            var chordElements = from line in song.Lines
                from element in line.Elements.OfType<ChordElement>()
                select element;

            foreach (var chordElement in chordElements)
            {
                var originalIndex = (int)chordElement.Chord.RawRootNote.ToNote();
                var newIndex = originalIndex + noteDifference;
                if (newIndex < 0)
                {
                    newIndex = NumberOfNotes + newIndex;
                }
                else if (newIndex > NumberOfNotes - 1)
                {
                    newIndex = newIndex - NumberOfNotes;

                }
                var rootNote = ((Note)newIndex).GetDisplayName();

                if (rootNote.Any())
                {
                    chordElement.Chord.SetRootNote(rootNote);
                }
            }

            return song.ToString();
        }




    }
}
