using SmartChord.Parser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartChord.ChordSheets.Pdf
{
    public class PdfSongLine
    {
        public string Line { get; set; }
        public bool IsChordLine { get; set; }
    }

    public class PdfChordsheet
    {
        public List<PdfSongLine> SongLines { get; set; }
    }
    public class PdfHelper
    {

        public static bool IsLineValidGuitarChords(string line)
        {
            string chordPattern = @"^([A-G](#|b)?(m|M|maj|min|dim|aug|sus|[24])?(\d{0,2})?(add\d)?(b|#)?\d{0,2}(\/[A-G](#|b)?)?)$";
            string[] chords = Regex.Split(line.Trim(), @"\s+");

            foreach (string chord in chords)
            {
                if (!Regex.IsMatch(chord, chordPattern))
                {
                    return false;
                }
            }

            return true;
        }

        public static string RemoveDuplicateSections(string input)
        {
            var sectionPattern = @"(?:(?:[A-G](?:#|b)?(?:m|M|maj|min|dim|aug|sus[24])?(?:\d{0,2})?(?:add\d)?(?:b|#)?\d{0,2}(?:\/[A-G](?:#|b)?)?)\s+)+.*(?:\n.*)*?";
            var regex = new Regex(sectionPattern, RegexOptions.Multiline);

            var sections = new HashSet<string>();
            var output = new List<string>();

            foreach (Match match in regex.Matches(input))
            {
                var sectionContent = match.Value.Trim();

                if (!sections.Contains(sectionContent))
                {
                    sections.Add(sectionContent);
                    output.Add(sectionContent);
                }
            }

            return string.Join("\n\n", output);
        }

    }

}
