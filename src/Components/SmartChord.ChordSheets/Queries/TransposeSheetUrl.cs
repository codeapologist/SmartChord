using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MediatR;
using Newtonsoft.Json.Linq;
using SmartChord.Extractions;
using SmartChord.Transpose;

namespace SmartChord.ChordSheets.Queries
{
    public static class TransposeSheetUrl
    {
        public class Query : IRequest<string>
        {
            public string Url { get; set; }
            public string NewKey { get; set; }

        }

        public class Handler : IRequestHandler<Query, string>
        {
            public async Task<string> Handle(Query request, CancellationToken cancellationToken)
            {
                var extactor = new UltimateGuitarExtractor();
                var chordsheet = await extactor.GetChordSheetText(request.Url);
                var transposer = new Transposer();
                return await transposer.ChangeKey(chordsheet, request.NewKey);
            }
        }
    }
}
    