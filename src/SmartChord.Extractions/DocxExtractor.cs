using System;
using System.Linq;
using System.Threading.Tasks;
using Xceed.Words.NET;

namespace SmartChord.Extractions
{
    public class DocxExtractor : IExtractor
    {

        public async Task<string> GetChordSheetText(string filePath)
        { 
            var docx = await Task.Run(() => DocX.Load(filePath));
            var paragraphs = docx.Paragraphs.Select(x => x.Text);//.Substring(16));
            var text = string.Join(Environment.NewLine, paragraphs);
            return text;
        }
    }
}
