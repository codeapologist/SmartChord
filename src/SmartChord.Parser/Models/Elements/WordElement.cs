using System.Linq;

namespace SmartChord.Parser.Models.Elements
{
    public class WordElement : BaseElement
    {
        private readonly string _text;

        public bool IsSlash { get; }
        public bool IsFirstCharacterAlphaNumeric { get; }
        public bool IsLastCharacterAlphaNumeric { get; }

        public WordElement(string text)
        {
            if (text == "/")
            {
                IsSlash = true;
            }

            if (char.IsLetterOrDigit(text.First()))
            {
                IsFirstCharacterAlphaNumeric = true;
            }

            if (char.IsLetterOrDigit(text.Last()))
            {
                IsLastCharacterAlphaNumeric = true;
            }

            _text = text;
        }

        public override string GetText()
        {
            return _text;
        }
    }
}