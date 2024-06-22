using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MediatR;
using Newtonsoft.Json.Linq;
using SmartChord.Extractions;
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
            public string OriginalKey { get; set; }
        }

        public class Result
        {
            public string OutputFilename { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            public ExtractorFactory ExtractorFactory { get; set; } = new ExtractorFactory();

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var extactor = ExtractorFactory.GetExtraactor(request.Url);
                var chordsheet = await extactor.GetChordSheetText(request.Url);

                if (!string.IsNullOrEmpty(request.NewKey))
                {
                    var transposer = new Transposer();
                    chordsheet = await transposer.ChangeKey(chordsheet, request.NewKey, request.OriginalKey);
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
                    p.Append(chordsheet).Font("Consolas");

                    // Save the document.
                    document.Save();
                }

                return new Result{OutputFilename = request.DestinationFilename};
            }
        }
    }
}