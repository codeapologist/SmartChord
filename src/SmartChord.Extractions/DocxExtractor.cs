using System;
using System.Linq;
using Xceed.Words.NET;

namespace SmartChord.Extractions
{
    public class DocxExtractor : IExtractor
    {

        public string GetChordSheetText(string filePath)
        { 
            var docx = DocX.Load(filePath);
            var paragraphs = docx.Paragraphs.Select(x => x.Text);//.Substring(16));
            var text = string.Join(Environment.NewLine, paragraphs);
            return text;
        }
    }
}
