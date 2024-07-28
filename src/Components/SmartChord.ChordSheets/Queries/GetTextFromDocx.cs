using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartChord.Extractions;
using SmartChord.Transpose;

namespace SmartChord.ChordSheets.Queries
{
    public static class GetTextFromDocx
    {
        public class Query : IRequest<string>
        {
            public string FilePath { get; set; }

        }

        public class Handler : IRequestHandler<Query, string>
        {
            public async Task<string> Handle(Query request, CancellationToken cancellationToken)
            {
                var extractor = new DocxExtractor();

                var text = await extractor.GetChordSheetText(request.FilePath);
                return text;
            }
        }
    }
}