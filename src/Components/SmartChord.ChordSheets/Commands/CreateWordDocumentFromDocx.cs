using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartChord.Transpose;
using Xceed.Words.NET;

namespace SmartChord.ChordSheets.Commands
{
    public static class CreateWordDocumentFromDocx
    {
        public class Command : IRequest<Result>
        {
            public string SourceFilename { get; set; }
            public string NewKey { get; set; }
            public string OriginalKey { get; set; }
            public string DestinationFilename { get; set; }

        }

        public class Result
        {
            public string OutputFilename { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var docx = DocX.Load(request.SourceFilename);
                var paragraphs = docx.Paragraphs.Select(x => x.Text);

                var chordsheet = string.Join(Environment.NewLine, paragraphs);

                if (!string.IsNullOrEmpty(request.NewKey))
                {
                    var transposer = new Transposer();
                    chordsheet = await transposer.ChangeKey(chordsheet, request.NewKey, request.OriginalKey);
                }

                using (var document = DocX.Create(request.DestinationFilename))
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

                return new Result {OutputFilename = request.DestinationFilename};
            }

        }
    }
}