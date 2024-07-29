using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace SmartChord.Extractions
{
    public class UltimateGuitarExtractor: IExtractor
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
            var targetClass = "js-store";

            var html = doc.DocumentNode
                .SelectNodes("//div[@class='" + targetClass + "']")
                .Single();

            var titleMatch = Regex.Match(html.OuterHtml, @"&quot;song_name&quot;:&quot;(.*?)&quot;");
            var title = titleMatch.Groups[1].Value;

            var match = Regex.Match(html.OuterHtml, @"&quot;content&quot;:&quot;(.*?)&quot;,&quot;revision_id");
            var chordsheet = match.Groups[1].Value;
            chordsheet = chordsheet.Replace("[ch]", string.Empty);
            chordsheet = chordsheet.Replace("[/ch]", string.Empty);
            chordsheet = chordsheet.Replace("[tab]", string.Empty);
            chordsheet = chordsheet.Replace("[/tab]", string.Empty);
            chordsheet = chordsheet.Replace("&#039;", "'");
            chordsheet = chordsheet.Replace("&rsquo;", "'");
            chordsheet = chordsheet.Replace("&lsquo;", "'");
            chordsheet = chordsheet.Replace("&amp;", "&");
            chordsheet = chordsheet.Replace("&lt;", "<");
            chordsheet = chordsheet.Replace("&gt;", ">");
            chordsheet = chordsheet.Replace("\\&quot;", "\"");
            chordsheet = chordsheet.Replace("&quot;", "\"");
            chordsheet = chordsheet.Replace("&ldquo;", "\"");
            chordsheet = chordsheet.Replace("&hellip;", "…");

            

            chordsheet = chordsheet.Replace(@"\r\n", Environment.NewLine);

            if (title != null)
            {
                chordsheet = $"{title}{Environment.NewLine}{chordsheet}";
            }

            return chordsheet;
        }
    }
}
