using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
            var args = _environment.GetCommandLineArgs();

            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    Run,
                    errs => 1);

        }

        private int Run(Options arg)
        {


            return 0;
        }

        public void OnLoaded()
        {
            var documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            Destination = Path.Combine(documentFolder, "SmartChord");
            Directory.CreateDirectory(Destination);
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

        private string _previewText;

        public string PreviewText
        {
            get => _previewText;
            set
            {
                _previewText = value;
                NotifyOfPropertyChange(() => PreviewText);
            }
        }

        public async void OnPdf()
        {

            CreatePdfFromText.Result result = await _mediator.Send(new CreatePdfFromText.Command
            {
                SongText = PreviewText,
                DestinationFilename = Destination
            });


            System.Diagnostics.Process.Start(result.OutputFilename);
        }

        public static string RemoveKeyOfPattern(string input)
        {
            // Define the regular expression pattern to match "(Key of X)" where X can be any value
            string pattern = @"\(Key of [^\)]+\)";

            // Use Regex.Replace to remove the matched patterns
            string result = Regex.Replace(input, pattern, string.Empty).Trim();

            // Return the modified string
            return result;
        }

        public static string KeyOfPostFix(string key)
        {
            return $"(Key of {key})";
        }


        public async Task OnTextChanged()
        {
            if (Uri.IsWellFormedUriString(SourceUrl, UriKind.Absolute))
            {

                PreviewText = await _mediator.Send(new GetChordSheetUrl.Query()
                {
                    Url = SourceUrl
                });

            }
        }


        public async Task OnBrowseSource()
        {
            var source = _dialogService.OpenFileDialog(Destination);

            if (string.IsNullOrWhiteSpace(source)) { return; }

            var extension = Path.GetExtension(source);

            if (string.Equals(extension, ".docx", StringComparison.OrdinalIgnoreCase))
            {
                PreviewText = await _mediator.Send(new GetTextFromDocx.Query()
                {
                    FilePath = source
                });
            }
            else
            {
                PreviewText = await _mediator.Send(new GetTextFromTxt.Query()
                {
                    FilePath = source
                });
            }


        }

        public void OnSave()
        {
            PreviewText = PreviewText.Trim();

            var title = PreviewText.Split('\n')[0].Trim();
            var destination = Path.Combine(Destination, $"{title}.txt");
            File.WriteAllText(destination, PreviewText);

            MessageBox.Show($"Save successful 😎");

        }

        public async Task TransposeDown()
        {
            if(string.IsNullOrWhiteSpace(PreviewText))
            {
                return;
            }

            PreviewText = await _mediator.Send(new TransposeDownHalfStep.Query()
            {
                SongText = PreviewText
            });
        }

        public async Task TransposeUp()
        {
            if (string.IsNullOrWhiteSpace(PreviewText))
            {
                return;
            }

            PreviewText = await _mediator.Send(new TransposeUpHalfStep.Query()
            {
                SongText = PreviewText
            });
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
