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
            //SourceIsVisible = true;
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

        private string _newKey;

        public string NewKey
        {
            get => _newKey;
            set
            {
                _newKey = value;
                NotifyOfPropertyChange(() => NewKey);
            }
        }

        public string SourceUrl { get; set; }


        //public bool SourceIsVisible
        //{
        //    get { return _sourceIsVisible; }
        //    set
        //    {
        //        _sourceIsVisible = value;
        //        NotifyOfPropertyChange(() => SourceIsVisible);
        //    }
        //}


        //public bool SourceUrlIsVisible
        //{
        //    get { return _sourceUrlIsVisible; }
        //    set
        //    {
        //        _sourceUrlIsVisible = value;
        //        NotifyOfPropertyChange(() => SourceUrlIsVisible);
        //    }
        //}



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

        //public async void OnPreview()
        //{

        //    string value = string.Empty;
        //    if (SourceUrlIsVisible)
        //    {
        //        value = await _mediator.Send(new TransposeSheetUrl.Query
        //        {
        //            Url = SourceUrl,
        //            NewKey = NewKey,
        //            OriginalKey = OriginalKey
        //        });
        //    }
        //    else
        //    {
        //        value = await _mediator.Send(new TransposeSheetDocx.Query
        //        {
        //            NewKey = NewKey,
        //            OriginalKey = OriginalKey,
        //        });
        //    }

        //    PreviewText = value;
        //}

        public async void OnPdf()
        {

            CreatePdfFromText.Result result = await _mediator.Send(new CreatePdfFromText.Command
            {
                SongText = PreviewText,
                NewKey = NewKey,
                OriginalKey = OriginalKey,
                DestinationFilename = Destination
            });


            System.Diagnostics.Process.Start(result.OutputFilename);
        }

        //public async void OnGo()
        //{
        //    if (SourceUrlIsVisible)
        //    {
        //        await _mediator.Send(new CreateWordDocumentFromUrl.Command
        //        {
        //            Url = SourceUrl,
        //            NewKey = NewKey,
        //            OriginalKey = OriginalKey,
        //            DestinationFilename = Destination
        //        });
        //    }
        //    else
        //    {
        //        await _mediator.Send(new CreateWordDocumentFromDocx.Command
        //        {
        //            SourceFilename = Source,
        //            NewKey = NewKey,
        //            OriginalKey = OriginalKey,
        //            DestinationFilename = Destination
        //        });
        //    }
        //}

        public void OnFileInputSelected()
        {
            //SourceIsVisible = true;
            //SourceUrlIsVisible = false;
        }

        public void OnUrlSelected()
        {
            //SourceIsVisible = false;
            //SourceUrlIsVisible = true;
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


        public async Task OnFileSourceTextChanged()
        {
            if (!File.Exists(Source)) return;

            var path = Path.GetDirectoryName(Source);
            var destination = Path.GetFileNameWithoutExtension(Source);
            var extension = Path.GetExtension(Source);

            destination = RemoveKeyOfPattern(destination);

            if (extension.Equals(".docx", StringComparison.CurrentCultureIgnoreCase))
            {
                var key = await _mediator.Send(new DetermineKeyFromWordDocument.Query()
                {
                    FilePath = Source
                });

                OriginalKey = key;

                if (string.IsNullOrEmpty(NewKey))
                {
                    NewKey = key;
                }

                destination = $"{destination} {KeyOfPostFix(NewKey)}";

                Destination = Path.Combine(path, $"{destination}{extension}");
            }

        }

        public async Task OnTextChanged()
        {
            if (Uri.IsWellFormedUriString(SourceUrl, UriKind.Absolute))
            {

                //var directory = SourceUrlIsVisible && !string.IsNullOrEmpty(Destination) ? Destination : Source;

                //if (Path.HasExtension(directory))
                //{
                //    directory = Path.GetDirectoryName(directory);

                //}


                //if (directory != null)
                //{
                    var uri = new Uri(SourceUrl);

                    PreviewText = await _mediator.Send(new GetChordSheetUrl.Query()
                    {
                        Url = SourceUrl
                    });





                //}
            }


        }


        public async Task OnNewKeyTextChanged()
        {

        }



        public async Task OnLoaded()
        {
            string key = "";
            if (!string.IsNullOrEmpty(SourceUrl))
            {
                key = await _mediator.Send(new DetermineKeyFromLink.Query()
                {
                    Url = SourceUrl
                });
            }
            else if (!string.IsNullOrEmpty(Source))
            {
                key = await _mediator.Send(new DetermineKeyFromWordDocument.Query()
                {
                    FilePath = Source
                });
            }
            else
            {
                var documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                Destination = Path.Combine(documentFolder, "SmartChord");
                Directory.CreateDirectory(Destination);

            }


            OriginalKey = key;
        }

        public async Task OnBrowseSource()
        {
            Source = _dialogService.OpenFileDialog(Destination);

            if (string.IsNullOrWhiteSpace(Source)) { return; }

            var extension = Path.GetExtension(Source);

            if (string.Equals(extension, ".docx", StringComparison.OrdinalIgnoreCase))
            {
                PreviewText = await _mediator.Send(new GetTextFromDocx.Query()
                {
                    FilePath = Source
                });
            }
            else
            {
                PreviewText = await _mediator.Send(new GetTextFromTxt.Query()
                {
                    FilePath = Source
                });
            }


        }

        public async Task OnSave()
        {
            PreviewText = PreviewText.Trim();

            var title = PreviewText.Split('\n')[0].Trim();
            var destination = Path.Combine(Destination, $"{title}.txt");
            File.WriteAllText(destination, PreviewText);

            MessageBox.Show($"Save successful - {destination}");
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
