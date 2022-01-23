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
            public ExtractorFactory ExtractorFactory { get; set; } = new ExtractorFactory();

            public async Task<string> Handle(Query request, CancellationToken cancellationToken)
            {
                var extactor = ExtractorFactory.GetExtraactor(request.Url);
                var chordsheet = await extactor.GetChordSheetText(request.Url);

                var transposer = new Transposer();
                return await transposer.ResolveSongKey(chordsheet);
            }
        }
    }
}