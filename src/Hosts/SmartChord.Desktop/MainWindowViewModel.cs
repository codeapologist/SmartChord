using System;
using System.IO;
using Caliburn.Micro;
using CommandLine;
using MediatR;
using SmartChord.ChordSheets.Commands;
using Thinktecture;

namespace SmartChord.Desktop
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        private readonly IMediator _mediator;
        private readonly IEnvironment _environment;

        public MainWindowViewModel(IMediator mediator, IEnvironment environment)
        {

            _mediator = mediator;
            _environment = environment;
            var args = _environment.GetCommandLineArgs();

            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    Run,
                    errs => 1);

        }

        private int Run(Options arg)
        {
            Source = arg.Source;
            NewKey = arg.NewKey;
            var extension = Path.GetExtension(Source);
            var filename = Path.GetFileNameWithoutExtension(Source);
            var directoryPath = Path.GetDirectoryName(Source);
            var destinationFilename = $"{filename}_{NewKey}{extension}";
            Destination = Path.Combine(directoryPath ?? throw new InvalidOperationException(), destinationFilename);

            return 0;
        }

        public string Source { get; set; }
        public string Destination { get; set; }
        public string NewKey { get; set; }
        public string SourceUrl { get; set; }

        public void OnGo()
        {
            if (!string.IsNullOrEmpty(SourceUrl))
            {
                _mediator.Send(new CreateWordDocumentFromUrl.Command
                {
                    Url = SourceUrl,
                    NewKey = NewKey,
                    DestinationFilename = Destination
                });


            }
            else
            {
                _mediator.Send(new CreateWordDocumentFromDocx.Command
                {
                    SourceFilename = Source,
                    NewKey = NewKey,
                    DestinationFilename = Destination
                });
            }
            _environment.Exit(0);
        }
    }

    public class Options
    {
        [Value(0, Required = true)]
        public string Exe { get; set; }

        [Value(1, Required = true)]
        public string Source { get; set; }

        [Value(2, Required = true)]
        public string NewKey { get; set; }
    }
}
