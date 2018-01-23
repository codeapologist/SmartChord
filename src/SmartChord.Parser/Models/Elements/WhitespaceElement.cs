namespace SmartChord.Parser.Models.Elements
{
    public class WhitespaceElement : BaseElement
    {
        private readonly string _text;

        public WhitespaceElement(string text)
        {
            _text = text;
        }

        public override string GetText()
        {
            return _text;
        }
    }
}