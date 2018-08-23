using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MediatR;
using Newtonsoft.Json.Linq;
using SmartChord.Transpose;
using Xceed.Words.NET;

namespace SmartChord.ChordSheets.Commands
{
    public static class CreateWordDocumentFromUrl
    {
        public class Command : IRequest<Result>
        {
            public string Url { get; set; }
            public string NewKey { get; set; }
            public string DestinationFilename { get; set; }
        }

        public class Result
        {
            public string OutputFilename { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            public Task<Result> Handle(Command request, CancellationToken cancellationToken)
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

                string chordsheet = (string)o["data"]["tab_view"]["wiki_tab"]["content"];


                chordsheet = chordsheet.Replace("[ch]", string.Empty);
                chordsheet = chordsheet.Replace("[/ch]", string.Empty);

                if (!string.IsNullOrEmpty(request.NewKey))
                {
                    var transposer = new Transposer();
                    chordsheet = transposer.ChangeKey(chordsheet, request.NewKey);
                }

                using (DocX document = DocX.Create(request.DestinationFilename))
                {
                    document.MarginTop = 36f;
                    document.MarginBottom = 36f;
                    document.MarginLeft = 36f;
                    document.MarginRight = 36f;

                    // Add a new Paragraph to the document.
                    var p = document.InsertParagraph();

                    // Append some text.
                    p.Append(chordsheet).Font("Courier New");

                    // Save the document.
                    document.Save();
                }

                return Task.FromResult(new Result{OutputFilename = request.DestinationFilename});
            }
        }
    }
}