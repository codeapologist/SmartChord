using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SmartChord.Parser.Models;
using SmartChord.Parser.Models.Elements;
using Sprache;

namespace SmartChord.Parser
{
    public class SongParser
    {
        private static readonly Parser<RawChord> AbsoluteChordParser;

        static SongParser()
        {
            Parser<char> baseNote = DefineBaseNote();

            Parser<string> pitch = DefinePitch();

            Parser<string> tone = DefineTone();

            Parser<string> added1 = DefineAdded1();

            Parser<string> sus = DefineSus();

            Parser<string> addedPitch = DefineAddedPitch();

            Parser<string> added2 = DefineAdded2();

            AbsoluteChordParser =
               from b in baseNote
               from p in pitch.Optional()
               from t in tone.Optional()
               from a1 in added1.Optional()
               from s in sus.Optional()
               from ap in addedPitch.Optional()
               from a2 in added2.Optional().End()
               select new RawChord()
               {
                   BaseNote = b.ToString(),
                   Pitch = p.GetOrDefault(),
                   Tone = t.GetOrDefault(),
                   Added1 = a1.GetOrDefault(),
                   Sus = s.GetOrDefault(),
                   SusPitch = ap.GetOrDefault(),
                   Added2 = a2.GetOrDefault()
               };

        }

        private static Parser<string> DefineAdded2()
        {
            return Parse.String("2").Text()
                .Or(Parse.String("4").Text())
                .Or(Parse.String("5").Text())
                .Or(Parse.String("6").Text())
                .Or(Parse.String("7").Text())
                .Or(Parse.String("9").Text())
                .Or(Parse.String("11").Text())
                .Or(Parse.String("13").Text());
        }

        private static Parser<string> DefineAddedPitch()
        {
            return Parse.String("#").Text()
                .Or(Parse.String("b").Text());
        }

        private static Parser<string> DefineSus()
        {
            return Parse.String("sus").Text()
                .Or(Parse.String("add").Text());
        }

        private static Parser<string> DefineAdded1()
        {
            return Parse.String("2").Text()
                .Or(Parse.String("4").Text())
                .Or(Parse.String("5").Text())
                .Or(Parse.String("6").Text())
                .Or(Parse.String("7").Text())
                .Or(Parse.String("9").Text())
                .Or(Parse.String("11").Text())
                .Or(Parse.String("13").Text());
        }

        private static Parser<string> DefineTone()
        {
            return Parse.String("Major").Text()
                .Or(Parse.String("Minor").Text())
                .Or(Parse.String("major").Text())
                .Or(Parse.String("minor").Text())
                .Or(Parse.String("maj").Text())
                .Or(Parse.String("Maj").Text())
                .Or(Parse.String("min").Text())
                .Or(Parse.String("Min").Text())
                .Or(Parse.String("dim").Text())
                .Or(Parse.String("aug").Text())
                .Or(Parse.String("M").Text())
                .Or(Parse.String("m").Text())
                .Or(Parse.String("+").Text())
                .Or(Parse.String("o").Text());
        }

        private static Parser<string> DefinePitch()
        {
            return Parse.String("#").Text()
                .Or(Parse.String("b").Text());
        }

        private static Parser<char> DefineBaseNote()
        {
            return Parse.Char('A')
                .Or(Parse.Char('B'))
                .Or(Parse.Char('C'))
                .Or(Parse.Char('D'))
                .Or(Parse.Char('E'))
                .Or(Parse.Char('F'))
                .Or(Parse.Char('G'));
        }


        public Song ParseSong(string chordSheet)
        {
            StringReader reader = new StringReader(chordSheet);
            var song = new Song();

            while (reader.Peek() != -1)
            {
                var line = reader.ReadLine();

                var songLine = new SongLine();

                songLine.Elements.AddRange(ProcessLine(line));

                var previousLine = song.Lines.LastOrDefault();

                songLine.PreviousLine = previousLine;

                if (previousLine != null)
                {
                    previousLine.NextLine = songLine;
                }

                song.Lines.Add(songLine);
            }

            return song;
        }

        private IEnumerable<BaseElement> ProcessLine(string line)
        {
            var songTokens = Regex.Matches(line, @"([\s/]*)([^\s/]*)");
            BaseElement element = null;

            foreach (Match token in songTokens)
            {
                int index = 0;
                foreach (Group g in token.Groups)
                {
                    if (index != 0)
                    {
                        BaseElement previousElement = element;

                        if (!string.IsNullOrWhiteSpace(g.Value))
                        {
                            var rawChord = ParseExact(g.Value);
                            
                            if (rawChord != null)
                            {
                                var chord = new Chord(rawChord);
                                element = new ChordElement(chord);
                            }
                            else
                            {
                                element = new WordElement(g.Value);
                            }

                        }
                        else
                        {
                            element = new WhitespaceElement(g.Value);
                        }

                        element.PreviousElement = previousElement;

                        if (previousElement != null)
                        {
                            previousElement.NextElement = element;
                        }

                        yield return element;
                    }
                    index++;
                }
            }
        }


        public RawChord ParseExact(string s)
        {
            var result = AbsoluteChordParser.TryParse(s);

            if (result.WasSuccessful)
            {
                return result.Value;
            }

            return null;
        }
    }
}
