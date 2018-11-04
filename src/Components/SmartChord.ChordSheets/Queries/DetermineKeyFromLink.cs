using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MediatR;
using Newtonsoft.Json.Linq;
using SmartChord.Transpose;

namespace SmartChord.ChordSheets.Queries
{
    public static class DetermineKeyFromLink
    {
        public class Query : IRequest<string>
        {
            public string Url { get; set; }
        }

        public class Handler : IRequestHandler<Query, string>
        {
            public async Task<string> Handle(Query request, CancellationToken cancellationToken)
            {
                var web = new HtmlWeb();
                var doc = web.Load(request.Url);


                var html = doc.DocumentNode
                    .SelectNodes("//script")
                    .Single(x => x.InnerText.Contains("window.UGAPP.store.page"));

                var json = Regex.Replace(html.InnerText, @"^\s*window.UGAPP\.store\.page\s*=", string.Empty);

                json = json.TrimEnd();
                json = json.TrimEnd(';');

                var o = JObject.Parse(json);

                //This will be "Apple"
                string chordsheet = (string)o["data"]["tab_view"]["wiki_tab"]["content"];


                chordsheet = chordsheet.Replace("[ch]", string.Empty);
                chordsheet = chordsheet.Replace("[/ch]", string.Empty);

                var transposer = new Transposer();
                return await transposer.ResolveSongKey(chordsheet);
            }
        }
    }
}