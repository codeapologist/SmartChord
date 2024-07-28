using System.Linq;

namespace SmartChord.Parser.Models.Elements
{
    public class WordElement : BaseElement
    {
        private readonly string _text;

        public WordElement(string text)
        {
            _text = text;
        }

        public override string GetText()
        {
            return _text;
        }
    }
}