using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartChord.Extractions;
using SmartChord.Parser;

namespace SmartChord.ChordSheets.Queries
{
    public static class IsValidChord
    {
        public class Query : IRequest<bool>
        {
            public string Input { get; set; }
        }

        public class Handler : IRequestHandler<Query, bool>
        {
            public ExtractorFactory ExtractorFactory { get; set; } = new ExtractorFactory();

            public async Task<bool> Handle(Query request, CancellationToken cancellationToken)
            {
                return await Task.FromResult( SongParser.ParseExact(request.Input) != null);
            }
        }
    }
}