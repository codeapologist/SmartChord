    using CommandLine;

namespace SmartChord.Cli
{
    [Verb("get-chordsheet", HelpText = "Transpose a chordsheet to a new key.")]
    public class GetChordsheetOptions
    {
        

        [Option( "key", Required = false, HelpText = "Transposes chordsheet to specified key")]
        public string NewKey { get; set; }

        [Option("url", Required = false, HelpText = "Url of source chordsheet.", SetName = "url")]
        public string Url { get; set; }

        [Option("docx", Required = false, HelpText = "Input Word chordsheet filename", SetName = "docs")]
        public string Docx { get; set; }

        [Option("txt", Required = false, HelpText = "Input text chordsheet filename", SetName = "txt")]
        public string Text { get; set; }

        [Option("output", Required = false, HelpText = "Output filename.")]
        public string Output { get; set; }

        [Option("format", Required = false, HelpText = "Output format. Supported types: docx, txt")]
        public string Format { get; set; }

    }


}