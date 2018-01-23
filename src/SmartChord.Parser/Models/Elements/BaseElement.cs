namespace SmartChord.Parser.Models.Elements
{
    public abstract class BaseElement
    {
        public abstract string GetText();
        public BaseElement PreviousElement { get; set; }
        public BaseElement NextElement { get; set; }

    }
}