using System;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using CommandLine;
using MediatR;
using Microsoft.Win32;
using SmartChord.ChordSheets.Commands;
using SmartChord.ChordSheets.Queries;
using Thinktecture;

namespace SmartChord.Desktop
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        private readonly IMediator _mediator;
        private readonly IEnvironment _environment;
        private readonly IDialogService _dialogService;

        public MainWindowViewModel(IMediator mediator, IEnvironment environment, IDialogService dialogService)
        {

            _mediator = mediator;
            _environment = environment;
            _dialogService = dialogService;
            SourceIsVisible = true;
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

        

        private string _source;

        public string Source
        {
            get => _source;
            set
            {
                _source = value;
                NotifyOfPropertyChange(() => Source);

            }
        }

        private string _destination;

        public string Destination
        {
            get => _destination;
            set
            {
                _destination = value;
                NotifyOfPropertyChange(() => Destination);
            }
        }

        public string NewKey { get; set; }
        private string _originalKey;
        private bool _sourceIsVisible;
        private bool _sourceUrlIsVisible;

        public string OriginalKey
        {
            get => _originalKey;
            set
            {
                _originalKey = value;
                NotifyOfPropertyChange(() => OriginalKey);
            }
        }

        public string SourceUrl { get; set; }


        public bool SourceIsVisible
        {
            get { return _sourceIsVisible; }
            set
            {
                _sourceIsVisible = value;
                NotifyOfPropertyChange(() => SourceIsVisible);
            }
        }


        public bool SourceUrlIsVisible
        {
            get { return _sourceUrlIsVisible; }
            set
            {
                _sourceUrlIsVisible = value;
                NotifyOfPropertyChange(() => SourceUrlIsVisible);
            }
        }

        public async void OnGo()
        {
            if (SourceUrlIsVisible)
            {
                await _mediator.Send(new CreateWordDocumentFromUrl.Command
                {
                    Url = SourceUrl,
                    NewKey = NewKey,
                    OriginalKey = OriginalKey,
                    DestinationFilename = Destination
                });
            }
            else
            {
                await _mediator.Send(new CreateWordDocumentFromDocx.Command
                {
                    SourceFilename = Source,
                    NewKey = NewKey,
                    OriginalKey = OriginalKey,
                    DestinationFilename = Destination
                });
            }
            _environment.Exit(0);
        }

        public void OnFileInputSelected()
        {
            SourceIsVisible = true;
            SourceUrlIsVisible = false;
        }

        public void OnUrlSelected()
        {
            SourceIsVisible = false;
            SourceUrlIsVisible = true;
        }

        public async Task OnTextChanged()
        {
            if (Uri.IsWellFormedUriString(SourceUrl, UriKind.Absolute))
            {
                var directory = Path.GetDirectoryName(Source);

                if (directory != null)
                {
                    var uri = new Uri(SourceUrl);
                    var destinationName = uri.Segments[uri.Segments.Length - 1];
                    Destination = Path.Combine(directory,  $"{destinationName}.docx");
                    var key = await _mediator.Send(new DetermineKeyFromLink.Query()
                    {
                        Url = SourceUrl
                    });
                    OriginalKey = key;
                }
            }

            
        }

        public async Task OnLoaded()
        {
            string key;
            if (!string.IsNullOrEmpty(SourceUrl))
            {
                key = await _mediator.Send(new DetermineKeyFromLink.Query()
                {
                    Url = SourceUrl
                });
            }
            else
            {
                key = await _mediator.Send(new DetermineKeyFromWordDocument.Query()
                {
                    FilePath = Source
                });
            }

            
            OriginalKey = key;
        }

        public void OnBrowseSource()
        {
            var directory = Path.GetDirectoryName(Source);
            Source = _dialogService.OpenFileDialog(directory);
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

    public class Options2
    {
        [Value(0, Required = true)]
        public string Exe { get; set; }

        [Value(1, Required = true)]
        public string Source { get; set; }

        [Value(2, Required = true)]
        public string NewKey { get; set; }
    }

    public interface IDialogService
    {
        string OpenFileDialog(string defaultPath);
    }

    public class DialogService: IDialogService
    {
        public string OpenFileDialog(string defaultPath)
        {
            var dialog = new OpenFileDialog { InitialDirectory = defaultPath };
            dialog.ShowDialog();

            return dialog.FileName;
        }
    }
}
