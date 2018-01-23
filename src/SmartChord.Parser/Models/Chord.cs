using System;
using System.Collections.Generic;

namespace SmartChord.Parser.Models
{
    public class Chord
    {
        private readonly Dictionary<string, Tone> _toneMap = new Dictionary<string, Tone>
        {
            {"Minor", Tone.Minor},
            {"minor", Tone.Minor},
            {"min", Tone.Minor},
            {"m", Tone.Minor},

            { "Major", Tone.Major},
            {"Maj", Tone.Major},
            {"maj", Tone.Major},
            {"major", Tone.Major},
            {"M", Tone.Major},

            {"dim", Tone.Diminished},
            {"o", Tone.Diminished},

            { "aug", Tone.Augmented},
            {"+", Tone.Augmented},

        };

        public string RawRootNote { get; private set; }
        public string RawTone { get; private set; }
        public Note RootNote { get; private set; }
        public Tone Tone { get; private set; }
        public string Added1 { get; private set; }
        public string Sus { get; private set; }
        public string SusPitch { get; private set; }
        public string Added2 { get; private set; }
        public string BaseChord => $"{RootNote.GetDisplayName()}{ GetTone()}";
       
        public Chord(RawChord raw)
        {
            RawRootNote = $"{raw.BaseNote}{raw.Pitch}";
            RawTone = raw.Tone;

            RootNote = RawRootNote.ToNote();

            if (RawTone != null)
            {
                Tone = MapTone(RawTone);
            }

            Added1 = raw.Added1;
            Sus = raw.Sus;
            SusPitch = raw.SusPitch;
            Added2 = raw.Added2;
        }

        public void SetRootNote(string note)
        {
            var newRootNote = note.ToNote();

            RootNote = newRootNote;
            RawRootNote = note;
        }

        private string GetTone()
        {
            return Tone == Tone.Major ? string.Empty : Tone.ToString();
        }

        private Tone MapTone(string rawTone)
        {
            if (_toneMap.TryGetValue(rawTone, out var tone))
            {
                return tone;
            }

            throw new InvalidOperationException($"{rawTone} is not a valid tone.");
        }

        public override string ToString()
        {
            return string.Concat(RawRootNote, RawTone, Added1, Sus, SusPitch, Added2);
        }

    }
}