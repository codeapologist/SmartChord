namespace SmartChord.Extractions
{
    public interface IExtractor
    {
        string GetChordSheetText(string filePath);
    }
}