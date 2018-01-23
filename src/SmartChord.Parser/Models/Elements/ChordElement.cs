namespace SmartChord.Parser.Models.Elements
{
    public class ChordElement : BaseElement
    {
        public ChordElement(Chord chord)
        {
            Chord = chord;
        }

        public Chord Chord { get; }
        public bool IsBassNote { get; set; }

        public override string GetText()
        {
            return Chord.ToString();
        }


    }
}