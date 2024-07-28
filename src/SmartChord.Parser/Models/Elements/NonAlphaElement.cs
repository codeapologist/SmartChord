namespace SmartChord.Parser.Models.Elements
{
    public class NonAlphaElement : BaseElement
    {
        private readonly string _text;

        public bool IsSlash { get; }

        public NonAlphaElement(string text)
        {
            if (text == "/")
            {
                IsSlash = true;
            }

            _text = text;
        }

        public override string GetText()
        {
            return _text;
        }
    }
}