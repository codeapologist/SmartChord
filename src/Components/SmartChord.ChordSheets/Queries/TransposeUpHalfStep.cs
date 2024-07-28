using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SmartChord.Extractions;
using SmartChord.Transpose;

namespace SmartChord.ChordSheets.Queries
{
    public static class TransposeUpHalfStep
    {
        public class Query : IRequest<string>
        {
            public string SongText { get; set; }
        }

        public class Handler : IRequestHandler<Query, string>
        {
            public async Task<string> Handle(Query request, CancellationToken cancellationToken)
            {
                var transposer = new Transposer();
                return await Task.FromResult( transposer.TransposeUp(request.SongText));
            }
        }
    }
}