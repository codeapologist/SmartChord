using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace SmartChord.Extractions
{
    public class WorshipChordsExtractor: IExtractor
    {

        public async Task<string> GetChordSheetText(string url)
        {
            var web = new HtmlWeb();
            var chordsheet = await Task.Run(() => Execute(url, web));

            return chordsheet;
        }

        private static string Execute(string url, HtmlWeb web)
        {
            var doc = web.Load(url);

            var html = doc.ParsedText;

            var match = Regex.Match(html, @"<pre.*>(.*?)<\/pre", RegexOptions.Singleline);
            var chordsheet = match.Groups[1].Value;
            chordsheet = chordsheet.Replace("<br />", string.Empty);
            chordsheet = chordsheet.Replace(@"\r\n", Environment.NewLine);

            return chordsheet;
        }
    }
}