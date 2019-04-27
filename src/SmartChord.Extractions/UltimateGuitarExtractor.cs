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

            var html = doc.DocumentNode
                .SelectNodes("//script")
                .Single(x => x.InnerText.Contains("window.UGAPP.store.page"));
            var json = Regex.Replace(html.InnerText, @"^\s*window.UGAPP\.store\.page\s*=", string.Empty);

            json = json.TrimEnd();

            int index = json.LastIndexOf("\n", StringComparison.Ordinal);
            if (index > 0)
                json = json.Substring(0, index);
            json = json.TrimEnd(';');

            var o = JObject.Parse(json);

            var chordsheet = (string) o["data"]["tab_view"]["wiki_tab"]["content"];


            chordsheet = chordsheet.Replace("[ch]", string.Empty);
            chordsheet = chordsheet.Replace("[/ch]", string.Empty);
            return chordsheet;
        }
    }
}