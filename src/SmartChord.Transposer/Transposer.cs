using System;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<string> ResolveSongKey(string chordsheet)
        {
            var song = _parser.ParseSong(chordsheet);
            return await Task.Run( () => ResolveSongKey(song));
        }

        public async Task<string> ResolveSongKey(Song song)
        {
            var analyzer = new SongAnalyzer();
            var key = await Task.Run(() => analyzer.DiscoverKeyOfSong(song));
            if (key == Note.Unknown)
            {
                throw new InvalidOperationException("Chordsheet does not contain any valid chords.");
            }
            var originalKey = key.GetDisplayName();
            return originalKey;
        }

        public async Task<string> ChangeKey(string chordsheet, string destinationKey, string originalKey = null)
        {
            var song = _parser.ParseSong(chordsheet);

            foreach(var line in song.Lines)
            {
                if(line.Elements.Any(x => x is ChordElement) && line.Elements.Any(y => y is WordElement))
                {
                    var isLyricLine =  line.Elements
                        .Where(x => x is ChordElement)
                        .All(x => SongAnalyzer.AmbiguousChords.Contains(x.GetText()));

                    var newSongLine = new SongLine();

                    for (var i = 0; i < line.Elements.Count; i++)
                    {
                        if (line.Elements[i] is ChordElement)
                        {
                            var wordElement = new WordElement(line.Elements[i].GetText());
                            newSongLine.Elements.Add(wordElement);
                        }
                        else
                        {
                            newSongLine.Elements.Add(line.Elements[i]);
                        }

                    }

                    line.Overwrite(newSongLine);
                }
            }
            
            
            if (string.IsNullOrWhiteSpace(originalKey))
            {
                originalKey = await ResolveSongKey(song);
            }

            

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
